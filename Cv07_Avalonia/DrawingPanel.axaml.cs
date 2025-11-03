using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cv07;

public partial class DrawingPanel : UserControl
{
    private Bitmap? _finalImage;

    /// <summary>
    /// Gets the final image to be displayed on the drawing panel.
    /// </summary>
    /// <value>
    /// The final image.
    /// </value>
    public Bitmap FinalImage
    {
        get
        {
            if (_finalImage == null)
            {
                //vytvor vysledny obrazek
                _finalImage = CreateFinalImage(
                    this.TransformedBackgroundImage,
                    this.ForegroundImage, this.Alpha);
            }
            return _finalImage;
        }
    }

    private Bitmap? _transformedBackgroundImage;
    /// <summary>
    /// Gets the transformed background.
    /// </summary>
    /// <value>
    /// The transformed background.
    /// </value>
    public Bitmap? TransformedBackgroundImage
    {
        get
        {
            if (this._transformedBackgroundImage == null)
            {
                this._transformedBackgroundImage = CreateTransformedBackgroundImage(this.BackgroundImage);
            }

            return this._transformedBackgroundImage;
        }
    }


    private Bitmap? _backgroundImage;

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public Bitmap? BackgroundImage
    {
        get => _backgroundImage;
        set
        {
            if (_backgroundImage != value)
            {
                _backgroundImage = value;
                _transformedBackgroundImage = null;
                _finalImage = null;

                this.InvalidateVisual(); //redraw
            }
        }
    }


    private Bitmap? _foregroundImage;

    /// <summary>
    /// Gets or sets the foreground image.
    /// </summary>
    /// <value>
    /// The foreground image.
    /// </value>
    public Bitmap? ForegroundImage
    {
        get => _foregroundImage; set
        {
            if (_foregroundImage != value)
            {
                _foregroundImage = value;
                _finalImage = null;
                this.InvalidateVisual();
            }
        }
    }

    private float _alpha = 1f;

    /// <summary>
    /// Gets or sets the alpha used to blend foreground and background images.
    /// </summary>
    /// <value>
    /// The alpha must be in range 0 (transparent) - 1 (opaque).
    /// </value>
    public float Alpha
    {
        get => _alpha;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentException("Alpha must be from the range <0-1>");
            }


            _alpha = value;
            _finalImage = null;
            this.InvalidateVisual();
        }
    }

    public DrawingPanel()
    {
        InitializeComponent();                      
    }

     /// <summary>
    /// Renders the visual to a <see cref="T:Avalonia.Media.DrawingContext" />.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public override void Render(DrawingContext context)
    {
        DrawScene(context, this.Bounds.Width, this.Bounds.Height);
    }

    /// <summary>
    /// Draws the scene containing the final image.
    /// </summary>
    /// <param name="context">The graphics context used to draw.</param>
    /// <param name="W">The width.</param>
    /// <param name="H">The height.</param>
    private void DrawScene(DrawingContext context, double W, double H)
    {
        context.FillRectangle(Brushes.DarkGray, new Rect(0, 0, W, H));

        //vykreslime obrazek v rozliseni 1:1
        var img = this.FinalImage;

        double scale_x = W / img.Size.Width;
        double scale_y = H / img.Size.Height;
        double scale = Math.Min(scale_x, scale_y);

        //context.DrawImage(img, new Rect(0, 0, img.Size.Width, img.Size.Height));
        //context.DrawImage(img, new Rect(0, 0, W, H));

        //TODO: zobraz obrazek roztazeny na velikost okna
        //se zachovanim pomeru stran
        //context.DrawImage(img, new Rect(0, 0, img.Size.Width * scale, img.Size.Height * scale));

        //TODO: SAMOSTATNE zobraz vycentrovane
        double scaledWidth = img.Size.Width * scale;
        double scaledHeight = img.Size.Height * scale;
        double left = (Bounds.Width - scaledWidth) / 2; //(místo dělení je lepší použít nasobní 0.5)
        double top = (Bounds.Height - scaledHeight) / 2;
        context.DrawImage(img, new Rect(left, top, scaledWidth, scaledHeight));
    }

    /// <summary>
    /// Creates the transformed version of the image.
    /// </summary>
    /// <param name="img">The original image.</param>
    /// <returns>The transformed version of the image.</returns>        
    private Bitmap? CreateTransformedBackgroundImage(Bitmap? img)
    {
        //TODO: vrat novy obrazek stejne velikosti
        //s pixely sedotonovymi -> prace s polem, shiftem
        if (img == null)
            return null;

        var out_img = new WriteableBitmap(img.PixelSize, img.Dpi, PixelFormats.Rgb24);

        //TODO: nakresli do obrazku pozadi cerveny kruh
        //relativni pozice jeho stredu v obrazku: 467/1000.0 a 122/689.0 
        //polomer: 1/50 sirky obrazk

        return out_img;
    }

    /// <summary>
    /// Merges the foreground and background image layers.
    /// </summary>
    /// <param name="bkgnd">The background image.</param>
    /// <param name="fgnd">The foreground image.</param>
    /// <param name="alpha">The alpha used to blend images.</param>
    /// <returns>
    /// Final image
    /// </returns>
    private Bitmap CreateFinalImage(Bitmap? bkgnd, Bitmap? fgnd, float alpha)
    {
        //pokud nemame nastavene pozadi, vrat popredi prip. dummy obrazek
        if (bkgnd == null)
        {
            if (fgnd != null)
                return fgnd;

            var img = new WriteableBitmap(
                new PixelSize(100, 100),    //100x100 px
                new Vector(96, 96),         //96 ppi pro vypocet fyzicke velikosti
                PixelFormats.Rgb24
                );
           
            using (var fb = img.Lock())
            {
                int totalBytes = fb.RowBytes * fb.Size.Height;
                byte[] buffer = new byte[totalBytes];
                Marshal.Copy(buffer, 0, fb.Address, totalBytes);
            }

            return img;
        }

        if (fgnd == null)
        {
            return bkgnd;
        }

        //mame TransformedBackgroundImage i ForegroundImage

        //TODO: zkombinuj TransformedBackgroundImage a ForegroundImage
        //s vyuzitim alpha pro vysledek
        return bkgnd;
    }

    /// <summary>
    /// Saves the image.
    /// </summary>
    /// <param name="path">The full path (including the filename and extension).</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void SaveImage(string path)
    {
        //TODO: uloz vysledny obrazek do souboru

        //TODO: Samostatne: uloz obrazek v rozliseni 4000xXXX px

        throw new NotImplementedException();
    }
}