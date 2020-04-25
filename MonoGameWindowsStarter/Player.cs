
using System;
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
        Falling,
        Hit, //flicker
        Dead //fade away
    }

    /// <summary>
    /// For which direction the player is facing
    /// </summary>
    public enum State
    {
        South = 0,
        North,
        West,
        East,
        Idle,
    }

    public class Player
    {

        // How much the animation moves per frames 
        const int FRAME_RATE = 124;

        // The duration of a player's jump, in milliseconds
        const int JUMP_TIME = 500;

        // The speed of the player
        public const float PLAYER_SPEED = 200;

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

        // The player's state
        State state;

        // A timer for jumping
        TimeSpan jumpTimer;

        // A timer for animations
        TimeSpan animationTimer;

        // Old keyboard state for in Update
        KeyboardState oldState;

        // The Game 
        Game1 game;
        
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
        /// Could change depending on spell cast/type of attack
        /// </summary>
        public int HitDamage { get; set; }

        /// <summary>
        /// Constructing the Player
        /// </summary>
        /// <param name="game">The game the Player belongs to</param>
        /// <param name="player">The Texture</param>
        /// <param name="position">The Position</param>
        /// <param name="health">The Player's starting health</param>
        public Player(Game1 game, Texture2D player)
        {
            this.game = game;
            this.player = player;
        }

        public void Initialize()
        {
            Position = new Vector2(240, 500);  // Start position could change with prefere
            health = 500;
            state = State.Idle;
            verticalState = VerticalMovementState.OnGround;
            Bounds.Width = FRAME_WIDTH;
            Bounds.Height = FRAME_HEIGHT;
        }

        public void LoadContent(ContentManager content)
        {
        }

        public void Update(GameTime gameTime)
        {
            //Movement
            KeyboardState keyboard = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Bounds.X = Position.X;
            Bounds.Y = Position.Y;

            // So the player can't go backwards, would need to change as they 
            // progress through the levels
            if (Position.X < 250)
            {
                Position.X = 250;
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
                    break;
                case VerticalMovementState.Jumping:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // Simple jumping and start fallings right after
                    Position.Y -= (600 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME)
                        verticalState = VerticalMovementState.Falling;
                    break;
                case VerticalMovementState.Falling:
                    Position.Y += delta * PLAYER_SPEED;
                    // Come back to the ground
                    if (Position.Y > 500)
                    {
                        Position.Y = 500;
                        verticalState = VerticalMovementState.OnGround;
                    }
                    break;
                case VerticalMovementState.Hit:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // When they hit a trap, should bounce away from it
                    // Add flickering 
                    Position.Y -= (350 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME)
                        verticalState = VerticalMovementState.Falling;
                    break;
                    // Add case for Dying
            }

            if (verticalState == VerticalMovementState.Hit)
            {
                Position.X -= 100 * delta;
                state = State.West;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                {
                    state = State.West;
                }                           
                else
                {
                    state = State.West;
                }
                Position.X -= delta * PLAYER_SPEED;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                {
                    state = State.East;
                }              
                else
                {
                    state = State.East;
                }
                Position.X += delta * PLAYER_SPEED;
            }
            else
            {
                state = State.Idle;
            }

            // update animation timer when the player is moving
            if (state != State.Idle)
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
                (int)state % 4 * FRAME_HEIGHT, // Y value
                FRAME_WIDTH,
                FRAME_HEIGHT
                );

            spriteBatch.Draw(player, Position, rectSource, Color.White);

        }
    }
}
