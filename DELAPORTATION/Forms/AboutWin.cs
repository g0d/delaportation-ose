/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System.Windows.Forms;
using static DELAPORTATION.Globals;

namespace DELAPORTATION
{
    public partial class AboutWin : Form
    {
        #region Bootstrap

        // Constructor
        public AboutWin()
        {
            InitializeComponent();

            SetupSystem();
        }

        // Setup the system by running initialization methods
        private void SetupSystem()
        {
            AboutLabel.Text = "DELAPORTATION is a freeware with paying extensions.\n\nFor more info please visit http://delaportation.eu.\n\n\n\nCopyright (C) 2017 - George Delaportas";
        }

        #endregion


        #region Utilities

        private void label2_Click(object sender, System.EventArgs e)
        {
            LastOpenedWindow = null;

            Close();
        }

        #endregion
    }
}
