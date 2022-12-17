/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Models;
using static DELAPORTATION.FTP;
using static DELAPORTATION.Notifications;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public static class Teleportation
    {
        private static Thread TeleportWorkerThread;
        private static Thread TeleportThread;
        
        // Teleport files (Upload/Download mode) [Copy/Paste]
        public static void TeleportFiles(Dictionary<string, Dictionary<string, ContentType>> Data, DataType DType, TransferMode TMode)
        {
            var WorkerData = new Dictionary<TransferMode, Dictionary<DataType, Dictionary<string, Dictionary<string, ContentType>>>>();
            
            WorkerData.Add(TMode, new Dictionary<DataType, Dictionary<string, Dictionary<string, ContentType>>> { { DType, Data } });

            UserModel.IsTeleporting = true;
            
            TeleportWorkerThread = new Thread(new ParameterizedThreadStart(TeleportWorkerDelegate));

            TeleportWorkerThread.IsBackground = true;
            TeleportWorkerThread.SetApartmentState(ApartmentState.STA);
            TeleportWorkerThread.Start(WorkerData);
        }

        // Abort any active teleportation
        public static void AbortTeleportation()
        {
            if (TeleportThread == null)
                return;

            if (TeleportThread.ThreadState != ThreadState.Aborted)
                TeleportThread.Abort();
            
            var Result = (string)SetNewDataCheck("0");

            if (Result != "OK")
                FinalNotify("A service error has occured during the teleportation!", 3000);
            else
            {
                InfoWinForm.Invoke(new Action(() =>
                {
                    InfoWinForm.Controls.Find("CancelTeleportButton", false)[0].Enabled = false;
                }));

                FinalNotify("Teleportation aborted!");
            }
            
            ResetTeleportationStatus();

            //File.Delete(TempFilesDir);
        }

        // Reset teleportation status
        public static void ResetTeleportationStatus()
        {
            UserModel.IsTeleporting = false;
            UserModel.HasMultipleFilesToTeleport = false;
            UserModel.HasIncomingData = false;
        }

        // Worker delegate
        public static void TeleportWorkerDelegate(object WorkerData)
        {
            var ActualWorkerData = (Dictionary<TransferMode, Dictionary<DataType, Dictionary<string, Dictionary<string, ContentType>>>>)WorkerData;
            var TMode = ActualWorkerData.Keys.First();
            var DType = ActualWorkerData.Values.First().Keys.First();
            var WorkerDataObjects = ActualWorkerData.Values.First().Values.First();
            
            if (TMode == TransferMode.UP)
            {
                foreach (var DataObjects in WorkerDataObjects.Values)
                {
                    foreach (var ThisDataObject in DataObjects)
                    {
                        if (ThisDataObject.Value == ContentType.FILE)
                        {
                            if (!CheckMaxDataSize(ThisDataObject.Key, DType))
                            {
                                UserModel.IsTeleporting = false;

                                FinalNotify("The data you want to teleport exceed the maximum size per file!", 3000);

                                return;
                            }
                        }
                    }
                }
            }

            var ObjectData = new Dictionary<Dictionary<string, Dictionary<string, ContentType>>, DataType> { { WorkerDataObjects, DType } };
            var ObjectArgument = new Dictionary<TransferMode, Dictionary<Dictionary<string, Dictionary<string, ContentType>>, DataType>>();

            string Action = null;

            if (!DoNotShowNotifications)
            {
                if (TMode == TransferMode.UP)
                    Action = "Sending";
                else
                    Action = "Fetching";

                LastNotificationMessage = "Teleporting in progress... [" + Action + "]";

                Notify(LastNotificationMessage);
            }

            ObjectArgument.Add(TMode, ObjectData);

            TeleportThread = new Thread(new ParameterizedThreadStart(FTPDelegate));

            TeleportThread.IsBackground = true;
            TeleportThread.SetApartmentState(ApartmentState.STA);
            TeleportThread.Start(ObjectArgument);
        }
    }
}
