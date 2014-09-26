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
    class ActorController
    {
        //Dictionary<World, List<Actor>> actorLists;
        List<ActionList> actionLists;
        List<Actor> actors;
        Actor curActor;

        public ActorController()
        {
            //actorLists = new Dictionary<World,List<Actor>>();
            actionLists = new List<ActionList>();
            actors = new List<Actor>();

            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        private void Actor_Death(Object sender, EventArgs e)
        {
            SoundManager.Instance.playSound(1);
            actors.Remove((Actor)sender);
        }

        private void Actor_Hurt(Object sender, EventArgs e)
        {
            SoundManager.Instance.playSound(3);
            Actor hurtActor = (Actor)sender;
            Recoil recoil = new Recoil(hurtActor.animationList, hurtActor);
            if (!hurtActor.animationList.has(recoil))
                hurtActor.animationList.pushFront(recoil);

        }

        private void Actor_Spawn(Object sender, SpawnEventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            ActionList actionList = new ActionList(spawnedActor);
            actionLists.Add(actionList);
            actors.Add(spawnedActor);
        }

        public void handleActorUse(Actor actor, Point usePoint)
        {
            
        }

        public void update(float dt)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                curActor = actors.ElementAt(i);
                curActor.update(dt);
            }
        }

        private void World_Change(object sender, EventArgs eventsArgs)
        {
            WorldManager worldManager = (WorldManager) sender;
            World curWorld = worldManager.curWorld;
            actors.RemoveAll(x => x.world.Equals(curWorld));
            actionLists.Clear();
        }

    }
}
