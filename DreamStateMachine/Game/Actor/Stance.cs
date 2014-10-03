using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class Stance
    {
        public String name;
        public Vector2 gripPoint;
        public Rectangle drawBox;
        

        public Stance(String name, Vector2 gripPoint, Rectangle drawBox)
        {
            this.name = name;
            this.gripPoint = gripPoint;
            this.drawBox = drawBox;
        }

        //public Object Clone(){
        //    Weapon clonedWeapon = new Weapon(this.tex, this.walk
        //}

    }
}
