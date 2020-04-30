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

        /// <summary>
        /// Type can change with preference
        /// </summary>
        public string Spell { get; protected set; } = "";

        public bool Start { get; protected set; } = false;

        public void LoadContent(ContentManager content)
        {
            menu = content.Load<Texture2D>("menu");
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState current = Keyboard.GetState();

            if(current.IsKeyDown(Keys.D1) || current.IsKeyDown(Keys.NumPad1))
            {
                Spell = "Fire";
                Start = true;
            }
            else if (current.IsKeyDown(Keys.D2) || current.IsKeyDown(Keys.NumPad2))
            {
                Spell = "Water";
                Start = true;
            }
            else if (current.IsKeyDown(Keys.D3) || current.IsKeyDown(Keys.NumPad3))
            {
                Spell = "Lightning";
                Start = true;
            }

            old = current;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Draw(menu,
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                Color.White);
        }

        public void Restart()
        {
            Start = false;
        }


    }
}
