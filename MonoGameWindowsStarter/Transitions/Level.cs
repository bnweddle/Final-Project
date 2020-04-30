using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Elemancy.Transitions
{
    public enum GameState
    {
        Forest,
        Cave,
        Dungeon
    }

    public class Level 
    {
        private GameState level;

        public int GetScrollStop(GameState scenes)
        {
            int scrollStop = 0;
            switch(scenes)
            {
                case GameState.Forest:
                    scrollStop = 3117;
                    break;
                case GameState.Cave:
                    scrollStop = 3117 * 2;
                    break;
                case GameState.Dungeon:
                    scrollStop = 3117 * 3;
                    break;
            }

            return scrollStop;
        }

        public GameState SetGameState(Player player)
        {

            if(player.Position.X >= 0 && player.Position.X <= 4167)
            {
                level = GameState.Forest;
            }
            if (player.Position.X >= 4168 && player.Position.X <= 8134)
            {
                level = GameState.Cave;
            }
            if(player.Position.X >= 8134 && player.Position.X <= 12451)
            {
                level = GameState.Dungeon;
            }

            return level;
        }
    }
}
