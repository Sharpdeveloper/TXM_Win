using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    [Serializable]
    public class Round2
    {
        public List<Pairing> Pairings { get; set; }
        public List<Player> Participants { get; set; }

        public Round2(List<Pairing> pairings, List<Player> participants)
        {
            Pairings = pairings;
            Participants = participants;
        }
    }
}
