/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.IO;
using System.Windows.Forms;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public static class Globals
    {
        // System stuff
        public static Form InfoWinForm;
        public static Form LastOpenedWindow = null;
        public static WinKeyCatcher WinKeyCatcherAction;
        public static ClipboardMonitor ClipboardMonitorAction;

        // Extra & flags
        public static string TempFilesDir = Path.GetPathRoot(Environment.SystemDirectory) + @"DP_Temp\";
        public static string[] RoutingParams;
        public static string LastNotificationMessage = null;
        public static bool TeleportToMode = false;
        public static bool DoNotShowNotifications = false;
    }
}
