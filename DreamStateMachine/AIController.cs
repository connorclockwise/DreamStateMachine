using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;

namespace DreamStateMachine.Behaviors
{
    class AIController
    {
        Dictionary<Actor, ActionList> behaviorLists;
        public Actor protagonist;

        public AIController()
        {

            behaviorLists = new Dictionary<Actor, ActionList>();

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
        }

        private void Actor_Spawn(object sender, SpawnEventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            if (e.spawnType == 1)
            {
                protagonist = spawnedActor;
            }
            else if (e.spawnType == 2)
            {
                ActionList behaviorList = new ActionList(spawnedActor);
                Idle idle = new Idle(behaviorList, spawnedActor);
                behaviorList.pushFront(idle);
                Alert alert = new Alert(behaviorList, spawnedActor, protagonist);
                behaviorList.pushFront(alert);
                behaviorLists[spawnedActor] = behaviorList;
            }
            //actors.Add(spawnedActor);
        }

        private void Actor_Hurt(object sender, AttackEventArgs attackArgs)
        {
            Actor victim = (Actor) sender;
            if (behaviorLists.ContainsKey(victim))
            {
                ActionList victimBehaviorList = behaviorLists[victim];
                Aggravated aggravated = new Aggravated(victimBehaviorList, victim, attackArgs.damageInfo.attacker);
                if (!victimBehaviorList.has(aggravated))
                    victimBehaviorList.pushFront(aggravated);
                Stunned stunned = new Stunned(victimBehaviorList, victim);
                if (!victimBehaviorList.has(stunned))
                    victimBehaviorList.pushFront(stunned);
                
            }

        }

        private void Actor_Death(object sender, EventArgs e)
        {
            behaviorLists.Remove((Actor)sender);
        }

        public void update(float dt){
            foreach (KeyValuePair<Actor, ActionList> entry in behaviorLists)
            {
                entry.Value.update(dt);
            }
        }

    }
}
