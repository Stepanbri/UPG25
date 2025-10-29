using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Cv06
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Moznost cislo 2
            this.KeyDown += (_, e) =>
            {
                if (e.Key == Key.R)
                {
                    this.drawingPanel.MakeStarDefault();
                    e.Handled = true;
                }
            };
        }

        private void BttnExit_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
        }

        private void BttnExit_Click(object? sender, RoutedEventArgs e)
        {
            //uzavreni okna
            this.Close();
        }

        private void BttnSmaller_Click(object? sender, RoutedEventArgs e)
        {
            this.drawingPanel.MakeStarSmaller();
        }

        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.R)
            {
                this.drawingPanel.MakeStarDefault();
                e.Handled = true;
            }
            else if (e.Key == Key.S)
            {
                this.drawingPanel.RotateStar(15);
                e.Handled = true;
            }
        } 

        /*
        // Moznost cislo 1
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.R)
            {
                this.drawingPanel.MakeStarDefault();
                e.Handled = true;
            }
        }
        */
    }
}