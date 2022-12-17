/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using static DELAPORTATION.Models;
using static DELAPORTATION.WebServices;
using static DELAPORTATION.RegistryControl;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public partial class AuthWin : Form
    {
        #region Bootstrap

        // Constructor
        public AuthWin()
        {
            InitializeComponent();

            SetupSystem();
        }

        // Setup the system by running initialization methods
        private void SetupSystem()
        {
            linkLabel1.Links.Add(0, 0, "http://delaportation.eu/en/register/");
        }

        #endregion


        #region Main methods

        private void TryAuthenticate()
        {
            if (AuthTextBox.Text.Length < 40)
                MessageBox.Show("Please insert a correct password. If you don't have one, please register!", "DELAPORTATION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                var Params = new Dictionary<string, object>();

                Params.Add("auth", AuthTextBox.Text);

                var Results = (WebResponseModel)WebService(WebServiceModel.URL, Params);

                if (Results.Auth.Equals(false))
                    MessageBox.Show("The password you provided is wrong!", "DELAPORTATION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                {
                    dynamic UserData = Results.Data;
                    
                    UserModel.Email = UserData["email"];
                    UserModel.Hash = AuthTextBox.Text;
                    UserModel.UserFolderID = UserData["user_folder_id"];
                    UserModel.IsAuthenticated = true;

                    StoreSettingsInRegistry();

                    AppRouting();

                    Hide();
                }
            }
        }

        #endregion


        #region Utilities

        private void AuthWin_Activated(object sender, System.EventArgs e)
        {
            AuthTextBox.Focus();
        }

        private void AuthWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void AuthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TryAuthenticate();
            }
        }

        private void AuthButton_Click(object sender, System.EventArgs e)
        {
            TryAuthenticate();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData.ToString());
        }

        #endregion
    }
}
