using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Elemancy.Transitions
{
    public enum GameState
    {
        MainMenu,
        Forest,
        Cave,
        Dungeon,
        Transition,
        GameOver
    }

    public class Scenes : ISprite
    {
        /// <summary>
        /// When the scrolling continues, where to reposition player at start of next level
        /// </summary>
        public Vector2 PlayerPosition { get; set; }

        /// <summary>
        /// Where to freeze scrolling according to each level
        /// </summary>
        public float ScrollStop { get; set; }

       

        public void GetGameState()
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }
    }
}
