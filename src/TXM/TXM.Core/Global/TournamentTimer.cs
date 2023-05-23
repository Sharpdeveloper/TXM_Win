using System.Timers;

using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core;

public delegate void ChangedEventHandler(object sender, EventArgs e);

public partial class TournamentTimer : ObservableObject
{
    private System.Timers.Timer timer;
    private int hour, min, sec;
    private int defaultTime;
    private bool startTimeReached = true;
    public int StartHour = 0, StartMinute = 0;
    public bool IsTimerRandom { get; set; }
    public int RandomMins { get; set; }

    private static TournamentTimer _instance = new TournamentTimer();
    public static TournamentTimer GetInstance() => _instance;

    public int DefaultTime
    {
        get { return defaultTime; }
        set
        {
            defaultTime = value;
            if (!Started)
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

    [ObservableProperty]
    private string _currentTime;
    public bool Started { get; private set; }

    public event ChangedEventHandler Changed;

    private TournamentTimer()
    {
        DefaultTime = 60;
        Started = false;

        timer = new System.Timers.Timer();
        timer.Interval = 1000;
        timer.Elapsed += Timer_Tick;
    }

    public void StartTimer()
    {
        SetTime();
        AktZeit();
        Start();
    }

    private void SetTime(bool reset = false)
    {
        min = DefaultTime;
        if (IsTimerRandom && !reset)
        {
            Random r = new Random();
            if (r.Next(0, 2) == 0)
                min += r.Next(0, RandomMins + 1);
            else
                min -= r.Next(0, RandomMins + 1);
        }

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
        if (!startTimeReached)
        {
            if (DateTime.Now.Hour >= StartHour && DateTime.Now.Minute >= StartMinute)
            {
                startTimeReached = true;
            }
            else
            {
                return;
            }
        }

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
    }
    
    private void AktZeit()
    {
        if (hour > 0)
            CurrentTime = hour.ToString("D2") + ":" + min.ToString("D2") + ":" + sec.ToString("D2");
        else
            CurrentTime = min.ToString("D2") + ":" + sec.ToString("D2");

        if (Changed != null)
        {
            Changed(this, new EventArgs());
        }
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
        SetTime(true);
        AktZeit();
    }

    private void Stop()
    {
        timer.Stop();
        Started = false;
        startTimeReached = true;
    }

    private void Start()
    {
        if (StartHour != 0 && StartMinute != 0)
        {
            startTimeReached = false;
        }

        Started = true;
        AktZeit();
        timer.Start();
    }
}