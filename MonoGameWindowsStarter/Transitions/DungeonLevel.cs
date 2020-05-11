using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Elemancy.Transitions
{
    public class DungeonLevel
    {
        private Game game;

        private Messages message;

        private List<IEnemy> dungeonEnemies = new List<IEnemy>();
        private EnemyBoss dungeonBoss;
        public IEnemy ActiveEnemy;

        private Random random = new Random();

        // the enemy's health bar
        HealthBar enemyHealth, enemyGauge;

        public DungeonLevel(Game game)
        {
            this.game = game;
            message = new Messages(game);

            enemyHealth = new HealthBar(game, new Vector2(822, 0), Color.Gray);  //Top right corner
            enemyGauge = new HealthBar(game, new Vector2(822, 0), Color.Red);
        }

        public void LoadContent(ContentManager content)
        {
            message.LoadContent(content);
            enemyHealth.LoadContent(content);
            enemyGauge.LoadContent(content);

            var dungeonLayer = new ParallaxLayer(game);

            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy dungeonEnemy = new BasicEnemy(game, GameState.Dungeon, new Vector2(8500 + offset, 500));
                dungeonEnemy.LoadContent(content);
                dungeonLayer.Sprites.Add(dungeonEnemy);
                dungeonEnemies.Add(dungeonEnemy);
                offset += random.Next(200, 300);
            }

            dungeonBoss = new EnemyBoss(game, GameState.Dungeon, new Vector2(11200, 600));
            dungeonBoss.LoadContent(content);
            dungeonLayer.Sprites.Add(dungeonBoss);
            dungeonEnemies.Add(dungeonBoss);

            dungeonEnemies[0].IsActive = true;
            ActiveEnemy = dungeonEnemies[0];

            game.Components.Add(dungeonLayer);
            dungeonLayer.DrawOrder = 2;

            dungeonLayer.ScrollController = new TrackingPlayer(game.player, 1.0f);
        }

        public void Update(GameTime gameTime)
        {

            if (ActiveEnemy.Hit)
            {
                enemyGauge.Update(gameTime, ActiveEnemy.Health, game.player.HitDamage);
                ActiveEnemy.Hit = false;
                ActiveEnemy.UpdateHealth(game.player.HitDamage);
            }

            if (ActiveEnemy.Dead)
            {
                dungeonEnemies[0].IsActive = false; // Don't draw the old one
                if (dungeonEnemies.Count > 1)
                {
                    dungeonEnemies.RemoveAt(0);
                    if (dungeonEnemies.Count == 0)
                    {
                        ActiveEnemy = dungeonBoss;
                        dungeonEnemies[0].IsActive = true;
                        if (dungeonBoss.Health <= 0)
                        {
                            dungeonBoss.Dead = true;
                        }
                        // Draw active enemy
                    }
                    else
                    {
                        ActiveEnemy = dungeonEnemies[0];
                        dungeonEnemies[0].IsActive = true; // Draw active enemy
                    }
                    enemyGauge.RestartHealth(); // for the next enemy;
                }
            }

            ActiveEnemy.Update(game.player, gameTime);
        }

        /// <summary>
        /// Will need to pass in componentsBatch, I think
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <summary>
        /// Will need to pass in componentsBatch, I think
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            enemyHealth.Draw(spriteBatch);
            enemyGauge.Draw(spriteBatch);

            if (dungeonBoss.Dead)
            {
                message.SetMessage(3, out game.player.Position.X);
                message.Update(gameTime);
                if (!message.BackMenu)
                {
                    message.Draw(spriteBatch, game.graphics);
                }
                else if (message.BackMenu)
                {
                    game.menu.Start = false;
                    game.music.SetGameState(game.player, false);
                    game.GameState = GameState.MainMenu;
                    game.Restart = true;
                }
                else
                {
                    game.Exit();
                }

                // So it shows the message again and doesn't skip right to MainMenu
                if (game.GameState == GameState.MainMenu)
                {
                    message.BackMenu = false;
                }

            }
            else if (game.player.IsDead)
            {
                message.SetMessage(-1, out game.player.Position.X);
                message.Update(gameTime);
                if (!message.BackMenu)
                {
                    message.Draw(spriteBatch, game.graphics);
                }
                else if (message.BackMenu)
                {
                    game.menu.Start = false;
                    game.music.SetGameState(game.player, false);
                    game.GameState = GameState.MainMenu;
                    game.Restart = true;
                }
                else
                {
                    game.Exit();
                }

                // So it shows the message again and doesn't skip right to MainMenu
                if (game.GameState == GameState.MainMenu)
                {
                    message.BackMenu = false;
                }
            }
        }

    }
}
