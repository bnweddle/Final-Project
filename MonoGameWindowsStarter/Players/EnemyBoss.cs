using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Elemancy.Parallax;
using Elemancy.Transitions;

namespace Elemancy
{
    //has same traits as a normal enemy but a more complicated update method
    public class EnemyBoss : IEnemy, ISprite
    {
        /// <summary>
        /// The Health the Enemy starts with, decremented as they are hit
        /// Hit -> the player's orb collides with them.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// the Enemy's weakness: how they lose health more/less
        /// </summary>
        public string Weakness { get; protected set; }

        /// <summary>
        /// true is enemy is dead, false if they are still alive
        /// </summary>
        public bool Dead { get; set; }

        /// <summary>
        /// If the orb collides with the Enemy
        /// </summary>
        public bool Hit { get; set; }

        /// <summary>
        /// Only draw the active boss
        /// </summary>
        public bool IsActive { get; set; } = false;


        private BoundingRectangle bounds;
       
        /// <summary>
        /// The Bounds of the Enemy
        /// </summary>
        public BoundingRectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        /// <summary>
        /// The Position of the Enemy
        /// </summary>
        public Vector2 Position;

        // game components
        Game game;
        GameState level;

        // texture components
        Texture2D enemyTexture;
        string enemyImage;

        // Timers for fading and flickering when dying and being hit
        InterpolationTimer fade;
        InterpolationTimer flicker;
        float multiple = 1;

        /// <summary>
        /// Set up the Enemy's health and image according to their Level
        /// NEED to input enemy image HERE!
        /// </summary>
        public void SetUpEnemy(GameState level)
        {
            if (level == GameState.Forest)
            {
                enemyImage = "tempEnemy"; // change for the Sprite
                Health = 200;
                Weakness = "Fire"; // Do a little extra damage if Player is using fire
            }
            else if (level == GameState.Cave)
            {
                enemyImage = "tempEnemy"; // Change for the Giant Troll
                Health = 250;
                Weakness = "Water"; // Do a little extra damage if player is using water 
            }
            else if (level == GameState.Dungeon)
            {
                enemyImage = "tempEnemy"; // Change for the dragon
                Health = 300;
                Weakness = "Lightning"; //Do a little extra damage if player is using lightning
            }
        }

        /// <summary>
        /// Sets up a new basic enemy
        /// </summary>
        /// <param name="health">health</param>
        /// <param name="damage">damage</param>
        /// <param name="weak">weakness</param>
        /// <param name="g">game</param>
        /// <param name="position">position</param>
        public EnemyBoss(Game g, GameState state, Vector2 position)
        {
            game = g;
            level = state;
            Position = position;
            Dead = false;
            SetUpEnemy(state);

            flicker = new InterpolationTimer(TimeSpan.FromSeconds(0.25), 0.0f, 1.0f);
            fade = new InterpolationTimer(TimeSpan.FromSeconds(2), 1.0f, 0.0f);
        }

        /// <summary>
        /// Loads content
        /// </summary>
        /// <param name="cm">Content Manager</param>
        /// <param name="name">Name of the image used for enemy</param>
        public void LoadContent(ContentManager content)
        {
            enemyTexture = content.Load<Texture2D>(enemyImage);
            bounds.Width = enemyTexture.Width;
            bounds.Height = enemyTexture.Height;
        }

        /// <summary>
        /// Takes in player to check bounds and update players health. Maybe also update if 
        /// enemy is hit with an attack as well?
        /// </summary>
        /// <param name="player"></param>
        public void Update(Player player, GameTime gameTime)
        {
            bounds.X = Position.X;
            bounds.Y = Position.Y;

            // SET HIT TO TRUE IF player orb collides with enemy.Bounds
            // Decrement health accordingly accounting for Weakness

            if (Health <= 0)
            {
                Dead = true;
            }

            if (Hit)
            {

                if (flicker.TimeElapsed.TotalSeconds >= 0.20)
                {
                    flicker.Stop();
                    flicker = new InterpolationTimer(TimeSpan.FromSeconds(0.25), 0.0f, 1.0f);
                    Hit = false;
                }
                else
                {
                    if (!flicker.IsRunning)
                        flicker.Start();

                    if (flicker.IsRunning)
                        flicker.Update(gameTime.ElapsedGameTime);

                    multiple = flicker.CurrentValue;
                }
            }

            if (Dead)
            {
                //CHANGE THE POSITION TO OUTER SPACE AFTER THEY FADE!!

                if (fade.TimeElapsed.TotalSeconds >= 1.75)
                {
                    fade.Stop();
                    multiple = 0;
                    //RIGHT HERE -> POSTION CHANGE!!
                }

                if (!fade.IsRunning && multiple != 0)
                {
                    fade.Start();
                }
                else if (multiple != 0)
                {
                    if (fade.IsRunning)
                        fade.Update(gameTime.ElapsedGameTime);

                    multiple = fade.CurrentValue;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (enemyTexture != null)
            {
                if (IsActive == true) // Only draw the active enemy
                {
                    spriteBatch.Draw(enemyTexture, Position, Bounds, Color.White);
                }
            }
        }
    }
}
