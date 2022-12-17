/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using static DELAPORTATION.Models;
using static DELAPORTATION.Notifications;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public static class Files
    {
        // Scan filepaths and return data of files and/or folders
        public static Dictionary<string, Dictionary<string, ContentType>> ScanFilePaths(StringCollection FilePaths, bool StopOnMaxFileSize = false)
        {
            var ScanData = new Dictionary<string, Dictionary<string, ContentType>>();

            foreach (var FilePath in FilePaths)
            {
                var ThisFilePathData = ScanDirsRecursively(FilePath, null, StopOnMaxFileSize);

                foreach (var PathData in ThisFilePathData)
                    ScanData.Add(PathData.Key, PathData.Value);
            }

            return ScanData;
        }

        // Scan dirs recursively
        private static Dictionary<string, Dictionary<string, ContentType>> ScanDirsRecursively(string FilePath, string RelativeDir = null, bool StopOnMaxFileSize = false)
        {
            var ScanData = new Dictionary<string, Dictionary<string, ContentType>>();

            try
            {
                var Filename = Path.GetFileName(FilePath);
                var FileAttributes = File.GetAttributes(FilePath);

                if (RelativeDir != null)
                    Filename = RelativeDir + @"\" + Filename;

                if (FileAttributes.HasFlag(FileAttributes.Directory))
                {
                    var NewFolder = new Dictionary<string, ContentType>();

                    NewFolder.Add(FilePath, ContentType.DIR);

                    ScanData.Add(Filename, NewFolder);

                    foreach (var FileRelativePath in Directory.GetFiles(FilePath, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        var ThisRelativeFilename = Path.GetFileName(FileRelativePath);
                        var NewFile = new Dictionary<string, ContentType>();

                        NewFile.Add(FileRelativePath, ContentType.FILE);

                        if (StopOnMaxFileSize)
                        {
                            if (!CheckMaxDataSize(FileRelativePath, DataType.FILE))
                            {
                                FinalNotify("The data you want to teleport exceed the maximum size per file!", 3000);

                                return null;
                            }
                        }

                        ScanData.Add(Filename + @"\" + ThisRelativeFilename, NewFile);
                    }

                    foreach (var SubDirPath in Directory.GetDirectories(FilePath, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        var RecurseScanData = ScanDirsRecursively(SubDirPath, Filename);

                        foreach (var Data in RecurseScanData)
                            ScanData.Add(Data.Key, Data.Value);
                    }
                }
                else
                {
                    var NewFile = new Dictionary<string, ContentType>();

                    if (StopOnMaxFileSize)
                    {
                        if (!CheckMaxDataSize(FilePath, DataType.FILE))
                        {
                            FinalNotify("The data you want to teleport exceed the maximum size per file!", 3000);

                            return null;
                        }
                    }

                    NewFile.Add(FilePath, ContentType.FILE);

                    ScanData.Add(Filename, NewFile);
                }
            }
            catch (Exception e)
            {
                FinalNotify("Oops... Some data were unable to be processed.\nPlease try again!", 3000);

                Console.WriteLine(e.Message);

                return null;
            }

            return ScanData;
        }
    }
}
