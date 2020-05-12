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
        private Game game;

        private Texture2D[] transitions = new Texture2D[4];

        private KeyboardState oldState;

        private Message message;

        private int index;

        public bool BackMenu { get; set; } = false;

        public bool Continue { get; set; } = false;

        public bool Exit { get; protected set; } = false;

        public Messages(Game game)
        {
            this.game = game;
        }

        public void SetMessage(int boss, out float position)
        {
            position = 0;
            
            if(boss == 1)
            {
                position = 4175;
                message = Message.Round1;
                index = 0;
            }
            else if(boss == 2)
            {
                position = 8375;
                message = Message.Round2;
                index = 1;
            }
            else if(boss == 3)
            {
                message = Message.Win;
                index = 2;
            }
            else
            {
                index = 3;
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
                game.GameState = GameState.MainMenu;
            }
            else if (current.IsKeyDown(Keys.C))
            {
                Continue = true;
            }
            else if (current.IsKeyDown(Keys.E))
            {
                game.Exit();
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
