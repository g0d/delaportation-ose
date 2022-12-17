/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Windows.Forms;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public partial class SplashWin : Form
    {
        #region General declarations

        private System.Timers.Timer HideTimer = new System.Timers.Timer();

        #endregion


        #region Bootstrap

        // Constructor
        public SplashWin(string[] Params)
        {
            InitializeComponent();

            SetupSystem();

            RoutingParams = Params;
        }

        // Setup the system by running initialization methods
        private void SetupSystem()
        {
            //UnregisterKey();                         /* TESTS: To be DELETED when application is ready */
            InitializeRegistryKey();
            InitializeWebServiceModel();
            InitializeUserModel();
            InitializeCrypto();

            DeleteTempFiles();

            HideTimer.Interval = 3000;
            HideTimer.Elapsed += HideSplashWin;
            HideTimer.Start();
        }

        #endregion


        #region Utilities

        private void HideSplashWin(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                HideTimer.Stop();

                Hide();

                AppRouting();
            }));
        }

        #endregion
    }
}
