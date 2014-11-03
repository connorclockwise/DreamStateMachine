using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//using MonoGame.Framework;
using DreamStateMachine.Actions;


namespace DreamStateMachine
{
    class Prop:ICloneable, IDrawable
    {
        public static event EventHandler<SpawnEventArgs> Spawn;
        public static event EventHandler<EventArgs> Remove;


        //public Attributes attributes;
        public Color color;
        public Point curAnimFrame;
        public Texture2D texture;
        
        public Rectangle hitBox;
        public Rectangle body;
        public String className;
        public float rotation;
       
        public World world;
        //public Weapon activeWeapon;
        

        public Prop(Texture2D tex, int width, int height, int texWidth, int texHeight)
        {
            texture = tex;
            
            color = new Color(255, 255, 255, 255);
            
            hitBox = new Rectangle(0, 0, width, height);
            body = new Rectangle(0, 0, texWidth, texHeight);
           
        }

        public virtual Object Clone()
        {
            Prop PropCopy = new Prop(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);
            
            PropCopy.className = className;
            PropCopy.texture = texture;
            PropCopy.color = color;

            return PropCopy;
        }


        public void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            Rectangle normalizedPosition;
            Rectangle sourceRectangle;
            Texture2D tex;
            tex = this.getTexture();
            normalizedPosition = new Rectangle(this.body.X - drawSpace.X,
                                               this.body.Y - drawSpace.Y,
                                               this.body.Width,
                                               this.body.Height);


            sourceRectangle = new Rectangle(this.curAnimFrame.X * this.body.Width,
                                            this.curAnimFrame.Y * this.body.Height,
                                            this.body.Width,
                                            this.body.Height);

            spriteBatch.Draw(
                tex,
                new Vector2(normalizedPosition.X + this.body.Width / 2, normalizedPosition.Y + this.body.Height / 2),
                sourceRectangle,
                this.color,
                this.rotation,
                new Vector2(normalizedPosition.Width / 2.0f, normalizedPosition.Height / 2.0f),
                1,
                SpriteEffects.None,
                0
            );
        }

        public bool isInDrawSpace(Rectangle drawSpace)
        {
            return this.hitBox.Intersects(drawSpace);
        }

        public void onSpawn( Point spawnTile, int spawnType )
        {
            SpawnEventArgs spawnEventArgs = new SpawnEventArgs(spawnTile, spawnType);
            Spawn(this, spawnEventArgs);
        }

        public void onUse()
        {
            
        }

        public void setAnimationFrame(int x, int y)
        {
            curAnimFrame.X = x;
            curAnimFrame.Y = y;
        }


        public void setPos(int x, int y)
        {
            hitBox.X = x;
            hitBox.Y = y;
            body.X = hitBox.Center.X - (int)(body.Width / 2.0);
            body.Y = hitBox.Center.Y - (int)(body.Height / 2.0);
        }

        public void setPos(Point point)
        {
            hitBox.X = point.X;
            hitBox.Y = point.Y;
            body.X = hitBox.Center.X - (int)(body.Width / 2.0);
            body.Y = hitBox.Center.Y - (int)(body.Height / 2.0);
        }

        public Rectangle getHitBox()
        {
            return hitBox;
        }

        public Rectangle getBodyBox()
        {
            return body;
        }

        public Texture2D getTexture()
        {
            return texture;
        }

        public void onRemove()
        {
            Remove(this, EventArgs.Empty);
        }

        virtual public void update(float dt)
        {
        }
    }
}
