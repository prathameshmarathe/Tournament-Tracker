using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;


//Load the text file
//Convert the text to list of prize model
// Find the id
//Add new record with the new id
//Convert prizes to list<string> 
//Save the list<strings> to tbe text file
namespace TrackerLibrary.DataAccess.TextHelpers

{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName) //PrizeModels.csv
        {
            // C:\Users\prath\source\repos\TournamentTracker\Data\PrizeModels.csv
            return $"{ConfigurationManager.AppSettings["FilePath"]}\\{fileName}";
        }

        public static List<string> LoadFile(this string file)
        {
            if(!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }
        #region Prize
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }
            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach(PrizeModel p in models)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        #endregion
        #region Person
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumb = cols[4];
                output.Add(p);
            }
            return output;
        }
        public static void SaveToPersonFile(this List<PersonModel>models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach(PersonModel p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellphoneNumb}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);

        }
        #endregion

        #region Team
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines,string peopleFileName) 
        {
            //id,teamName,set of ids seperated by pipe
            //1,Class of 92, 1|2|3|4|5
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];
                string[] personIds = cols[2].Split('|');
                foreach(string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;

        }
        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach(TeamModel t in models)
            {
                lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);

        }
        private static string ConvertPeopleListToString(List<PersonModel>people)
        {
            string output = string.Empty;
            if(people.Count==0)
            {
                return "";
            }
            foreach(PersonModel p in people)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines,string teamFileName,string peopleFileName,string prizeFileName)
        {
            //id,TournamentName,EntryFee,(id|id|id|id -Entered Teams),(id|id|id - Prizes),(Rounds- id^id^id|id^id^id|id^id^id)
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);
                //List<TeamModel> t = new List<TeamModel>();
                string[] teamIds = cols[3].Split('|');
                foreach(string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }
                string[] prizeIds = cols[4].Split('|');
                foreach (string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                }


                //TODO - Capture Round Information
                output.Add(tm);
            }
            return output;

        }

        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (TournamentModel t in models)
            {
                lines.Add($@"{t.Id},
                {t.TournamentName},
                {t.EntryFee},
                {ConverTeamListToString(t.EnteredTeams)},
                {ConvertPrizeListToString(t.Prizes)}),
                {ConvertRoundListToString(t.Rounds)}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);

        }
        private static string ConverTeamListToString(List<TeamModel> teams)
        {
            string output = string.Empty;
            if (teams.Count == 0)
            {
                return "";
            }
            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;

        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = string.Empty;
            if (prizes.Count == 0)
            {
                return "";
            }
            foreach (PrizeModel t in prizes)
            {
                output += $"{t.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;

        }

        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            //Rounds - id ^ id ^ id | id ^ id ^ id | id ^ id ^ id
            string output = string.Empty;
            if (rounds.Count == 0)
            {
                return "";
            }
            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ConvertMatchupListToString(r)}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;

        }
        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = string.Empty;
            if (matchups.Count == 0)
            {
                return "";
            }
            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id}^";
            }
            output = output.Substring(0, output.Length - 1);
            return output;

        }


    }
    #endregion

    
}
