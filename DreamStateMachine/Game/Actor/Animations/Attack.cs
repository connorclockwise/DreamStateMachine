using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Attack:Animation
    {
        ActionList ownerList;
        Actor owner;
        AnimationInfo animationInfo;
        FrameInfo frameInfo;
        Point frameIndex;
        Rectangle damageRect;
        List<Rectangle> attackBoxes;

        public Attack(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            isBlocking = true;
            frameIndex = new Point(0, 0);
            damageRect = new Rectangle(0, 0, 0, 0);
            attackBoxes = new List<Rectangle>();
            setAnimationInfo();
            duration = ((float)animationInfo.frameCount)/ animationInfo.fps;
            curFrame = 0;
            lastFrame = -1;

            owner.HitActor += Actor_Hit;
        }

        private void setAnimationInfo()
        {
            if (owner.activeWeapon != null && owner.animations.ContainsKey(owner.activeWeapon.animations["light_weapon_attack"]))
            {
                this.animationInfo = owner.animations[owner.activeWeapon.animations["light_weapon_attack"]];
            }
            else if (owner.animations.ContainsKey("unnarmed_light_attack"))
            {
                this.animationInfo = owner.animations["unnarmed_light_attack"];
            }
        }

        override public void onStart()
        {
        }

        override public void onEnd()
        {
            //owner.setAnimationFrame(0, 0);
            ownerList.remove(this);
        }

        public override void onEnterFrame(int frame)
        {
            setAnimationInfo();
            frameIndex.X = animationInfo.texColumn + curFrame;
            frameIndex.Y = animationInfo.texRow;
            owner.setAnimationFrame(frameIndex.X, frameIndex.Y);


            if (frame < animationInfo.frames.Length)
            {
                frameInfo = animationInfo.frames[frame];
                if (owner.activeWeapon != null)
                {
                    owner.gripPoint = frameInfo.gripPoint;
                    owner.activeWeapon.setStance(frameInfo.stance);
                    owner.activeWeapon.frameRotation = frameInfo.rotation;
                    
                }
                if (frameInfo.attackPoints != null && frameInfo.attackPoints.Count > 0)
                {
                    attackBoxes = new List<Rectangle>();
                    foreach (Rectangle attackPoint in frameInfo.attackPoints)
                    {
                        Point offset = new Point(attackPoint.X, attackPoint.Y);
                        float s = (float)(Math.Sin(owner.bodyRotation));
                        float c = (float)(Math.Cos(owner.bodyRotation));
                        Point newOffset = new Point((int)(offset.X * c - offset.Y * s), (int)(offset.X * s + offset.Y * c));
                        damageRect = new Rectangle();
                        damageRect.Width = attackPoint.Width;
                        damageRect.Height = attackPoint.Height;
                        damageRect.X = owner.hitBox.Center.X + newOffset.X - damageRect.Width / 2;
                        damageRect.Y = owner.hitBox.Center.Y + newOffset.Y - damageRect.Height / 2;
                        
                        attackBoxes.Add(damageRect);
                    }
                    DamageInfo damageInfo = new DamageInfo(owner, frameInfo.attackDamage, this.attackBoxes);
                    owner.onAttack(damageInfo);
                    attackBoxes.Clear();
                }


            }
        }

        override public void update(float dt)
        {
            elapsed += dt;
            curFrame = (int)(elapsed * animationInfo.fps);
            if (curFrame < animationInfo.frameCount)
            {
                if (curFrame != lastFrame)
                {
                    onEnterFrame(curFrame);
                    lastFrame = curFrame;
                }
            }
            else
            {
                onEnd();
            }
        }

        private void Actor_Hit(object sender, EventArgs eventArgs)
        {
        }
    }
}
