using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TXM.Core.Global;

namespace TXM.Core.ViewModels;

public partial class SupportViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;
    
    [ObservableProperty]
    private string _supportText;

    [ObservableProperty]
    private string _docuText;
    
    [ObservableProperty]
    private string _nextText;
    
    [ObservableProperty]
    private string _langText;
    
    [ObservableProperty]
    private string _lastText;
    
    [ObservableProperty]
    private string _donateText;

    [RelayCommand]
    private void OpenNewGameDocu() => State.Controller.OpenNewGameDocu();
    
    [RelayCommand]
    private void OpenNewLangDocu() => State.Controller.OpenNewLangDocu();
    
    [RelayCommand]
    private void OpenDonate() => State.Controller.OpenDonate();
}