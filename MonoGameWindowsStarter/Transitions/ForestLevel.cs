using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Elemancy.Transitions
{
    public class ForestLevel
    {
        private Game game;

        private Messages message;

        public List<IEnemy> forestEnemies = new List<IEnemy>();
        public List<IEnemy> respawned = new List<IEnemy>();
        public EnemyBoss forestBoss;
        public IEnemy ActiveEnemy;

        // the enemy's health bar
        HealthBar enemyHealth, enemyGauge;

        TrackingPlayer forestT;

        private Random random = new Random();

        ParallaxLayer forestLayer;

        public ForestLevel(Game game)
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

            forestLayer = new ParallaxLayer(game);

            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy forestEnemy = new BasicEnemy(game, GameState.Forest, new Vector2(300 + offset, 500));
                forestEnemy.LoadContent(content);
                forestLayer.Sprites.Add(forestEnemy);
                forestEnemies.Add(forestEnemy);
                offset += random.Next(200, 300);
            }

            forestBoss = new EnemyBoss(game, GameState.Forest, new Vector2(3500, 420));
            forestBoss.LoadContent(content);
            forestLayer.Sprites.Add(forestBoss);
            forestEnemies.Add(forestBoss);

            ActiveEnemy = forestEnemies[0];
            ActiveEnemy.IsActive = true;

            foreach (IEnemy e in forestEnemies)
            {
                System.Diagnostics.Debug.WriteLine($"{e.IsActive} activity");
            }

            game.Components.Add(forestLayer);
            forestLayer.DrawOrder = 2;
            forestT = new TrackingPlayer(game.player, 1.0f);

            forestLayer.ScrollController = forestT;
        }

        /// <summary>
        /// Update enemies for this level.
        /// </summary>
        /// <param name="gameTime">The Game's gameTime</param>
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
                forestEnemies[0].IsActive = false; // Don't draw the old one
                if (forestEnemies.Count > 1)
                {
                    respawned.Add(forestEnemies[0]);
                    forestEnemies.RemoveAt(0);
                    if (forestEnemies.Count == 0)
                    {
                        ActiveEnemy = forestBoss;
                        forestEnemies[0].IsActive = true;
                        if(forestBoss.Health <= 0)
                        {
                            forestBoss.Dead = true;
                        }
                    }
                    else
                    {
                        ActiveEnemy = forestEnemies[0];
                        forestEnemies[0].IsActive = true; // Draw active enemy
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
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            enemyHealth.Draw(spriteBatch);
            enemyGauge.Draw(spriteBatch);

            if (forestBoss.Dead)
            {
                message.SetMessage(1, out game.player.Position.X); // Round 1
                message.Update(gameTime);
                if (message.Continue == false)
                {
                    game.TransitionCave = false;
                    message.Draw(spriteBatch, game.graphics);
                }
                else if(message.Continue == true)
                {
                    game.TransitionCave = true;
                    message.Continue = false;
                }
            }
            else if (game.player.IsDead)
            {
                message.SetMessage(-1, out game.player.Position.X); // you lose
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
                if(game.GameState == GameState.MainMenu)
                {
                    message.BackMenu = false;
                }
                
            }
        }

        /// <summary>
        /// Reloads all basic enemies and boss enemies for the level.
        /// </summary>
        public void Reload()
        {
            while (forestLayer.Sprites.Count > 0)
            {
                forestLayer.Sprites.RemoveAt(0);
            }
            forestEnemies = new List<IEnemy>();
            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy caveEnemy = new BasicEnemy(game, GameState.Forest, new Vector2(300 + offset, 500));
                caveEnemy.LoadContent(game.Content);
                forestLayer.Sprites.Add(caveEnemy);
                forestEnemies.Add(caveEnemy);
                offset += random.Next(200, 300);
            }

            forestBoss = new EnemyBoss(game, GameState.Forest, new Vector2(3500, 400));
            forestBoss.LoadContent(game.Content);
            forestLayer.Sprites.Add(forestBoss);
            forestEnemies.Add(forestBoss);

            ActiveEnemy = forestEnemies[0];
            ActiveEnemy.IsActive = true;
            enemyGauge.RestartHealth();
        }

    }
}
