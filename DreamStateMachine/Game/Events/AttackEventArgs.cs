using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine
{
    class AttackEventArgs : EventArgs
    {
        public DamageInfo damageInfo;

        public AttackEventArgs(DamageInfo damageInfo):base()
        {
            this.damageInfo = damageInfo;
        }
    }
}
