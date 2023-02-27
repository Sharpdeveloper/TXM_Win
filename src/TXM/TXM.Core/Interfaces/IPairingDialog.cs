using System.Collections.Generic;
using System.Collections.ObjectModel;

using TXM.Core.Models;

namespace TXM.Core
{
    public interface IPairingDialog
    {
        void SetPairings(ObservableCollection<Pairing> pairings);
        void SetParticipants(ObservableCollection<Models.Player> participants);
        void ShowDialog();
        ObservableCollection<Pairing> GetPairings();
        bool GetDialogResult();
    }
}
