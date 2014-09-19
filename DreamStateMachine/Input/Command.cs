using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine.Input
{
    abstract class Command
    {
        public abstract void Execute(Actor player);
    }
}
