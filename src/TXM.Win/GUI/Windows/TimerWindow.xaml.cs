using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Reflection;

using TXM.Core;

namespace TXM.GUI.Windows
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window
    {
        private Timer timer;
        private int min, sec, Minute = 75;
        public bool Started { get; private set; }
        private bool whiteText = false;
        public IO Io { get; set; }
        private string emptyTime, pauseText, continueText, imageText, invalidText;

        public event ChangedEventHandler Changed;

        public TimerWindow(IO io, Language lang)
        {
            Io = io;

            whiteText = io.GetColor();
            
            bool whiteTextTemp = whiteText;
            InitializeComponent();
            whiteText = whiteTextTemp;
            Started = false;

            TextBoxMinutes.Text = Minute.ToString();
                    
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);

            string imgurl = Io.GetImagePath();
            if(imgurl != "" && imgurl != null)
            {
                Io.CopyImage();
                SetImage();
            }

            if (whiteText)
                SliderText.Value = 1.0;
            else
                SliderText.Value = 2.0;
            ChangeLabelColor();
            SetLanguage(lang);
            LabelTime.Content = emptyTime;
        }

        public void SetLanguage(Language lang)
        {
            this.Title = lang.GetTranslation(StaticLanguage.Timer); 
            ButtonStart.Content = lang.GetTranslation(StaticLanguage.StartTime);
            ButtonPause.Content = lang.GetTranslation(StaticLanguage.PauseTime);
            ButtonReset.Content = lang.GetTranslation(StaticLanguage.Reset);
            ButtonSetImage.Content = lang.GetTranslation(StaticLanguage.SetImage);
            LabelMinutes.Content = lang.GetTranslation(StaticLanguage.Minutes);
            LabelBlackText.Content = lang.GetTranslation(StaticLanguage.BlackText);
            LabelWhiteText.Content = lang.GetTranslation(StaticLanguage.WhiteText);
            emptyTime = lang.GetTranslation(StaticLanguage.EmptyHour) + ":" + lang.GetTranslation(StaticLanguage.EmptyMinute);
            pauseText = lang.GetTranslation(StaticLanguage.PauseTime);
            continueText = lang.GetTranslation(StaticLanguage.ContinueTime);
            imageText = lang.GetTranslation(StaticLanguage.TheImage);
            invalidText = lang.GetTranslation(StaticLanguage.IsInvalid);
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartTimer();
        }

        public void StartTimer()
        {
            min = Minute;
            sec = 0;
            aktZeit();
            start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (sec == 0)
            {
                if (min == 0)
                    stop();
                else
                {
                    min--;
                    sec = 59;
                }
            }
            else
                sec--;

            Dispatcher.Invoke(new Action(aktZeit));

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        public string ActualTime
        {
            get
            {
                return LabelTime.Content.ToString();
            }
        }

        private void aktZeit()
        {
            LabelTime.Content = min.ToString("D2") + ":" + sec.ToString("D2");
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            PauseTimer();
        }

        public void PauseTimer()
        {
            if (Started)
                stop();
            else
                start();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer();
        }

        public void ResetTimer()
        {
            stop();
            min = Minute;
            sec = 0;
            aktZeit();
            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void stop()
        {
            timer.Stop();
            //ButtonPause.Content = continueText;
            Started = false;
        }

        private void start()
        {
            timer.Start();
            //ButtonPause.Content = pauseText;
            Started = true;
        }

        private void SetImage_Click(object sender, RoutedEventArgs e)
        {
            Io.NewImage();
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
                t = 75;
            }
            Minute = t;
        }

        private void Slider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            whiteText = SliderText.Value == 1.0;
            Io.WriteColor(whiteText);
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
                BackGroundImage.Source = new BitmapImage(new Uri(Io.TempImgPath));
            }
            catch (Exception e)
            {
                Io.ShowMessage(imageText + " " + Io.TempImgPath + " " + invalidText + ".");
            }
        }
    }
}
