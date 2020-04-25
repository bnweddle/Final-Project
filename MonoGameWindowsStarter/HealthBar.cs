using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy
{
    public class HealthBar
    {
        private KeyboardState oldstate;

        private Game1 game;

        private Texture2D healthbar;

        private Vector2 Position;

        public BoundingRectangle Bounds;

        /// <summary>
        /// The Damage done for the hit, should be set when an attack is successful
        /// </summary>
        public int Damage { get; set; }

        public HealthBar(Game1 g, Vector2 position)
        {
            this.game = g;
            this.Position = position;
        }

        public void LoadContent(ContentManager content)
        {
            healthbar = content.Load<Texture2D>("healthbar6");
            Bounds.Width = healthbar.Width;
            Bounds.Height = healthbar.Height;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(healthbar, Position, Bounds, color); // The Health bar
        }

        /// <summary>
        /// Restart the Health gauge when game is started again/New enemy appears
        /// </summary>
        public void RestartHealth()
        {
            Bounds.Width = healthbar.Width;
        }
    }
}
