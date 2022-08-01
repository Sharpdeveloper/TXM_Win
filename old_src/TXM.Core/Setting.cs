﻿namespace TXM.Core
{
    public sealed class Settings
    {
        #region Constants
        private const string fileextension = "txmtournament";
        private const string fileextensionName = "TXM Tournaments";
        private const string txmversion = "V2.8.1";
        private const string copyrightYear = "2014 - 2022";
        #endregion

        #region Properties
        public static string FILEEXTENSION
        {
            get
            {
                return fileextension;
            }
        }

        public static string TXMVERSION
        {
            get
            {
                return txmversion;
            }
        }

        public static string FILEEXTENSIONSNAME
        {
            get
            {
                return fileextensionName;
            }
        }

        public static string COPYRIGHTYEAR
        {
            get
            {
                return copyrightYear;
            }
        }
        #endregion
    }
}