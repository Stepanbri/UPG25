using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.IO;

namespace Cv07
{
    public partial class MainWindow : Window
    {
        //relativni cesta k datum - zmente dle potreby v zavislosti na
        //tom, odkud spoustite vytvorenou binarku
        const string DATA_ROOT = "../../../data";
        const string BG_IMAGE = "jungle.jpg";
        const string FG_IMAGE = "girl.jpg";
        const string OUT_IMAGE = "vystup.jpg";

        public MainWindow()
        {
            InitializeComponent();

            //nacteme obrazky
            //NOTE: spravne bychom meli jeste osetrit moznost, ze se 
            //obrazky nepovede nacist, abychom zabranili padu cele aplikace

            //TODO: nacti "data/jungle.jpg" jako obrazek pozadi do drawingPanel

            this.drawingPanel.BackgroundImage = new Bitmap(Path.Combine(DATA_ROOT, BG_IMAGE));
        }

        private void BttnExit_Click(object? sender, RoutedEventArgs e)
        { 
            //uzavreni okna
            this.Close();
        }

        private void BttnSave_Click(object? sender, RoutedEventArgs e)
        {
            //ulozeni obrazku
            //NOTE: spravne bychom meli jeste osetrit moznost, ze se 
            //obrazek nepovede ulozit, abychom zabranili padu cele aplikace
            this.drawingPanel.SaveImage(Path.Combine(DATA_ROOT, OUT_IMAGE));
        }
    }
}