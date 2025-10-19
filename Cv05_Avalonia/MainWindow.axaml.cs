using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Cv05
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnExitClick(object? sender, RoutedEventArgs e)
        {
            // Close this window (and thus exit the app)
            this.Close();
        }
    }
}