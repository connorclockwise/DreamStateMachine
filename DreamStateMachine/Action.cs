using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine
{
    abstract class Action
    {
        public abstract void update( float dt );
        public abstract void onStart();
        public abstract void onEnd();
        public bool isFinished = false;
        public bool isStarted = false;
        public bool isBlocking = false;
        public uint lanes;
        public float startTime;
        public float elapsed;
        public float duration;
 
        private ActionList ownerList;
    };
}
