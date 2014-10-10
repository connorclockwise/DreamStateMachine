using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreamStateMachine.Game.GUI
{
    abstract class UIComponent:IDrawable
    {
        public UIComponent parent;
        public List<UIComponent> children;
        public Rectangle dimensions;
        public bool hasFocus;
        public abstract void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false);
        public abstract bool isInDrawSpace(Rectangle drawSpace);
    }
}
