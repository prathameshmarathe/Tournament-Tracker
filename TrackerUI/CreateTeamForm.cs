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
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        private ITeamRequester callingForm;
        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
            //CreateSampleData();
            WireUpLists();
        }

   
        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "Tim", LastName = "Corey" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Paul", LastName = "Scholes" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "David", LastName = "Beckham" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Ryan", LastName = "Giggs" });
        }

        private void WireUpLists()
        {
            selectTeamMemberDropdown.DataSource = null;
            selectTeamMemberDropdown.DataSource = availableTeamMembers;
            selectTeamMemberDropdown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if(ValidateForm())
            {
                PersonModel p = new PersonModel();
                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.EmailAddress = emailValue.Text;
                p.CellphoneNumb = cellphoneValue.Text;
                p=GlobalConfig.Connection.CreatePerson(p);
                selectedTeamMembers.Add(p);
                WireUpLists();
                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = ""; 
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again");
            }
            

        }

        private bool ValidateForm()
        {
            //TODO - Add validation later
            if (firstNameValue.Text.Length == 0)
            {
                return false;

            }
            if (lastNameValue.Text.Length == 0)
            {
                return false;

            }
            if (emailValue.Text.Length == 0)
            {
                return false;

            }
            if (cellphoneValue.Text.Length == 0)
            {
                return false;

            }

            return true;
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropdown.SelectedItem;
            if (p!=null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
                WireUpLists(); 
            }
        }

        private void removeSelectedMembersButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;
            if (p!=null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);
                WireUpLists(); 
            }

        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new TeamModel();
            t.TeamName = teamNameValue.Text;
            t.TeamMembers = selectedTeamMembers;
            t=GlobalConfig.Connection.CreateTeam(t);
            callingForm.TeamComplete(t);
            this.Close(); 

            //TODO - If we aren't closing this form, reset the values
        }
    }
}
