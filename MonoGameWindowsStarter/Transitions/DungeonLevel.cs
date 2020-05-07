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

        private Messages message = new Messages();

        private List<IEnemy> dungeonEnemies = new List<IEnemy>();
        private EnemyBoss dungeonBoss;
        private IEnemy ActiveEnemy;

        private Random random = new Random();

        public DungeonLevel(Game game)
        {
            this.game = game;
        }

        public void LoadContent(ContentManager content)
        {
            message.LoadContent(content);

            var dungeonLayer = new ParallaxLayer(game);

            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy dungeonEnemy = new BasicEnemy(game, GameState.Dungeon, new Vector2(8500 + offset, 543));
                dungeonLayer.Sprites.Add(dungeonEnemy);
                dungeonEnemies.Add(dungeonEnemy);
                offset += random.Next(200, 300);
            }

            dungeonBoss = new EnemyBoss(game, GameState.Dungeon, new Vector2(300, 10000));
            dungeonLayer.Sprites.Add(dungeonBoss);
            dungeonEnemies.Add(dungeonBoss);

            ActiveEnemy = dungeonEnemies[0];
            ActiveEnemy.LoadContent(content); 

            game.Components.Add(dungeonLayer);
            dungeonLayer.DrawOrder = 2;

            dungeonLayer.ScrollController = new TrackingPlayer(game.player, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            message.Update(gameTime);

            ActiveEnemy.Update(game.player, gameTime);

            if (ActiveEnemy.Dead)
            {
                if (dungeonEnemies.Count > 0)
                {
                    dungeonEnemies.RemoveAt(0);
                    if (dungeonEnemies.Count == 0)
                    {
                        ActiveEnemy = dungeonBoss;
                    }
                    else ActiveEnemy = dungeonEnemies[0];
                }
            }
        }

        /// <summary>
        /// Will need to pass in componentsBatch, I think
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (dungeonBoss.Dead)
            {
                message.SetMessage(1, out game.player.Position.X);
                if (!message.Continue)
                {
                    message.Draw(spriteBatch, game.graphics);
                }
                else if (game.player.IsDead)
                {
                    message.SetMessage(-1, out game.player.Position.X);
                    if (!message.BackMenu || !message.Exit)
                    {
                        message.Draw(spriteBatch, game.graphics);
                    }
                    else if (message.BackMenu)
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
