using System.Windows;

using TXM.Core;

namespace TXM.GUI.Windows
{
    /// <summary>
    /// Interaction logic for BeamerWindow.xaml
    /// </summary>
    public partial class ProjectorWindow : Window, IProjectorWindow
    {
        private bool closed = false;

        public ProjectorWindow()
        {
            InitializeComponent();
        }

        public new void Show()
        {
            base.Show();
        }

        public void SetURL(string path)
        {
            wbBrowser.Navigate(path);
        }

        public void SetTitle(string title)
        {
            this.Title = title;
        }

        public bool IsClosed()
        {
            return closed;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            closed = true;

            base.OnClosing(e);
        }
    }
}
