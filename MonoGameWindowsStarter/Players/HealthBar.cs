/* Author: Bethany Weddle
 * Class: HealthBar.cs
 * */
using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elemancy
{
    public class HealthBar
    {
        private Game game;

        // The health bar texture
        private Texture2D healthbar;

        // The position should be const
        private Vector2 Position;

        // The Bounds of the Healh bar
        public BoundingRectangle Bounds;

        // The Color of the Healthbar
        public Color Color;

        /// <summary>
        /// The health of the current player/enemy
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// The Damage done for the hit, should be set when an attack is successful
        /// </summary>
        public int Damage { get; set; }

        public HealthBar(Game g, Vector2 position, Color color)
        {
            this.game = g;
            this.Position = position;
            this.Color = color;
        }

        public void LoadContent(ContentManager content)
        {
            healthbar = content.Load<Texture2D>("healthbar6");
            Bounds.Width = healthbar.Width;
            Bounds.Height = healthbar.Height;
        }

        public void Update(GameTime gameTime, int health, int damage)
        {
            Bounds.Width -= (Bounds.Width / health) * damage;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthbar, Position, Bounds, Color); // The Health bar
        }

        /// <summary>
        /// Restart the Health gauge when game is started again/New enemy appears
        /// </summary>
        public void RestartHealth()
        {
            Bounds.Width = 200 ;
        }
    }
}
