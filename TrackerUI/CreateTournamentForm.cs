using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form,IPrizeRequester,ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }
        private void WireUpLists()
        {
            selectTeamDropdown.DataSource = null;
            selectTeamDropdown.DataSource = availableTeams;
            selectTeamDropdown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropdown.SelectedItem;
            if(t!=null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
                WireUpLists();
            }
             

        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Call Prize Form
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
           

        }

        public void PrizeComplete(PrizeModel model)
        {
            //Get back PrizeModel from the form

            //Bind Prizelistbox to PrizeModel
            selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;
            if(p!=null)
            {
                selectedPrizes.Remove(p);
                WireUpLists();
            }
            
        }

        private void removeSelectedTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;
            if(t!=null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);
                WireUpLists(); 
            }

        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            //Validate data
            decimal fee = 0;
            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text,out fee);

            if(!feeAcceptable)
            {
                MessageBox.Show("Enter valid team entry",
                    "Invalid Fee", MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            // Create Tournament Entry
            TournamentModel tm = new TournamentModel();
            tm.TournamentName = tournamentNameValue.Text;
            tm.EntryFee = fee;
            tm.Prizes = selectedPrizes;
            tm.EnteredTeams = selectedTeams;
            // Wire our matchups
            TournamentLogic.CreateRounds(tm);
            // order our team list randomly
            // Take in the list, check if it is big enough. If not, add in byes(2^n teams)
            //Create first round of matchups
            //Create every round after that


            //Create Prizes all of prizes entries
            // Create all of the team entries
            GlobalConfig.Connection.CreateTournament(tm);

            

        }
    }
}
