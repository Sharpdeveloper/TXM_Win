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

            InitializeComponent();
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

            whiteText = io.GetColor();

            bool whiteTextTemp = whiteText;
            
            whiteText = whiteTextTemp;

            string imgurl = io.GetImagePath();
            if (imgurl != "" && imgurl != null)
            {
                io.CopyImage();
                SetImage();
            }

            ChangeLabelColor();
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

        private void ChangeLabelColor()
        {
            if (whiteText)
            {
                LabelTime.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
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
