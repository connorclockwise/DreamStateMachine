using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DreamStateMachine.Input
{
    class InputHandler
    {

        public event EventHandler<EventArgs> pauseButtonPressed;

        public bool controller = false;
        float pauseCoolDown = 0;
        KeyboardState keyBoardState;
        MouseState mouseState;
        GamePadState padState;
        Point origin;

        public InputHandler(Point origin)
        {
            this.origin = origin;
        }

        public List<Command> handleInput() 
        {
            List<Command> c;
            if (controller)
                c = handleController();
            else
                c = handleKeyBoardMouse();
            return c;
        }

        private List<Command> handleController()
        {
            List<Command> commands = new List<Command>();
            padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            
            Vector2 move = new Vector2();
            move.X = 0;
            move.Y = 0;
            if (padState.ThumbSticks.Left.Length() > .7f)
            {
                move.X = padState.ThumbSticks.Left.X * .7f;
                move.Y = -padState.ThumbSticks.Left.Y * .7f;   
            }
            commands.Add(new MoveCommand(move));


            Vector2 gazeDir = new Vector2();
            gazeDir.X = padState.ThumbSticks.Right.X;
            gazeDir.Y = -padState.ThumbSticks.Right.Y;
            commands.Add(new GazeCommand(gazeDir));

            if (padState.IsButtonDown(Buttons.RightShoulder))
                commands.Add(new PunchCommand());
            return commands;
        }

        private List<Command> handleKeyBoardMouse() 
        {
            List<Command> commands = new List<Command>();
            mouseState = Mouse.GetState();
            keyBoardState = Keyboard.GetState();

            Vector2 moveDir = new Vector2();
            moveDir.X = 0;
            moveDir.Y = 0;
            if (keyBoardState.IsKeyDown(Keys.A))
                moveDir.X = -.7f;
            if (keyBoardState.IsKeyDown(Keys.D))
                moveDir.X = .7f;
            if (keyBoardState.IsKeyDown(Keys.W))
                moveDir.Y = -.7f;
            if (keyBoardState.IsKeyDown(Keys.S))
                moveDir.Y = .7f;

            commands.Add(new MoveCommand(moveDir));

            if (keyBoardState.IsKeyDown(Keys.Space))
                commands.Add(new PunchCommand());
            if (keyBoardState.IsKeyDown(Keys.E))
            {

                commands.Add(new UseCommand());
            }
            //if (keyBoardState.IsKeyDown(Keys.G))
            //{
            //    commands.Add(new DebugCommand());
            //}
            if (keyBoardState.IsKeyDown(Keys.Q))
            {
                commands.Add(new RotateWeaponCommand());
            }
            if (keyBoardState.IsKeyDown(Keys.Escape))
            {
                if (pauseCoolDown <= 0)
                {
                    pauseButtonPressed(this, EventArgs.Empty);
                    pauseCoolDown = .25f;
                }
            }

            float curMousePosX = mouseState.X;
            float curMousePosY = mouseState.Y;

            Vector2 playerDir = new Vector2(curMousePosX - origin.X, curMousePosY - origin.Y);

            commands.Add(new GazeCommand(playerDir));

            if (mouseState.LeftButton == ButtonState.Pressed)
                commands.Add(new PunchCommand());
            
            return commands;
        }

        public void update(float dt)
        {
            if (pauseCoolDown > 0)
                pauseCoolDown -= dt;
        }

    }
}