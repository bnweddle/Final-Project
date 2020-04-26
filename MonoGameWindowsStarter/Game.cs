using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
    public class Game : Microsoft.Xna.Framework.Game
    {
        List<Enemy> forestEnemies = new List<Enemy>();
        List<Enemy> caveEnemies = new List<Enemy>();
        List<Enemy> dungeonEnemies = new List<Enemy>();
        EnemyBoss forestBoss;
        EnemyBoss caveBoss;
        EnemyBoss dungeonBoss;
        Enemy activeEnemy;

        Player player;
        Texture2D playerText;

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

        // Basic Particle Stuff
        Random random = new Random();
        ParticleSystem particleSystem;
        Texture2D particleTexture;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Creating and Positioning Healthbars
            wizardHealth = new HealthBar(this, new Vector2(20, 0), Color.Gray);  //Top left corner
            wizardGauge = new HealthBar(this, new Vector2(20, 0), Color.Red);  

            enemyHealth = new HealthBar(this, new Vector2(822, 0), Color.Gray);  //Top right corner
            enemyGauge = new HealthBar(this, new Vector2(822, 0), Color.Red);
            
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
            base.Initialize();
            graphics.PreferredBackBufferWidth = 1042;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            player.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            wizardHealth.LoadContent(Content);
            wizardGauge.LoadContent(Content);

            enemyHealth.LoadContent(Content);
            enemyGauge.LoadContent(Content);

            playerText = Content.Load<Texture2D>("player");
            player = new Player(this, playerText, Color.White);

            // Player Layer
            var playerLayer = new ParallaxLayer(this);
            playerLayer.Sprites.Add(player);
            playerLayer.Sprites.Add(wizardHealth);
            playerLayer.Sprites.Add(wizardGauge);
            playerLayer.Sprites.Add(enemyHealth);
            playerLayer.Sprites.Add(enemyGauge);
            playerLayer.DrawOrder = 2;
            Components.Add(playerLayer);

            var levelsLayer = new ParallaxLayer(this);

            // Levels Layer - Can just add to to them for other levels, not displaying?
            var levelTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("forest1"),
                Content.Load<Texture2D>("forest2")
                // Cave
                // Dungeon
            };

            var levelSprites = new StaticSprite[]
            {
                new StaticSprite(levelTextures[0], new Vector2(-200,0)),
                new StaticSprite(levelTextures[1], new Vector2(1189, 0))
            };

            levelsLayer.Sprites.AddRange(levelSprites);
            levelsLayer.DrawOrder = 1;
            Components.Add(levelsLayer);

            /* var playerScroll = playerLayer.ScrollController as AutoScrollController;
            playerScroll.Speed = 0f;

            var levelScroll = levelsLayer.ScrollController as AutoScrollController;
            levelScroll.Speed = 40f; */

            // The health bar may need it's own layer to stay put
            playerLayer.ScrollController = new TrackingPlayer(player, 0.5f);
            levelsLayer.ScrollController = new TrackingPlayer(player, 1.0f);

            //add for loop for enemies when we get texture files

            // Basic Particle Loading
            particleTexture = Content.Load<Texture2D>("particle");
                particleSystem = new ParticleSystem(this, 1000, particleTexture);
                particleSystem.Emitter = new Vector2(100, 100);
                particleSystem.SpawnPerFrame = 4;
                // Set the SpawnParticle method
                particleSystem.SpawnParticle = (ref Particle particle) =>
                {
                    MouseState mouse = Mouse.GetState();
                    particle.Position = new Vector2(mouse.X, mouse.Y);
                    particle.Velocity = new Vector2(
                        MathHelper.Lerp(-50, 50, (float)random.NextDouble()), // X between -50 and 50
                        MathHelper.Lerp(0, 100, (float)random.NextDouble()) // Y between 0 and 100
                        );
                    particle.Acceleration = 0.1f * new Vector2(0, (float)-random.NextDouble());
                    particle.Color = Color.Gold;
                    particle.Scale = 1f;
                    particle.Life = 1.0f;
                };

                // Set the UpdateParticle method
                particleSystem.UpdateParticle = (float deltaT, ref Particle particle) =>
                {
                    particle.Velocity += deltaT * particle.Acceleration;
                    particle.Position += deltaT * particle.Velocity;
                    particle.Scale -= deltaT;
                    particle.Life -= deltaT;
                };
                Components.Add(particleSystem);
            particleSystem.DrawOrder = 2;
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

            player.Update(gameTime);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
