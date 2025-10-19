using System.Globalization;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Cv02;

public partial class DrawingPanel : UserControl
{
    public DrawingPanel()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        //base.Render(context);
        context.FillRectangle(Brushes.White, this.Bounds);

        var center = this.Bounds.Center;
        const int SUN_RADIUS = 20;

        var redPen = new Pen(Colors.Red.ToUInt32(), 1); 
        context.DrawEllipse(Brushes.Yellow, redPen, new Rect(center.X - SUN_RADIUS, center.Y - SUN_RADIUS, 2 * SUN_RADIUS, 2 * SUN_RADIUS));

        const int EYE_RADIUS = 5;
        context.DrawEllipse(Brushes.Black, null, new Rect(center.X - SUN_RADIUS/2 - EYE_RADIUS, center.Y - SUN_RADIUS/2, 2 * EYE_RADIUS, 2 * EYE_RADIUS));
        context.DrawEllipse(Brushes.Black, null, new Rect(center.X + SUN_RADIUS/2 - EYE_RADIUS, center.Y - SUN_RADIUS/2, 2 * EYE_RADIUS, 2 * EYE_RADIUS));

        const int MOUTH_RADIUS = 20;

        var path = new StreamGeometry();
        using (var ctx = path.Open())
        {
            ctx.BeginFigure(new Point(center.X - MOUTH_RADIUS, center.Y), false);
            ctx.ArcTo(new Point(center.X + MOUTH_RADIUS, center.Y), 
                new Size(MOUTH_RADIUS, MOUTH_RADIUS), 
                0, 
                false, 
                SweepDirection.CounterClockwise
                );
        }
        context.DrawGeometry(null, redPen, path);

        const int N = 8; // pocet paprsku
        for (int i = 0; i < N; i++)
        {
            var angle = i * 2 * Math.PI / N;
            int x1 = (int)(center.X + SUN_RADIUS * Math.Cos(angle));
            int y1 = (int)(center.Y + SUN_RADIUS * Math.Sin(angle));
            int x2 = (int)(center.X + 10 * SUN_RADIUS * Math.Cos(angle));
            int y2 = (int)(center.Y + 10 * SUN_RADIUS * Math.Sin(angle));
            context.DrawLine(redPen, new Point(x1, y1), new Point(x2, y2));

            var text = new FormattedText("Hello World",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("sans-serif", FontStyle.Italic),
                72,
                Brushes.Black);
            context.DrawText(text, new Point(center.X - text.Width / 2, center.Y + 3 * SUN_RADIUS));
        }
    }
}