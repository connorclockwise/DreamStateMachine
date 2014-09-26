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

        public Attack(ActionList ownerList, Actor owner, AnimationInfo animationInfo)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            this.animationInfo = animationInfo;
            isBlocking = true;
            frameIndex = new Point(0, 0);
            damageRect = new Rectangle(0, 0, 0, 0);
            attackBox = new Rectangle(0, 0, 0, 0);
            duration = ((float)animationInfo.frames )/ animationInfo.fps;
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

            frameIndex.X = animationInfo.texColumn + frame;
            frameIndex.Y = animationInfo.texRow;
            owner.setAnimationFrame(frameIndex.X, frameIndex.Y);

            if (animationInfo.attackPoints.ContainsKey(frame))
            {
                damageRect.Width = animationInfo.attackPoints[curFrame][0].Width;
                damageRect.Height = animationInfo.attackPoints[curFrame][0].Height;
                damageRect.X = (int)Math.Cos(owner.bodyRotation) * animationInfo.attackPoints[curFrame][0].X;
                damageRect.Y = (int)Math.Sin(owner.bodyRotation) * animationInfo.attackPoints[curFrame][0].Y;
                damageRect.X -= damageRect.Width / 2;
                damageRect.Y -= damageRect.Height / 2;
                DamageInfo damageInfo = new DamageInfo(owner, animationInfo.attackDamage[curFrame], damageRect);
                owner.onAttack(damageInfo);
            }
        }

        override public void update(float dt)
        {
            elapsed += dt;
            curFrame = (int)(elapsed * animationInfo.fps);
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
