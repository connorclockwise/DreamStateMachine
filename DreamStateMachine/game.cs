using System;
using System.Collections.Generic;
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
using System.Windows.Forms;

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
        InputHandler inputHandler;
        PhysicsController physicsController;
        PropManager propManager;
        WorldManager worldManager;

        Actor player;
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Random random;
        Rectangle tileRect;
        Point origin;
        Camera cam;
        SpriteBatch spriteBatch;

        bool paused;

        


        public delegate void GameUpdate(GameTime gameTime);
        public delegate void GameDraw(GameTime gameTime);
        public Stack<GameDraw> gameDrawStack;
        public Stack<GameUpdate> gameUpdateStack;
        GameUpdate gameUpdate;
        GameDraw gameDraw;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            var screen = Screen.AllScreens.First(e => e.Primary);
            Window.IsBorderless = true;
            Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
            graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            graphics.PreferredBackBufferHeight = screen.Bounds.Height;
            graphics.IsFullScreen = true;
            //graphics.ToggleFullScreen();
            //graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            gameDrawStack = new Stack<GameDraw>();
            gameUpdateStack = new Stack<GameUpdate>();
            gameDrawStack.Push(StartMenuUpdate);
            gameUpdateStack.Push(StartMenuDraw);
            //gameDrawStack.Enqueue(MainGameUpdate);
            //gameUpdateStack.Enqueue(MainGameDraw);
            gameUpdate = gameUpdateStack.Peek();
            gameDraw = gameDrawStack.Peek();
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Use += new EventHandler<EventArgs>(Actor_Use);
            
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
            //Window.IsBorderless = true;
            //graphics.IsFullScreen = true;
            //graphics.ApplyChanges();
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

            propManager = new PropManager();
            propManager.initPropConfig(Content, "Content/Props.xml");

            cam.enterStartMenu();
            cam.NewGame += new EventHandler<EventArgs>(NewGameSelected);
            cam.Tutorial += new EventHandler<EventArgs>(TutorialSelected);
            cam.Credits += new EventHandler<EventArgs>(CreditsSelected);
            cam.CreditsExit += new EventHandler<EventArgs>(CreditsExited);
            cam.ExitPause += new EventHandler<EventArgs>(MainGameExited);
            inputHandler.pauseButtonPressed += new EventHandler<EventArgs>(Pause);
            WorldManager.worldChange += new EventHandler<EventArgs>(WorldManager_worldChange);

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
        }

        public void startTutorial()
        {
            worldManager.initTutorial();
        }

        public void MainGameUpdate(GameTime gameTime)
        {
             float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            UpdateInput();
            inputHandler.update(dt);
            if (!paused)
            {
               
                
                actorController.update(dt);
                aiController.update(dt);
                physicsController.update(dt);
                SoundManager.Instance.update(dt);
                cam.gameUpdate(dt);
                cam.notificationsUpdate(dt);

                base.Update(gameTime);
            }
        }

        //public void LoadWorldUpdate(GameTime gameTime)
        //{
        //    while (isLoadingWorld)
        //    {
        //        float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
        //        //UpdateInput();
        //        //actorManager.update(dt);
        //        //UpdateCamera(cam, player);

        //        base.Update(gameTime);
        //    }
        //    gameUpdateStack.Pop();
        //    gameDrawStack.Pop();

        //    //gameUpdate = MainGameUpdate;
        //    //gameDraw = MainGameDraw;
        //}

        public void StartMenuUpdate(GameTime gameTime)
        {
            float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            cam.startMenuUpdate(dt);
            UpdateInput();
        }

        public void CreditsUpdate(GameTime gameTime)
        {
            float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            cam.creditsUpdate(dt);
            UpdateInput();
        }

        private void UpdateInput()
        {
            
            if (player != null)
            {
                List<Command> commands = inputHandler.handleInput();
                foreach (Command c in commands)
                {
                    if (player != null)
                        c.Execute(player);
                }
            }
            
            if (cam.menuEnabled && cam.rootGUIElement != null && inputHandler.controller)
            {
                List<Command> commands = inputHandler.handleInput();
                foreach (Command c in commands)
                    c.Execute(cam.rootGUIElement);
            }

            if ((cam.creditsEnabled || cam.menuEnabled) && cam.rootGUIElement != null)
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
                cam.drawFloor();
                cam.drawProps();
                cam.drawActors();
                cam.drawGUI();
                if (paused)
                    cam.drawPauseMenu();
            spriteBatch.End();

            base.Draw(gameTime);

        }

        public void CreditsDraw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                cam.drawCredits();
            spriteBatch.End();

            base.Draw(gameTime);

        }

        

        //public void LoadWorldDraw(GameTime gameTime)
        //{

        //    GraphicsDevice.Clear(Color.Black);

        //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
        //    // Draw Hello World
        //    string output = "Loading World";

        //    // Find the center of the string
        //    Vector2 FontOrigin = arielBlackFont.MeasureString(output) / 2;
        //    Vector2 screenorigin = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
        //    // Draw the string
        //    spriteBatch.DrawString(arielBlackFont, output, screenorigin, Color.White,
        //        0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        //    spriteBatch.End();

        //    base.Draw(gameTime);

        //}

        public void LoadNextWorld()
        {

        }

        public void NewGameSelected(Object sender, EventArgs eventArgs)
        {
            //Console.Write("new game selected");
            cam.menuEnabled = false;
            startNewGame();
            //SoundManager.Instance.stopAllSounds();

            gameUpdateStack.Push(MainGameUpdate);
            gameDrawStack.Push(MainGameDraw);
            gameUpdate = gameUpdateStack.Peek();
            gameDraw = gameDrawStack.Peek();
        }

        public void TutorialSelected(Object sender, EventArgs eventArgs)
        {
            cam.menuEnabled = false;
            startTutorial();

            gameUpdateStack.Push(MainGameUpdate);
            gameDrawStack.Push(MainGameDraw);
            gameUpdate = gameUpdateStack.Peek();
            gameDraw = gameDrawStack.Peek();
        }

        public void CreditsSelected(Object sender, EventArgs eventArgs)
        {
            cam.menuEnabled = false;
            cam.creditsEnabled = true;
            cam.rollCredits();
            gameUpdateStack.Push(CreditsUpdate);
            gameDrawStack.Push(CreditsDraw);
            gameUpdate = gameUpdateStack.Peek();
            gameDraw = gameDrawStack.Peek();
            SoundManager.Instance.playSong("creditsTheme");
        }

        public void CreditsExited(Object sender, EventArgs eventArgs)
        {
            cam.creditsEnabled = false;
            SoundManager.Instance.stopAllSounds();
            cam.enterStartMenu();
            cam.drawSpace.X = 0;
            cam.drawSpace.Y = 0;
            gameUpdateStack.Pop();
            gameDrawStack.Pop();
            gameUpdate = gameUpdateStack.Peek();
            gameDraw = gameDrawStack.Peek();
            SoundManager.Instance.stopSong("creditsTheme");
        }

        public void MainGameExited(Object sender, EventArgs eventArgs){
            if(gameUpdateStack.Count > 1)
            {
                paused = false;
                player = null;
                cam.tutorialGui.Clear();
                SoundManager.Instance.stopAllSounds();
                cam.enterStartMenu();
                cam.drawSpace.X = 0;
                cam.drawSpace.Y = 0;
                gameUpdateStack.Pop();
                gameDrawStack.Pop();
                gameUpdate = gameUpdateStack.Peek();
                gameDraw = gameDrawStack.Peek();
                SoundManager.Instance.stopSong("creditsTheme");
            }
            
        }


        private void Actor_Spawn(Object sender, EventArgs eventArgs)
        {
            Actor actor = (Actor)sender;
            if (actor.className == "player")
                player = actor;
        }

        private void Actor_Use(Object sender, EventArgs eventArgs)
        {
            Actor deadActor = (Actor)sender;
            if (deadActor.className == "player" && deadActor.health <= 0 && gameUpdateStack.Count > 1)
            {
                player = null;
                SoundManager.Instance.stopAllSounds();

                cam.enterStartMenu();
                cam.drawSpace.X = 0;
                cam.drawSpace.Y = 0;
                worldManager.restart();
                gameUpdateStack.Pop();
                gameDrawStack.Pop();
                gameUpdate = gameUpdateStack.Peek();
                gameDraw = gameDrawStack.Peek();
            }
        }

        private void Pause(Object sender, EventArgs eventArgs)
        {
            if (!paused)
                cam.enterPauseMenu();
            else
                cam.menuEnabled = false;
            paused = !paused;

        }

        private void WorldManager_worldChange(Object sender, EventArgs eventArgs)
        {
            WorldManager worldManager = (WorldManager)sender;
            if (worldManager.curWorld != null && worldManager.curWorld.isTutorial && gameUpdateStack.Count > 1)
            {
                player = null;
                SoundManager.Instance.stopAllSounds();

                cam.enterStartMenu();
                cam.drawSpace.X = 0;
                cam.drawSpace.Y = 0;
                worldManager.restart();
                gameUpdateStack.Pop();
                gameDrawStack.Pop();
                gameUpdate = gameUpdateStack.Peek();
                gameDraw = gameDrawStack.Peek();
            }
        }
    }
}
