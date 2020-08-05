using Dapper;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "tracker";
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("FirstName", model.FirstName);
                p.Add("LastName", model.LastName);
                p.Add("EmailAddress", model.EmailAddress);
                p.Add("CellphoneNumb", model.CellphoneNumb);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("spPeople_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("id");
                return model;

            }
        }

        // TODO - Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves PrizeModel to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>the prize information, including the unique identifier</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("PlaceNumber", model.PlaceNumber);
                p.Add("PlaceName", model.PlaceName);
                p.Add("PrizeAmount", model.PrizeAmount);
                p.Add("PricePercentage", model.PrizePercentage);
                p.Add("id", 0, dbType: DbType.Int32,direction:ParameterDirection.Output);
                connection.Execute("spPrizes_Insert", p,commandType:CommandType.StoredProcedure);
                model.Id = p.Get<int>("id");
                return model;
               
            }
        }


        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using(IDbConnection connection=new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("spPeople_GetAll").ToList();
            }
            return output;
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("TeamName", model.TeamName);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("spTeams_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("id");

                foreach(PersonModel tm in model.TeamMembers)
                {

                    p = new DynamicParameters();
                    p.Add("PersonId", tm.Id);
                    p.Add("TeamId", model.Id);
                    connection.Execute("spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
                return model;
            }


        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("spTeams_GetAll").ToList();
                
                foreach ( TeamModel t in output)
                {
                    var p = new DynamicParameters();
                    p.Add("TeamId", t.Id);
                    t.TeamMembers = connection.Query<PersonModel>("spTeamMembers_GetByTeam",p, commandType: CommandType.StoredProcedure).ToList();
                }
                
            }
            return output;
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection,model);
                SaveTournamentPrizes(connection, model);
                SaveTournamentEntries(connection, model);
                SaveTournamentRounds(connection, model);

            }
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            // List<List<MatchupModel>> Rounds
            //List<MatchupEntryModel> Entries

            //Loop through rounds
                //Loop through matchups
                //Save matchup
                    //Loop through entries and save them


            foreach(List<MatchupModel> round in model.Rounds)
            {
                foreach(MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("TournamentId", model.Id);
                    p.Add("MatchupRound", matchup.MatchupRound);
                    p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    connection.Execute("spMatchups_Insert", p, commandType: CommandType.StoredProcedure);
                    matchup.Id = p.Get<int>("id");

                    foreach(MatchupEntryModel entry in matchup.Entries)
                    {

                        p = new DynamicParameters();
                        p.Add("MatchupId", matchup.Id);
                        if(entry.ParentMatchup==null)
                        {
                            p.Add("ParentMatchupId",null);
                        }
                        else
                        {
                          p.Add("ParentMatchupId", entry.ParentMatchup.Id);
                        }
                        
                        if(entry.TeamCompeting==null)
                        {
                            p.Add("TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        
                        p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute("sp_MatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                        //matchup.Id = p.Get<int>("id");
                    }

                }
            }

        }

        private void SaveTournament(IDbConnection connection,TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("TournamentName", model.TournamentName);
            p.Add("EntryFee", model.EntryFee);
            p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
            connection.Execute("spTournaments_Insert", p, commandType: CommandType.StoredProcedure);
            model.Id = p.Get<int>("id");

        }

        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pz in model.Prizes)
            {
                var p = new DynamicParameters();
                p = new DynamicParameters();
                p.Add("PrizeId", pz.Id);
                p.Add("TournamentId", model.Id);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_TournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }


        }

        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p = new DynamicParameters();
                p.Add("TeamId", tm.Id);
                p.Add("TournamentId", model.Id);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_TournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }

        }

        //public List<PrizeModel> GetPrizes_All()
        //{
        //    List<PrizeModel> output;
        //    using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.CnnString(db)))
        //    {
        //        output = connection.Query<PrizeModel>("spPrizes_GetAll").ToList();
        //    }
        //    return output;
        //}
    }
}
