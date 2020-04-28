﻿using Elemancy.Parallax;
using Elemancy.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Elemancy
{
    /// <summary>
    /// My TO DO:
    ///  1. Work on Transition screen, stop scrolling when reach end of level
    ///  5. Look at implementing SpriteFont for displaying messages on transition screen
    ///  6. Think about Menu construction: IMenu for transitioning easier maybe
    ///     > Maybe an Enum for the SpriteFont Messages: 
    ///       * Round1, Round2, Win -> Go to Menu or Exit, Lose -> Go to Menu or Exit
    ///  7. EXTRA: Think about Sound effects:
    ///     > Like forest song
    ///     > Cave song
    ///     > Menu song, Success wav, Fail wav
    ///     > Dungeon song    
    /// </summary>

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Enemies
        /// </summary>
        List<IEnemy> enemyList = new List<IEnemy>();
        IEnemy activeEnemy;

        Player player;
        Camera camera = new Camera();
        Viewport view;

        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        KeyboardState oldState;

        ParallaxLayer componentsLayer, playerLayer, levelsLayer;
        TrackingPlayer componentT, playerT, levelsT;

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
                enemyList.Add(new BasicEnemy(30, 5, "fire", this, new Vector2(300, 700)));               
                
            }
            enemyList.Add(new EnemyBoss(60, 10, "fire", this, new Vector2(300, 700)));
            for (int i = 0; i < 10; i++)
            {
                enemyList.Add(new BasicEnemy(40, 10, "water", this, new Vector2(300, 700)));
            }
            enemyList.Add(new EnemyBoss(80, 20, "water", this, new Vector2(300, 700)));
            for (int i = 0; i < 10; i++)
            {
                enemyList.Add(new BasicEnemy(50, 15, "lightning", this, new Vector2(300, 700)));
            }
            enemyList.Add(new EnemyBoss(100, 30, "lightning", this, new Vector2(300, 700)));

            //setting the first active enemy to be the first enemy in the forest level
            activeEnemy = enemyList[0];
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

            view.Width = 1042;
            view.Height = 768;

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

            
            player = new Player(this, Color.White);
            player.LoadContent(Content);

            componentsLayer = new ParallaxLayer(this);
            componentsLayer.Sprites.Add(wizardHealth);
            componentsLayer.Sprites.Add(wizardGauge);
            componentsLayer.Sprites.Add(enemyHealth);
            componentsLayer.Sprites.Add(enemyGauge);
            componentsLayer.DrawOrder = 2;
            Components.Add(componentsLayer);

            // Player Layer
            playerLayer = new ParallaxLayer(this);
            playerLayer.Sprites.Add(player);
            playerLayer.DrawOrder = 2;
            Components.Add(playerLayer);

            levelsLayer = new ParallaxLayer(this);
            // Levels Layer - Can just add to to them for other levels
            var levelTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("forest1"),
                Content.Load<Texture2D>("forest2"), // about 2700 width
                // Cave
                Content.Load<Texture2D>("dungeon") // about 2800 width
            };
            var levelSprites = new StaticSprite[]
            {
                new StaticSprite(levelTextures[0], new Vector2(-50,0)), 
                new StaticSprite(levelTextures[1], new Vector2(1339, 0)),
                new StaticSprite(levelTextures[2], new Vector2(2728, 0))
            };

            levelsLayer.Sprites.AddRange(levelSprites);
            levelsLayer.DrawOrder = 1;
            Components.Add(levelsLayer);

            componentT = new TrackingPlayer(player, 0.0f);
            playerT = new TrackingPlayer(player, 1.0f);
            levelsT = new TrackingPlayer(player, 1.0f);

            // The health bar may need it's own layer to stay put
            componentsLayer.ScrollController = componentT;
            playerLayer.ScrollController = playerT;
            levelsLayer.ScrollController = levelsT;


            //add for loop for enemies when we get texture files
            //Add Enemies to Components with DrawOrder so they appear on top of layers

            // Probably use SpaceBar for triggering spell casting for Player
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

            System.Diagnostics.Debug.WriteLine($"{player.Position.X } player's X Position");
            // If player is hit Update, using Keyboard for now for testing purposes
            KeyboardState current = Keyboard.GetState();

            //enemy update
            activeEnemy.Update(player, gameTime);
            if (activeEnemy.dead)
            {
                enemyList.Remove(activeEnemy);
                if(enemyList.Count > 0)
                {
                    activeEnemy = enemyList[0];
                }
            }

            if (current.IsKeyDown(Keys.H))
            {
                player.IsHit = true;
                // Minus the Health by the damage done when player was hit/Is collided with, using -1 for now
                wizardGauge.Bounds.Width -= 1;
                player.UpdateHealth(1);
            }

            player.Update(gameTime);
            camera.Update(player, view);

            if(player.Position.X >= 1735)
            {
                levelsT.ScrollRatio = 0.0f;
                playerT.ScrollRatio = 0.0f;

            }

            base.Update(gameTime);
            oldState = current;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                             RasterizerState.CullCounterClockwise, null, camera.Matrix);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
