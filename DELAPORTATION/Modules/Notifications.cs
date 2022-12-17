/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Threading;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Models;

namespace DELAPORTATION
{
    public static class Notifications
    {
        private static Thread NotificationThread;

        // Final notification
        public static void FinalNotify(string Message, int HideTime = 2000)
        {
            LastNotificationMessage = null;

            Notify(Message);

            HideNotification(HideTime);
        }

        // Notify users with messages
        public static void Notify(string Message)
        {
            InfoWinForm.Invoke(new Action(() =>
            {
                InfoWinForm.Controls.Find("NotificationMsg", false)[0].Text = Message;

                InfoWinForm.Show();
            }));
        }

        // Hide notifications (thread caller)
        public static void HideNotification(int HideTime = 2000)
        {
            try
            {
                NotificationThread = new Thread(new ParameterizedThreadStart(HideNotificationDelegate));

                NotificationThread.IsBackground = true;
                NotificationThread.SetApartmentState(ApartmentState.STA);
                NotificationThread.Start(HideTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Hide notifications delegate for thread
        private static void HideNotificationDelegate(object HideTime)
        {
            Thread.Sleep((int)HideTime);

            InfoWinForm.Invoke(new Action(() =>
            {
                InfoWinForm.Controls.Find("NotificationMsg", false)[0].Text = "";
                InfoWinForm.Controls.Find("CancelTeleportButton", false)[0].Enabled = true;

                InfoWinForm.Hide();
            }));

            NotificationCallback();

            Thread.CurrentThread.Abort();
        }

        // Notification callback method
        private static void NotificationCallback()
        {
            if (!UserModel.IsTeleporting)
                return;

            Notify(LastNotificationMessage);
        }
    }
}
