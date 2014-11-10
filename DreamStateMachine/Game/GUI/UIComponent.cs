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

        public static event EventHandler<EventArgs> Initialize;
        public static event EventHandler<EventArgs> Remove;

        public UIComponent parent;
        public List<UIComponent> children;
        public Rectangle dimensions;
        public bool hasFocus = false;
        public bool canHaveFocus = false;
        public abstract void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false);
        public abstract bool isInDrawSpace(Rectangle drawSpace);
        public abstract UIComponent findFocusedChild();
        public delegate void clickDelegate();
        public clickDelegate onClick;
        public abstract void giveFocus();
        public abstract void takeFocus();
        public abstract void setPos(Point pos);
        public abstract void setPos(int x, int y);

        public UIComponent()
        {
            children = new List<UIComponent>();
        }

        public void rotateFocusNext()
        {
            UIComponent neighbor;
            neighbor = findNextNeighbor();
            while (neighbor != null)
            {
                if (neighbor.canHaveFocus)
                {
                    neighbor.giveFocus();
                    neighbor = null;
                }
                else {
                    neighbor = findNextNeighbor();
                }
            }
        }

        public void rotateFocusPrev()
        {
            UIComponent neighbor;
            neighbor = findPrevNeighbor();
            while (neighbor != null)
            {
                if (neighbor.canHaveFocus)
                {
                    neighbor.giveFocus();
                    neighbor = null;
                }
                else
                {
                    neighbor = findPrevNeighbor();
                }
            }
        }

        public UIComponent findNextNeighbor()
        {
            if (parent.children.Count > 0)
            {
                int curIndex = parent.children.IndexOf(this);
                curIndex++;
                UIComponent curChild;
                if (curIndex == parent.children.Count)
                {
                    curIndex = 0;
                    curChild = parent.children[curIndex];
                }
                else
                {
                    curChild = parent.children[curIndex];
                }
                return curChild;
            }
            return null;
        }

        public UIComponent findPrevNeighbor()
        {
            if (parent.children.Count > 0)
            {
                int curIndex = parent.children.IndexOf(this);
                curIndex--;
                UIComponent curChild;
                if (curIndex == -1)
                {
                    curIndex = parent.children.Count - 1;
                    curChild = parent.children[curIndex];
                }
                else
                {
                    curChild = parent.children[curIndex];
                }
                return curChild;
            }
            return null;
        }

        public void initialize()
        {
            Initialize(this, EventArgs.Empty);
            foreach (UIComponent uiChild in children)
            {
                uiChild.initialize();
            }
        }

        public void remove()
        {
            Remove(this, EventArgs.Empty);
            foreach (UIComponent uiChild in children)
            {
                uiChild.remove();
            }
        }

        public virtual void update(float dt)
        {
        }
    }
}
