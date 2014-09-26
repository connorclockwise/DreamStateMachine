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

            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
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
                Stunned stunned = new Stunned(victimBehaviorList, victim);
                if (!victimBehaviorList.has(aggravated) && !victimBehaviorList.has(stunned))
                    victimBehaviorList.pushFront(aggravated);
                if (!victimBehaviorList.has(stunned))
                    victimBehaviorList.pushFront(stunned);
                
            }

        }

        private void Actor_Death(object sender, EventArgs e)
        {
            Actor victim = (Actor)sender;
            if (behaviorLists.ContainsKey(victim))
            {
                behaviorLists.Remove(victim);
            }
            else if(victim.className == "player")
            {
                Aggravated aggravated = new Aggravated(null, null);
                Alert alert = new Alert(null, null);
                foreach(KeyValuePair<Actor, ActionList> entry in behaviorLists)
                {
                    if (entry.Value.has(aggravated))
                    {
                        entry.Value.remove(aggravated);
                    }
                    if (entry.Value.has(alert))
                    {
                        entry.Value.remove(alert);
                    }
                }
            }
        }

        public void update(float dt){
            foreach (KeyValuePair<Actor, ActionList> entry in behaviorLists)
            {
                entry.Value.update(dt);
            }
        }

        private void World_Change(object sender, EventArgs eventsArgs)
        {
            behaviorLists.Clear();
        }

    }
}
