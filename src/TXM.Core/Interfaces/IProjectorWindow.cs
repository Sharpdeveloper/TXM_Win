namespace TXM.Core
{
    public interface IProjectorWindow
    {
        void SetURL(string path);
        void Show();
        void SetTitle(string title);
        bool IsClosed();
    }
}
