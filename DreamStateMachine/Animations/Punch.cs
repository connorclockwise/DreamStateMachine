using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Punch:Action
    {
        ActionList ownerList;
        ActorManager actorManager;
        Actor owner;
        Rectangle attackBox;
        bool hitActor;
        int curFrame;
        int lastFrame;

        public Punch(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            isBlocking = true;
            attackBox = new Rectangle(0, 0, 9, 9);
            duration = (5f / 12f);
            curFrame = 0;
            hitActor = false;
        }

        override public void onStart()
        {
            //owner.lockMovement();
        }

        override public void onEnd()
        {
            owner.setAnimationFrame(0, 0);
            ownerList.remove(this);
        }

        override public void update(float dt)
        {
            elapsed += dt;
            curFrame = (int)(elapsed * 12);

            switch (curFrame)
            {
                case(0):
                    owner.setAnimationFrame(0, 0);
                    break;
                case(3):
                    if(!hitActor){
                        Point offset = new Point(0, 40);
                        float s = (float)(Math.Sin(owner.bodyRotation));
                        float c = (float)(Math.Cos(owner.bodyRotation));
                        Point newOffset = new Point((int)(offset.X * c - offset.Y * s), (int)(offset.X * s + offset.Y * c));
                        this.attackBox.X = owner.hitBox.Center.X + newOffset.X;
                        this.attackBox.Y = owner.hitBox.Center.Y + newOffset.Y;
                        this.attackBox.Width = 30;
                        this.attackBox.Height = 30;
                        owner.attackBox = this.attackBox;
                        owner.velocity.X += (float)Math.Cos(owner.bodyRotation + MathHelper.Pi / 2) * 2;
                        owner.velocity.Y += (float)Math.Sin(owner.bodyRotation + MathHelper.Pi / 2) * 2;
                        DamageInfo damageInfo = new DamageInfo(owner, 20, this.attackBox);
                        owner.onAttack(damageInfo);
                        //hitActor = actorManager.handleActorAttack(damageInfo);
                    }
                    owner.setAnimationFrame(curFrame, 1);
                    break;
                default:
                    owner.setAnimationFrame(curFrame, 1);
                    owner.isAttacking = false;
                    break;
            }
        }
    }
}
