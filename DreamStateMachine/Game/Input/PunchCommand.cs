using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework;

namespace DreamStateMachine.Input
{
    class PunchCommand: Command
    {
        public override void Execute(Actor player)
        {
            player.Light_Attack();
        }

        public override void Execute(Game.GUI.UIComponent uiComponent)
        {
            throw new NotImplementedException();
        }
    }
}
