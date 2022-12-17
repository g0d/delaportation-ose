/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using static DELAPORTATION.Globals;
using static DELAPORTATION.Models;
using static DELAPORTATION.RegistryControl;
using static DELAPORTATION.WebServices;
using static DELAPORTATION.Teleportation;
using static DELAPORTATION.Files;
using static DELAPORTATION.Notifications;

namespace DELAPORTATION
{
    public static class Utilities
    {
        #region Initialization

        // Initialize cryptographic support
        public static void InitializeCrypto()
        {
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
        }

        // Initialize "DELAPORTATION" key registry and values
        public static void InitializeRegistryKey()
        {
            if (CheckRegisteredKey() == false)
                RegisterKey();

            TeleportationKeyRegistryFix();
        }

        // Initialize the web service model
        public static void InitializeWebServiceModel()
        {
            ServicePointManager.Expect100Continue = false;

            WebServiceModel.Username = "delaportation";
            WebServiceModel.Password = "8M29kf6!w";
        }

        // Initialize the user model for client <---> server communication
        public static void InitializeUserModel()
        {
            LoadSettingsFromRegistry();
        }

        #endregion


        #region Main methods


        #region Public

        // Windows keyboard catcher [BETA]
        public class WinKeyCatcher : NativeWindow, IDisposable
        {
            [DllImport("user32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RegisterHotKey(IntPtr hwnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32", SetLastError = true)]
            public static extern int UnregisterHotKey(IntPtr hwnd, int id);

            [DllImport("kernel32", SetLastError = true)]
            private static extern short GlobalAddAtom(string lpString);

            [DllImport("kernel32", SetLastError = true)]
            private static extern short GlobalDeleteAtom(short nAtom);

            public const int MOD_ALT = 1;
            public const int MOD_CONTROL = 2;
            public const int MOD_SHIFT = 4;
            public const int MOD_WIN = 8;
            public const int WM_HOTKEY = 0x312;

            private readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

            public WinKeyCatcher()
            {
                CreateHandle(new CreateParams() { Parent = HWND_MESSAGE });
            }

            ~WinKeyCatcher()
            {
                Dispose();
            }

            public short HotKeyID { get; private set; }

            public void RegisterGlobalHotKey(int hotkey, int modifiers)
            {
                UnregisterGlobalHotKey();

                try
                {
                    string atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + GetType().FullName;

                    HotKeyID = GlobalAddAtom(atomName);

                    if (HotKeyID == 0)
                        throw new Exception(Marshal.GetLastWin32Error().ToString());

                    if (!RegisterHotKey(Handle, HotKeyID, (uint)modifiers, (uint)hotkey))
                        throw new Exception(Marshal.GetLastWin32Error().ToString());
                }
                catch (Exception e)
                {
                    Dispose();

                    Console.WriteLine(e);
                }
            }

            public void UnregisterGlobalHotKey()
            {
                if (HotKeyID != 0)
                {
                    UnregisterHotKey(Handle, HotKeyID);

                    GlobalDeleteAtom(HotKeyID);

                    HotKeyID = 0;
                }
            }

            public void Dispose()
            {
                UnregisterGlobalHotKey();
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_HOTKEY:


                        break;
                    default:
                        m.Result = IntPtr.Zero;

                        break;
                }

                base.WndProc(ref m);
            }
        }

        // Clipboard monitor
        public class ClipboardMonitor : NativeWindow, IDisposable
        {
            [DllImport("user32.dll")]
            private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

            [DllImport("user32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RegisterHotKey(IntPtr hwnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32", SetLastError = true)]
            public static extern int UnregisterHotKey(IntPtr hwnd, int id);

            [DllImport("kernel32", SetLastError = true)]
            private static extern short GlobalAddAtom(string lpString);

            [DllImport("kernel32", SetLastError = true)]
            private static extern short GlobalDeleteAtom(short nAtom);

            public const int MOD_ALT = 1;
            public const int MOD_CONTROL = 2;
            public const int MOD_SHIFT = 4;
            public const int MOD_WIN = 8;
            public const int WM_HOTKEY = 0x312;

            private readonly IntPtr HWND_MESSAGE = new IntPtr(-3);
            private bool Disposed = false;
            private bool CtrlXPressed = false;
            private IntPtr NextClipboardWindow;
            public event EventHandler ClipboardCopyEvent;

            public short HotKeyID { get; private set; }

            public ClipboardMonitor()
            {
                CreateHandle(new CreateParams() { Parent = HWND_MESSAGE });

                NextClipboardWindow = SetClipboardViewer(Handle);

                RegisterGlobalHotKey((int)Keys.X, WinKeyCatcher.MOD_CONTROL);
            }

            ~ClipboardMonitor()
            {
                Dispose(false);
            }

            public bool IsCtrlX()
            {
                return CtrlXPressed;
            }

            public void RegisterGlobalHotKey(int hotkey, int modifiers)
            {
                UnregisterGlobalHotKey();

                try
                {
                    string atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + GetType().FullName;

                    HotKeyID = GlobalAddAtom(atomName);

                    if (HotKeyID == 0)
                        throw new Exception(Marshal.GetLastWin32Error().ToString());

                    if (!RegisterHotKey(Handle, HotKeyID, (uint)modifiers, (uint)hotkey))
                        throw new Exception(Marshal.GetLastWin32Error().ToString());
                }
                catch (Exception e)
                {
                    Dispose();

                    Console.WriteLine(e);
                }
            }

            public void UnregisterGlobalHotKey()
            {
                if (HotKeyID != 0)
                {
                    UnregisterHotKey(Handle, HotKeyID);

                    GlobalDeleteAtom(HotKeyID);

                    HotKeyID = 0;
                }
            }

            private void Dispose(bool Disposing)
            {
                if (!Disposed)
                {
                    Disposed = true;

                    ChangeClipboardChain(Handle, NextClipboardWindow);
                }
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            protected virtual void OnClipboardCopyEvent(EventArgs e)
            {
                ClipboardCopyEvent?.Invoke(this, e);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY && (short)m.WParam == HotKeyID)
                    CtrlXPressed = true;
                else
                    CtrlXPressed = false;

                switch ((WindowsMessages)m.Msg)
                {
                    case WindowsMessages.DRAWCLIPBOARD:
                        if (NextClipboardWindow != null)
                            SendMessage(NextClipboardWindow, (uint)m.Msg, m.WParam, m.LParam);

                        OnClipboardCopyEvent(new EventArgs());

                        break;
                    case WindowsMessages.CHANGECBCHAIN:
                        if (m.LParam != Handle)
                        {
                            if (m.LParam == NextClipboardWindow)
                                NextClipboardWindow = m.LParam;
                            else
                                SendMessage(NextClipboardWindow, (uint)m.Msg, m.WParam, m.LParam);
                        }

                        m.Result = IntPtr.Zero;

                        break;
                }

                base.WndProc(ref m);
            }
        }

        // Application routing (Logic flow)
        public static void AppRouting()
        {
            if (UserModel.IsAuthenticated == false)
                new AuthWin().Show();
            else
            {
                if (RoutingParams.Length == 0)
                {
                    InfoWinForm = new InfoWin();

                    InfoWinForm.Opacity = 0;

                    InfoWinForm.Show();
                    InfoWinForm.Hide();

                    InfoWinForm.Opacity = 100;
                }
                else
                {
                    if (RoutingParams[0] == "/tt")
                    {
                        TeleportToMode = true;

                        new TeleportWin().Show();
                    }
                }
            }
        }

        // Copy trap delegate
        public static void CopyTrapDelegate(object sender, EventArgs e)
        {
            if (ClipboardMonitorAction.IsCtrlX())
                return;

            if (!UserModel.IsTeleporting)
                ClipboardGetData();
            else
            {
                if (UserModel.TeleportTransferMode == TransferMode.UP)
                    return;
                
                Notify("Incoming teleported data. Please wait...");

                HideNotification(2500);
            }
        }

        // Paste trap delegate for periodical checks (Data arrival event)
        public static void PasteTrapCheckDelegate(object sender, EventArgs e)
        {
            dynamic IncomingData = JObject.FromObject(CheckForNewData());

            if (IncomingData["UID"] == UserModel.UID)
                return;

            if (IncomingData["Status"] != "1")
                return;

            if (UserModel.IsTeleporting)
                return;

            var RemoteData = FetchRemoteRecentFilesList();

            if (RemoteData == null)
                return;

            UserModel.HasIncomingData = true;

            var JSONFilesList = JObject.FromObject(RemoteData);
            var FilesList = JSONFilesList.ToObject<Dictionary<string, Dictionary<string, string>>>();
            var ListData = new Dictionary<string, Dictionary<string, ContentType>>();
            var DataObject = new Dictionary<string, ContentType>();
            var DType = DataType.FILE;
            var ZipID = FilesList.First().Key;

            foreach (var Record in FilesList.First().Value)
            {
                ContentType CType;
                
                if (Record.Value == ContentType.DIR.ToString())
                    CType = ContentType.DIR;
                else
                {
                    if (Record.Key.Contains("dp_"))
                    {
                        if (Record.Key.Contains("image"))
                            DType = DataType.IMAGE;
                        else if (Record.Key.Contains("audio"))
                            DType = DataType.AUDIO;
                        else
                            DType = DataType.TEXT;

                        CType = ContentType.MEMORY;
                    }
                    else
                        CType = ContentType.FILE;
                }
                
                DataObject.Add(Record.Key, CType);
            }
            
            ListData.Add(ZipID, DataObject);

            TeleportFiles(ListData, DType, TransferMode.DOWN);
        }

        // Set data to clipboard that have been teleported from another computer to you (Paste)
        public static void ClipboardSetData(Dictionary<string, Dictionary<string, ContentType>> PasteData)
        {
            ClipboardMonitorAction.ClipboardCopyEvent -= new EventHandler(CopyTrapDelegate);

            Thread.Sleep(100);

            try
            {
                var UnZippedDir = PasteData.Keys.First().Substring(0, PasteData.Keys.First().Length - 4) + @"\";
                var ContentType = PasteData.Values.First().Values.First();

                if (ContentType == ContentType.MEMORY)
                {
                    var FileName = TempFilesDir + UnZippedDir + PasteData.Values.First().Keys.First();

                    if (UserModel.ObjectDataType == DataType.IMAGE)
                    {
                        var NewImage = Image.FromFile(FileName);

                        Clipboard.SetImage(NewImage);
                    }
                    else if (UserModel.ObjectDataType == DataType.AUDIO)
                    {
                        using (var NewFileStream = new FileStream(FileName, FileMode.Open))
                        {
                            using (var NewMemoryStream = new MemoryStream())
                            {
                                NewFileStream.CopyTo(NewMemoryStream);

                                Clipboard.SetAudio(NewMemoryStream.ToArray());
                            }
                        }
                    }
                    else if (UserModel.ObjectDataType == DataType.TEXT)
                    {
                        var ClipboardData = File.ReadAllText(FileName);

                        if (ClipboardData.Length != 0)
                            Clipboard.SetText(ClipboardData);
                    }
                }
                else
                {
                    var Directories = Directory.EnumerateDirectories(TempFilesDir + UnZippedDir).ToArray();
                    var Files = Directory.EnumerateFiles(TempFilesDir + UnZippedDir).ToArray();
                    var FilePaths = Directories.Concat(Files).ToArray();
                    
                    StringCollection FilePathsCollection = new StringCollection();

                    FilePathsCollection.AddRange(FilePaths);

                    Clipboard.SetFileDropList(FilePathsCollection);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Thread.Sleep(100);

            ClipboardMonitorAction.ClipboardCopyEvent += new EventHandler(CopyTrapDelegate);
        }
        
        // Update new data check status in the user's profile
        public static object SetNewDataCheck(string Status)
        {
            var ParamsData = new Dictionary<string, object>();

            DataStatusModel NewData;

            NewData.UID = UserModel.UID;
            NewData.Status = Status;

            ParamsData.Add("hash", UserModel.Hash);
            ParamsData.Add("check", "0");
            ParamsData.Add("status_model", NewData);

            var Params = new Dictionary<string, Dictionary<string, object>>();

            Params.Add("data", ParamsData);

            var Results = (WebResponseModel)WebService(WebServiceModel.URL, Params);

            if (Results.Status == (int)ResponseStatusCodes.ERROR)
                return null;

            return Results.Data;
        }

        // Update all files lists in the user's profile
        public static bool UpdateRemoteFilesLists(Dictionary<string, Dictionary<string, string>> ZipFilesList, DataType DType)
        {
            var ParamsData = new Dictionary<string, object>();

            ParamsData.Add("hash", UserModel.Hash);
            ParamsData.Add("type", DType.ToString());
            ParamsData.Add("mode", TransferMode.UP.ToString());
            ParamsData.Add("files", ZipFilesList);

            var Params = new Dictionary<string, Dictionary<string, object>>();

            Params.Add("data", ParamsData);

            var Results = (WebResponseModel)WebService(WebServiceModel.URL, Params);

            if (Results.Status == (int)ResponseStatusCodes.ERROR)
                return false;

            return true;
        }
        
        // Delete remote file paths (if exist)
        public static bool DeleteRemoteFilePaths(string[] FilePaths)
        {
            var ParamsData = new Dictionary<string, object>();

            Array.Reverse(FilePaths);

            ParamsData.Add("hash", UserModel.Hash);
            ParamsData.Add("operation", DataOperations.DELETE.ToString());
            ParamsData.Add("files", FilePaths);

            var Params = new Dictionary<string, Dictionary<string, object>>();

            Params.Add("data", ParamsData);

            var Results = (WebResponseModel)WebService(WebServiceModel.URL, Params);

            if (Results.Status == ResponseStatusCodes.ERROR)
                return false;

            return true;
        }

        // Check maximum data size
        public static bool CheckMaxDataSize(object Data, DataType DType)
        {
            long DataLength = 0;

            if (DType == DataType.FILE)
                DataLength = new FileInfo((string)Data).Length;
            else if (DType == DataType.IMAGE)
                DataLength = ((MemoryStream)Data).Length;
            else if (DType == DataType.AUDIO)
                DataLength = ((MemoryStream)Data).Length;
            else if (DType == DataType.TEXT)
                DataLength = Encoding.UTF8.GetBytes((string)Data).Length;

            if (IsOverMaxFileSize(DataLength))
                return false;

            return true;
        }

        // Check if given size surpasses the global max file size
        public static bool IsOverMaxFileSize(long FileSize)
        {
            if (FileSize > UserModel.MaxFileSize)
                return true;

            return false;
        }

        // Clean special characters from filename
        public static string EscapeSpecialCharacters(string Filename)
        {
            if (Filename.Contains("#"))
                return Filename.Replace("#", "%23");

            if (Filename.IndexOf(".") == 0)
                return Filename.Replace(".", "[.]");

            if (Filename.IndexOf("[.]") == 0)
                return Filename.Replace("[.]", ".");

            return Filename;
        }

        // Delete all temporary files in local cache
        public static void DeleteTempFiles()
        {
            try
            {
                Directory.Delete(TempFilesDir, true);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);

                return;
            }
        }

        // Convert object into a byte array
        public static byte[] ConvertObjectToByteArray(object AnyObject)
        {
            if (AnyObject == null)
                return null;

            var NewBinaryFormater = new BinaryFormatter();
            var NewMemoryStream = new MemoryStream();

            NewBinaryFormater.Serialize(NewMemoryStream, AnyObject);

            return NewMemoryStream.ToArray();
        }

        // Convert a byte array into object
        public static object ConvertByteArrayToObject(byte[] BytesArray)
        {
            if (BytesArray == null)
                return null;

            var NewMemoryStream = new MemoryStream();
            var NewBinaryFormater = new BinaryFormatter();

            NewMemoryStream.Write(BytesArray, 0, BytesArray.Length);
            NewMemoryStream.Seek(0, SeekOrigin.Begin);

            return NewBinaryFormater.Deserialize(NewMemoryStream);
        }

        // Convert a file into a byte array
        public static byte[] ConvertFileToByteArray(string Filename)
        {
            if (Filename == null)
                return null;

            var BytesArray = File.ReadAllBytes(Filename);

            return BytesArray;
        }

        // X509 Certificate generation (SSL)
        public static X509CertificateCollection GenerateCertificate()
        {
            var NewCert = X509Certificate.CreateFromCertFile(Environment.CurrentDirectory + @"\DPCert.crt");
            var CertsCollection = new X509CertificateCollection();

            CertsCollection.Add(NewCert);

            return CertsCollection;
        }
        
        #endregion


        #region Private
        
        // Fetch data from clipboard and teleport them from your computer to others (Copy)
        private static void ClipboardGetData()
        {
            var Data = new Dictionary<string, Dictionary<string, ContentType>>();
            var DataObject = new Dictionary<string, ContentType>();

            Notify("Prepairing for teleportation...");

            try
            {
                if (Clipboard.ContainsFileDropList())
                {
                    var FilePaths = Clipboard.GetFileDropList();
                    
                    Data = ScanFilePaths(FilePaths, true);

                    if (Data == null)
                        return;
                    
                    if (Data.Count > 1)
                        UserModel.HasMultipleFilesToTeleport = true;

                    TeleportFiles(Data, DataType.FILE, TransferMode.UP);
                }
                else if (Clipboard.ContainsImage())
                {
                    var NewMemoryStream = new MemoryStream();
                    var Image = Clipboard.GetImage();
                    var ImageName = "dp_image.jpg";
                    
                    Image.Save(NewMemoryStream, ImageFormat.Bmp);

                    if (NewMemoryStream.Length > 1048576)
                    {
                        var JPGEncoder = ImageCodecInfo.GetImageEncoders().First(Codec => Codec.FormatID == ImageFormat.Jpeg.Guid);
                        var NewEncoder = System.Drawing.Imaging.Encoder.Quality;
                        var NewEncoderParameters = new EncoderParameters(1);
                        var ThisEncoderParameter = new EncoderParameter(NewEncoder, 85L);

                        NewMemoryStream = new MemoryStream();

                        NewEncoderParameters.Param[0] = ThisEncoderParameter;

                        Image.Save(NewMemoryStream, JPGEncoder, NewEncoderParameters);
                    }

                    using (var NewFileStream = new FileStream(TempFilesDir + ImageName, FileMode.Create, FileAccess.Write))
                        NewMemoryStream.WriteTo(NewFileStream);
                    
                    DataObject.Add(TempFilesDir + ImageName, ContentType.MEMORY);
                    Data.Add(ImageName, DataObject);

                    TeleportFiles(Data, DataType.IMAGE, TransferMode.UP);
                }
                else if (Clipboard.ContainsAudio())
                {
                    var AudioName = "dp_audio.wav";

                    using (var NewFileStream = new FileStream(TempFilesDir + AudioName, FileMode.Create, FileAccess.Write))
                        using (var NewMemoryStream = Clipboard.GetAudioStream())
                        NewMemoryStream.CopyTo(NewFileStream);
                    
                    DataObject.Add(TempFilesDir + AudioName, ContentType.MEMORY);
                    Data.Add(AudioName, DataObject);

                    TeleportFiles(Data, DataType.AUDIO, TransferMode.UP);
                }
                else if (Clipboard.ContainsText())
                {
                    var Text = Clipboard.GetText();
                    var NewMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Text));
                    
                    using (var NewFileStream = new FileStream(TempFilesDir + "dp_raw", FileMode.Create, FileAccess.Write))
                        NewMemoryStream.WriteTo(NewFileStream);

                    DataObject.Add(TempFilesDir + "dp_raw", ContentType.MEMORY);
                    Data.Add("dp_raw", DataObject);

                    TeleportFiles(Data, DataType.TEXT, TransferMode.UP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Check for new data (changes or files) in the user's profile
        private static object CheckForNewData()
        {
            var ParamsData = new Dictionary<string, object>();

            ParamsData.Add("hash", UserModel.Hash);
            ParamsData.Add("check", "1");

            var Params = new Dictionary<string, Dictionary<string, object>>();

            Params.Add("data", ParamsData);

            var Results = (WebResponseModel)WebService(WebServiceModel.URL, Params);

            if (Results.Status == (int)ResponseStatusCodes.ERROR)
                return null;

            return Results.Data;
        }
        
        // Fetch recent files list from the user's profile
        private static object FetchRemoteRecentFilesList()
        {
            var ParamsData = new Dictionary<string, object>();

            ParamsData.Add("hash", UserModel.Hash);
            ParamsData.Add("type", DataType.FILE.ToString());
            ParamsData.Add("mode", TransferMode.DOWN.ToString());

            var Params = new Dictionary<string, Dictionary<string, object>>();

            Params.Add("data", ParamsData);

            var Results = (WebResponseModel)WebService(WebServiceModel.URL, Params);

            if (Results.Status == (int)ResponseStatusCodes.ERROR)
                return null;

            return Results.Data;
        }

        // Update / fix registration key for teleportation option in the system
        private static void TeleportationKeyRegistryFix()
        {
            var FixRegistryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION\command");

            FixRegistryKey.SetValue("", Application.ExecutablePath + " /tt");
            FixRegistryKey.Close();
        }
        
        // Enable SSL certificate in all cases
        private static bool ServerCertificateValidationCallback(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SSLPolicyErrors)
        {
            bool AllowCertificate = true;

            if (SSLPolicyErrors != SslPolicyErrors.None)
            {
                Console.WriteLine("Accepting the certificate with errors:");

                if ((SSLPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    Console.WriteLine("\tThe certificate subject {0} does not match.", Certificate.Subject);
                }

                if ((SSLPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    Console.WriteLine("\tThe certificate chain has the following errors:");
                    foreach (X509ChainStatus chainStatus in Chain.ChainStatus)
                    {
                        Console.WriteLine("\t\t{0}", chainStatus.StatusInformation);

                        if (chainStatus.Status == X509ChainStatusFlags.Revoked)
                        {
                            AllowCertificate = false;
                        }
                    }
                }

                if ((SSLPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) == SslPolicyErrors.RemoteCertificateNotAvailable)
                {
                    AllowCertificate = false;

                    Console.WriteLine("No certificate available.");
                }

                Console.WriteLine();
            }

            return AllowCertificate;
        }

        #endregion


        #endregion
    }
}
