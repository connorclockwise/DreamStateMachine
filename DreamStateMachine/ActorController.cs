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
        List<ActionList> actionLists;
        List<Actor> actors;
        Actor curActor;
        SoundManager soundManager;

        public ActorController(SoundManager s)
        {
            actionLists = new List<ActionList>();
            actors = new List<Actor>();

            soundManager = s;

            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            actors.Remove((Actor)sender);
        }

        private void Actor_Spawn(object sender, SpawnEventArgs e)
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

    }
}
