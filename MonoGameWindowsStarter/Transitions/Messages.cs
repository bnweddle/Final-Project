using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        /// <summary>
        /// The Sprite Message for the SpriteFont to be displayed
        /// </summary>
        public string SpriteMessage { get; protected set; }

        private SpriteFont font;

        private Texture2D transition;

        private Vector2 messagePostion = new Vector2(515, 335);

        private Color color;

        public void LoadContent(ContentManager content)
        {
            // Why can't make font bold??
            font = content.Load<SpriteFont>("font");
            transition = content.Load<Texture2D>("magicalForest");
            
            color.R = 45;
            color.G = 47;
            color.B = 132;
            color.A = 255;
        }

        public void GetMessage(Message transition)
        {
            //Change position if needed
            switch (transition)
            {
                case Message.Round1:
                    SpriteMessage = "Round 1 \n   Completed"; //Time before continue or something
                    break;
                case Message.Round2:
                    SpriteMessage = "Round 2 \n   Completed";  //Time before continue or something
                    break;
                case Message.Win:
                    SpriteMessage = "You Win! \n   E for Exit \n   M for Menu"; // If finished final level
                    break;
                case Message Lose:
                    SpriteMessage = "Better Luck Next Time. \n   E for Exit \n   M for Menu"; // If they die
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Draw(transition,
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                Color.White);
            spriteBatch.DrawString(font, "Better Luck Next Time. \n         E for Exit \n        M for Menu", messagePostion, color);
        }
    }
}
