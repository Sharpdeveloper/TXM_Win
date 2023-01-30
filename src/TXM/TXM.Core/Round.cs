using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TXM.Core
{
    [Serializable]
    public class Round : ISerializable
    {
        private int version = 1;

        public List<Pairing> Pairings { get; set; }
        public List<Player> Participants { get; set; }
        public string Scenario { get; set; }

        public Round(List<Pairing> pairings, List<Player> participants, string scenario)
        {
            Pairings = pairings;
            Participants = participants;
            Scenario = scenario;
        }

        public Round(SerializationInfo info, StreamingContext context)
        {
            version = (int)info.GetValue("Round_Version", typeof(int));
            if (version == 0)
            {
                Pairings = (List<Pairing>)info.GetValue("Round_Pairings", typeof(List<Pairing>));
                Participants = (List<Player>)info.GetValue("Round_Participants", typeof(List<Player>));
                Scenario = "";
            }
            else if (version == 1)
            {
                Pairings = (List<Pairing>)info.GetValue("Round_Pairings", typeof(List<Pairing>));
                Participants = (List<Player>)info.GetValue("Round_Participants", typeof(List<Player>));
                Scenario = (string)info.GetValue("Round_Scenario", typeof(string));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Round_Version", version, typeof(int));
            info.AddValue("Round_Pairings", Pairings, typeof(List<Pairing>));
            info.AddValue("Round_Participants", Participants, typeof(List<Player>));
            info.AddValue("Round_Scenario", Scenario, typeof(string));
        }
    }
}
