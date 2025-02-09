using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class Polygon
    {
        // Unikátny identifikátor polygonu
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Zoznam vrcholov polygonu; použijeme Avalonia.Point pre reprezentáciu bodov
        public List<Point> Vertices { get; set; } = new List<Point>();

        // Zoznam alfa hodnôt; napr. môže predstavovať priehľadnosť jednotlivých častí polygonu.
        // Počet hodnôt môže zodpovedať počtu vrcholov alebo inej logike, ktorú si implementujete.
        public List<double> Alphas { get; set; } = new List<double>();
    }
}
