using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    abstract class Animation:Action
    {
        public abstract void onEnterFrame(int frame);
        public int curFrame;
        public int lastFrame;
    };
}
