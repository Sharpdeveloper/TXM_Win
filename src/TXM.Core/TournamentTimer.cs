using System;
using System.Timers;

namespace TXM.Core
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public class TournamentTimer
    {
        private Timer timer;
        private int hour, min, sec;
        private int defaultTime;
        public int DefaultTime {
            get
            {
                return defaultTime;
            }
            set
            {
                defaultTime = value;
                if(!Started)
                {
                    SetTime();
                    AktZeit();

                    if (Changed != null)
                    {
                        Changed(this, new EventArgs());
                    }
                }
            }
        }
        private string currentTime;
        public bool Started { get; private set; }

        public event ChangedEventHandler Changed;

        public TournamentTimer()
        {
            DefaultTime = 60;
            Started = false;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
        }

        public void StartTimer()
        {
            SetTime();
            AktZeit();
            Start();
        }

        private void SetTime()
        {
            min = DefaultTime;
            if (min > 60)
            {
                hour = min / 60;
                min = min % 60;
            }
            else
                hour = 0;
            sec = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (sec == 0)
            {
                if (min == 0)
                {
                    if (hour == 0)
                        Stop();
                    else
                    {
                        hour--;
                        min = 59;
                        sec = 59;
                    }
                }  
                else
                {
                    min--;
                    sec = 59;
                }
            }
            else
                sec--;

            AktZeit();

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        public string ActualTime
        {
            get
            {
                return currentTime;
            }
        }

        private void AktZeit()
        {
            if(hour > 0)
                currentTime = hour.ToString("D2") + ":" + min.ToString("D2") + ":" + sec.ToString("D2");
            else
                currentTime = min.ToString("D2") + ":" + sec.ToString("D2");
        }

        public void PauseTimer()
        {
            if (Started)
                Stop();
            else
                Start();
        }

        public void ResetTimer()
        {
            Stop();
            SetTime();
            AktZeit();
            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void Stop()
        {
            timer.Stop();
            Started = false;
        }

        private void Start()
        {
            timer.Start();
            Started = true;
        }
    }
}
