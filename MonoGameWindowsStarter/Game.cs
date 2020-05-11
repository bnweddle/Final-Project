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
    /// Team left to do:
    ///  1. Adjust enemies and collision for player/enemy    -> PARTIAL
    ///     being hit and dying
    ///     > Create images for types of enemies             -> STARTED
    ///  2. Include narrator when wanting to play it         -> STARTED
    ///     > figure out when to play the wav                 
    ///  3. Adjust particles to follow ball                  -> COMPLETE!
    ///  4. Adjust the player's health and enemies health 
    ///     to fit with health bar
    ///  5. Code for when Boss of specific level dies to     -> PARTIAL
    ///     show Transition
    ///  6. Include created drawn players                    -> STARTED
    ///  7. Test to make sure transitions happen smoothly    -> UNTESTED
    ///  8. Implement restarting the game if the user goes   -> UNTESTED
    ///     back to the Menu
    ///  9. Restrict Player's movement to the level, maybe?  -> POTENTIAL?
    /// </summary>

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public IEnemy ActiveEnemy;  
        /// <summary>
        /// The levels with the enemy logic
        /// </summary>
        ForestLevel forestLevel;
        CaveLevel caveLevel;
        DungeonLevel dungeonLevel;
        public bool TransitionCave = false;
        public bool TransitionDungeon = false;
        public bool Restart = false;

        // the enemy's health bar
        HealthBar enemyHealth, enemyGauge;

        public Player player;

        public GraphicsDeviceManager graphics;

        /// <summary>
        /// SpriteBatch is for Parallax Layers
        /// Player, Levels, etc.
        /// </summary>
        public SpriteBatch spriteBatch;

        /// <summary>
        /// Is for Game Components that don't move
        /// HealthBar, Messages, Transitions Screens, etc.
        /// </summary>
        public SpriteBatch componentsBatch;
        public Menu menu;
        public Music music = new Music();

        KeyboardState oldState;

        ParallaxLayer playerLayer, levelsLayer;
        public TrackingPlayer playerT, levelsT;

        /// <summary>
        /// The enemy Health when enemy dies -> disappear
        /// When another enemy appears -> appear and start at full health
        /// </summary>
        HealthBar wizardHealth, wizardGauge;


        public Narrator narrator;

        private GameState gameState;
        int scroll; // first level

        public GameState GameState 
        { 
            get { return gameState; } 
            set 
            {
                gameState = value;
                music.SetGameState(player, menu.Start);
            } 
        }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            menu = new Menu(this);

            enemyHealth = new HealthBar(this, new Vector2(822, 0), Color.Gray);  //Top right corner
            enemyGauge = new HealthBar(this, new Vector2(822, 0), Color.Red);

            // Creating and Positioning Healthbars
            wizardHealth = new HealthBar(this, new Vector2(20, 0), Color.Gray);  //Top left corner
            wizardGauge = new HealthBar(this, new Vector2(20, 0), Color.Red);  

            narrator = new Narrator(this);
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
            scroll = 3117;
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

            menu.LoadContent(Content);
            music.LoadContent(Content);
            narrator.LoadContent(Content);

            // Player Layer
            player = new Player(this, Color.White);
            player.LoadContent(Content);
            playerLayer = new ParallaxLayer(this);
            playerLayer.Sprites.Add(player);
            playerLayer.DrawOrder = 2;
            Components.Add(playerLayer);

            dungeonLevel = new DungeonLevel(this);
            caveLevel = new CaveLevel(this);
            forestLevel = new ForestLevel(this);

            dungeonLevel.LoadContent(Content);
            caveLevel.LoadContent(Content);
            forestLevel.LoadContent(Content);

            enemyHealth.LoadContent(Content);
            enemyGauge.LoadContent(Content);

            levelsLayer = new ParallaxLayer(this);
            // Levels Layer - Can just add to to them for other levels

            var levelTextures = new List<Texture2D>()
            {
               Content.Load<Texture2D>("forest1"),
               Content.Load<Texture2D>("forest2"),
               Content.Load<Texture2D>("forest1"), // 4167
               Content.Load<Texture2D>("cave1"),
               Content.Load<Texture2D>("cave2"),
               Content.Load<Texture2D>("cave1"),
               Content.Load<Texture2D>("dungeon2"),
               Content.Load<Texture2D>("dungeon2"),
               Content.Load<Texture2D>("dungeon2")
            };

            var levelSprites = new List<StaticSprite>();
            for (int i = 0; i < levelTextures.Count; i++)
            {
                var position = new Vector2((i * 1389) - 50, 0);
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

            playerLayer.ScrollController = playerT;
            levelsLayer.ScrollController = levelsT;
            GameState = GameState.MainMenu;

            //add for loop for enemies when we get texture files
            //Add Enemies to Components with DrawOrder so they appear on top of layers
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

            menu.Update(gameTime);

            
            switch (gameState)
            {
                case GameState.MainMenu:
                    menu.Update(gameTime);


                    break;
                default:

                    if (player.Bounds.CollidesWith(forestLevel.ActiveEnemy.Bounds)
                        || player.Bounds.CollidesWith(caveLevel.ActiveEnemy.Bounds) ||
                        player.Bounds.CollidesWith(dungeonLevel.ActiveEnemy.Bounds))
                    {
                        player.IsHit = true;
                        wizardGauge.Update(gameTime, player.Health, 5);
                        player.UpdateHealth(5);
                    }

                    player.Update(gameTime);
                    forestLevel.Update(gameTime);
                    caveLevel.Update(gameTime);
                    dungeonLevel.Update(gameTime);

                    if (player.Element == Element.None)
                    {
                        player.Element = menu.selectedElement;
                    }

                    if (TransitionCave)
                    {
                        // Cheat way to get song to switch right now
                        if (player.Position.X >= 4150 && player.Position.X <= 8334 && !music.IsPLaying)
                        {
                            gameState = music.SetGameState(player, menu.Start);
                            music.IsPLaying = true;
                        }
                        TransitionCave = false;
                    }
                    if(TransitionDungeon)
                    {
                        if (player.Position.X >= 8375 && music.IsPLaying)
                        {
                            music.IsPLaying = false;
                            gameState = music.SetGameState(player, menu.Start);
                        }
                        //System.Diagnostics.Debug.WriteLine($"{player.Position.X } player position");
                        //System.Diagnostics.Debug.WriteLine($"{gameState } gameState");
                        TransitionDungeon = false;
                    }                      

                    scroll = music.GetScrollStop(gameState);
                    System.Diagnostics.Debug.WriteLine($"{scroll } scrolling stop");
                    System.Diagnostics.Debug.WriteLine($"{player.Position.X } player position");

                    if (player.Position.X >= scroll)
                    {
                        playerT.ScrollRatio = 0.0f;
                        levelsT.ScrollRatio = 0.0f;
                        levelsT.ScrollStop = scroll;
                    }
                    else
                    {
                        levelsT.ScrollRatio = 1.0f;
                        playerT.ScrollRatio = 1.0f;
                    }

                    break; // END OF DEFAULT
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

            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(gameTime);

            componentsBatch.Begin();

            switch(gameState)
            {
                case GameState.MainMenu:
                    if(!menu.Start)
                    {
                        menu.Draw(componentsBatch, graphics);

                        if (Restart == true)
                        {
                            wizardGauge.RestartHealth();
                            player.Initialize();
                            scroll = 3117;
                            menu.Update(gameTime);

                            if (menu.Start)
                            {
                                Restart = false;
                            }
                        }
                    }                  
                    break;
                case GameState.Forest:
                    wizardHealth.Draw(componentsBatch);
                    wizardGauge.Draw(componentsBatch);

                    forestLevel.Draw(componentsBatch, gameTime);
                    break;
                case GameState.Cave:
                    wizardHealth.Draw(componentsBatch);
                    wizardGauge.Draw(componentsBatch);

                    caveLevel.Draw(componentsBatch, gameTime);
                    break;
                case GameState.Dungeon:                    
                    wizardHealth.Draw(componentsBatch);
                    wizardGauge.Draw(componentsBatch);

                    dungeonLevel.Draw(componentsBatch, gameTime);
                    break;
            }

            componentsBatch.End();
        }
    }
}
