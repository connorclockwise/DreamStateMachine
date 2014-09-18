using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamStateMachine.Actions;

namespace DreamStateMachine.Input
{
    class PunchCommand: Command
    {
        public override void Execute(Actor player)
        {
            Punch punchAnimation = new Punch(player.animationList, player);
            if (!player.animationList.has(punchAnimation))
                player.animationList.pushFront(punchAnimation);
        }
    }
}
