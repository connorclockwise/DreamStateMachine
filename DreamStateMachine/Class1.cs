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
using Microsoft.Xna.Framework.Storage;

namespace InputDisconnect
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        // Game states.
        enum GameState
        {
            InputSetup = 0,
            Game,
            Disconnected
        }

        // Current state of the game.
        GameState gameState = GameState.InputSetup;

        // Controllers that are in use.
        bool[] activeControllers = new bool[4];

        // Disconnected controller detected.
        bool disconnectDetected = false;

        // Background color.  Use this to indicate a disconnect.
        Color backColor = Color.CornflowerBlue;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allow the game to exit.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();

            UpdateInput();

            switch (gameState)
            {
                case GameState.InputSetup:
                    // Allow players to join the game, 
                    // and determine active controllers.
                    // In this example, there is only one player.
                    activeControllers[0] = true;

                    // When ready, proceed to the game.
                    gameState = GameState.Game;
                    break;

                case GameState.Game:
                    // If disconnected, go to the disconnect loop.
                    if (disconnectDetected)
                    {
                        backColor = Color.Black;
                        gameState = GameState.Disconnected;
                    }
                    break;

                case GameState.Disconnected:
                    // If reconnected, continue to the game.
                    if (!disconnectDetected)
                    {
                        backColor = Color.CornflowerBlue;
                        gameState = GameState.Game;
                    }
                    // Otherwise, pause the game and display a message.
                    break;
            }

            base.Update(gameTime);
        }

        void UpdateInput()
        {
            disconnectDetected = false;
            PlayerIndex index = PlayerIndex.One;
            for (int i = 0; i < 4; i++, index++)
            {
                if (activeControllers[i] &&
                    !GamePad.GetState(index).IsConnected)
                {
                    disconnectDetected = true;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(backColor);
            base.Draw(gameTime);
        }
    }
}