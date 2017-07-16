using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TXM.Core
{
    [Serializable]
    public class Pairing : ISerializable
    {
        #region Static Fields
        private static int tableNr = 0;
        private int version = 0;
        #endregion

        #region Properties
        public int TableNr { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public string Winner { get; set; }
        public bool ResultEdited { get; set; }
        #endregion

        #region Constructor
        public Pairing()
        {
            Winner = "Automatic";
            TableNr = ++tableNr;
            ResultEdited = false;
        }

        public Pairing(int tableNr)
        {
            Winner = "Automatic";
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

		public Pairing(SerializationInfo info, StreamingContext context)
		{
			version = (int)info.GetValue("Pairing_Version", typeof(int));
			if (version == 0)
			{
				tableNr = (int)info.GetValue("Pairing_tableNr", typeof(int));
                TableNr = (int)info.GetValue("Pairing_TableNr", typeof(int));
                Player1 = (Player)info.GetValue("Pairing_Player1", typeof(Player));
                Player2 = (Player)info.GetValue("Pairing_Player2", typeof(Player));
                Player1Score = (int)info.GetValue("Pairing_Player1Score", typeof(int));
                Player2Score = (int)info.GetValue("Pairing_Player2Score", typeof(int));
                Winner = (string)info.GetValue("Pairing_Winner", typeof(string));
                ResultEdited = (bool)info.GetValue("Pairing_ResultEdited", typeof(bool));
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Pairing_Version", version, typeof(int));
			info.AddValue("Pairing_tableNr", tableNr, typeof(int));
			info.AddValue("Pairing_TableNr", TableNr, typeof(int));
			info.AddValue("Pairing_Player1", Player1, typeof(Player));
			info.AddValue("Pairing_Player2", Player2, typeof(Player));
			info.AddValue("Pairing_Player1Score", Player1Score, typeof(int));
			info.AddValue("Pairing_Player2Score", Player2Score, typeof(int));
			info.AddValue("Pairing_Winner", Winner, typeof(string));
			info.AddValue("Pairing_ResultEdited", ResultEdited, typeof(bool));
		}
    }
}
