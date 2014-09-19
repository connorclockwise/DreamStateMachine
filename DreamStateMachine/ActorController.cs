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
        Actor spawnedActor;
        Actor curActor;
        Actor victim;
        Actor protagonist;
        Random random;
        SoundManager soundManager;

        public ActorController(SoundManager s)
        {
            actionLists = new List<ActionList>();
            actors = new List<Actor>();

            soundManager = s;

            Actor.Attack += new EventHandler<AttackEventArgs>(Actor_Attack);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
        }

        private void Actor_Attack(object sender, AttackEventArgs e)
        {
            handleActorAttack(e.damageInfo);
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

        public bool handleActorAttack(DamageInfo damageInfo)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                victim = actors.ElementAt(i);
                if (victim.Equals(damageInfo.attacker))
                    continue;
                else if (victim.hitBox.Intersects(damageInfo.attackRect))
                {
                    victim.velocity += damageInfo.attacker.sightVector * 20;
                    victim.onHurt(damageInfo);
                    Recoil recoil = new Recoil(victim.animationList, victim);
                    if (!victim.animationList.has(recoil))
                        victim.animationList.pushFront(recoil);
                    if (victim.health <= 0)
                    {
                        victim.onKill(damageInfo);
                        actors.Remove(victim);
                        if (victim.isPlayer)
                            soundManager.playSound(1);
                    }
                    return true;
                }
            }

            return false;
            
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
