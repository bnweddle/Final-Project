using Elemancy.Parallax;
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
    ///  1. Need to synch damage and player heaith with Healthbar width
    ///
    ///  EXTRA: Think about Sound effects:
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

        GraphicsDeviceManager graphics;

        /// <summary>
        /// SpriteBatch is for Parallax Layers
        /// Player, Levels, etc.
        /// </summary>
        public SpriteBatch spriteBatch;

        /// <summary>
        /// Is for Game Components that don't move
        /// HealthBar, Messages, Transitions Screens, etc.
        /// </summary>
        SpriteBatch componentsBatch;
        Messages messages = new Messages();
        Menu menu = new Menu();
        Level scene = new Level();

        KeyboardState oldState;

        ParallaxLayer playerLayer, levelsLayer;
        TrackingPlayer playerT, levelsT;

        /// <summary>
        /// The enemy Health when enemy dies -> disappear
        /// When another enemy appears -> appear and start at full health
        /// </summary>
        HealthBar wizardHealth, wizardGauge;
        HealthBar enemyHealth, enemyGauge;

        // Basic Particle Stuff
        Random random = new Random();
        ParticleSystem particleSystem;
        Texture2D particleTexture;

        GameState level;
        int scroll = 0;

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
            componentsBatch = new SpriteBatch(GraphicsDevice);

            wizardHealth.LoadContent(Content);
            wizardGauge.LoadContent(Content);

            enemyHealth.LoadContent(Content);
            enemyGauge.LoadContent(Content);

            menu.LoadContent(Content);
            messages.LoadContent(Content);

            // Player Layer
            player = new Player(this, Color.White);
            player.LoadContent(Content);
            playerLayer = new ParallaxLayer(this);
            playerLayer.Sprites.Add(player);
            playerLayer.DrawOrder = 2;
            Components.Add(playerLayer);

            levelsLayer = new ParallaxLayer(this);
            // Levels Layer - Can just add to to them for other levels

            var levelTextures = new List<Texture2D>()
            {
               Content.Load<Texture2D>("forest1"),
               Content.Load<Texture2D>("forest2"),
               Content.Load<Texture2D>("forest1"),
               Content.Load<Texture2D>("cave1"),
               Content.Load<Texture2D>("cave2"),
               Content.Load<Texture2D>("cave1"),
               Content.Load<Texture2D>("dungeon1"),
               Content.Load<Texture2D>("dungeon2")
            };

            var levelSprites = new List<StaticSprite>();
            for (int i = 0; i < levelSprites.Count; i++)
            {
                var position = Vector2.Zero;
                if(i == 7) position = new Vector2((9 * 1389) - 50, 0);
                else  position = new Vector2((i * 1389) - 50, 0);

                var sprite = new StaticSprite(levelTextures[i], position);
                levelSprites.Add(sprite);
            }

            foreach (var sprite in levelSprites)
            {
                levelsLayer.Sprites.Add(sprite);
            }
           
            levelsLayer.DrawOrder = 1;
            Components.Add(levelsLayer);

            playerT = new TrackingPlayer(player, 1.0f);
            levelsT = new TrackingPlayer(player, 1.0f);

            // The health bar may need it's own layer to stay put
            // componentsLayer.ScrollController = componentT;
            playerLayer.ScrollController = playerT;
            levelsLayer.ScrollController = levelsT;
            //transitionsLayer.ScrollController = transitionT;


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

            menu.Update(gameTime);

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
                // Need to synch damage and player heaith with Healthbar width
                wizardGauge.Bounds.Width -= 1;
                player.UpdateHealth(1);
            }

            player.Update(gameTime);
            level = scene.SetGameState(player);
            scroll = scene.GetScrollStop(level);

            if(player.Position.X >= scroll)
            {
                levelsT.ScrollStop = scroll;
                levelsT.ScrollRatio = 0.0f;
                playerT.ScrollRatio = 0.0f;
            }

            // Transition screen will be shown when the boss for that level is 
            // dead, and then If they hit C for Continue will change Player position 
            // and  scroll will change.

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

            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(gameTime);

            componentsBatch.Begin();

            wizardHealth.Draw(componentsBatch);
            wizardGauge.Draw(componentsBatch);

            enemyHealth.Draw(componentsBatch);
            enemyGauge.Draw(componentsBatch);

            if (!menu.Start)
            {
                menu.Draw(componentsBatch, graphics);
                //restart the game / re-initialize player at the beginning
            }

            // if current level Boss is dead 
               // if !Messages.Continue
                   // draw Message for Round1 / round 2 / or you Win depending on GameLevel
            // else if player is dead
              // if !BackMenu || !Exit
                   // draw Lose
                 // else if BackMenu
                    // Menu.Restart
                  // else terminate game

            //messages.Draw(componentsBatch, graphics);

            componentsBatch.End();
        }
    }
}
