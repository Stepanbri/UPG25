using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Cv05;

public partial class DrawingPanel : UserControl
{
    private readonly int m_StartTime;
    public DrawingPanel()
    {
        InitializeComponent();

        //TODO: pridat casovac pro animace [HOTOVO]
        m_StartTime = Environment.TickCount;
        var timer = new DispatcherTimer()
        {
            Interval = new TimeSpan(500), //kazdych 500 ms
        };
        timer.Tick += (_, _) => this.InvalidateVisual();
        timer.Start();
    }

    /// <summary>
    /// Renders the visual to a <see cref="T:Avalonia.Media.DrawingContext" />.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.White, this.Bounds);

        //draw grass
        const double HR = 30.0; //the height of the grass
        context.FillRectangle(Brushes.Green,
            new Rect(0, this.Bounds.Height - HR, this.Bounds.Width, HR));

        //TODO: cilem je vykresli na trave van, kdyz vime, ze jeho vyska je 150 px [HOTOVO]
        context.PushTransform(Matrix.CreateTranslation(0, this.Bounds.Height - HR - 150));

        double elapsed = (Environment.TickCount - m_StartTime) / 1000.0;
        using (context.PushTransform(Matrix.CreateTranslation(elapsed*50, 0)))
        {
            DrawVan(context);
        }
    }

    /// <summary>
    /// Draws a car, 300x150 px big, 
    /// with top left corner of its bounding 
    /// box being at (0,0)
    /// </summary>
    /// <param name="context">Graphics context used to draw</param>
    private void DrawVan(DrawingContext context)
    {
        // Van body shape

        var skelet = new StreamGeometry();
        using (var ctx = skelet.Open())
        {
            ctx.BeginFigure(new Point(0, 125), isFilled: true);
            ctx.LineTo(new Point(0, 70));
            ctx.LineTo(new Point(30, 7));
            ctx.LineTo(new Point(60, 0));
            ctx.LineTo(new Point(270, 0));
            ctx.LineTo(new Point(290, 7));
            ctx.LineTo(new Point(300, 70));
            ctx.LineTo(new Point(300, 125));
            ctx.EndFigure(true);

            // Window
            ctx.BeginFigure(new Point(40, 10), isFilled: true);
            ctx.LineTo(new Point(10, 70));
            ctx.LineTo(new Point(100, 70));
            ctx.LineTo(new Point(100, 10));
            ctx.EndFigure(true);
        }

        context.DrawGeometry(Brushes.Red, null, skelet);

        // Wheels
        //TODO: vykreslit kola na 50,120 a 250,120 [HOTOVO]
        using (context.PushTransform(Matrix.CreateTranslation(50, 120)))
        {
            DrawWheel(context);
        }

        using (context.PushTransform(Matrix.CreateTranslation(250, 120)))
        {
            DrawWheel(context);
        }

        // Symbol
        //TODO: vykreslit symbol na stred auta (175,70), [HOTOVO]
        //otoceny o 20 stupnu
        using (context.PushTransform(Matrix.CreateTranslation(175, 70)))
        using (context.PushTransform(Matrix.CreateRotation(Math.PI*20/180.0)))
        {
            DrawSymbol(context);
        }
    }

    /// <summary>
    /// Draws a wheel with centre at (0,0) and radius 30 px.
    /// </summary>
    /// <param name="context">Graphics context used to draw</param>
    private void DrawWheel(DrawingContext context)
    {
        const double R = 30.0;

        context.DrawEllipse(Brushes.Black, null, 
            new Rect(-R, -R, 2 * R, 2 * R));

        double r2 = 0.75 * R; // disk
        context.DrawEllipse(Brushes.DarkGray, null, 
            new Rect(-r2, -r2, 2 * r2, 2 * r2));

        const double r3 = 5; // center
        context.DrawEllipse(Brushes.Orange, null, 
            new Rect(-r3, -r3, 2 * r3, 2 * r3));

        double elapsed = (Environment.TickCount - m_StartTime) / 1000.0;
        const double w = Math.PI / 50; //otoceni za 1 sekundu

        const double d = 10; // bolt distance
        using (context.PushTransform(Matrix.CreateRotation((2*Math.PI*R) + w*elapsed)))
        {
            for (int i = 0; i < 5; i++)
            {
                //TODO: rozesadit srouby [HOTOVO]
                // using (context.PushTransform(Matrix.CreateRotation(0.25 * Math.PI))) // lze pouzit na cely for cyklus jednou
                using (context.PushTransform(Matrix.CreateRotation(i * Math.PI / 2.5)))
                using (context.PushTransform(Matrix.CreateTranslation(d, 0)))
                {
                    DrawBolt(context);
                }
            }
        }

        //TODO: pridat animaci kolecek
    }

    /// <summary>
    /// Draws a tiny bolt in the centre of coordinate system (i.e, at 0,0)
    /// </summary>
    /// <param name="context">Graphics context used to draw</param>
    private void DrawBolt(DrawingContext context)
    {
        const double rb = 2;
        context.DrawEllipse(Brushes.Orange, null,
            new Rect(-rb, -rb, 2 * rb, 2 * rb));
    }

    /// <summary>
    /// Draws a symbol with the centre at (0,0) and radius 40.
    /// </summary>
    /// <param name="context">Graphics context used to draw</param>
    private void DrawSymbol(DrawingContext context)
    {
        int elapsed = (Environment.TickCount - m_StartTime) / 1000;
        Pen pen;
        if (elapsed % 2 == 0)
        {
            pen = new Pen(Brushes.Red);
        }
        else
        {
            pen = new Pen(Brushes.White);
        }


        

        const double R = 20;

        context.DrawEllipse(null, pen, new Rect(-R, -R, 2 * R, 2 * R));
        context.DrawLine(pen, new Point(0, -R), new Point(0, R));

        double rc = R * Math.Cos(Math.PI / 4);
        double rs = R * Math.Sin(Math.PI / 4);

        context.DrawLine(pen, new Point(0, 0), new Point(rc, rs));
        context.DrawLine(pen, new Point(0, 0), new Point(-rc, rs));
    }
}