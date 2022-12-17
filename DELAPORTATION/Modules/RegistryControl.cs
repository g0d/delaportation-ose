/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using Microsoft.Win32;
using static DELAPORTATION.Models;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public static class RegistryControl
    {
        // Check if "DELAPORTATION" key has already been registered in the system
        public static bool CheckRegisteredKey()
        {
            var ExistingRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION");

            if (ExistingRegistryKey == null)
                return false;

            if (ExistingRegistryKey.GetValue("Installed") == null)
                return false;

            if ((string)ExistingRegistryKey.GetValue("Installed") == "true")
                UserModel.IsAuthenticated = true;

            return true;
        }

        // Register "DELAPORTATION" key in the system
        public static void RegisterKey()
        {
            var NewRegistryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION");

            NewRegistryKey.SetValue("MUIVerb", "Teleport to...");
            NewRegistryKey.SetValue("Position", "Bottom");
            NewRegistryKey.SetValue("Installed", "false");
            NewRegistryKey.SetValue("Teleport", "true");
            NewRegistryKey.SetValue("Email", "");
            NewRegistryKey.SetValue("Hash", "");
            NewRegistryKey.SetValue("UserFolderID", "");
            NewRegistryKey.SetValue("MaxFileSize", 10485760);
            NewRegistryKey.SetValue("DoNotShowInfoNotificationsAgain", "false");
            NewRegistryKey.Close();

            NewRegistryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION\command");

            NewRegistryKey.SetValue("", Application.ExecutablePath + " /tt");
            NewRegistryKey.Close();
        }

        // Unregister "DELAPORTATION" key
        public static void UnregisterKey()
        {
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION\command", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION", false);
        }

        // Store user settings in registry
        public static void StoreSettingsInRegistry()
        {
            var MyRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION", true);
            var SHA1Crypto = SHA1.Create();
            var DataObject = ConvertObjectToByteArray(Guid.NewGuid().ToString() + Environment.MachineName + Environment.UserDomainName);

            UserModel.UID = BitConverter.ToString(SHA1Crypto.ComputeHash(DataObject)).Replace("-", "").ToLower();

            MyRegistryKey.SetValue("Installed", "true");
            MyRegistryKey.SetValue("UID", UserModel.UID);
            MyRegistryKey.SetValue("Email", UserModel.Email);
            MyRegistryKey.SetValue("Hash", UserModel.Hash);
            MyRegistryKey.SetValue("UserFolderID", UserModel.UserFolderID);
            MyRegistryKey.Close();
        }

        // Load user settings from registry
        public static void LoadSettingsFromRegistry()
        {
            try
            {
                var MyRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION");

                UserModel.UID = (string)MyRegistryKey.GetValue("UID");
                UserModel.Email = (string)MyRegistryKey.GetValue("Email");
                UserModel.Hash = (string)MyRegistryKey.GetValue("Hash");
                UserModel.UserFolderID = (string)MyRegistryKey.GetValue("UserFolderID");
                UserModel.MaxFileSize = (int)MyRegistryKey.GetValue("MaxFileSize");

                MyRegistryKey.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Critical error. Registry is inaccessible!", "DELAPORTATION", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Console.WriteLine(e.Message);
            }
        }
    }
}
