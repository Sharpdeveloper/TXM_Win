using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Global;

namespace TXM.Core.ViewModels;

public partial class TimerViewModel : ObservableObject
{
    [ObservableProperty]
    private TournamentTimer _timer = State.Timer;

    [ObservableProperty]
    private string _title = "Timer";

    [ObservableProperty]
    private Settings _setting = State.Setting;
}