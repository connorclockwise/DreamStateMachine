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
        ItemManager itemManager;
        PhysicsController physicsController;
        WorldManager worldManager;

        Actor player;
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Random random;
        Rectangle tileRect;
        Point origin;
        Camera cam;
        SpriteBatch spriteBatch;
        SpriteFont arielBlackFont;
        Texture2D debugSquare;
        Texture2D healthBar;
        Texture2D whiteSquare;
        Texture2D splashScreen;
        
        InputHandler inputHandler;

        bool isLoadingWorld;
        //bool usingGamePad = false;

        


        public delegate void GameUpdate(GameTime gameTime);
        public delegate void GameDraw(GameTime gameTime);
        public Stack<GameDraw> gameDrawQueue;
        public Stack<GameUpdate> gameUpdateQueue;
        GameUpdate gameUpdate;
        GameDraw gameDraw;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            var screen = Screen.AllScreens.First(e => e.Primary);
            graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            graphics.PreferredBackBufferHeight = screen.Bounds.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            gameDrawQueue = new Stack<GameDraw>();
            gameUpdateQueue = new Stack<GameUpdate>();
            gameDrawQueue.Push(StartMenuUpdate);
            gameUpdateQueue.Push(StartMenuDraw);
            //gameDrawQueue.Enqueue(MainGameUpdate);
            //gameUpdateQueue.Enqueue(MainGameDraw);
            gameUpdate = gameUpdateQueue.Peek();
            gameDraw = gameDrawQueue.Peek();
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
            graphics.IsFullScreen = true;
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            //arielBlackFont = Content.Load<SpriteFont>("SpriteFont1");
            //splashScreen;

            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            origin.X = graphics.PreferredBackBufferWidth / 2;
            origin.Y = graphics.PreferredBackBufferHeight / 2;
            inputHandler = new InputHandler(origin);
            random = new Random();
            tileRect = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //tileRect = Window.ClientBounds;
            cam = new Camera(spriteBatch, tileRect);
            cam.loadGuiTextures(Content);

            aiController = new AIController();
            physicsController = new PhysicsController();
            worldManager = new WorldManager(random);
            worldManager.initWorldConfig(Content, "Content/Worlds.xml");

            SoundManager.Instance.initSoundConfig(Content, "Content/Sounds.xml", "Content/Music.xml");
            actorController = new ActorController();
            actorManager = new ActorManager();
            actorManager.initAnimationConfig(Content, "Content/Animations.xml");
            actorManager.initActorConfig(Content, "Content/Actors.xml");
            
            itemManager = new ItemManager();
            itemManager.initWeaponConfig(Content, "Content/Weapons.xml");

            cam.enterStartMenu();
            cam.NewGame += new EventHandler<EventArgs>(NewGameSelected);

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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            gameDraw(gameTime);
        }

        public void startNewGame()
        {
            worldManager.initStartingWorld();
            SoundManager.Instance.playSong("templeMusic");
        }

        public void MainGameUpdate(GameTime gameTime)
        {
            float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            UpdateInput();
            actorController.update(dt);
            aiController.update(dt);
            physicsController.update(dt);
            SoundManager.Instance.update(dt);
            cam.gameUpdate(dt);
           
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
            gameUpdateQueue.Pop();
            gameDrawQueue.Pop();

            //gameUpdate = MainGameUpdate;
            //gameDraw = MainGameDraw;
        }

        public void StartMenuUpdate(GameTime gameTime)
        {
            float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            cam.startMenuUpdate(dt);
            UpdateInput();
        }

        private void UpdateInput()
        {
            
            if (player != null)
            {
                List<Command> commands = inputHandler.handleInput();
                foreach (Command c in commands)
                    c.Execute(player);
            }
            else if (cam.menuEnabled && cam.rootGUIElement != null && inputHandler.controller)
            {
                List<Command> commands = inputHandler.handleInput();
                foreach (Command c in commands)
                    c.Execute(cam.rootGUIElement);
            }
            else if(cam.menuEnabled && cam.rootGUIElement != null)
            {
                cam.handleGuiControls();
            }
        }

        public void StartMenuDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
                cam.drawGUI();
            spriteBatch.End();
        }

        public void MainGameDraw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
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

        public void NewGameSelected(Object sender, EventArgs eventArgs)
        {
            //Console.Write("new game selected");
            cam.menuEnabled = false;
            startNewGame();

            gameUpdateQueue.Push(MainGameUpdate);
            gameDrawQueue.Push(MainGameDraw);
            gameUpdate = gameUpdateQueue.Peek();
            gameDraw = gameDrawQueue.Peek();
        }

        private void Actor_Spawn(Object sender, EventArgs eventArgs)
        {
            Actor actor = (Actor)sender;
            if (actor.className == "player")
                player = actor;
        }
    }
}
