using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine.Input
{
    abstract class Command
    {
        public abstract void Execute(Actor player);
        public abstract void Execute(UIComponent uiComponent);
    }
}
