using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elemancy.Transitions
{
    public enum Message
    {
        Round1,
        Round2,
        Win,
        Lose
    }

    public class Messages 
    {
        private Texture2D[] transitions = new Texture2D[4];

        private KeyboardState oldState;

        private Message message;

        private int index;

        public bool BackMenu { get; protected set; } = false;

        public bool Continue { get; protected set; } = false;

        public bool Exit { get; protected set; } = false;

        public void SetMessage(int boss, out int position)
        {
            position = 0;
            
            if(boss == 1)
            {
                position += 100;
                message = Message.Round1;
            }
            else if(boss == 2)
            {
                position += 100;
                message = Message.Round2;
            }
            else if(boss == 3)
            {
                position += 100;
                message = Message.Win;
            }
            else
            {
                position += 100;
                message = Message.Lose;
            }
        }

        public void LoadContent(ContentManager content)
        {
            transitions = new Texture2D[]
            {
               content.Load<Texture2D>("round1"),
               content.Load<Texture2D>("round2"),
               content.Load<Texture2D>("win"),
               content.Load<Texture2D>("lose"),
            };
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState current = Keyboard.GetState();

            if (current.IsKeyDown(Keys.M))
            {
                BackMenu = true;
            }
            else if (current.IsKeyDown(Keys.C))
            {
                Continue = true;
            }
            else if (current.IsKeyDown(Keys.E))
            {
                Exit = true;
            }

            oldState = current;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            if (message == Message.Round1) index = 0;
            if (message == Message.Round2) index = 1;
            if (message == Message.Win) index = 2;
            if (message == Message.Lose) index = 3;

            spriteBatch.Draw(transitions[index],
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                Color.White);
        }
    }
}
