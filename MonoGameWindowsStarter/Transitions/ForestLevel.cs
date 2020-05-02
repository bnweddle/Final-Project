using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy.Transitions
{
    public class ForestLevel
    {
        // Also put textures in here.
        // probably need a Message instance in here and not in Game

        // Probably will need a bool IsPlaying, to indicate that the music is 
        // playing, so it will not continually restart the music

        // Have a bool ForestStart
        // if the Start is true Draw everything for level
        // set the Start false if player deads or boss is dead

        // This will change for each level:
        // if current level Boss is dead 
            // if !Messages.Continue
            // draw for Round1 // Round 2 // Win
            // Update game.GameState to Cave
        // else if player is dead
            // if !BackMenu || !Exit
            // draw for Lose
            // else if BackMenu
            // State = GameState.Menu;
            // else terminate game

        private Song forestSong;

        private GameState state;

        private int ScrollStop;

        private List<IEnemy> forestEnemies = new List<IEnemy>();

        private EnemyBoss forestBoss;

        private IEnemy activeEnemy;

        public void LoadContent(ContentManager content)
        {
            forestSong = content.Load<Song>("Nature_Forest");

            MediaPlayer.IsMuted = true;
            MediaPlayer.IsRepeating = true;
            

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
