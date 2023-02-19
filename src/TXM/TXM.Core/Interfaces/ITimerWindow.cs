namespace TXM.Core
{
    public interface ITimerWindow
    {
        void Show();
        void SetTimer(TournamentTimer t);
        bool SetImage(System.Uri path);
        void SetLabelColor(string color);
        void SetTextSize(double size);
        void Quit();
    }
}
