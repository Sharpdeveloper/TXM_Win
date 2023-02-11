using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TXM.Core
{
    [Serializable]
    public class Round : ISerializable
    {
        private int version = 2;


        public int RoundNo { get; set; }
        public List<Pairing> Pairings { get; set; }
        public List<Player> Participants { get; set; }
        public string Scenario { get; set; }

        public Round(int roundNo, List<Pairing> pairings, List<Player> participants, string scenario)
        {
            RoundNo = roundNo;
            Pairings = pairings;
            Participants = participants;
            Scenario = scenario;
        }

        public Round(SerializationInfo info, StreamingContext context)
        {
            int _version = (int)info.GetValue("Round_Version", typeof(int));
            if (version == 0)
            {
                Pairings = (List<Pairing>)info.GetValue("Round_Pairings", typeof(List<Pairing>));
                Participants = (List<Player>)info.GetValue("Round_Participants", typeof(List<Player>));
                Scenario = "";
                RoundNo = -1;
            }
            else if (_version == 1)
            {
                Pairings = (List<Pairing>)info.GetValue("Round_Pairings", typeof(List<Pairing>));
                Participants = (List<Player>)info.GetValue("Round_Participants", typeof(List<Player>));
                Scenario = (string)info.GetValue("Round_Scenario", typeof(string));
                RoundNo = -1;
            }
            else if (_version == 2)
            {
                RoundNo = (int)info.GetValue("Round_RoundNo", typeof(int));
                Pairings = (List<Pairing>)info.GetValue("Round_Pairings", typeof(List<Pairing>));
                Participants = (List<Player>)info.GetValue("Round_Participants", typeof(List<Player>));
                Scenario = (string)info.GetValue("Round_Scenario", typeof(string));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Round_Version", version, typeof(int));
            info.AddValue("Round_RoundNo", RoundNo, typeof(int));
            info.AddValue("Round_Pairings", Pairings, typeof(List<Pairing>));
            info.AddValue("Round_Participants", Participants, typeof(List<Player>));
            info.AddValue("Round_Scenario", Scenario, typeof(string));
        }
    }
}
