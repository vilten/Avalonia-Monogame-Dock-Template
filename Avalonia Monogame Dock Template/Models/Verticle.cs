using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class Verticle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Microsoft.Xna.Framework.Vector2 position { get; set; } = new Microsoft.Xna.Framework.Vector2(0,0);
        public float opacity { get; set; } = 1f;
        public float size { get; set; } = 1f;
        public Color Color { get; set; } = Color.White;
    }
}
