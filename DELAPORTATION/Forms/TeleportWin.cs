/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Windows.Forms;
using static DELAPORTATION.Globals;

namespace DELAPORTATION
{
    public partial class TeleportWin : Form
    {
        #region Bootstrap

        // Constructor
        public TeleportWin()
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

        private void TeleportWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TeleportToMode)
                Environment.Exit(0);

            LastOpenedWindow = null;
        }

        #endregion
    }
}
