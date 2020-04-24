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

        public void Initialize()
        {
            Bounds.X = Position.X;
            Bounds.Y = Position.Y;
            Bounds.Height = healthbar.Height;
        }

        public void LoadContent(ContentManager content)
        {
            healthbar = content.Load<Texture2D>("healthbar");
        }

        public void Update(GameTime gameTime)
        {
            // If player is hit Update, using Keyboard for now for testing purposes
            KeyboardState current = Keyboard.GetState();

            if(current.IsKeyDown(Keys.H))
            {
                // Minus the Health by the damage done when player was hit
                Bounds.Width -= Damage;
            }

            oldstate = current;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(healthbar, Position, Bounds, Color.White); // The Health bar
            spriteBatch.Draw(healthbar, Position, Bounds, Color.Red); // The Health gauge
        }
        public void RestartHealth()
        {
            Bounds.Width = healthbar.Width;
        }
    }
}
