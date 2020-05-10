using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Elemancy.Transitions
{
    public enum GameState
    {
        Forest,
        Cave,
        Dungeon,
        MainMenu
    }
    public class Music // need to make abstract 
    {
        // DungeonLevel inherits level, constructor specific, list of enemies and boss, 
        Song forest, cave, dungeon, menu;

        private GameState level = GameState.MainMenu;

        public bool IsPLaying { get; set; } = false;

        public void LoadContent(ContentManager content)
        {
            forest = content.Load<Song>("Nature_Forest");
            cave = content.Load<Song>("Cave_Bats");
            dungeon = content.Load<Song>("Thunder");
            menu = content.Load<Song>("WizardTheme");

            MediaPlayer.IsMuted = true;
            MediaPlayer.IsRepeating = true;
        } 

        public int GetScrollStop(GameState scenes)
        {
            int scrollStop = 0;
            switch (scenes)
            {
                case GameState.Forest:
                    scrollStop = 3117;
                    break;
                case GameState.Cave:
                    scrollStop = 7284;
                    break;
                case GameState.Dungeon:
                    scrollStop = 11451;
                    break;
            }

            return scrollStop;
        }

        public GameState SetGameState(Player player, bool start)
        {
            if(start && !IsPLaying)
            {
                if (player.Position.X >= 0 && player.Position.X <= 4100)
                {
                    level = GameState.Forest;
                    MediaPlayer.Play(forest);
                }
                if (player.Position.X >= 4101 && player.Position.X <= 8134)
                {
                    level = GameState.Cave;
                    MediaPlayer.Play(cave);
                }
                if (player.Position.X >= 8134 && player.Position.X <= 12451)
                {
                    level = GameState.Dungeon;
                    MediaPlayer.Play(dungeon);
                }
            }
            else
            {
                MediaPlayer.Play(menu);
            }

            MediaPlayer.IsMuted = false;
            return level;
        }
    }
}
