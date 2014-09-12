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
        List<ActionList> behaviorLists;
        public Actor protagonist;

        public AIController()
        {

            behaviorLists = new List<ActionList>();

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
        }

        private void Actor_Spawn(object sender, SpawnEventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            if (e.spawnType == 1)
            {
                protagonist = spawnedActor;
            }
            if (e.spawnType == 2)
            {
                ActionList behaviorList = new ActionList(spawnedActor);
                Idle idle = new Idle(behaviorList, spawnedActor);
                behaviorList.pushFront(idle);
                Alert alert = new Alert(behaviorList, spawnedActor, protagonist);
                behaviorList.pushFront(alert);
                behaviorLists.Add(behaviorList);
            }
            //actors.Add(spawnedActor);
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            //actors.Remove((Actor)sender);
        }

        public void update(float dt){
            foreach (ActionList behaviorList in behaviorLists)
            {
                behaviorList.update(dt);
            }
        }

        //public void initEnemy(Actor actor){
        //    ActionList newBehaviorList = new ActionList(actor);
        //    Idle idle = new Idle(newBehaviorList, actor, worldManager.curWorld, random, this);
        //    enemy.behaviorList.pushFront(idle);
        //    Alert alert = new Alert(enemy.behaviorList, enemy, protagonist, worldManager.curWorld, this);
        //    enemy.behaviorList.pushFront(alert);

        //}

    }
}
