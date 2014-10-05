using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine.Input
{
    class GazeCommand: Command
    {
        Vector2 gazeVec;
        public GazeCommand(Vector2 gazeVec)
        {
            this.gazeVec = gazeVec;
        }

        public override void Execute(Actor player)
        {
            gazeVec.Normalize();
            player.setGaze(gazeVec);
        }

    }
}
