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
        Dictionary<Actor, ActionList> actionLists;
        List<Actor> actors;
        Actor curActor;
        Walk walk = new Walk();
        public SoundManager soundManager;

        public ActorController()
        {
            //actorLists = new Dictionary<World,List<Actor>>();
            actionLists = new Dictionary<Actor, ActionList>();
            actors = new List<Actor>();

            Actor.LightAttack += new EventHandler<EventArgs>(Actor_Light_Attack);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        private void Actor_Light_Attack(Object sender, EventArgs e)
        {
            Actor inputtingActor = (Actor)sender;
            Punch punch = new Punch(actionLists[inputtingActor], inputtingActor);
            Recoil recoil = new Recoil(actionLists[inputtingActor], inputtingActor);
            if (!actionLists[inputtingActor].has(punch) && !actionLists[inputtingActor].has(recoil))
            {
                actionLists[inputtingActor].pushFront(punch);
            }
        }

        private void Actor_Death(Object sender, EventArgs e)
        {
            soundManager.playSound(1);
            actors.Remove((Actor)sender);
        }

        private void Actor_Hurt(Object sender, EventArgs e)
        {
            soundManager.playSound(3);
            Actor hurtActor = (Actor)sender;
            Recoil recoil = new Recoil(actionLists[hurtActor], hurtActor);
            if (!actionLists[hurtActor].has(recoil))
                actionLists[hurtActor].pushFront(recoil);

        }

        private void Actor_Spawn(Object sender, SpawnEventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            ActionList actionList = new ActionList(spawnedActor);
            actionLists[spawnedActor] = actionList;
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

                if ((curActor.velocity.X != 0 || curActor.velocity.Y != 0) && curActor.isWalking && !actionLists[curActor].has(walk))
                {
                    walk = new Walk(actionLists[curActor], curActor);
                    actionLists[curActor].pushFront(walk);
                }
                curActor.update(dt);
                actionLists[curActor].update(dt);

            }


        }

        private void World_Change(object sender, EventArgs eventsArgs)
        {
            WorldManager worldManager = (WorldManager) sender;
            World curWorld = worldManager.curWorld;
            actors.RemoveAll(x => x.world.Equals(curWorld));
            //actionLists.RemoveAll();
            actionLists.Clear();
        }

    }
}
