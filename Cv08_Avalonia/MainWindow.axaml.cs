using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;

namespace Cv08
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var startTime = Environment.TickCount;
            var timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(500), //kazdych 50 ms
            };
            timer.Tick += (_, _) =>
            {
                //pocet sekund, ktere uplynuly
                float elapsed = (Environment.TickCount - startTime) / 1000.0f;

                //TODO: animovat automaticky jednotlive rezy             
                const int fps = 20; // 20 rezu za sekundu
                int index_rezu = (int)(elapsed * fps); 

                this.drawingPanel.Z = index_rezu % this.drawingPanel.ZMax;

                this.InvalidateVisual();
            };

            timer.Start();
        }

        private void BttnExit_Click(object? sender, RoutedEventArgs e)
        {
            //uzavreni okna
            this.Close();
        }
    }
}