using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    struct Stance
    {
        public String name;
        public Vector2 gripPoint;
        public Rectangle drawBox;
    }


    class Weapon: ICloneable, IDrawable
    {
        public Dictionary<String, String> animations;
        public Dictionary<String, Stance> stances;
        public String name;
        public Stance curStance;
        public Texture2D tex;
        public Rectangle dimensions;
        public float frameRotation;
        public float rotation;

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

        public void calcWeaponPos(Actor owner)
        {
            Vector2 gripVector = new Vector2(owner.gripPoint.X - owner.body.Width / 2,
                                        -((owner.body.Height - owner.gripPoint.Y) - owner.body.Height / 2));
            Point rotatedGrip = new Point();
            rotatedGrip.X = (int)(gripVector.X * Math.Cos(owner.bodyRotation)) - (int)(gripVector.Y * Math.Sin(owner.bodyRotation));
            rotatedGrip.Y = (int)(gripVector.X * Math.Sin(owner.bodyRotation)) + (int)(gripVector.Y * Math.Cos(owner.bodyRotation));

            owner.activeWeapon.dimensions.X = owner.body.Center.X + rotatedGrip.X;
            owner.activeWeapon.dimensions.Y = owner.body.Center.Y + rotatedGrip.Y;
            owner.activeWeapon.rotation = owner.bodyRotation + owner.activeWeapon.frameRotation;
        }

        public void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            
            if (drawSpace.Intersects(dimensions))
            {
                Rectangle normalizedRect = new Rectangle(dimensions.X - drawSpace.X,
                                                     dimensions.Y - drawSpace.Y,
                                                     dimensions.Width,
                                                     dimensions.Height);

                spriteBatch.Draw(tex,
                                 normalizedRect,
                                 curStance.drawBox,
                                 Color.White,
                                 rotation,
                                 curStance.gripPoint,
                                 SpriteEffects.None,
                                 0);
            }
        }

        public void setStance(String name)
        {
            if (stances.ContainsKey(name))
            {
                dimensions.Width = stances[name].drawBox.Width;
                dimensions.Height = stances[name].drawBox.Height;
                curStance = stances[name];
            }
        }

    }
}
