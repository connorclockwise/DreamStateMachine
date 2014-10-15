using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamStateMachine.Actions;

namespace DreamStateMachine.Input
{
    class UseCommand: Command
    {
        public override void Execute(Actor player)
        {
            player.onUse();
        }


        public override void Execute(Game.GUI.UIComponent uiComponent)
        {
            throw new NotImplementedException();
        }
    }
}
