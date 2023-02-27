namespace TXM.Core
{
    public interface ITournamentDialog
    {
        Logic.Tournament GetTournament();
        bool GetDialogResult();
        bool IsChanged();
        void ShowDialog();
        void SetIO(IO io);
        void SetTournament(Logic.Tournament tournament);
        void SetGameSystemIsChangeable(bool isGametypeChangeable);
    }
}
