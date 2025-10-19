using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Globalization;
using System.Linq;

namespace Cv04;

public partial class DrawingPanel : UserControl
{
    private const double buttonSizeX = 200;
    private const double buttonSizeY = 100;
    private const double PACMAN_R = 350;

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
        context.FillRectangle(Brushes.White, this.Bounds);

        using (context.PushTransform(Matrix.CreateTranslation(this.Bounds.Width / 2, this.Bounds.Height / 2)))
        {
            //DrawPacman(PACMAN_R, context);
            DrawButton(buttonSizeX, buttonSizeY, 20, context);
        }
    }

    private void DrawButton(double SizeX, double SizeY, double radius, DrawingContext g)
    {
        double x0 = -(SizeX/2); //lav� 
        double y0 = -(SizeY / 2); //horn�
        double x1 = SizeX / 2; //prav�
        double y1 = SizeY / 2; //doln�

        var linearBrush = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0, y0, RelativeUnit.Absolute),
            EndPoint = new RelativePoint(0, y1, RelativeUnit.Absolute),
            GradientStops = new GradientStops()
            {
                new GradientStop(Colors.LightGray, 0),
                new GradientStop(Color.FromArgb(255,255,0,0) , 0.5),
                new GradientStop(Colors.LightGray, 1)
            }
        };

        g.DrawRectangle(linearBrush, null, new Rect(x0,y0, SizeX, SizeY));

        var orez = new StreamGeometry();
        using (var ctx = orez.Open())
        {
            ctx.BeginFigure(new Point(x0, y0), true);
            ctx.LineTo(new Point(x1, y0));
            ctx.LineTo(new Point(x1, y1));
            ctx.LineTo(new Point(x0, y1));
            ctx.EndFigure(true);
        }

        using (g.PushGeometryClip(orez))
        {
            g.DrawEllipse(Brushes.LightGray, null, new Point(x0, 0), radius, SizeY / 2);
            g.DrawEllipse(Brushes.LightGray, null, new Point(x1, 0), radius, SizeY / 2);
        }

        var text = new FormattedText("TEXT",
               CultureInfo.CurrentCulture,
               FlowDirection.LeftToRight,
               new Typeface("sans-serif", FontStyle.Normal),
               72,
               Brushes.Orange);
        text.SetFontWeight(FontWeight.UltraBold);
        text.TextAlignment = TextAlignment.Center;
        g.DrawText(text, new Point(-(text.Width/2), -(text.Height / 2)));
    }

    /// <summary>Draws the pacman with the center in (0,0) and radius R.</summary>
    /// <param name="R">The radius of the pacman.</param>
    /// <param name="g">The graphics context where to draw.</param>
    private void DrawPacman(double R, DrawingContext g)
    {
        double ecx = 0;
        double ecy = -R * 0.5;
        double ro = R * 0.25;
        double ri = ro * 0.5;

        var eye = new StreamGeometry();
        using (var ctx = eye.Open())
        {
            ctx.BeginFigure(new Point(ecx + ro, ecy), true);
            ctx.LineTo(new Point(ecx, ecy + ro));
            ctx.LineTo(new Point(ecx - ro, ecy));
            ctx.LineTo(new Point(ecx, ecy - ro));
            ctx.EndFigure(true);

            //TODO: samostatne pridej vnitrni kosoctverec (ri)
            //s body definovanymi v obracenem poradi
            ctx.BeginFigure(new Point(ecx + ri, ecy), true);
            ctx.LineTo(new Point(ecx, ecy - ri));
            ctx.LineTo(new Point(ecx - ri, ecy));
            ctx.LineTo(new Point(ecx, ecy + ri));
            ctx.EndFigure(true);
        }


        //TODO: pridej Region s pacmanem (bez pusy)
        var head = new EllipseGeometry(new Rect(-R, -R, 2 * R, 2 * R));
        var pacman = new CombinedGeometry(GeometryCombineMode.Exclude, head, eye);

        //TODO: samostatne vyriznete pusu - pouzijte dalsi StreamGeometry
        double mouthSize = R * 0.3;
        var mouth = new StreamGeometry();
        using (var ctx = mouth.Open())
        {
            ctx.BeginFigure(new Point(0, 0), true);
            ctx.LineTo(new Point(2 * R, -(2 * mouthSize)));
            ctx.LineTo(new Point(2 * R, 2 * mouthSize));
            ctx.EndFigure(true);
        }

        var pacmanWithMouth = new CombinedGeometry(GeometryCombineMode.Exclude, pacman, mouth);

        var radialBrush = new RadialGradientBrush()
        {
            Center = new RelativePoint(0, 0, RelativeUnit.Absolute),
            RadiusX = new RelativeScalar(2*R, RelativeUnit.Absolute),
            RadiusY = new RelativeScalar(2*R, RelativeUnit.Absolute),
            GradientStops = new GradientStops()
            {
                new GradientStop(Colors.Red, 0),
                new GradientStop(Colors.Yellow, 1)
            }
        };

        g.DrawGeometry(radialBrush, null, pacmanWithMouth);

        var linearBrush = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0, R, RelativeUnit.Absolute),
            EndPoint = new RelativePoint(0, -R, RelativeUnit.Absolute),
            GradientStops = new GradientStops()
            {
                new GradientStop(Colors.Black, 0),
                new GradientStop(Color.FromArgb(80,0,0,0) , 0.5),
                new GradientStop(Color.FromArgb(0,0,0,0), 1)
            }
        };

        g.DrawGeometry(linearBrush, null, pacmanWithMouth);

        using (g.PushGeometryClip(pacmanWithMouth))
        {
            DrawPolka(R, g);
        }
    }

    /// <summary>Draws the polka dots in the area -R..R x -R..R.</summary>
    /// <param name="R">The range of the area.</param>
    /// <param name="g">The graphics context where to draw.</param>
    private void DrawPolka(double R, DrawingContext g)
    {
        double delta = R * 0.2;
        double fr = R * 0.05;
        for (double y = -R; y < R; y += delta)
        {
            for (double x = -R; x < R; x += delta)
            {
                g.DrawEllipse(Brushes.Green, null,
                    new Point(x, y), fr, fr);
            }
        }
    }
}