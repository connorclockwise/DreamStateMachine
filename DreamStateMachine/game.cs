using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DreamStateMachine.Actions;
using DreamStateMachine.Behaviors;
using System.Xml;
using System.Threading;
using DreamStateMachine;
using DreamStateMachine.Input;

namespace DreamStateMachine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        ActorController actorController;
        ActorManager actorManager;
        AIController aiController;
        PhysicsController physicsController;

        Actor player;
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Random random;
        Rectangle tileRect;
        Point origin;
        Camera cam;
        SpriteBatch spriteBatch;
        SpriteFont arielBlackFont;
        SoundManager soundManager;
        Texture2D debugSquare;
        Texture2D floorTiles;
        Texture2D playerTexture;
        WorldManager worldManager;
        InputHandler inputHandler;

        bool isLoadingWorld;
        bool usingGamePad = false;

        public delegate void GameUpdate(GameTime gameTime);
        public delegate void GameDraw(GameTime gameTime);

        GameUpdate gameUpdate;
        GameDraw gameDraw;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            gameUpdate = MainGameUpdate;
            gameDraw = MainGameDraw;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
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
            Window.Title = "Dream State Machine";
            Window.IsBorderless = false;
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            floorTiles = Content.Load<Texture2D>("TempleFloor.png");
            //playerTexture = Content.Load<Texture2D>("protagonistBodyAnimations");
            playerTexture = Content.Load<Texture2D>("dreamManAnimations.png");
            debugSquare = Content.Load<Texture2D>("debugSquare");
            Texture2D healthBar = Content.Load<Texture2D>("debugSquare");
            arielBlackFont = Content.Load<SpriteFont>("SpriteFont1");

            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            origin.X = graphics.PreferredBackBufferWidth / 2;
            origin.Y = graphics.PreferredBackBufferHeight / 2;
            inputHandler = new InputHandler(origin);
            //inputHandler.controller = usingGamePad;
            random = new Random();
            tileRect = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            cam = new Camera(spriteBatch, tileRect, debugSquare, healthBar);
            //actors = new List<Actor>();
            aiController = new AIController();
            physicsController = new PhysicsController();
            worldManager = new WorldManager(random);
            worldManager.initWorldConfig(Content, "Content/Worlds.xml");
            worldManager.initStartingWorld();
            SoundManager.Instance.initSoundConfig(Content, "Content/sfx/Sounds.xml");
            actorController = new ActorController();
            actorManager = new ActorManager();
            actorManager.initActorConfig(Content, "Content/Actors.xml");
            actorManager.spawnActors(worldManager.curWorld.getSpawns());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
        /// 
        protected override void Update(GameTime gameTime)
        {
            gameUpdate(gameTime);
            base.Update(gameTime);
        }

        public void MainGameUpdate(GameTime gameTime)
        {
            float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            UpdateInput();
            actorController.update(dt);
            aiController.update(dt);
            physicsController.update(dt);
            cam.update();
           
            base.Update(gameTime);
        }

        public void LoadWorldUpdate(GameTime gameTime)
        {
            while (isLoadingWorld)
            {
                float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                //UpdateInput();
                //actorManager.update(dt);
                //UpdateCamera(cam, player);

                base.Update(gameTime);
            }
            gameUpdate = MainGameUpdate;
            gameDraw = MainGameDraw;
        }

        private void UpdateInput()
        {
            
            if (player != null)
            {
                List<Command> commands = inputHandler.handleInput();
                foreach (Command c in commands)
                    c.Execute(player);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            gameDraw(gameTime);
        }

        public void MainGameDraw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
                cam.drawFloor();
                cam.drawActors();
                cam.drawGUI();
            spriteBatch.End();

            base.Draw(gameTime);

        }

        public void LoadWorldDraw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
            // Draw Hello World
            string output = "Loading World";

            // Find the center of the string
            Vector2 FontOrigin = arielBlackFont.MeasureString(output) / 2;
            Vector2 screenorigin = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            // Draw the string
            spriteBatch.DrawString(arielBlackFont, output, screenorigin, Color.White,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);

        }

        public void LoadNextWorld()
        {

        }

        private void Actor_Spawn(object sender, EventArgs eventArgs)
        {
            Actor actor = (Actor)sender;
            if (actor.className == "player")
                player = actor;
        }
    }
}
