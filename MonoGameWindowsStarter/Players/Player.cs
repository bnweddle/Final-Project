/* Author: Bethany Weddle
 * Class: Player.cs
 * */
using System;
using Elemancy.Parallax;
using Elemancy.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elemancy
{
    /// <summary>
    /// An enumeration of possible player veritcal movement states
    /// </summary>
    public enum VerticalMovementState
    {
        OnGround,
        Jumping,
        DoubleJump,
        Falling,
    }

    /// <summary>
    /// For which direction the player is facing
    /// </summary>
    public enum Direction
    {
        West = 2,
        East,
        Idle
    }

    /// <summary>
    /// For the current 'Elemental Power' of the player
    /// </summary>
    public enum Element
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Lightning = 3,        
    }


    public class Player : ISprite
    {
        // Timers for fading and flickering when dying and being hit
        InterpolationTimer fade;
        InterpolationTimer flicker;
        float multiple = 1;

        // How much the animation moves per frames 
        const int FRAME_RATE = 124;

        // The duration of a player's jump, in milliseconds
        const int JUMP_TIME = 500;

        // The speed of the player
        public const float PLAYER_SPEED = 75;

        // The speed that the player falls
        public const float FALL_SPEED = 125;

        // Width of animation frames
        public const int FRAME_WIDTH = 67;

        // height of animation frames
        const int FRAME_HEIGHT = 100;

        // current frame
        int frame;

        // the health of the player, need to bind with healthbar
        int health;

        // The player's vertical movement state
        VerticalMovementState verticalState;

        // The player's facing direction
        Direction direction;

        // A timer for jumping
        TimeSpan jumpTimer;

        // A timer for animations
        TimeSpan animationTimer;

        // Old keyboard state for in Update
        KeyboardState oldState;

        // 'Elemental Power' state of the player
        public Element Element = Element.None;

        // 'Elemental Power' Orb
        public ElementalOrb elementalOrb;

        // The Game 
        Game game;
        
        // The player texture
        Texture2D player;


        /// <summary>
        /// The Player's position
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The Bounds of the Player
        /// </summary>
        public BoundingRectangle Bounds;

        /// <summary>
        /// The color of the player: white for lightning, red for fire, blue for water
        /// </summary>
        public Color Color;

        /// <summary>
        /// Could change depending on spell cast/type of attack
        /// </summary>
        public int HitDamage { get; set; }

        public bool IsDead { get; set; } = false;

        public bool IsHit { get; set; } = false;

        /// <summary>
        /// Constructing the Player
        /// </summary>
        /// <param name="game">The game the Player belongs to</param>
        /// <param name="player">The Texture</param>
        /// <param name="position">The Position</param>
        /// <param name="health">The Player's starting health</param>
        public Player(Game game, Color color)
        {
            this.game = game;
            this.Color = color;
            elementalOrb = new ElementalOrb(game, this);
        }

        public void Initialize()
        {
            Position = new Vector2(40,600);  // Start position could change with preference
            health = 25; // Could also change with preference
            direction = Direction.Idle;
            verticalState = VerticalMovementState.OnGround;
            Bounds.Width = FRAME_WIDTH;
            Bounds.Height = FRAME_HEIGHT;

            flicker = new InterpolationTimer(TimeSpan.FromSeconds(0.25), 0.0f, 1.0f);
            fade = new InterpolationTimer(TimeSpan.FromSeconds(2), 1.0f, 0.0f);

            Element = Element.None;
            elementalOrb.Initialize();
        }

        public void LoadContent(ContentManager content)
        { 
            player = content.Load<Texture2D>("player");

            elementalOrb.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            //Movement
            KeyboardState keyboard = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Bounds.X = Position.X;
            Bounds.Y = Position.Y;

            System.Diagnostics.Debug.WriteLine($"Player's X Position: {Position.X}");

            // So the player can't go backwards, would need to change as they 
            // progress through the levels
            if (Position.X < 40)
            {
                Position.X = 40;
            }

            // Vertical movement
            switch (verticalState)
            {
                case VerticalMovementState.OnGround:
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        verticalState = VerticalMovementState.Jumping;
                        jumpTimer = new TimeSpan(0);
                    }
                    if (keyboard.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
                    {
                        verticalState = VerticalMovementState.DoubleJump;
                        jumpTimer = new TimeSpan(0);
                    }
                    break;
                case VerticalMovementState.Jumping:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // Simple jumping and start fallings right after
                    Position.Y -= (600 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME)
                        verticalState = VerticalMovementState.Falling;
                    break;
               case VerticalMovementState.DoubleJump:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // Simple jumping and start fallings right after
                    Position.Y -= (900 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME)
                        verticalState = VerticalMovementState.Falling;
                    break;
                case VerticalMovementState.Falling:
                    Position.Y += delta * FALL_SPEED;
                    // Come back to the ground
                    if (Position.Y > 600)
                    {
                        Position.Y = 600;
                        verticalState = VerticalMovementState.OnGround;
                    }
                    break;                 
            }

            if(IsHit)
            {
                /* jumpTimer += gameTime.ElapsedGameTime;
                 // When they hit a enemy, should bounce away from it
                 // So they don't continue being hit
                 Position.Y -= (350 / (float)jumpTimer.TotalMilliseconds);
                 if (jumpTimer.TotalMilliseconds >= JUMP_TIME)
                     verticalState = VerticalMovementState.Falling; */

                if (flicker.TimeElapsed.TotalSeconds >= 0.20)
                {
                    flicker.Stop();
                    flicker = new InterpolationTimer(TimeSpan.FromSeconds(0.25), 0.0f, 1.0f);
                    IsHit = false;
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

            if (IsDead)
            {
                if (fade.TimeElapsed.TotalSeconds >= 1.75)
                {
                    fade.Stop();
                    multiple = 0;
                }

                if (!fade.IsRunning && multiple != 0)
                {
                    fade.Start();
                }
                else if(multiple != 0)
                {
                    if (fade.IsRunning)
                        fade.Update(gameTime.ElapsedGameTime);

                    multiple = fade.CurrentValue;
                }        
            }

            if (IsHit)
            {
               // for hitting the enemy
               /* Position.X -= 100 * delta;
                direction = Direction.West; */
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                    direction = Direction.West;                       
                else
                    direction = Direction.West;
                Position.X -= delta * PLAYER_SPEED;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                    direction = Direction.East;            
                else
                    direction = Direction.East;
                Position.X += delta * PLAYER_SPEED;
            }
            else
            {
                direction = Direction.Idle;
            }

            // Elemental Orb Activate and Update
            if (keyboard.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space) && elementalOrb.State == ElementalOrb.ActiveState.Idle)
            {
                Vector2 orbVelocity = new Vector2(1, 0);
                switch (direction)
                {
                    case Direction.East:
                        orbVelocity = new Vector2(1, 0);
                        break;
                    case Direction.West:
                        orbVelocity = new Vector2(-1, 0);
                        break;
                    case Direction.Idle:
                        orbVelocity = new Vector2(1, 0);
                        break;
                }

                elementalOrb.Attack(Position, orbVelocity, Element);            
            }
            elementalOrb.Update(gameTime);
            if (keyboard.IsKeyDown(Keys.LeftAlt) && !oldState.IsKeyDown(Keys.LeftAlt) && elementalOrb.State == ElementalOrb.ActiveState.Idle)
            {
                CycleElement();
            }
            System.Diagnostics.Debug.WriteLine($"Player Element Type: {Element}");
            

            // update animation timer when the player is moving
            if (direction != Direction.Idle)
                animationTimer += gameTime.ElapsedGameTime;

            // Check if animation should increase by more than one frame
            while (animationTimer.TotalMilliseconds > FRAME_RATE)
            {
                // increase frame
                frame++;
                // Decrease the timer by one frame duration
                animationTimer -= new TimeSpan(0, 0, 0, 0, FRAME_RATE);
            }

            frame %= 4;
            oldState = keyboard;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            Rectangle rectSource = new Rectangle(
                frame * FRAME_WIDTH,  // X value
                (int)direction % 4 * FRAME_HEIGHT, // Y value
                FRAME_WIDTH,
                FRAME_HEIGHT
                );

            spriteBatch.Draw(player, Position, rectSource, Color * multiple);

            elementalOrb.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Update the player's health. Currently only for damaging the player.
        /// </summary>
        /// <param name="damage">The damage done to the player's health.</param>
        public void UpdateHealth(int damage)
        {           
            health -= damage;
            if(health <= 0)
            {
                IsDead = true;
                IsHit = false;
            }
        }

        /// <summary>
        /// Method for switching element type - Probably using just for testing
        /// </summary>
        public void CycleElement()
        {
            if (Element == Element.None) Element = Element.Fire;
            else if (Element == Element.Fire) Element = Element.Water;
            else if (Element == Element.Water) Element = Element.Lightning;
            else if (Element == Element.Lightning) Element = Element.Fire;
        }
    }
}
