using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Idle:Animation
    {
        AnimationInfo animationInfo;
        Actor owner;
        FrameInfo frameInfo;

        public Idle(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            setAnimationInfo();
            duration = -1;
            curFrame = 0;
            lastFrame = -1;
        }

        //dummy constructor
        public Idle()
        {
        }

        private void setAnimationInfo()
        {
            if (owner.activeWeapon != null)
            {
                animationInfo = owner.animations[owner.activeWeapon.animations["holding_weapon_idle"]];
            }
            else
            {
                animationInfo = owner.animations["unarmed_idle"];
            }
        }

        public override void onEnterFrame(int frame)
        {
            setAnimationInfo();
            owner.setAnimationFrame(frame, animationInfo.texRow);
            if (frame < animationInfo.frames.Length)
            {
                frameInfo = animationInfo.frames[frame];
                if (owner.activeWeapon != null)
                {
                    owner.gripPoint = frameInfo.gripPoint;
                    owner.activeWeapon.setStance(frameInfo.stance);
                    owner.activeWeapon.frameRotation = frameInfo.rotation;
                }
            }
        }

        override public void onStart()
        {
            elapsed = 0;
        }

        override public void onEnd()
        {
            
        }

        override public void update(float dt)
        {
            elapsed += dt;
            curFrame = (int)(elapsed * animationInfo.fps);
            if (curFrame >= animationInfo.frameCount)
            {
                curFrame = 0;
                elapsed = 0;
                onEnterFrame(curFrame);
                
                
            }
            if (curFrame != lastFrame)
            {
                onEnterFrame(curFrame);
                lastFrame = curFrame;
            }
        }
    }
}
