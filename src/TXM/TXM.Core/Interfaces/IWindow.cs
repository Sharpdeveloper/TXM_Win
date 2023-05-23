namespace TXM.Core.Interfaces;

public interface IWindow
{
    object DataContext { get; set; }
    void Close();
}