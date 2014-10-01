using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreamStateMachine
{
    class Camera
    {
        public Rectangle drawSpace;
        //public List<Actor> actors;
        public List<IDrawable> actors;
        public Dictionary<IDrawable,IDrawable> healthBars;
        Actor protagonist;
        HealthBar healthBar;
        SpriteBatch spriteBatch;
        bool debug; 
        Texture2D debugTex;
        Texture2D healthBarTexture;
        IDrawable curWorld;


        public Camera(SpriteBatch sB, Rectangle dS, Texture2D debugSq, Texture2D healthBarTexture)
        {
            spriteBatch = sB;
            drawSpace = dS;
            //actors = new List<Actor>();
            actors = new List<IDrawable>();
            healthBars = new Dictionary<IDrawable, IDrawable>();
            debug = true;
            debugTex = debugSq;
            this.healthBarTexture = healthBarTexture;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        private void Actor_Spawn(object sender, EventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            actors.Add(spawnedActor);
            if (spawnedActor.className == "player")
            {
                protagonist = spawnedActor;
                //healthBar = new HealthBar(protagonist, healthBarTexture);
            }
        }

        private void Actor_Hurt(object sender, AttackEventArgs e)
        {
            Actor hurtActor = (Actor)sender;
            if (healthBars.ContainsKey(hurtActor))
            {
            }
            else
            {
                healthBars[hurtActor] = new HealthBar(hurtActor, healthBarTexture);
            }
            
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            healthBars.Remove((Actor)sender);
            actors.Remove((Actor)sender);
        }

        public void ToggleDebug(){
            debug = !debug;
        }

        public void drawActors()
        {
            IDrawable curActor;
            //Rectangle normalizedPosition;
            //Rectangle sourceRectangle;
            //Texture2D tex;

            for (int i = 0; i < actors.Count; i++)
            {
                curActor = actors.ElementAt(i);
                if (curActor.isInDrawSpace(drawSpace))
                {
                    curActor.draw(spriteBatch, drawSpace);
                }
            }
        }

        public void drawFloor()
        {
            curWorld.draw(spriteBatch, drawSpace);
        }

        public void drawGUI()
        {
            foreach( KeyValuePair<IDrawable,  IDrawable> entry in healthBars){
                entry.Value.draw(spriteBatch, drawSpace);
            }
            //healthBar.draw();
        }

        public void setDrawSpace(Rectangle rect)
        {
            drawSpace = rect;
        }

        public void setFocus(int x, int y)
        {
            drawSpace.X = (int)MathHelper.Max((float)(x - (drawSpace.Width / 2)), 0);
            drawSpace.Y = (int)MathHelper.Max((float)(y - (drawSpace.Height / 2)), 0);                         
        }

        public void update()
        {
            if (drawSpace.Center.X != protagonist.hitBox.Center.X || drawSpace.Center.Y != protagonist.hitBox.Center.Y)
                setFocus(protagonist.hitBox.Center.X, protagonist.hitBox.Center.Y);
        }

        private void World_Change(Object sender, EventArgs eventArgs)
        {

            WorldManager worldManager = (WorldManager)sender;
            this.curWorld = worldManager.curWorld;
            healthBars.Clear();
            actors.Clear();
        }
    }
}
