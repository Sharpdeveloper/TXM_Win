namespace TXM.Core
{
    public interface ITimerWindow
    {
        void Show();
        void SetTimer(TournamentTimer t);
        void SetIO(IO io);
    }
}
