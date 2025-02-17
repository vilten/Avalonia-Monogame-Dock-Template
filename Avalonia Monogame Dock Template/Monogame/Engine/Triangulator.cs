using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Avalonia_Monogame_Dock_Template.Monogame.Engine;

public static class Triangulator
{
    /// <summary>
    /// Trianguluje zadaný polygon (zoznam bodov) pomocou ear clipping algoritmu.
    /// Vracia zoznam indexov, kde každá trojica indexov predstavuje jeden trojuholník.
    /// </summary>
    public static List<int> Triangulate(IList<Vector2> vertices)
    {
        List<int> indices = new List<int>();

        if (vertices.Count < 3)
            return indices;

        // Vytvoríme zoznam indexov, ktorý reprezentuje náš polygon.
        List<int> V = new List<int>();
        for (int i = 0; i < vertices.Count; i++)
            V.Add(i);

        int n = vertices.Count;
        int count = 2 * n; // Fail-safe, aby sa cyklus nezacyklil v prípade zložitého polygonu
        int i0 = 0;

        while (n > 3)
        {
            if (count-- <= 0)
            {
                // Polygon je pravdepodobne nepravidelný alebo neplatný (napr. samopretnikajúci sa).
                break;
            }

            int i1 = (i0 + 1) % n;
            int i2 = (i0 + 2) % n;

            Vector2 A = vertices[V[i0]];
            Vector2 B = vertices[V[i1]];
            Vector2 C = vertices[V[i2]];

            // Skontrolujeme, či trojuholník ABC je konvexný.
            if (IsConvex(A, B, C))
            {
                bool earFound = true;
                // Skontrolujeme, či vnútri trojuholníka nepatrí žiadny iný bod polygonu.
                for (int j = 0; j < n; j++)
                {
                    if (j == i0 || j == i1 || j == i2)
                        continue;

                    if (PointInTriangle(vertices[V[j]], A, B, C))
                    {
                        earFound = false;
                        break;
                    }
                }

                if (earFound)
                {
                    // Ak sme našli "ucho" (ear), pridáme trojuholník do indexov.
                    indices.Add(V[i0]);
                    indices.Add(V[i1]);
                    indices.Add(V[i2]);
                    // Odstránime stredný vrchol (i1) z nášho zoznamu
                    V.RemoveAt(i1);
                    n--;
                    // Resetujeme fail-safe counter
                    count = 2 * n;
                    // Začneme znovu od začiatku
                    i0 = 0;
                    continue;
                }
            }

            i0 = (i0 + 1) % n;
        }

        // Posledný zostávajúci trojuholník
        if (n == 3)
        {
            indices.Add(V[0]);
            indices.Add(V[1]);
            indices.Add(V[2]);
        }

        return indices;
    }

    /// <summary>
    /// Skontroluje, či trojuholník ABC je konvexný.
    /// Použijeme krížový súčin – ak je výsledok kladný, trojuholník je konvexný.
    /// </summary>
    private static bool IsConvex(Vector2 A, Vector2 B, Vector2 C)
    {
        return Cross(B - A, C - A) > 0;
    }

    private static float Cross(Vector2 v, Vector2 w)
    {
        return v.X * w.Y - v.Y * w.X;
    }

    /// <summary>
    /// Overí, či bod P leží vo vnútri trojuholníka ABC.
    /// Používame barycentrické súradnice.
    /// </summary>
    private static bool PointInTriangle(Vector2 P, Vector2 A, Vector2 B, Vector2 C)
    {
        // Vypočítame plochu trojuholníka ABC (aj s orientáciou)
        float area = Cross(B - A, C - A);
        // Ak je plocha nulová, trojuholník je degenerovaný
        if (Math.Abs(area) < 0.0001f)
            return false;

        float sign = area < 0 ? -1 : 1;
        float s = (A.Y * C.X - A.X * C.Y + (C.Y - A.Y) * P.X + (A.X - C.X) * P.Y) * sign;
        float t = (A.X * B.Y - A.Y * B.X + (A.Y - B.Y) * P.X + (B.X - A.X) * P.Y) * sign;
        return (s >= 0) && (t >= 0) && (s + t <= area * sign);
    }
}
