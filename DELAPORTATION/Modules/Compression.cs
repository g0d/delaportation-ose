/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Models;
using static DELAPORTATION.Utilities;

namespace DELAPORTATION
{
    public static class Compression
    {
        // Compress files
        public static string CompressFiles(Dictionary<string, Dictionary<string, ContentType>> DataEntries, DataType DType)
        {
            var FileHash = MD5.Create();
            var DataObject = ConvertObjectToByteArray(Guid.NewGuid().ToString() + DateTime.Now.ToString());
            var ZipID = BitConverter.ToString(FileHash.ComputeHash(DataObject)).Replace("-", "").ToLower() + ".dpz";
            var FinalZipName = TempFilesDir + ZipID;

            try
            {
                var TempZipDir = Guid.NewGuid().ToString().Replace("-", "").ToLower() + ".dpf";

                Directory.CreateDirectory(TempZipDir);
                ZipFile.CreateFromDirectory(TempZipDir, FinalZipName, CompressionLevel.Optimal, true);
                
                if (!File.Exists(FinalZipName))
                    return null;

                Directory.Delete(TempZipDir);

                using (var ExistingZip = new FileStream(FinalZipName, FileMode.Open))
                {
                    using (var NewZipArchive = new ZipArchive(ExistingZip, ZipArchiveMode.Update))
                    {
                        var ExistingEntry = NewZipArchive.GetEntry(TempZipDir + @"\");

                        if (ExistingEntry != null)
                            ExistingEntry.Delete();
                    }
                }

                using (var ExistingZip = new FileStream(FinalZipName, FileMode.Open))
                {
                    using (var NewZipArchive = new ZipArchive(ExistingZip, ZipArchiveMode.Update))
                    {
                        foreach (var DataEntry in DataEntries)
                        {
                            var ThisFilename = DataEntry.Key;
                            var ThisDataObject = DataEntry.Value;
                            var ThisFilePath = ThisDataObject.First().Key;
                            var ThisFileInfo = new FileInfo(ThisFilePath);

                            if (ThisFileInfo.Attributes == FileAttributes.Directory ||
                                ThisFileInfo.Attributes == (FileAttributes.Directory | FileAttributes.Archive))
                                continue;

                            var NewEntry = NewZipArchive.CreateEntry(ThisFilename);

                            using (var NewEntryStream = NewEntry.Open())
                            {
                                byte[] FileData = null;

                                FileData = ConvertFileToByteArray(ThisFilePath);

                                using (var NewMemoryStream = new MemoryStream(FileData))
                                    NewMemoryStream.CopyTo(NewEntryStream);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (File.Exists(FinalZipName))
                    File.Delete(FinalZipName);

                Console.WriteLine(e.Message);

                return null;
            }

            return ZipID;
        }

        // Decompress files
        public static bool DecompressFiles(string ZipID)
        {
            try
            {
                ZipFile.ExtractToDirectory(TempFilesDir + ZipID, TempFilesDir + ZipID.Substring(0, ZipID.Length - 4));

                File.Delete(TempFilesDir + ZipID);
            }
            catch (Exception e)
            {
                if (File.Exists(TempFilesDir + ZipID))
                    File.Delete(TempFilesDir + ZipID);

                Console.WriteLine(e.Message);

                return false;
            }

            return true;
        }
    }
}
