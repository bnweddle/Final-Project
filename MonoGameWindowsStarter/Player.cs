
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elemancy
{
    public class Player
    {
        private KeyboardState oldstate;

        private Game1 game;

        private Texture2D player;

        private Vector2 Position;

        public BoundingRectangle Bounds;

        /// <summary>
        /// Could change depending on spell cast/type of attack
        /// </summary>
        public int HitDamage { get; set; }
    }
}
