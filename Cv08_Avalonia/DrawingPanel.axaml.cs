using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cv08;

public partial class DrawingPanel : UserControl
{
    private const string DATA_FILENAME = "../../../data/mrt8_angio2.raw";
    protected const int DATA_SZ = 128;    //pocet rezu (obrazku)
    protected const int DATA_SY = 320;    //pocet radek v kazdem rezu
    protected const int DATA_SX = 256;    //pocet hodnot v kazdem radku

    private byte[]? _data;

    /// <summary>
    /// Gets the test volumetric data.
    /// </summary>
    /// <value>
    /// The test data stored slice by slice, row by row.
    /// </value>
    protected byte[] Data
    {
        get
        {
            if (_data == null)
            {
                //nacti data
                _data = new byte[DATA_SX * DATA_SY * DATA_SZ];
                using (FileStream sr = new FileStream(DATA_FILENAME, FileMode.Open))
                {
                    sr.ReadExactly(_data, 0, _data.Length);
                }
            }

            return _data;
        }
    }

    private Bitmap? _slice_XY;   //rez kolmy na osu z (prochazejici Z)
    private Bitmap? _slice_XZ;   //rez kolmy na osu y (prochazejici Y)
    private Bitmap? _slice_YZ;   //rez kolmy na osu x (prochazejici X)

    private int _x;
    private int _y;
    private int _z;

    /// <summary>
    /// Gets the maximum value of Z.
    /// </summary>
    public int ZMax => DATA_SZ - 1;

    /// <summary>
    /// Gets or sets the position of SliceXY.
    /// </summary>
    /// <value>
    /// The z coordinate from 0 to ZMax.
    /// </value>
    public int Z
    {
        get => _z;
        set
        {
            //Contract.Requires<ArgumentException>(value >= 0 && value <= ZMax);

            if (_z != value)
            {
                _slice_XY = null;   //zrus rez
                _z = value;

                this.InvalidateVisual(); //vynut prekresleni
            }
        }
    }

    /// <summary>
    /// Gets the maximum value of Y.
    /// </summary>
    public int YMax => DATA_SY - 1;

    /// <summary>
    /// Gets or sets the position of SliceXZ.
    /// </summary>
    /// <value>
    /// The z coordinate from 0 to YMax.
    /// </value>
    public int Y
    {
        get => _y;
        set
        {
            Contract.Requires<ArgumentException>(value >= 0 && value <= YMax);

            if (_y != value)
            {
                _slice_XZ = null;   //zrus rez
                _y = value;

                this.InvalidateVisual(); //vynut prekresleni
            }
        }
    }

    /// <summary>
    /// Gets the maximum value of X.
    /// </summary>
    public int XMax => DATA_SX - 1;

    /// <summary>
    /// Gets or sets the position of SliceXZ.
    /// </summary>
    /// <value>
    /// The z coordinate from 0 to YMax.
    /// </value>
    public int X
    {
        get => _x;
        set
        {
            Contract.Requires<ArgumentException>(value >= 0 && value <= XMax);

            if (_x != value)
            {
                _slice_YZ = null;   //zrus rez
                _x = value;

                this.InvalidateVisual(); //vynut prekresleni
            }
        }
    }

    /// <summary>
    /// Gets the slice XY constructed in Z.
    /// </summary>
    protected Bitmap SliceXY
    {
        get
        {
            //TODO: nevytvaret vzdy, udelat cachovani

            var img = new WriteableBitmap(
                new PixelSize(DATA_SX, DATA_SY), 
                new Vector(96, 96),         //96 ppi pro vypocet fyzicke velikosti
                PixelFormats.Rgb24
                );

            //cerny obrazek
            using (var img_buf = img.Lock())
            {
                byte[] pixels = new byte[img_buf.RowBytes * img_buf.Size.Height];
                Marshal.Copy(pixels, 0, img_buf.Address, pixels.Length);
            }

            
            //TODO: vytvorit a vratit obrazek z rezem XY (v Z)

            //TODO: pouzit LUT pro urychleni

            return img;
        }
    }

    /// <summary>
    /// Gets the slice XZ constructed in Y.
    /// </summary>
    protected Bitmap SliceXZ
    {
        get
        {
            var img = new WriteableBitmap(
                            new PixelSize(DATA_SX, DATA_SZ),
                            new Vector(96, 96),         //96 ppi pro vypocet fyzicke velikosti
                            PixelFormats.Rgb24
                            );

            //TODO: samostatne

            return img;
        }
    }

    /// <summary>
    /// Gets the slice YZ constructed in X.
    /// </summary>
    protected Bitmap SliceYZ
    {
        get
        {
            var img = new WriteableBitmap(
                            new PixelSize(DATA_SY, DATA_SZ),
                            new Vector(96, 96),         //96 ppi pro vypocet fyzicke velikosti
                            PixelFormats.Rgb24
                            );

            //TODO: samostatne

            return img;
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
        context.FillRectangle(Brushes.White, new Rect(0, 0, this.Bounds.Width, this.Bounds.Height));

        //do leveho horniho rohu zobrazime rez XY
        context.DrawImage(SliceXY, new Rect(0, 0, SliceXY.Size.Width, SliceXY.Size.Height));
      
        //vypis, kde se nachazime
        const float GAP = 16;                
        var text = new FormattedText("z = " + Z, System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, new Typeface("Arial"), 16, Brushes.Black);        
        context.DrawText(text, new Point(GAP, SliceXY.Size.Height + GAP));
    }
}