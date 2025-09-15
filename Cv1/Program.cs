
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
