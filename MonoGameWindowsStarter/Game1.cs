using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Elemancy
{
    /// <summary>
    /// For switching between level more easily, maybe?
    /// </summary>
    enum GameState
    {
        MainMenu,
        Forest,
        Cave,
        Dungeon,
        Transition,
        GameOver
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        List<Enemy> forestEnemies;
        List<Enemy> caveEnemies;
        List<Enemy> dungeonEnemies;
        EnemyBoss forestBoss;
        EnemyBoss caveBoss;
        EnemyBoss dungeonBoss;
        Enemy activeEnemy;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// The wizard's Health
        /// </summary>
        HealthBar wizardHealth, wizardGauge;

        /// <summary>
        /// The enemy Health when enemy dies -> disappear
        /// When another enemy appears -> appear and start at full health
        /// </summary>
        HealthBar enemyHealth, enemyGauge;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Creating and Positioning Healthbars
            wizardHealth = new HealthBar(this, new Vector2(20, 0));  //Top left corner
            wizardGauge = new HealthBar(this, new Vector2(20, 0));  

            enemyHealth = new HealthBar(this, new Vector2(822, 0));  //Top right corner
            enemyGauge = new HealthBar(this, new Vector2(822, 0));  
            for(int i = 0; i < 10; i++)
            {
                //Vector position is subjected to change when we know where the "ground" is
                //and where the enemies need to be placed
                forestEnemies.Add(new BasicEnemy(30, 5, "fire", this, new Vector2(300, 700)));
                caveEnemies.Add(new BasicEnemy(40, 10, "water", this, new Vector2(300, 700)));
                dungeonEnemies.Add(new BasicEnemy(50, 15, "lightning", this, new Vector2(300, 700)));
            }
            forestBoss = new EnemyBoss(60, 10, "fire", this, new Vector2(300, 700));
            caveBoss = new EnemyBoss(80, 20, "water", this, new Vector2(300, 700));
            dungeonBoss = new EnemyBoss(100, 30, "lightning", this, new Vector2(300, 700));

            //setting the first active enemy to be the first enemy in the forest level
            activeEnemy = forestEnemies[0];
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
            base.Initialize();
            graphics.PreferredBackBufferWidth = 1042;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

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

            enemyHealth.LoadContent(Content);
            enemyGauge.LoadContent(Content);

            //add for loop for enemies when we get texture files
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
                // Minus the Health by the damage done when player was hit, using -1 for now
                wizardGauge.Bounds.Width -= 1;
            }

            //enemy update
            activeEnemy.Update();
            if (activeEnemy.dead)
            {

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
            wizardHealth.Draw(spriteBatch, Color.DarkSlateGray);
            wizardGauge.Draw(spriteBatch, Color.Red);

            enemyHealth.Draw(spriteBatch, Color.DarkSlateGray);
            enemyGauge.Draw(spriteBatch, Color.Red);
           
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
