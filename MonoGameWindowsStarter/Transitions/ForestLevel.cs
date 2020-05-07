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
        private Game game;

        private Messages message = new Messages();

        private List<IEnemy> forestEnemies = new List<IEnemy>();
        private EnemyBoss forestBoss;
        private IEnemy activeEnemy;

        private Random random = new Random();

        public ForestLevel(Game game)
        {
            this.game = game;
        }

        public void LoadContent(ContentManager content)
        {
            message.LoadContent(content);

            var forestLayer = new ParallaxLayer(game);

            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy forestEnemy = new BasicEnemy(game, GameState.Forest, new Vector2(300 + offset, 543));
                forestLayer.Sprites.Add(forestEnemy);
                forestEnemies.Add(forestEnemy);
                offset += random.Next(200, 300);
            }

            forestBoss = new EnemyBoss(game, GameState.Forest, new Vector2(300, 3800));
            forestLayer.Sprites.Add(forestBoss);
            forestEnemies.Add(forestBoss);

            activeEnemy = forestEnemies[0];

            game.Components.Add(forestLayer);
            forestLayer.DrawOrder = 2;

            forestLayer.ScrollController = new TrackingPlayer(game.player, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            message.Update(gameTime);
        }

        /// <summary>
        /// Will need to pass in componentsBatch, I think
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if(forestBoss.Dead)
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
