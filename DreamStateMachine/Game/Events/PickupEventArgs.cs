using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class PickupEventArgs : EventArgs
    {
        public String itemClassName;

        public PickupEventArgs(String itemClassName)
            : base()
        {
            this.itemClassName = itemClassName;
        }
    }
}
