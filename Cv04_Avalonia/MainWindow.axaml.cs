using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Cv04
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnPrintClick(object? sender, RoutedEventArgs e)
        {
            //TODO: Pridat na formular PrintDocument
            //TODO: Pridat na formular PrintPreviewDialog a napojit ho na PrintDocument
            //TODO: Pridat sem zavolani dialogu PrintPreviewDialog
            //TODO: Pridat do MainForm obsluhu udalosti PrintPage pridaneho PrintDocument
        }

        private void OnExitClick(object? sender, RoutedEventArgs e)
        {
            // Close this window (and thus exit the app)
            this.Close();
        }
    }
}