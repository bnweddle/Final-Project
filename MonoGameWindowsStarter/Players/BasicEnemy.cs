using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Elemancy.Transitions;

namespace Elemancy
{
    public class BasicEnemy : IEnemy
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

        public Vector2 Position;

        private bool wasSpawned;

        private BoundingCircle attack;

        private Texture2D attackTexture;

        private Vector2 attackPosition;

        public bool canAttack;

        public float direction;

        //sets up the initial direction for the attack
        public Vector2 attackDirection;


        //true is enemy is dead, false if they are still alive
        public bool dead { get; set; }

        /// <summary>
        /// Sets up a new basic enemy
        /// </summary>
        /// <param name="h">health</param>
        /// <param name="d">damage</param>
        /// <param name="w">weakness</param>
        /// <param name="g">game</param>
        /// <param name="p">position</param>
        public BasicEnemy(int h, int d, string w, Game g, Vector2 p)
        {
            health = h;
            damage = d;
            weakness = w;
            game = g;
            Position = p;
            dead = false;
            canAttack = true;
            attackPosition = p;
        }

        /// <summary>
        /// Loads content
        /// </summary>
        /// <param name="cm">Content Manager</param>
        /// <param name="name">Name of the image used for enemy</param>
        public void LoadContent(ContentManager cm, string name, string attackName)
        {
            enemyTexture = cm.Load<Texture2D>(name);
            Bounds.Width = enemyTexture.Width;
            Bounds.Height = enemyTexture.Height;
            attackTexture = cm.Load<Texture2D>(attackName);
            attack.Radius = attackTexture.Width;
        }

       

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(enemyTexture, Position, Bounds, color);
            if (!canAttack)
            {
                spriteBatch.Draw(attackTexture, attackPosition, attack, color);
            }
        }

        //spawn the enemy with animation or just a time delay to make 
        //transition between enemies a little more seemless
        private void Spawn()
        {
            wasSpawned = true;
        }

        /// <summary>
        /// Takes in player to check bounds and update players health. Maybe also update if 
        /// enemy is hit with an attack as well?
        /// </summary>
        /// <param name="player"></param>
        public void Update(Player player, GameTime gameTime)
        {
            if (!wasSpawned)
            {
                Spawn();
            }

            if(health <= 0)
            {
                dead = true;
            }

            //keeping away from player but not too far away from the player
            if(Position.X + 100 < player.Position.X)
            {
                direction = 1;
            }
            else if(Position.X + 350 > player.Position.X)
            {
                direction = -1;
            }
            Position.X += direction;
            //updates position to throw attack
            if (canAttack)
            {
                //attack is active so it should be drawn
                //set a direction for attack to go
                canAttack = false;
            }
            else
            {
                attack.X -= 2;
                if(attack.X < player.Position.X - 50)
                {
                    canAttack = true;
                }
                //move attack and check if attack goes off screen
                //if goes off screen, dont draw attack and 'canAttack' is set to true
            }
            attackPosition.X = attack.X;
            attackPosition.Y = attack.Y;
            //sprite animation?
            if (attack.CollidesWith(player.Bounds))
            {
                //player takes damage, either affecting the hit bar or the actual player
                canAttack = true;
                if(!dead)
                {
                    // Commenting out for testing fading purposes
                    //player.UpdateHealth(damage);
                }
            }

            //check if enemy was hit
        }

    }
}
