using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using DreamStateMachine.Game.GUI;
using Microsoft.Xna.Framework.Input;

namespace DreamStateMachine
{
    class Camera
    {

        public Rectangle drawSpace;
        public Texture2D debugTex;
        public List<IDrawable> actors;
        public List<IDrawable> props;
        public List<IDrawable> gui;
        
        Actor protagonist;
        World curWorld;
        SpriteBatch spriteBatch;
        bool debug;

        public Camera(SpriteBatch sB, Rectangle dS)
        {
            spriteBatch = sB;
            drawSpace = dS;
            actors = new List<IDrawable>();
            props = new List<IDrawable>();
            gui = new List<IDrawable>();
            
            debug = false;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Prop.Remove += new EventHandler<EventArgs>(Prop_Remove);
            Prop.Spawn += new EventHandler<SpawnEventArgs>(Prop_Spawn);
            UIComponent.Initialize += new EventHandler<EventArgs>(UIComponent_Initialize);
            UIComponent.Remove += new EventHandler<EventArgs>(UIComponent_Remove);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }


        public void loadDebugTex(ContentManager content)
        {
            debugTex = content.Load<Texture2D>("debugSquare");
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            Actor deadActor = (Actor)sender;
            actors.Remove(deadActor);
        }

        private void Actor_Spawn(object sender, EventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            actors.Add(spawnedActor);
            if (spawnedActor.className == "player")
            {
                protagonist = spawnedActor;
            }
        }

        private void Prop_Spawn(object sender, EventArgs e)
        {
            Prop spawnedProp = (Prop)sender;
            props.Add(spawnedProp);
        }

        private void Prop_Remove(object sender, EventArgs e)
        {
            Prop toRemove = (Prop)sender;
            props.Remove(toRemove);
        }



        public void ToggleDebug(){
            debug = !debug;
        }

        public void drawActors()
        {
            IDrawable curActor;

            for (int i = 0; i < actors.Count; i++)
            {
                curActor = actors.ElementAt(i);
                if (curActor.isInDrawSpace(drawSpace))
                {
                    curActor.draw(spriteBatch, drawSpace, debugTex, debug);
                }
            }
        }

        public void drawFloor()
        {
            curWorld.draw(spriteBatch, drawSpace, debugTex, debug);
        }

        public void drawGUI()
        {
            foreach (IDrawable guiItem in gui)
            {
                guiItem.draw(spriteBatch, drawSpace, debugTex, debug);
            }
        }

        public void drawPauseMenu()
        {
            //pausePanel.draw(spriteBatch, drawSpace, debugTex, false);
            //pauseLabel.draw(spriteBatch, drawSpace, debugTex, false);
            //pauseButton.draw(spriteBatch, drawSpace, debugTex, false);
        }

        public void drawProps()
        {
            foreach (Prop prop in props)
            {
                prop.draw(spriteBatch, drawSpace, debugTex, false);
            }
        }

        public void setDrawSpace(Rectangle rect)
        {
            drawSpace = rect;
        }

        public void setFocus(int x, int y)
        {  
            drawSpace.X = x - (drawSpace.Width / 2);
            drawSpace.Y = y - (drawSpace.Height / 2);   
        }

        public void gameUpdate(float dt)
        {
            if (drawSpace.Center.X != protagonist.hitBox.Center.X || drawSpace.Center.Y != protagonist.hitBox.Center.Y)
                setFocus(protagonist.hitBox.Center.X, protagonist.hitBox.Center.Y);
        }

        private void UIComponent_Initialize(Object sender, EventArgs eventArgs)
        {
            UIComponent component = (UIComponent)sender;
            gui.Add(component);
        }

        private void UIComponent_Remove(Object sender, EventArgs eventArgs)
        {
            UIComponent component = (UIComponent)sender;
            gui.Remove(component);
        }

        private void World_Change(Object sender, EventArgs eventArgs)
        {

            WorldManager worldManager = (WorldManager)sender;
            curWorld = worldManager.curWorld;
            actors.Clear();
        }
    }
}
