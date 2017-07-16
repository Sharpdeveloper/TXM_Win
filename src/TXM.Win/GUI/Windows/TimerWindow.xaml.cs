using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TXM.Core;

namespace TXM.GUI.Windows
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window, ITimerWindow
    {
        private TournamentTimer timer;
        private bool whiteText = false;
        private IO io;

        public TimerWindow()
        {
            whiteText = io.GetColor();
            
            bool whiteTextTemp = whiteText;
            InitializeComponent();
            whiteText = whiteTextTemp;

            string imgurl = io.GetImagePath();
            if(imgurl != "" && imgurl != null)
            {
                io.CopyImage();
                SetImage();
            }

            if (whiteText)
                SliderText.Value = 1.0;
            else
                SliderText.Value = 2.0;
            ChangeLabelColor();
            LabelTime.Content = "MM:SS";
        }

        public void SetTimer(TournamentTimer t)
        {
            timer = t;
            t.Changed += TimeChanged;
        }

        public void SetIO(IO _io)
        {
            io = _io;
        }

        private void TimeChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(AktZeit));
        }

        private void AktZeit()
        {
            LabelTime.Content = timer.ActualTime;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            timer.StartTimer();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            timer.PauseTimer();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            timer.ResetTimer();
        }

        private void SetImage_Click(object sender, RoutedEventArgs e)
        {
            io.NewImage();
            SetImage();
        }

        private void Minutes_Changed(object sender, TextChangedEventArgs e)
        {
            int t;
            try
            {
                t = Int32.Parse(TextBoxMinutes.Text);
            }
            catch
            {
                t = 60;
            }
            timer.DefaultTime = t;
        }

        private void Slider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            whiteText = SliderText.Value == 1.0;
            io.WriteColor(whiteText);
            ChangeLabelColor();
        }

        private void ChangeLabelColor()
        {
            if (whiteText)
            {
                LabelMinutes.Foreground = new SolidColorBrush(Colors.White);
                LabelTime.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                LabelMinutes.Foreground = new SolidColorBrush(Colors.Black);
                LabelTime.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void SetImage()
        {
            try
            {
                BackGroundImage.Source = new BitmapImage(new Uri(io.TempImgPath));
            }
            catch (Exception)
            {
                io.ShowMessage("The Image " + io.TempImgPath + " is invalid.");
            }
        }

        private new void Show()
        {
            base.Show();
        }
    }
}
