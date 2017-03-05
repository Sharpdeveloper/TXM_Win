using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    [Serializable]
    public class Pairing
    {
        #region Static Fields
        private static int tableNr = 0;
        #endregion

        #region Properties
        public int TableNr { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public char Winner { get; set; }
        public bool ResultEdited { get; set; }
        #endregion

        #region Constructor
        public Pairing()
        {
            Winner = 'A';
            TableNr = ++tableNr;
            ResultEdited = false;
        }

        public Pairing(int tableNr)
        {
            Winner = 'A';
            TableNr = tableNr;
            ResultEdited = false;
        }

        public Pairing(Pairing p)
        {
            this.TableNr = p.TableNr;
            this.Player1 = p.Player1;
            this.Player2 = p.Player2;
            this.Player1Score = p.Player1Score;
            this.Player2Score = p.Player2Score;
            this.ResultEdited = p.ResultEdited;
            this.Winner = p.Winner;
        }
        #endregion

        #region Indirect Properties
        public string Player1Name
        {
            get
            {
                return Player1.DisplayName;
            }
        }

        public string Player2Name
        {
            get
            {
                return Player2.DisplayName;
            }
        }
        #endregion

        #region Static Functions
        public static void ResetTableNr()
        {
            tableNr = 0;
        }
        #endregion


        public void GetTableNr()
        {
            TableNr = ++tableNr;
        }
    }
}
