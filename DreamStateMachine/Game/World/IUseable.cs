using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamStateMachine;

namespace DreamStateMachine2.game.World
{
    interface IUseable
    {
        void Use(Actor actor);
        bool CanUse(Actor actor);
    }
}
