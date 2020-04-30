using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Elemancy
{
    //has same traits as a normal enemy but a more complicated update method
    public class EnemyBoss: IEnemy
    {
        //the amount of health the enemy has
        public int health { get; }

        //the amount of damage the enemy does
        public int damage { get; }

        //currently just an idea. The type of damage the enemy is weak to
        public string weakness { get; }

        public BoundingRectangle Bounds;

        private Game game;

        private Texture2D enemyTexture;

        public Vector2 Position { get; set; }

        //true is enemy is dead, false if they are still alive
        public bool dead { get; set; }

        /// <summary>
        /// Sets up a new boss
        /// </summary>
        /// <param name="h">health</param>
        /// <param name="d">damage</param>
        /// <param name="w">weakness</param>
        /// <param name="g">game</param>
        /// <param name="p">position</param>
        public EnemyBoss(int h, int d, string w, Game g, Vector2 p)
        {
            health = h;
            damage = d;
            weakness = w;
            game = g;
            Position = p;
            dead = false;
        }

        /// <summary>
        /// Loads content
        /// </summary>
        /// <param name="cm">Content Manager</param>
        /// <param name="name">Name of the image used for enemy</param>
        public void LoadContent(ContentManager cm, string name)
        {
            enemyTexture = cm.Load<Texture2D>(name);
            Bounds.Width = enemyTexture.Width;
            Bounds.Height = enemyTexture.Height;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(enemyTexture, Position, Bounds, color);
        }

        public void Update(Player player, GameTime gameTime)
        {
            if (health <= 0)
            {
                dead = true;
            }

            //update movement, perhaps move a little randomly?
            //sprite animation?
            if (Bounds.CollidesWith(player.Bounds))
            {
                //player takes damage, either affecting the hit bar or the actual player
                if (!dead)
                {
                    // commenting out for testing purposes
                    // player.UpdateHealth(damage);
                }
            }

            //checks if enemy was hit
        }

    }
}
