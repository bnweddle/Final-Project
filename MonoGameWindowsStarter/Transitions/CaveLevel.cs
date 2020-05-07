using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Elemancy.Transitions
{
    public class CaveLevel
    {
        private Game game;

        private Messages message = new Messages();

        private List<IEnemy> caveEnemies = new List<IEnemy>();
        private EnemyBoss caveBoss;
        private IEnemy activeEnemy;

        private Random random = new Random();

        public CaveLevel(Game game)
        {
            this.game = game;
        }

        public void LoadContent(ContentManager content)
        {
            message.LoadContent(content);

            var caveLayer = new ParallaxLayer(game);

            float offset = 200;
            for (int i = 0; i < 10; i++)
            {

                BasicEnemy caveEnemy = new BasicEnemy(game, GameState.Forest, new Vector2(4500 + offset, 543));
                caveLayer.Sprites.Add(caveEnemy);
                caveEnemies.Add(caveEnemy);
                offset += random.Next(200, 300);
            }

            caveBoss = new EnemyBoss(game, GameState.Forest, new Vector2(300, 8300));
            caveLayer.Sprites.Add(caveBoss);
            caveEnemies.Add(caveBoss);

            activeEnemy = caveEnemies[0];

            game.Components.Add(caveLayer);
            caveLayer.DrawOrder = 2;

            caveLayer.ScrollController = new TrackingPlayer(game.player, 1.0f);
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
            if (caveBoss.Dead)
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
