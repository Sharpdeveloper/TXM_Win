using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core.ViewModels;

public partial class ThanksViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _text;
}