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

    public class Messages : ISpriteFont
    {
        /// <summary>
        /// The Sprite Message for the SpriteFont to be displayed
        /// </summary>
        public string SpriteMessage { get; protected set; }

        private SpriteFont font;

        private Vector2 MessagePostion = new Vector2(368, 396);

        private Color Color = Color.FromNonPremultiplied(45, 47, 132, 1);

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("font");
        }

        public void GetMessage(Message transition)
        {
            //Change position if needed
            switch (transition)
            {
                case Message.Round1:
                    SpriteMessage = "Round 1 \nCompleted";
                    break;
                case Message.Round2:
                    SpriteMessage = "Round 2 \nCompleted";
                    break;
                case Message.Win:
                    SpriteMessage = "You Win! \nESC to Exit or SPACE for Menu";
                    break;
                case Message Lose:
                    SpriteMessage = "Maybe Next Time. \nESC to Exit or SPACE for Menu";
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, "Hello World", MessagePostion, Color);
        }
    }
}
