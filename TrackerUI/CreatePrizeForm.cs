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
    public partial class CreatePrizeForm : Form
    {
        IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNameValue.Text,
                    placeNumberValue.Text,
                    priceAmountValue.Text,
                    pricePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);
                callingForm.PrizeComplete(model);
                this.Close();
                    //placeNameValue.Text = "";
                    //placeNumberValue.Text = "";
                    //priceAmountValue.Text = "0";
                    //pricePercentageValue.Text = "0";
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again");
            }

        }

        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValid = int.TryParse(placeNumberValue.Text, out placeNumber);
            if (!placeNumberValid)
                output = false;
            if (placeNumber < 1)
                output = false;

            if (placeNameValue.Text.Length == 0)
                output = false;

            decimal priceAmount = 0;
            int pricePercentage = 0;
            bool priceAmountValid = decimal.TryParse(priceAmountValue.Text, out priceAmount);
            bool prizePercentageValid = int.TryParse(pricePercentageValue.Text, out pricePercentage);

            if ((!priceAmountValid) || (!prizePercentageValid))
            {
                output = false;
            }

            if (priceAmount <= 0 && pricePercentage <= 0)
            { 
                output = false;
            }
            
            if(pricePercentage<0 || pricePercentage>100)
            {
                output = false;
            }


            return output;
        }
    }
}
