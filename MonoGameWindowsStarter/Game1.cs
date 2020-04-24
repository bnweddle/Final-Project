using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Elemancy
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// The wizard's Health
        /// </summary>
        HealthBar wizardHealth;
        HealthBar wizardGauge;

        /// <summary>
        /// The enemy Health when enemy dies -> disappear
        /// When another enemy appears -> appear and start at full health
        /// </summary>
        HealthBar enemyHealth;
        HealthBar enemyGauge;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Creating and Positioning Healthbars
            wizardHealth = new HealthBar(this, new Vector2(20, 0));  //Top left corner
            wizardGauge = new HealthBar(this, new Vector2(20, 0));  //Top left corner

            enemyHealth = new HealthBar(this, new Vector2(800, 0));  //Top right corner
            enemyGauge = new HealthBar(this, new Vector2(800, 0));  
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            wizardHealth.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            wizardHealth.LoadContent(Content);
            wizardGauge.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // If player is hit Update, using Keyboard for now for testing purposes
            KeyboardState current = Keyboard.GetState();

            if (current.IsKeyDown(Keys.H))
            {
                // Minus the Health by the damage done when player was hit
                wizardGauge.Bounds.Width -= 1;
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            wizardHealth.Draw(spriteBatch, Color.Gray);
            wizardGauge.Draw(spriteBatch, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
