using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Walk:Animation
    {
        AnimationInfo animationInfo;
        Actor owner;
        Stance curStance;
        Vector2 walkDirection;
        FrameInfo frameInfo;
        double dotProduct;

        public Walk(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            setAnimationInfo();
            isBlocking = true;
            duration = ((float)animationInfo.frameCount) / animationInfo.fps;
            curFrame = 0;
            lastFrame = -1;
        }

        //dummy constructor
        public Walk()
        {
        }

        private void setAnimationInfo()
        {
            if (owner.activeWeapon != null)
            {
                animationInfo = owner.animations[owner.activeWeapon.animations["holding_weapon_walk"]];
            }
            else
            {
                animationInfo = owner.animations["unarmed_walk"];
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
            if (owner.isWalking)
                elapsed = 0;
            else
                ownerList.remove(this);

            //owner.setAnimationFrame(0, animationInfo.texRow);
            

        }

        override public void update(float dt)
        {
            
            if (owner.isWalking){

                curFrame = (int)(elapsed * animationInfo.fps);
                if (curFrame >= animationInfo.frameCount)
                    curFrame = 0;

                if (curFrame != lastFrame)
                {
                    onEnterFrame(curFrame);
                    lastFrame = curFrame;
                }

                walkDirection.X = owner.velocity.X;
                walkDirection.Y = owner.velocity.Y;
                walkDirection.Normalize();
                dotProduct = Vector2.Dot(walkDirection, owner.sightVector);

                if (dotProduct > .5)
                {
                    elapsed += dt;           
                }
                else if (dotProduct < -.5)
                {
                    elapsed -= dt;
                    if (elapsed <= 0)
                    {
                        elapsed = (duration - .0001f);
                    }
                }
            }else{
                this.onEnd();
            }
        }
    }
}
