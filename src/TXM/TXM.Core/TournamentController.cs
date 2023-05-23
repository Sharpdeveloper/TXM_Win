using System.Diagnostics;
using System.Text;

using TXM.Core.Global;
using TXM.Core.Interfaces;
using TXM.Core.Models;

namespace TXM.Core
{
    public class TournamentController
    {
        public Logic.Tournament ActiveTournament { get; private set; }
        private bool firststart = false;
        private ITimerWindow timerWindow;
        public TournamentTimer ActiveTimer { get; private set; }
        public InputOutput ActiveIO { get; private set; }
        public bool Started { get; private set; }
        private IWindow projectorWindow;

        public string GetRandomTime()
        {
            if(ActiveTournament != null && ActiveTournament.Rule != null && ActiveTournament.Rule.DefaultRandomMins != 0)
            {
                ActiveTimer.IsTimerRandom = true;
                if (ActiveTimer.RandomMins != 0)
                    return ActiveTimer.RandomMins.ToString();
                //else
                  //  return SetRandomTime(ActiveTournament.Rule.DefaultRandomMins.ToString());
            }
            ActiveTimer.IsTimerRandom = false;
            return "";
        }

        public void Close()
        {

        }
    }
}
