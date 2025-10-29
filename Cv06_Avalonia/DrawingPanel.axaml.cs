using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace Cv06;

public partial class DrawingPanel : UserControl
{
    private const int N = 6;                //pocet cipu
    private const double DEFAULT_R = 100;   //vychozi polomer hvezdy
    private const double DEAULT_ROTATION = 0; //vychozi rotace hvezdy

    private double R = DEFAULT_R;           //aktualni polomer
    private double rotation = 60;            //aktuální rotace hvezdy
    private bool isBlueHover = false;
    private Geometry? _star;	            //hvezda k vykresleni


    public DrawingPanel()
    {
        InitializeComponent();   
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (IsStarHit(e.GetPosition(this)))
        {
            MakeStarSmaller();
        }
    }


    protected override void OnPointerMoved(PointerEventArgs e)
        {
            var pos = e.GetPosition(this);
            if (this.IsStarHit(pos))
            {
                if (!isBlueHover)
                {
                    StarHover();
                }
            }
            else
            {
                if (isBlueHover)
                {
                    StopHover();
                }
            }
        }

    /// <summary>
    /// Renders the visual to a <see cref="T:Avalonia.Media.DrawingContext" />.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.White, this.Bounds);

        //vycentruj vse
        //using (context.PushTransform(Matrix.CreateTranslation(this.Bounds.Width / 2, this.Bounds.Height / 2)))
        //{
            DrawStar(context);            
        //}
    }

    /// <summary>Creates the internal graphical object representing the regular star.</summary>
    /// <remarks>The star has centre in the origin of the coordinate system (0,0),
    /// outer radius R, inner radius R/2 and N points.</remarks>    
    private void CreateStar()
    {
        //first, create the points
        var pts = new Point[2 * N];
        double delta_fi = 2 * Math.PI / N;

        for (int i = 0; i < N; i++)
        {
            pts[2 * i] = new Point
                (
                    R * Math.Cos(i * delta_fi + 1.5 * Math.PI),
                    R * Math.Sin(i * delta_fi + 1.5 * Math.PI)
                );

            pts[2 * i + 1] = new Point
                (
                    0.5 * R * Math.Cos(i * delta_fi + delta_fi * 0.5 + 1.5 * Math.PI),
                    0.5 * R * Math.Sin(i * delta_fi + delta_fi * 0.5 + 1.5 * Math.PI)
            );
        }

        //and now create the stream geometry to work with
        var star = new StreamGeometry();
        using (var ctx = star.Open())
        {
            ctx.BeginFigure(pts[0], true);
            for (int i = 1; i < pts.Length; i++)
            {
                ctx.LineTo(pts[i]);
            }

            ctx.EndFigure(true);
        }

        _star = star;
    }


    /// <summary>Draws the star.</summary>
    /// <param name="context">The graphics context used to draw.</param>
    private void DrawStar(DrawingContext context)
    {
        if (_star == null)
            CreateStar();        

        if (_star != null)
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform(rotation));
            transformGroup.Children.Add(new TranslateTransform(this.Bounds.Width / 2, this.Bounds.Height / 2));
            _star.Transform = transformGroup;
            
            if (isBlueHover)
            {
                context.DrawGeometry(Brushes.Blue, null, _star);
            }
            else
            {
                context.DrawGeometry(Brushes.Orange, null, _star);
            }
        }
    }

    /// <summary>Resets the star to its default shape.</summary>
    internal void MakeStarDefault()
    {
        R = DEFAULT_R; //smaller star
        rotation = DEAULT_ROTATION;

        this._star = null;
        this.InvalidateVisual();  //vynut prekresleni
    }


    /// <summary>Makes the star smaller.</summary>
    internal void MakeStarSmaller()
    {
        R *= 0.5f; //smaller star

        this._star = null;
        this.InvalidateVisual();  //vynut prekresleni
    }

    //
    internal void StarHover()
    {
        isBlueHover = true;
        this._star = null;
        this.InvalidateVisual();  //vynut prekresleni
    }   

    internal void StopHover()
    {
        isBlueHover = false;
        this._star = null;
        this.InvalidateVisual();  //vynut prekresleni
    }

    internal void RotateStar(double angle)
    {
        rotation += angle;

        this._star = null;
        this.InvalidateVisual();  //vynut prekresleni
    }

    /// <summary>
    /// Determines whether the star is hit by at the specified [x,y].
    /// </summary>
    /// <param name="x">The x coordinate of the query.</param>
    /// <param name="y">The y coordinate of the query.</param>
    /// <returns>
    ///   <c>true</c> if the star is hit; otherwise, <c>false</c>.
    /// </returns>
    internal bool IsStarHit(double x, double y)
    {
        return IsStarHit(new Point(x, y));
    }

    /// <summary>
    /// Determines whether the star is hit by at the specified point [x,y].
    /// </summary>
    /// <param name="p">The query point.</param>
    /// <returns>
    ///   <c>true</c> if the star is hit; otherwise, <c>false</c>.
    /// </returns>
    internal bool IsStarHit(Point p)
    {
        System.Diagnostics.Debug.WriteLine(p);

        return (_star != null) ? _star.FillContains(p) : false;
    }
}