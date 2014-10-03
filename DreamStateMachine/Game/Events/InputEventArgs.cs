using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class InputEventArgs : EventArgs
    {
        public String inputType;
        public Vector2 vectorControl;

        public InputEventArgs(String inputType): base()
        {
            this.inputType = inputType;
        }

        public InputEventArgs(String inputType, Vector2 vectorControl)
            : base()
        {
            this.inputType = inputType;
            this.vectorControl = vectorControl;
        }
    }
}
