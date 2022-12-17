/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

namespace DELAPORTATION
{
    public static class Models
    {
        // User model class
        public static class UserModel
        {
            public static string UID = null;
            public static string Email = null;
            public static string Hash = null;
            public static string UserFolderID = null;
            public static long MaxFileSize = 0;
            public static bool IsAuthenticated = false;
            public static bool IsTeleporting = false;
            public static bool HasIncomingData = false;
            public static bool HasMultipleFilesToTeleport = false;
            public static TransferMode TeleportTransferMode;
            public static DataType ObjectDataType;
        }

        // Web app model class
        public static class WebServiceModel
        {
            public static string URL = "https://delaportation.eu/";             /* USE FOR LOCAL TESTING: "https://localhost:8080/" */
            public static string Username = null;
            public static string Password = null;
        }

        // Web reponse model
        public struct WebResponseModel
        {
            public bool Auth;
            public ResponseStatusCodes Status;
            public object Data;
        }

        // Data status model
        public struct DataStatusModel
        {
            public string UID;
            public string Status;
        }

        // Respnse status codes
        public enum ResponseStatusCodes
        {
            OK = 1,
            ERROR = 0
        };

        // Transfer mode
        public enum TransferMode
        {
            UP,
            DOWN
        };

        // Data type
        public enum DataType
        {
            FILE,
            IMAGE,
            AUDIO,
            TEXT
        };

        // Data operations
        public enum DataOperations
        {
            MOVE,
            DELETE
        }

        // Content type
        public enum ContentType
        {
            DIR,
            FILE,
            MEMORY
        }

        // FTP commands
        public enum FTPCommands
        {
            MKD,
            RMD,
            NLST,
            DELE,
            NOP
        }

        // Windows messages codes
        public enum WindowsMessages : int
        {
            DRAWCLIPBOARD = 0x308,
            CHANGECBCHAIN = 0x30d
        }
    }
}
