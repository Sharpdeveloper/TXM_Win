using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Global;

namespace TXM.Core.ViewModels;

public partial class OutputViewModel : ObservableObject
{
    [ObservableProperty]
    private Texts _text = State.Text;
    
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isPairingOutput;

    [ObservableProperty]
    private bool _isResultOutput;

    [ObservableProperty]
    private bool _isTableOutput;
}