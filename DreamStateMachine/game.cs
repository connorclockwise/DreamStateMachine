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

namespace DreamStateMachine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        ActorController actorController;
        List<Actor> actors;
        Actor player;
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        KeyboardState keyBoardState;
        MouseState mouseState;
        Random random;
        Rectangle tileRect;
        Point origin;
        Camera cam;
        SpriteBatch spriteBatch;
        SpriteFont arielBlackFont;
        Texture2D debugSquare;
        Texture2D floorTiles;
        Texture2D playerTexture;
        WorldManager worldManager;
        bool isLoadingWorld;
        int curMousePosX;
        int curMousePosY;

        public delegate void GameUpdate( GameTime gameTime);
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
            arielBlackFont = Content.Load<SpriteFont>("SpriteFont1");

            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            origin.X = graphics.PreferredBackBufferWidth / 2;
            origin.Y = graphics.PreferredBackBufferHeight / 2;

            random = new Random();

            actors = new List<Actor>();
            worldManager = new WorldManager(random);
            worldManager.initWorldConfig(Content, "content/Worlds.xml");
            worldManager.initStartingWorld();
            actorController = new ActorController(worldManager, actors, random);
            actorController.initActorConfig(Content, "content/Actors.xml");

            //spawnTile = worldManager.curWorld.getSpawnTile();
            tileRect = new Rectangle(0,0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            cam = new Camera(spriteBatch, tileRect, worldManager, actors, debugSquare);
            player = new Actor(playerTexture, 25, 25, 64, 64);
            player.isPlayer = true;
            //Point spawnPosition = new Point(spawnTile.X * tileSize + tileSize / 2, spawnTile.Y * tileSize + tileSize / 2);
            actorController.spawnActor(player, worldManager.curWorld.getSpawnPos());
            actorController.spawnActors(worldManager.curWorld.getSpawns());
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
        }

        public void MainGameUpdate(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            UpdateInput();
            actorController.update(dt);
            cam.update(cam, player);

            base.Update(gameTime);
        }

        public void LoadWorldUpdate(GameTime gameTime)
        {
            while (isLoadingWorld)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                float dt = (gameTime.ElapsedGameTime.Seconds) + (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                //UpdateInput();
                //actorController.update(dt);
                //UpdateCamera(cam, player);

                base.Update(gameTime);
            }
            gameUpdate = MainGameUpdate;
            gameDraw = MainGameDraw;
        }

        private void UpdateInput()
        {
            keyBoardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            curMousePosX = mouseState.X;
            curMousePosY = mouseState.Y;

            Vector2 playerDirection = new Vector2(curMousePosX - origin.X, curMousePosY - origin.Y);
            playerDirection.Normalize();
            player.setGaze(playerDirection);

            if (!player.lockedMovement)
            {
                if (keyBoardState.IsKeyDown(Keys.A))
                {
                    player.movementIntent.X = -.7f;
                    player.isWalking = true;
                }

                if (keyBoardState.IsKeyDown(Keys.D))
                {
                    player.movementIntent.X = .7f;
                    player.isWalking = true;
                }
                if (keyBoardState.IsKeyDown(Keys.W))
                {
                    player.movementIntent.Y = -.7f;
                    player.isWalking = true;
                }

                if (keyBoardState.IsKeyDown(Keys.S))
                {
                    player.movementIntent.Y = .7f;
                    player.isWalking = true;
                }
            }
            if (keyBoardState.IsKeyUp(Keys.A) && keyBoardState.IsKeyUp(Keys.D))
            {
                player.movementIntent.X = 0;
            }
            if (keyBoardState.IsKeyUp(Keys.W) && keyBoardState.IsKeyUp(Keys.S))
            {
                player.movementIntent.Y = 0;
            }
            if (keyBoardState.IsKeyDown(Keys.F))
            {
                //Follow follow = new Follow(badGuy.behaviorList, badGuy, player, world);
                //if (!badGuy.behaviorList.has(follow))
                //    badGuy.behaviorList.pushFront(follow);
            }
            if (keyBoardState.IsKeyDown(Keys.E))
            {
                //Wander wander = new Wander(badGuy.behaviorList, badGuy, world, r);
                //if (!badGuy.behaviorList.has(wander))
                //    badGuy.behaviorList.pushFront(wander);
                Point mouseWorldPoint = new Point(curMousePosX + cam.drawSpace.X, curMousePosY + cam.drawSpace.Y);
                Vector2 playerToMouse = new Vector2(mouseWorldPoint.X - player.body.Center.X, mouseWorldPoint.Y - player.body.Center.Y);
                float length = playerToMouse.Length();
                if (length < worldManager.curWorld.tileSize)
                {
                    Point mouseTilePos = new Point(mouseWorldPoint.X / worldManager.curWorld.tileSize, mouseWorldPoint.Y / worldManager.curWorld.tileSize);
                    actorController.handleActorUse(player, mouseTilePos);
                    //LoadNextWorld();
                }
            }
            if (keyBoardState.IsKeyDown(Keys.G))
            {
                //gameUpdate = LoadWorldUpdate;
                //gameDraw = LoadWorldDraw;

                //isLoadingWorld = true;
                //LoadNextWorld();
                //Thread thread = new Thread(new ThreadStart(LoadNextWorld));
                //thread.IsBackground = true;
                //thread.Start();
            }


            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Punch punchAnimation = new Punch(player.animationList, player, actorController);
                if (!player.animationList.has(punchAnimation))
                    player.animationList.pushFront(punchAnimation);
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
            cam.DrawFloor();
            cam.DrawActors();
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
                Vector2 screenorigin = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight/2);
                // Draw the string
                spriteBatch.DrawString(arielBlackFont, output, screenorigin, Color.White,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();

            base.Draw(gameTime);

        }

        public void LoadNextWorld()
        {
            
        }

        
    }
}
