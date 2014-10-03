using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class Weapon: ICloneable, IDrawable
    {
        public Dictionary<String, String> animations;
        public Dictionary<String, Stance> stances;
        public String name;
        public String curStance;
        public Texture2D tex;
        public Rectangle dimensions;

        public Weapon(String name, Texture2D tex, Dictionary<String, String> animations, Dictionary<String, Stance> stances)
        {
            this.stances = stances;
            this.animations = animations;
            this.name = name;
            this.tex = tex;
            dimensions = new Rectangle();
        }

        public Object Clone(){
            Weapon clonedWeapon = new Weapon(name, tex, animations, stances);
            return clonedWeapon;
        }

        public bool isInDrawSpace(Rectangle drawSpace)
        {
            return drawSpace.Intersects(dimensions);
        }

        public void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
        }

        public void setStance(String name)
        {
            if (stances.ContainsKey(name))
            {
                dimensions.Width = stances[name].drawBox.Width;
                dimensions.Height = stances[name].drawBox.Height;
                curStance = name;
            }
        }

    }
}
