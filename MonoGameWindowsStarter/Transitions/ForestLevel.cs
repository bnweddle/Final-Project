using Elemancy.Parallax;
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
        // Have a bool ForestStart
        // if the Start is true Draw everything for level
        // set the Start false if player deads or boss is dead

        private Game game;

        private Messages message;

        private List<IEnemy> forestEnemies = new List<IEnemy>();

        private EnemyBoss forestBoss;

        private IEnemy activeEnemy;

        public void LoadContent(ContentManager content)
        {
            // Load Enemy Components
        }

        public void Update(GameTime gameTime)
        {
            // Update Enemy Components
            message.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if(forestBoss.dead)
            {
                message.SetMessage(1, out game.player.Position.X);
                if(!message.Continue)
                {
                    message.Draw(spriteBatch, game.graphics);
                }
                else if(game.player.IsDead)
                {
                    message.SetMessage(-1, out game.player.Position.X);
                    if(!message.BackMenu || !message.Exit)
                    {
                        message.Draw(spriteBatch, game.graphics);
                    }
                    else if(message.BackMenu)
                    {
                        game.GameState = GameState.MainMenu;
                    }
                    else
                    {
                        game.Exit();
                    }

                }
            }
        }

    }
}
