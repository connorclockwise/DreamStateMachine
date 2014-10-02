using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Attack:Action
    {
        ActionList ownerList;
        Actor owner;
        AnimationInfo animationInfo;
        Point frameIndex;
        Rectangle damageRect;
        Rectangle attackBox;
        int curFrame;
        int lastFrame;

        public Attack(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            isBlocking = true;
            frameIndex = new Point(0, 0);
            damageRect = new Rectangle(0, 0, 0, 0);
            attackBox = new Rectangle(0, 0, 0, 0);
            if(owner.activeWeapon != null && owner.animations.ContainsKey(owner.activeWeapon.lightAttackAnimation)){
                this.animationInfo = owner.animations[owner.activeWeapon.lightAttackAnimation];
            }
            else if (owner.animations.ContainsKey("unnarmed_light_attack"))
            {
                this.animationInfo = owner.animations["unnarmed_light_attack"];
            }
            duration = ((float)animationInfo.frames)/ animationInfo.fps;
            curFrame = 0;
            lastFrame = -1;

            owner.HitActor += Actor_Hit;
        }

        override public void onStart()
        {
        }

        override public void onEnd()
        {
            owner.setAnimationFrame(0, 0);
            ownerList.remove(this);
        }

        public void onEnterFrame(int frame)
        {
            if (animationInfo.attackPoints.ContainsKey(frame))
            {
                damageRect.Width = animationInfo.attackPoints[curFrame][0].Width;
                damageRect.Height = animationInfo.attackPoints[curFrame][0].Height;

                Point offset = new Point(animationInfo.attackPoints[curFrame][0].X, animationInfo.attackPoints[curFrame][0].Y);
                float s = (float)(Math.Sin(owner.bodyRotation));
                float c = (float)(Math.Cos(owner.bodyRotation));
                Point newOffset = new Point((int)(offset.X * c - offset.Y * s), (int)(offset.X * s + offset.Y * c));
                damageRect.X = owner.hitBox.Center.X + newOffset.X - damageRect.Width/2;
                damageRect.Y = owner.hitBox.Center.Y + newOffset.Y - damageRect.Height/2;
                DamageInfo damageInfo = new DamageInfo(owner, animationInfo.attackDamage[curFrame], this.damageRect);
                owner.onAttack(damageInfo);
            }
        }

        override public void update(float dt)
        {
            elapsed += dt;
            curFrame = (int)(elapsed * animationInfo.fps);
            frameIndex.X = animationInfo.texColumn + curFrame;
            frameIndex.Y = animationInfo.texRow;
            owner.setAnimationFrame(frameIndex.X, frameIndex.Y);
            if (curFrame != lastFrame)
            {
                onEnterFrame(curFrame);
                lastFrame = curFrame;
            }
        }

        private void Actor_Hit(object sender, EventArgs eventArgs)
        {
        }
    }
}
