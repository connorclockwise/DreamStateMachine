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
        float coolDownTimer;

        public Aggravated(ActionList ownerList, Actor owner, Actor toFollow)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            target = toFollow;

            nextPathPoint = new Point(0,0);
            elapsed = 0;
            duration = -1;
            coolDownTimer = 0;
            isBlocking = true;
            //toFollow.onHurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
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
            //Console.WriteLine(path.Count);
            double distance = Math.Sqrt(Math.Pow(target.hitBox.Center.X - owner.hitBox.Center.X, 2) + Math.Pow(target.hitBox.Center.Y - owner.hitBox.Center.Y, 2));
            if (distance > owner.reach)
            {
                attackVector = new Vector2(owner.hitBox.Center.X - target.hitBox.Center.X, owner.hitBox.Center.Y - target.hitBox.Center.Y);
                attackVector.Normalize();
                attackVector *= owner.reach;
                attackVector.X += target.hitBox.Center.X;
                attackVector.Y += target.hitBox.Center.Y;
                attackPos = new Point((int)attackVector.X, (int)attackVector.Y);
                movement = new Vector2((float)(attackPos.X - owner.hitBox.Center.X), (float)(attackPos.Y - owner.hitBox.Center.Y));
                movement.Normalize();
                movement *= owner.maxSpeed;
                //owner.movementIntent /= 3f;
                owner.velocity.X = movement.X;
                owner.velocity.Y = movement.Y;
                owner.isWalking = true;
                owner.setGaze(attackPos);
            }else{
                if (coolDownTimer <= 0)
                {
                    owner.setGaze(target.hitBox.Center);
                    Punch punch = new Punch(owner.animationList, owner);
                    Recoil recoil = new Recoil(owner.animationList, owner);
                    if (!owner.animationList.has(punch) && !owner.animationList.has(recoil))
                    {
                        owner.animationList.pushFront(punch);
                    }
                    coolDownTimer = (float)(6 / 12f);
                }
                else
                {
                    coolDownTimer -= dt;
                }
            }
        }

        void Actor_Hurt(Object sender, AttackEventArgs attackEventArgs)
        {
            this.onEnd();
        }
    }
}
