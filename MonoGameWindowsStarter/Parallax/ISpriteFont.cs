using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy.Parallax
{
    public interface ISpriteFont
    {
        /// <summary>
        /// Draws the ISpriteFont.  This method should be invoked between calls to 
        /// SpriteBatch.Begin() and SpriteBatch.End() with the supplied SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        /// <param name="gameTime">The GameTime object</param>
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
