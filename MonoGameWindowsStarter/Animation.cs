using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Elemancy {
    class Animation {

        Texture2D[] frames;
        int FramesPerSecond;
        double frame = 0;

        public Animation(Texture2D[] frames, int fps) {
            this.frames = frames;
            this.FramesPerSecond = fps;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, Boolean flipped, Color? color = null, float? multiple = 1) {

            color = color ?? Color.White;
            color *= multiple;


            frame += gameTime.ElapsedGameTime.TotalSeconds * FramesPerSecond;
            frame %= frames.Length;

            SpriteEffects s = SpriteEffects.None;
            if (flipped) s = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(frames[(int)frame], position, null, (Color)color, 0.0f, new Vector2(0, 0), new Vector2(1, 1), s, 0.0f);
        }

    }
}
