using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;

namespace DreamStateMachine
{
    class Enemy:Actor
    {
        public ActionList behaviorList;
        public int sight;
      
        public Enemy(Texture2D tex, int width, int height, int texWidth, int texHeight) : base(tex, width, height, texWidth, texHeight)
        {
            behaviorList = new ActionList(this);
            sight = 240;
        }

        //override public void onHurt(DamageInfo damageInfo, World world)
        //{
        //    base.onHurt(damageInfo);
            //Stunned stunned = new Stunned(behaviorList, this);
            //if (!behaviorList.has(stunned))
            //    behaviorList.pushFront(stunned);
            //Aggravated aggravated = new Aggravated(behaviorList, this, damageInfo.attacker, world, );
            //if (!behaviorList.has(stunned))
            //    behaviorList.pushFront(stunned);
            
        //}

        //override public void onSpawn(){
        //    Idle idle = new Idle(behaviorList, this, World w, r);
        //    behaviorList.pushFront(idle);
        //}

        override public void update(float dt)
        {
            base.update(dt);
            behaviorList.update(dt);
            
        }

        public void walkToPoint(){
        }

    }
}
