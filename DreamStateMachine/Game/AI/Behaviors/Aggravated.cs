using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Aggravated:Behavior
    {
        Actor owner;
        Actor target;
        Point attackPos;
        Vector2 attackVector;
        Vector2 movement;
        float attackCoolDown;
        float pursueCountDown;
        TraceInfo traceInfo;

        public Aggravated(ActionList ownerList, Actor owner, Actor toFollow)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            target = toFollow;

            nextPathPoint = new Point(0,0);
            elapsed = 0;
            duration = -1;
            attackCoolDown = 0;
            pursueCountDown = (2.5f);
            isBlocking = true;
        }

        //dummy constructor
        public Aggravated(Action ownerList, Actor owner)
        {
        }

        override public void onStart()
        {
        }

        override public void onEnd()
        {
            ownerList.remove(this);
        }

        override public void update(float dt)
        {
            if (pursueCountDown >= 0)
            {
                pursueCountDown -= dt;

                attackVector = new Vector2(owner.hitBox.Center.X - target.hitBox.Center.X, owner.hitBox.Center.Y - target.hitBox.Center.Y);
                attackVector.Normalize();
                attackVector *= owner.reach;
                attackVector.X += target.hitBox.Center.X;
                attackVector.Y += target.hitBox.Center.Y;
                double distance = Math.Sqrt(Math.Pow(attackVector.X - owner.hitBox.Center.X, 2) + Math.Pow(attackVector.Y - owner.hitBox.Center.Y, 2));
                if (distance > owner.reach)
                {
                    attackPos = new Point((int)attackVector.X, (int)attackVector.Y);
                    movement = new Vector2((float)(attackPos.X - owner.hitBox.Center.X), (float)(attackPos.Y - owner.hitBox.Center.Y));
                    movement.Normalize();
                    movement *= owner.maxSpeed;
                    owner.velocity.X = movement.X;
                    owner.velocity.Y = movement.Y;
                    owner.isWalking = true;
                    owner.setGaze(attackPos);
                }
                else
                {
                    if (attackCoolDown <= 0)
                    {
                        owner.setGaze(target.hitBox.Center);
                        owner.Light_Attack();
                        attackCoolDown = (float)(.5f);
                    }
                    else
                    {
                        attackCoolDown -= dt;
                    }
                }
                
            }
            else
            {
                elapsed = 0;
                traceInfo = owner.world.traceWorld(owner.hitBox.Center, target.hitBox.Center);
                if (traceInfo.hitWorld)
                {
                    Pursue pursue = new Pursue(ownerList, owner, target);
                    ownerList.pushFront(pursue);
                    //onEnd();
                }
                else
                    pursueCountDown = 2.5f;
            }
        }

        //void Actor_Hurt(Object sender, AttackEventArgs attackEventArgs)
        //{
        //    this.onEnd();
        //}
    }
}
