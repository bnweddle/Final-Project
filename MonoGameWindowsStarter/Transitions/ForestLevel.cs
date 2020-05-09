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
        public IEnemy ActiveEnemy;

        // the enemy's health bar
        HealthBar enemyHealth, enemyGauge;

        private Random random = new Random();

        public ForestLevel(Game game)
        {
            this.game = game;

            enemyHealth = new HealthBar(game, new Vector2(822, 0), Color.Gray);  //Top right corner
            enemyGauge = new HealthBar(game, new Vector2(822, 0), Color.Red);
        }

        public void LoadContent(ContentManager content)
        {
            message.LoadContent(content);
            enemyHealth.LoadContent(content);
            enemyGauge.LoadContent(content);

            var forestLayer = new ParallaxLayer(game);

            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy forestEnemy = new BasicEnemy(game, GameState.Forest, new Vector2(300 + offset, 500));
                forestEnemy.LoadContent(content);
                forestLayer.Sprites.Add(forestEnemy);
                forestEnemies.Add(forestEnemy);
                offset += random.Next(200, 300);
            }

            forestBoss = new EnemyBoss(game, GameState.Forest, new Vector2(300, 3500));
            forestBoss.LoadContent(content);
            forestLayer.Sprites.Add(forestBoss);
            forestEnemies.Add(forestBoss);

            forestEnemies[10].IsActive = true;
            ActiveEnemy = forestEnemies[10];

            game.Components.Add(forestLayer);
            forestLayer.DrawOrder = 2;

            forestLayer.ScrollController = new TrackingPlayer(game.player, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            message.Update(gameTime);
            ActiveEnemy.Update(game.player, gameTime);

            if(ActiveEnemy.Hit)
            {
                enemyGauge.Update(gameTime, ActiveEnemy.Health, game.player.HitDamage);              
            }

            if (ActiveEnemy.Dead)
            {
                forestEnemies[0].IsActive = false; // Don't draw the old one
                if (forestEnemies.Count > 0)
                {
                    forestEnemies.RemoveAt(0);
                    if (forestEnemies.Count == 0)
                    {
                        ActiveEnemy = forestBoss;
                    }
                    else
                    {
                        ActiveEnemy = forestEnemies[0];
                    }
                    forestEnemies[0].IsActive = true; // Draw active enemy
                    enemyGauge.RestartHealth(); // for the next enemy;
                }
            }
        }

        /// <summary>
        /// Will need to pass in componentsBatch, I think
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            enemyHealth.Draw(spriteBatch);
            enemyGauge.Draw(spriteBatch);

            if (forestBoss.Dead)
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
