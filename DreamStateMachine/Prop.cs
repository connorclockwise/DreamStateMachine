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
    class Prop:ICloneable
    {
        

        public static event EventHandler<EventArgs> Use;
        public static event EventHandler<SpawnEventArgs> Spawn;
        


        //public Attributes attributes;
        public Color color;
        public Point curAnimFrame;
        public Texture2D texture;
        
        public Rectangle hitBox;
        public Rectangle body;
        public String className;
       
        public World world;
        //public Weapon activeWeapon;

        


        

        public Prop(Texture2D tex, int width, int height, int texWidth, int texHeight)
        {
            texture = tex;
            
            color = new Color(255, 255, 255, 255);
            
            hitBox = new Rectangle(0, 0, width, height);
            body = new Rectangle(0, 0, texWidth, texHeight);

           
        }

        public Object Clone()
        {
            Prop PropCopy = new Prop(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);
            
            PropCopy.className = className;
            PropCopy.color = color;

            return PropCopy;
        }







        public void onSpawn( Point spawnTile, int spawnType )
        {
            SpawnEventArgs spawnEventArgs = new SpawnEventArgs(spawnTile, spawnType);
            Spawn(this, spawnEventArgs);
        }

        public void onUse()
        {
            Use(this, EventArgs.Empty);
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

        virtual public void update(float dt)
        {
            
        }
    }
}
