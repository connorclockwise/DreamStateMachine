using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace DreamStateMachine.Input
{
    class MoveCommand: Command
    {
        Vector2 movement;
        public MoveCommand(Vector2 movement)
        {
            this.movement = movement;
        }
        public override void Execute(Actor player)
        {
            if (!player.lockedMovement && movement.Length() > .1f)
            {
                movement.Normalize();
                player.movementIntent = movement * 0.7f;
                player.isWalking = true;
            }
            else
            {
                player.movementIntent.X = 0;
                player.movementIntent.Y = 0;
                player.isWalking = false;
            }
        }
    }
}
