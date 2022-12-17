/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System.Windows.Forms;
using static DELAPORTATION.Globals;

namespace DELAPORTATION
{
    public partial class SettingsWin : Form
    {
        #region Bootstrap

        // Constructor
        public SettingsWin()
        {
            InitializeComponent();

            SetupSystem();
        }

        // Setup the system by running initialization methods
        private void SetupSystem()
        {
            
        }

        #endregion


        #region Utilities

        private void SettingsWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            LastOpenedWindow = null;
        }

        #endregion
    }
}
