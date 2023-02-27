using System.Collections.Generic;

using TXM.Core.Models;

namespace TXM.Core
{
    public interface IAutoSaveDialog
    {
        void Init(List<AutoSaveFile> files);
        void ShowDialog();
        bool GetDialogReturn();
        string GetFileName();
    }
}
