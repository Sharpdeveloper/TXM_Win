using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    [Serializable]
    public class Round
    {
        public Pairing[] Pairings { get; set; }
        public List<Player> Participants { get; set; }

        public Round(Pairing[] pairings, List<Player> participants)
        {
            Pairings = pairings;
            Participants = participants;
        }
    }
}
