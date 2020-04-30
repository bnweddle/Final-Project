using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy.Transitions
{
    public class Menu
    {
        private Texture2D menu;

        private KeyboardState old;

        private InterpolationTimer fade;

        private float multiple = 1;

        public Element selectedElement = Element.None;

        /// <summary>
        /// Type can change with preference
        /// </summary>
        public string Spell { get; protected set; } = "";

        public bool Start { get; protected set; } = false;

        public void LoadContent(ContentManager content)
        {
            menu = content.Load<Texture2D>("menu");
            fade = new InterpolationTimer(TimeSpan.FromSeconds(1), 1.0f, 0.0f);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState current = Keyboard.GetState();

            if(current.IsKeyDown(Keys.D1) || current.IsKeyDown(Keys.NumPad1))
            {
                Spell = "Fire";
                selectedElement = Element.Fire;
                Start = true;
            }
            else if (current.IsKeyDown(Keys.D2) || current.IsKeyDown(Keys.NumPad2))
            {
                Spell = "Water";
                selectedElement = Element.Ice;
                Start = true;
            }
            else if (current.IsKeyDown(Keys.D3) || current.IsKeyDown(Keys.NumPad3))
            {
                Spell = "Lightning";
                selectedElement = Element.Electric;
                Start = true;
            }

            // Can do with or without preference.
            if (Start)
            {
                if (fade.TimeElapsed.TotalSeconds >= 0.75)
                {
                    fade.Stop();
                    multiple = 0;
                }

                if (!fade.IsRunning && multiple != 0)
                {
                    fade.Start();
                }
                else if (multiple != 0)
                {
                    if (fade.IsRunning)
                        fade.Update(gameTime.ElapsedGameTime);

                    multiple = fade.CurrentValue;
                }
            }

            old = current;
        }


        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Draw(menu,
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                Color.White * multiple);
        }


    }
}
