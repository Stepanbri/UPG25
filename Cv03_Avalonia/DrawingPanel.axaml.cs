using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Globalization;
using System.Linq;

namespace Cv03;

public partial class DrawingPanel : UserControl
{
    //souradnice pozemku v realnych jednotkach
    private readonly double[] obj_x = {33.261905, 73.327375, 37.79762,
            -70.303568, -15.119048, -55.18452};

    private readonly double[] obj_y = {67.27976, 3.023812, 75.595238,
            39.30952, 46.113096, 1.5119};

    //obdelnik v realnem svete v realnych jednotkach (napr. m)
    private double x_min, x_max, y_min, y_max;
    private double world_width, world_height;

    

    public DrawingPanel()
    {
        InitializeComponent();

        x_min = obj_x.Min();
        x_max = obj_x.Max();
        y_min = obj_y.Min();
        y_max = obj_y.Max();

        world_width = x_max - x_min;
        world_height = y_max - y_min;
    }

    /// <summary>
    /// Renders the visual to a <see cref="T:Avalonia.Media.DrawingContext" />.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public override void Render(DrawingContext context)
    {        
        context.FillRectangle(Brushes.White, this.Bounds);

        // mìøítko pro zobrazení
        double scale_x = this.Bounds.Width / world_width;
        double scale_y = this.Bounds.Height / world_height;
        double scale = Math.Min(scale_x, scale_y);

        //vytvorime souradnice polygonu pro zobrazeni (ze souradnic realneho sveta na px)
        var points = new Point[obj_x.Length];
        for (int i = 0; i < obj_x.Length; i++)
        {
            points[i] = new Point(
                (obj_x[i]- x_min) * scale, 
                (obj_y[i] - y_min) * scale
                );
        }

        //a prostrednictvim cesty vytvorime graficky objekt polygonu
        var geometry = new StreamGeometry();
        using (var gctx = geometry.Open())
        {
            gctx.BeginFigure(points[0], true);
            for (int i = 1; i < points.Length; i++)
            {
                gctx.LineTo(points[i]);
            }
            gctx.EndFigure(true); //uzavrit lomenou caru do polygonu
        }

        context.DrawGeometry(Brushes.Magenta, null, geometry);

        //a vykresli sipku
        DrawArrow(points[0], points[1], 40, context);
    }

    /// <summary>
    /// Zobrazí šipku z bodu A = (x1,y1) do bodu B = (x2,y2) 
    /// </summary>
    /// <param name="x1">A(x)</param>
    /// <param name="y1">A(y)</param>
    /// <param name="x2">B(x)</param>
    /// <param name="y2">B(y)</param>
    /// <param name="tip_length">delka spicky v px</param>
    /// <param name="g">graficky kontext</param>
    private void DrawArrow(double x1, double y1,
        double x2, double y2, double tip_length,
        DrawingContext g)
    {
        DrawArrow(new Point(x1, y1), new Point(x2, y2), tip_length, g);
    }

    /// <summary>
    /// Zobrazi sipku z bodu A do bodu B
    /// </summary>
    /// <param name="A">pocatecni bod (v px)</param>
    /// <param name="B">koncovy bod  (v px)</param>
    /// <param name="tip_length">delka spicky v px</param>
    /// <param name="g">graficky kontext</param>
    private void DrawArrow(Point A, Point B,
        double tip_length, DrawingContext g)
    {
        //double u_x = B.X - A.X;
        //double u_y = B.Y - A.Y;
        //double u_len1 = 1 / Math.Sqrt(u_x * u_x + u_y * u_y);
        //u_x *= u_len1;
        //u_y *= u_len1;
        //u ma delku 1; it is a unit vector

        Vector u = B - A;
        u = u.Normalize(); //u je jednotkový vektor

        Point C = B - u * tip_length; //Bod C je zacatek spicky

        var pen = new Pen(Brushes.Black, 4);
        pen.LineJoin = PenLineJoin.Miter;
        pen.LineCap = PenLineCap.Round;

        g.DrawLine(pen, A, B);

        double d = 0.25 *  tip_length; // polovina šíøky spicky
        Vector v = new Vector(-u.Y, u.X); 

        Point D = C + v * d; 
        Point E = C - v * d; 
        //g.DrawLine(pen, D, B);
        //g.DrawLine(pen, E, B);

        var geometry = new StreamGeometry();
        using (var gctx = geometry.Open())
        {
            gctx.BeginFigure(D, false); 
            gctx.LineTo(B);
            gctx.LineTo(E);
            gctx.EndFigure(false); 
        }

        g.DrawGeometry(null, pen, geometry);
    }

}