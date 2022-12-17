/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.IO;
using System.Windows.Forms;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Models;
using static DELAPORTATION.Teleportation;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public partial class InfoWin : Form
    {
        #region General declarations

        // System stuff
        private NotifyIcon TrayIcon;
        private static System.Timers.Timer NewDataArrivalTimer = new System.Timers.Timer();

        // Extra & flags

        
        #endregion
        
        
        #region Bootstrap

        // Constructor
        public InfoWin()
        {
            InitializeComponent();
            
            SetupSystem();
        }

        // Setup the system by running initialization methods
        private void SetupSystem()
        {
            InitializeUI();
            InitializeTrayIcon();
            InitializeMain();
            InitializeThreads();
            InitializeTimers();
        }

        #endregion


        #region Initialization

        // Initialize any UI stuff
        private void InitializeUI()
        {
            Left = Screen.PrimaryScreen.Bounds.Width - Width - 10;
            Top = Screen.PrimaryScreen.Bounds.Height - Height - 50;
        }

        // Initizalize tray icon
        private void InitializeTrayIcon()
        {
            TrayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.AppIcon,
                Text = "DELAPORTATION - Teleport your files anywhere!",
                ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("About", TrayAboutEventDelegate),
                                                               new MenuItem("Teleport", 
                                                                            new MenuItem[] { new MenuItem("To friends", TrayTeleportToFriendsEventDelegate),
                                                                                             new MenuItem("To cloud", TrayTeleportToCloudEventDelegate),
                                                                                             new MenuItem("Cancel", TrayCancelTeleportEventDelegate) }),
                                                               new MenuItem("Settings", TraySettingsEventDelegate),
                                                               new MenuItem("Exit", TrayExitEventDelegate) }),
                Visible = true
            };

            TrayIcon.MouseClick += TrayIconMouseClickDelegate;
        }

        // Initialize main stuff
        private void InitializeMain()
        {
            if (!Directory.Exists(TempFilesDir))
            {
                var DirInfo = Directory.CreateDirectory(TempFilesDir);

                DirInfo.Attributes = FileAttributes.Hidden;
            }

            CopyTrap();
        }

        // Initialize all threads
        private void InitializeThreads()
        {
            // Future usage...
        }

        // Initialize all timers
        private void InitializeTimers()
        {
            NewDataArrivalTimer.Interval = 3000;
            NewDataArrivalTimer.Elapsed += PasteTrapCheckDelegate;
            NewDataArrivalTimer.Start();
        }

        #endregion


        #region Main methods

        // Copy trap (Setup a system trap for the copy method)
        private void CopyTrap()
        {
            ClipboardMonitorAction = new ClipboardMonitor();

            ClipboardMonitorAction.ClipboardCopyEvent += new EventHandler(CopyTrapDelegate);
        }

        // Window "on click" event
        private void InfoWin_Click(object sender, EventArgs e)
        {
            Hide();
        }

        // Label "on click" event
        private void label1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        // Notification message "on click" event
        private void NotificationMsg_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void CancelTeleportButton_Click(object sender, EventArgs e)
        {
            AbortTeleportation();
        }

        private void NoNotificationsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NoNotificationsCheckBox.Checked)
                DoNotShowNotifications = true;
            else
                DoNotShowNotifications = false;
        }

        #endregion


        #region Utilities

        // Tray icon - Mouse click delegate (Handler)
        private void TrayIconMouseClickDelegate(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Visible && UserModel.IsTeleporting)
                    TrayIcon.ShowBalloonTip(1000, "DELAPORTATION", LastNotificationMessage, ToolTipIcon.Info);
            }
        }

        // Tray icon - About event delegate (Handler)
        private void TrayAboutEventDelegate(object sender, EventArgs e)
        {
            if (LastOpenedWindow != null && (string)LastOpenedWindow.Tag == "about")
                return;

            LastOpenedWindow = new AboutWin();

            LastOpenedWindow.Show();
        }

        // Tray icon - Teleport to friends event delegate (Handler)
        private void TrayTeleportToFriendsEventDelegate(object sender, EventArgs e)
        {
            if (LastOpenedWindow != null && (string)LastOpenedWindow.Tag == "teleport")
                return;

            LastOpenedWindow = new TeleportWin();

            LastOpenedWindow.Show();
        }

        // Tray icon - Teleport to cloud event delegate (Handler)
        private void TrayTeleportToCloudEventDelegate(object sender, EventArgs e)
        {
            if (LastOpenedWindow != null && (string)LastOpenedWindow.Tag == "teleport")
                return;

            LastOpenedWindow = new TeleportWin();

            LastOpenedWindow.Show();
        }

        // Tray icon - Cancel teleportation event delegate (Handler)
        private void TrayCancelTeleportEventDelegate(object sender, EventArgs e)
        {
            AbortTeleportation();
        }

        // Tray icon - Settings event delegate (Handler)
        private void TraySettingsEventDelegate(object sender, EventArgs e)
        {
            if (LastOpenedWindow != null && (string)LastOpenedWindow.Tag == "settings")
                return;

            LastOpenedWindow = new SettingsWin();

            LastOpenedWindow.Show();
        }

        // Tray icon - Exit event delegate (Handler)
        private void TrayExitEventDelegate(object sender, EventArgs e)
        {
            if (UserModel.IsTeleporting)
            {
                var Answer = MessageBox.Show("A teleportation is in progress. Do you really want to exit?", "DELAPORTATION", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (Answer == DialogResult.No)
                    return;
            }

            DeleteTempFiles();

            TrayIcon.Visible = false;
            TrayIcon.Dispose();

            Environment.Exit(0);
        }

        #endregion
    }
}
