# Analýza projektů z předmětu "Úvod do počítačové grafiky"

Tento dokument slouží jako rozbor domácích úkolů z předmětu UPG. Cílem je identifikovat a popsat klíčové funkce, metody a postupy, které byly v jednotlivých projektech použity.
---

## Cv1: Kreslení v konzoli

Tento projekt je základním úvodem do generování grafiky pomocí programového kódu, i když v textovém režimu konzole.

### Použité techniky a postupy:

*   **Základy C#:** Práce s proměnnými, načítání vstupu z konzole (`Console.ReadLine()`), konverze datových typů (`Convert.ToInt32`).
*   **Vnořené cykly:** Použití dvou `for` cyklů pro iteraci přes 2D mřížku (reprezentovanou šířkou `m` a výškou `n`).
*   **Implicitní rovnice přímky:** Hlavní logikou je využití dvou lineárních rovnic (`z = n * x - m * y;` a `z2 = n * x + m * y - m * n;`). Tyto rovnice definují přímky. Podmínka `z <= 0 && z2 <= 0` pak určuje, které body (`*`) leží uvnitř oblasti vymezené těmito přímkami (v tomto případě trojúhelníku), a které leží vně (`.`). Jedná se o nejzákladnější formu rasterizace.

### Kód (`Program.cs`):

```csharp
int m;

if (args.Length > 0)
{
    m = Convert.ToInt32(args[0]);
}
else
{
    Console.Write("Zadej šířku M: ");
    m = Convert.ToInt32(Console.ReadLine());
}

int n = (m * 2) / 3;

Console.WriteLine($"Velikost: {m} x {n}");

for (int y = 0; y < n; y++)
{
    for (int x = 0; x < m; x++)
    {
        int z = n * x - m * y;

        int z2 = n * x + m * y - m * n;

        if (z <= 0 && z2 <= 0)
        {
            Console.Write("*");
        }
        else
        {
            Console.Write(".");
        }
    }
    Console.WriteLine();
}
```

---

## Cv02_Avalonia: Základy kreslení v Avalonia UI

První projekt využívající framework Avalonia pro 2D grafiku. Cílem je seznámit se se základními kreslícími operacemi.

### Použité techniky a postupy:

*   **Avalonia `UserControl`:** Vytvoření vlastního ovládacího prvku `DrawingPanel`, který dědí z `UserControl`.
*   **`Render` metoda:** Přepsání metody `public override void Render(DrawingContext context)`, která je vstupním bodem pro veškeré kreslení. `DrawingContext` je klíčový objekt, který poskytuje kreslící API.
*   **Základní tvary:**
    *   `context.FillRectangle()`: Vykreslení vyplněného obdélníku (použito pro pozadí).
    *   `context.DrawEllipse()`: Vykreslení elipsy/kruhu (tělo slunce, oči). Lze specifikovat výplň (`Brushes`) a obrys (`Pen`).
*   **Kreslení cest (`StreamGeometry`):**
    *   Použití `StreamGeometry` pro definici složitějších tvarů, které nejsou standardními primitivy.
    *   `ctx.BeginFigure()`: Začátek definice části cesty.
    *   `ctx.ArcTo()`: Vykreslení oblouku, použito pro úsměv slunce.
*   **Kreslení čar:** `context.DrawLine()` pro vykreslení paprsků.
*   **Vykreslování textu:** Použití `FormattedText` pro definici stylu textu (font, velikost, barva) a `context.DrawText()` pro jeho vykreslení.
*   **Cykly v kreslení:** Použití `for` cyklu pro generování a vykreslování pravidelně rozmístěných paprsků slunce.
*   **Základní transformace:** Výpočty pozic pomocí goniometrických funkcí (`Math.Cos`, `Math.Sin`) pro rotaci paprsků kolem středu.

### Kód (`DrawingPanel.axaml.cs`):

```csharp
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
```

---

## Cv03_Avalonia: Transformace souřadnic a vektorová matematika

Tento projekt se zaměřuje na transformaci mezi různými souřadnicovými systémy (světový vs. obrazovkový) a na základy vektorové matematiky pro kreslení složitějších prvků.

### Použité techniky a postupy:

*   **Transformace souřadnic (World-to-Screen):**
    *   Definice objektu v "reálných" souřadnicích (např. metrech).
    *   Výpočet ohraničujícího obdélníku (`x_min`, `x_max`, ...).
    *   Výpočet měřítka (`scale`) pro přizpůsobení velikosti objektu velikosti okna se zachováním poměru stran (`Math.Min`).
    *   Přepočet světových souřadnic na souřadnice obrazovky (pixely) pomocí posunutí a škálování.
*   **Kreslení polygonů:** Použití `StreamGeometry` s `BeginFigure` a sérií `LineTo` volání pro vytvoření lomené čáry, která je na konci uzavřena (`EndFigure(true)`).
*   **Vektorová matematika:**
    *   Reprezentace směrů a posunů pomocí `Vector`.
    *   Normalizace vektoru (`vector.Normalize()`) pro získání jednotkového vektoru (vektor délky 1), který udává směr.
    *   Výpočet kolmého vektoru (`new Vector(-u.Y, u.X)`) pro určení směru šířky šipky.
    *   Aritmetika s vektory a body (sčítání, odčítání, násobení skalárem) pro výpočet pozic hrotu šipky.
*   **Vlastnosti pera (`Pen`):** Nastavení vlastností jako `LineJoin` (jak se spojují segmenty čáry) a `LineCap` (zakončení čáry).
*   **Strukturování kódu:** Vytvoření pomocné metody `DrawArrow` pro znovupoužitelnost a lepší čitelnost hlavního kreslícího kódu.

### Kód (`DrawingPanel.axaml.cs`):

```csharp
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

    public override void Render(DrawingContext context)
    {        
        context.FillRectangle(Brushes.White, this.Bounds);

        // mtko pro zobrazen
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

    private void DrawArrow(Point A, Point B,
        double tip_length, DrawingContext g)
    {
        Vector u = B - A;
        u = u.Normalize(); //u je jednotkov vektor

        Point C = B - u * tip_length; //Bod C je zacatek spicky

        var pen = new Pen(Brushes.Black, 4);
        pen.LineJoin = PenLineJoin.Miter;
        pen.LineCap = PenLineCap.Round;

        g.DrawLine(pen, A, B);

        double d = 0.25 *  tip_length; // polovina ky spicky
        Vector v = new Vector(-u.Y, u.X); 

        Point D = C + v * d; 
        Point E = C - v * d; 

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
```

---

## Cv04_Avalonia: Pokročilé geometrie a barevné přechody

Tento projekt demonstruje složitější operace s geometriemi, jako je jejich kombinování a ořezávání, a použití barevných přechodů pro realističtější vzhled.

### Použité techniky a postupy:

*   **Kombinování geometrií (`CombinedGeometry`):**
    *   Vytváření komplexních tvarů pomocí logických operací mezi jednoduššími geometriemi.
    *   `GeometryCombineMode.Exclude`: Použito pro "vyříznutí" oka a úst z těla Pac-Mana. Tím vznikne výsledný tvar s otvory.
*   **Ořezávání (`PushGeometryClip`):**
    *   `context.PushGeometryClip(geometry)` omezí veškeré následující kreslící operace pouze na oblast danou ořezovou geometrií.
    *   Použito pro vykreslení puntíků pouze uvnitř těla Pac-Mana a pro vytvoření zaoblených rohů tlačítka.
*   **Barevné přechody (Gradients):**
    *   `LinearGradientBrush`: Barevný přechod podél linie. Použito pro stínování tlačítka a Pac-Mana.
    *   `RadialGradientBrush`: Kruhový barevný přechod od středu k okrajům. Použito pro hlavní barvu Pac-Mana.
    *   `GradientStops`: Kolekce bodů v přechodu, které definují barvu v určité pozici (od 0.0 do 1.0).
*   **Pokročilé formátování textu:** `FormattedText` s nastavením `FontWeight` a `TextAlignment` pro přesné umístění a vzhled textu na tlačítku.
*   **Transformace souřadnic:** Použití `context.PushTransform` s `Matrix.CreateTranslation` pro posunutí počátku souřadnic do středu okna, což zjednodušuje výpočty pozic.

### Kód (`DrawingPanel.axaml.cs`):

```csharp
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
        double x0 = -(SizeX/2);
        double y0 = -(SizeY / 2);
        double x1 = SizeX / 2;
        double y1 = SizeY / 2;

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

            ctx.BeginFigure(new Point(ecx + ri, ecy), true);
            ctx.LineTo(new Point(ecx, ecy - ri));
            ctx.LineTo(new Point(ecx - ri, ecy));
            ctx.LineTo(new Point(ecx, ecy + ri));
            ctx.EndFigure(true);
        }

        var head = new EllipseGeometry(new Rect(-R, -R, 2 * R, 2 * R));
        var pacman = new CombinedGeometry(GeometryCombineMode.Exclude, head, eye);

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
```

---

## Cv05_Avalonia: Hierarchické transformace a animace

Poslední projekt se soustředí na skládání transformací pro vytváření hierarchických modelů a na jednoduchou animaci pomocí časovače.

### Použité techniky a postupy:

*   **Hierarchické modelování:**
    *   Objekt (dodávka) je složen z menších, samostatně definovaných částí (tělo, kolo, symbol).
    *   Každá část je kreslena ve svém vlastním lokálním souřadnicovém systému (např. střed kola je v bodě [0,0]).
    *   `context.PushTransform(Matrix)`: Uloží aktuální transformační matici na zásobník a aplikuje novou. `using` blok zajišťuje, že po jeho skončení je matice automaticky odstraněna ze zásobníku (`PopTransform`), čímž se kreslení vrátí do předchozího stavu.
    *   Skládání transformací: Posunutí na pozici kola a následné vykreslení kola. Posunutí na střed symbolu, otočení a následné vykreslení symbolu. Toto umožňuje snadno měnit pozici celého auta bez nutnosti přepočítávat souřadnice všech jeho částí.
*   **Transformační matice:**
    *   `Matrix.CreateTranslation(x, y)`: Vytvoří matici pro posunutí.
    *   `Matrix.CreateRotation(angle)`: Vytvoří matici pro otočení kolem počátku. Úhel musí být v radiánech.
*   **Animace:**
    *   `DispatcherTimer`: Časovač, který v pravidelných intervalech (`Interval`) spouští událost `Tick`.
    *   `this.InvalidateVisual()`: Klíčové volání, které řekne Avalonia frameworku, že ovládací prvek je neplatný a je potřeba ho překreslit. Tím se znovu zavolá metoda `Render`.
    *   Výpočet uplynulého času (`Environment.TickCount`) pro řízení animace. Pohyb a rotace jsou závislé na čase, což zajišťuje konzistentní rychlost animace nezávisle na výkonu počítače.
*   **Podmíněné kreslení:** Změna barvy pera (`Pen`) v závislosti na uplynulém čase (`elapsed % 2 == 0`), což vytváří blikající efekt.

### Kód (`DrawingPanel.axaml.cs`):

```csharp
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

        m_StartTime = Environment.TickCount;
        var timer = new DispatcherTimer()
        {
            Interval = new TimeSpan(500), //kazdych 500 ms
        };
        timer.Tick += (_, _) => this.InvalidateVisual();
        timer.Start();
    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.White, this.Bounds);

        //draw grass
        const double HR = 30.0; //the height of the grass
        context.FillRectangle(Brushes.Green,
            new Rect(0, this.Bounds.Height - HR, this.Bounds.Width, HR));

        context.PushTransform(Matrix.CreateTranslation(0, this.Bounds.Height - HR - 150));

        double elapsed = (Environment.TickCount - m_StartTime) / 1000.0;
        using (context.PushTransform(Matrix.CreateTranslation(elapsed*50, 0)))
        {
            DrawVan(context);
        }
    }

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
        using (context.PushTransform(Matrix.CreateTranslation(50, 120)))
        {
            DrawWheel(context);
        }

        using (context.PushTransform(Matrix.CreateTranslation(250, 120)))
        {
            DrawWheel(context);
        }

        // Symbol
        using (context.PushTransform(Matrix.CreateTranslation(175, 70)))
        using (context.PushTransform(Matrix.CreateRotation(Math.PI*20/180.0)))
        {
            DrawSymbol(context);
        }
    }

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
                using (context.PushTransform(Matrix.CreateRotation(i * Math.PI / 2.5)))
                using (context.PushTransform(Matrix.CreateTranslation(d, 0)))
                {
                    DrawBolt(context);
                }
            }
        }
    }

    private void DrawBolt(DrawingContext context)
    {
        const double rb = 2;
        context.DrawEllipse(Brushes.Orange, null,
            new Rect(-rb, -rb, 2 * rb, 2 * rb));
    }

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
```
