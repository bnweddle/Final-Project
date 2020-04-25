using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy.Parallax
{
    public class StaticSprite : ISprite
    {
        /// <summary>
        /// The sprite's position in the game world
        /// </summary>
        public Vector2 position = Vector2.Zero;

        /// <summary>
        /// The texture this sprite uses
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// Scaling the Sprite
        /// </summary>
        float Scale;

        /// <summary>
        /// Creates a new static sprite
        /// </summary>
        /// <param name="texture">the texture to use</param>
        public StaticSprite(Texture2D texture, float scale)
        {
            this.Scale = scale;
            this.texture = texture;
        }

        /// <summary>
        /// Creates a new static sprite
        /// </summary>
        /// <param name="texture">the texture to use</param>
        /// <param name="position">the upper-left hand corner of the sprite</param>
        public StaticSprite(Texture2D texture, Vector2 Position, float scale)
        {
            this.Scale = scale;
            this.texture = texture;
            this.position = Position;
        }

        /// <summary>
        /// Draws the sprite using the provided SpriteBatch.  This
        /// method should be invoked between SpriteBatch.Begin() 
        /// and SpriteBatch.End() calls.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
