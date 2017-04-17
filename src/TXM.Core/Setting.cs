using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    public sealed class Settings
    {
        #region Constants
        private const string fileversion = "V1.0.0";
        private const string fileextension = ".txmb2";
        private static string[] oldextensions = new string[] {".txmb"};
        private const string squadfilecards = "https://raw.githubusercontent.com/geordanr/xwing/master/coffeescripts/cards-common.coffee";
        private const string squadfiletranslation = "https://raw.githubusercontent.com/geordanr/xwing/master/coffeescripts/cards-de.coffee";
        private const string languagesFile = "http://apps.piratesoftatooine.de/Languages/Languages.txt";
        private const string fileextensionName = "X-Wing Turnier";
        private const string binfile = "data.txmstats";
        #endregion

        #region Properties
        public static string FILEVERSION
        {
            get
            {
                return fileversion;
            }
        }

        public static string LANGUAGELIST
        {
            get
            {
                return languagesFile;
            }
        }

        public static string FILEEXTENSION
        {
            get
            {
                return fileextension;
            }
        }

        public static string[] OLDEXTENSIONS
        {
            get
            {
                return oldextensions;
            }
        }

        public static string SQUADFILECARDS
        {
            get
            {
                return squadfilecards;
            }
        }

        public static string SQUADFILETRANSLATION
        {
            get
            {
                return squadfiletranslation;
            }
        }

        public static string FILEEXTENSIONSNAME
        {
            get
            {
                return fileextensionName;
            }
        }

        public static string BINFILE
        {
            get
            {
                return binfile;
            }
        }
        #endregion
    }
}