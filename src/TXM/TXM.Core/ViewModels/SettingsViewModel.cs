using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TXM.Core.Global;
using TXM.Core.Models;

namespace TXM.Core.ViewModels;

public partial class SettingsViewModel : ObservableValidator
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _language;

    [ObservableProperty]
    private string _textColor;

    [ObservableProperty]
    private double _textSize;

    [ObservableProperty]
    private bool _isTimerRandom;

    [ObservableProperty]
    private int _time;

    [ObservableProperty]
    private Texts _text = State.Text;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(SettingsViewModel), nameof(ValidateStartTime))]
    private string _startTime = "00:00";
    
    public int StartingHour { get; set; }
    public int StartingMinute { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageName))]
    private string? _bgImagePath;
    
    public char PathSeparator { get; set; }

    public string ImageName => _bgImagePath?[(_bgImagePath.LastIndexOf(PathSeparator) + 1)..] ?? "";

        [ObservableProperty]
    private int _randomMinutes;

    public List<string> Languages
    {
        get => LanguageData?.Select(x => x.FileName).ToList() ?? Enumerable.Empty<string>().ToList();
    }

    public List<LocalFile>? LanguageData { get; set; }

    public List<string> TextColors { get; set; }

    [RelayCommand]
    private void SelectImageFile()
    {
        var image = State.Io.NewImage();
        if (image.NewImage)
        {
            BgImagePath = image.Path;
        }
    }

    public static ValidationResult ValidateStartTime(string sTime, ValidationContext context)
    {
        var instance = (SettingsViewModel)context.ObjectInstance;
        var pos = sTime.IndexOf(':');
        var check = sTime.LastIndexOf(':');
        var startHour = 0;
        var startMin = 0;
        var isValid = pos == check;
        if (pos is >= 3 or 0)
        {
            isValid = false;
        }

        if ((pos == 1 && sTime.Length >= 5) || sTime.Length >= 6)
        {
            isValid = false;
        }

        try
        {
            startHour = int.Parse(sTime[..pos]);
        }
        catch (Exception)
        {
            isValid = false;
        }

        try
        {
            startMin = int.Parse(sTime[(pos + 1)..]);
        }
        catch (Exception)
        {
            isValid = false;
        }

        if (isValid)
        {
            if (startHour is > 23 or < 0)
            {
                State.Io.ShowMessage(State.Text.InvalidHour);
                return new(State.Text.InvalidHour);
            }

            instance.StartingHour = startHour;

            if (startMin is > 59 or < 0)
            {
                State.Io.ShowMessage(State.Text.InvalidMinute);
                return new(State.Text.InvalidMinute);
            }

            instance.StartingMinute = startMin;
        }
        else
        {
            State.Io.ShowMessage(State.Text.StartTimeFormat);
            return new(State.Text.StartTimeFormat);
        }

        return ValidationResult.Success;
    }
}