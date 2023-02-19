using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TXM.Core
{
    public interface IPairingDialog
    {
        void SetPairings(ObservableCollection<Pairing> pairings);
        void SetParticipants(ObservableCollection<Player> participants);
        void ShowDialog();
        ObservableCollection<Pairing> GetPairings();
        bool GetDialogResult();
    }
}
