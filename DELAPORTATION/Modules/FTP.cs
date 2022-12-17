/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Models;
using static DELAPORTATION.Compression;
using static DELAPORTATION.Teleportation;
using static DELAPORTATION.Notifications;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public static class FTP
    {
        private static string FTPStorageURL = "ftp://delaportation.eu/httpdocs/delaported/";

        // Factory for web requests
        public static FtpWebRequest SetupFTPRequest(string Filename, FTPCommands FTPCom, TransferMode TMode)
        {
            var URI = new Uri(FTPStorageURL + UserModel.UserFolderID + "/files/" + Filename);
            var NewWebRequest = (FtpWebRequest)WebRequest.Create(URI);

            NewWebRequest.KeepAlive = false;
            NewWebRequest.Timeout = -1;
            NewWebRequest.UseBinary = true;
            NewWebRequest.EnableSsl = true;
            NewWebRequest.ClientCertificates = GenerateCertificate();
            NewWebRequest.Credentials = new NetworkCredential(WebServiceModel.Username, WebServiceModel.Password);

            if (FTPCom == FTPCommands.NOP)
            {
                if (TMode == TransferMode.UP)
                    NewWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                else
                    NewWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            }
            else
                NewWebRequest.Method = FTPCom.ToString();

            return NewWebRequest;
        }

        // FTP multi-purpose delegate (FTP) [Copy/Paste]
        public static void FTPDelegate(object Data)
        {
            var ActualData = (Dictionary<TransferMode, Dictionary<Dictionary<string, Dictionary<string, ContentType>>, DataType>>)Data;
            var TMode = ActualData.First().Key;
            var ObjectData = ActualData.First().Value;
            var DataEntries = ObjectData.First().Key;
            var DType = ObjectData.First().Value;
            var ZipFilesList = new Dictionary<string, Dictionary<string, string>>();

            object Result = null;
            string ZipID = null;
            bool TransferResult = false;

            UserModel.TeleportTransferMode = TMode;
            UserModel.ObjectDataType = DType;
            
            if (TMode == TransferMode.UP)
            {
                ZipID = CompressFiles(DataEntries, DType);

                if (ZipID == null)
                {
                    ResetTeleportationStatus();

                    FinalNotify("A file error has occured.\nPlease try again!", 3000);

                    return;
                }

                TransferResult = FTPTransferFileUp(ZipID);
            }
            else
            {
                ZipID = DataEntries.First().Key;

                TransferResult = FTPTransferFileDown(ZipID);
            }

            if (!TransferResult)
            {
                if (File.Exists(TempFilesDir + ZipID))
                    File.Delete(TempFilesDir + ZipID);

                ResetTeleportationStatus();

                FinalNotify("An error has occured during the teleportation!", 3000);

                return;
            }

            if (TMode == TransferMode.UP)
            {
                var FilesList = new Dictionary<string, string>();

                File.Delete(TempFilesDir + ZipID);

                foreach (var DataEntry in DataEntries)
                {
                    var FixedFilePath = DataEntry.Key;
                    var FilePathsContentType = DataEntry.Value.Select(All => All.Value).First().ToString();

                    FilesList.Add(FixedFilePath, FilePathsContentType);
                }

                ZipFilesList.Add(ZipID, FilesList);
                
                Result = UpdateRemoteFilesLists(ZipFilesList, DType);

                if (!(bool)Result)
                {
                    ResetTeleportationStatus();

                    FinalNotify("A service error has occured during the teleportation.\nPlease try again!", 3000);

                    return;
                }

                Result = (string)SetNewDataCheck("1");

                if ((string)Result != "OK")
                {
                    ResetTeleportationStatus();

                    FinalNotify("A service error has occured during the teleportation.\nPlease try again!", 3000);

                    return;
                }
            }
            else if (TMode == TransferMode.DOWN)
            {
                Result = DecompressFiles(ZipID);

                if ((bool)Result == false)
                {
                    ResetTeleportationStatus();

                    FinalNotify("A file error has occured!", 3000);

                    return;
                }

                Result = (string)SetNewDataCheck("0");

                if ((string)Result != "OK")
                {
                    ResetTeleportationStatus();

                    FinalNotify("A service error has occured during the teleportation!", 3000);

                    return;
                }

                ClipboardSetData(DataEntries);
            }

            ResetTeleportationStatus();

            FinalNotify("Data have been successfully teleported!");
        }

        // Upload file (FTP) [Copy]
        private static bool FTPTransferFileUp(string Filename)
        {
            try
            {
                var ThisWebRequest = SetupFTPRequest(Filename, FTPCommands.NOP, TransferMode.UP);
                var NewStream = ThisWebRequest.GetRequestStream();
                var FileData = ConvertFileToByteArray(TempFilesDir + Filename);
                
                WebResponse Response = null;
                
                NewStream.Write(FileData, 0, FileData.Length);
                NewStream.Flush();
                NewStream.Close();
                NewStream.Dispose();

                Response = ThisWebRequest.GetResponse();

                Response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }

            return true;
        }

        // Download file (FTP) [Paste]
        private static bool FTPTransferFileDown(string Filename)
        {
            try
            {
                var FileDir = TempFilesDir + Filename;
                var ThisWebRequest = SetupFTPRequest(Filename, FTPCommands.NOP, TransferMode.DOWN);
                var Response = ThisWebRequest.GetResponse();

                using (var ResponseStream = Response.GetResponseStream())
                {
                    using (var NewFileStream = new FileStream(FileDir, FileMode.Create, FileAccess.Write))
                    {
                        ResponseStream.CopyTo(NewFileStream);

                        if (IsOverMaxFileSize(NewFileStream.Length))
                        {
                            FinalNotify("The data you want to teleport exceed the maximum size per file!", 3000);

                            NewFileStream.Close();

                            ResponseStream.Close();

                            Response.Close();

                            return false;
                        }
                    }
                }

                Response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }

            return true;
        }

        // List all FTP directories & files
        private static List<string> FTPListFilesDirs(string FilePath)
        {
            var AllFilesDirsList = FTPRecursiveListFilesDirs(FilePath, FilePath);

            return AllFilesDirsList;
        }

        // List FTP directories & files recursively
        private static List<string> FTPRecursiveListFilesDirs(string Origin, string FilePath)
        {
            var ThisWebRequest = SetupFTPRequest(FilePath, FTPCommands.NLST, TransferMode.UP);
            var FilesDirsList = new List<string>();

            //if (FilePath.Contains(".D0P"))
            //{
            //    FTPDeleteFile(Origin + "/" + FilePath);

            //    return FilesDirsList;
            //}

            FilesDirsList.Add(FilePath);

            var Response = ThisWebRequest.GetResponse();
            var NewStream = Response.GetResponseStream();
            var NewStreamReader = new StreamReader(NewStream);

            while (!NewStreamReader.EndOfStream)
            {
                var NewFilesDirs = FTPRecursiveListFilesDirs(Origin, NewStreamReader.ReadLine());

                foreach (var Record in NewFilesDirs)
                    FilesDirsList.Add(Origin + "/" + Record);
            }

            NewStreamReader.Close();
            Response.Close();

            return FilesDirsList;
        }
    }
}
