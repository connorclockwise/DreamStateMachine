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
        public List<Actor> actors;
        Actor protagonist;
        SpriteBatch spriteBatch;
        bool debug; 
        Texture2D debugTex;
        World curWorld;


        public Camera(SpriteBatch sB, Rectangle dS, Texture2D debugSq)
        {
            spriteBatch = sB;
            drawSpace = dS;
            actors = new List<Actor>();
            //debug = true;
            debugTex = debugSq;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        private void Actor_Spawn(object sender, EventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            actors.Add(spawnedActor);
            if (spawnedActor.className == "player")
                protagonist = spawnedActor;
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            actors.Remove((Actor)sender);
        }

        public void ToggleDebug(){
            debug = !debug;
        }

        public void DrawActors()
        {
            Actor curActor;
            Rectangle normalizedPosition;
            Rectangle sourceRectangle;
            Texture2D tex;

            for (int i = 0; i < actors.Count; i++)
            {
                curActor = actors.ElementAt(i);
                if (this.isInView(curActor.hitBox))
                {
                    tex = curActor.getTexture();
                    normalizedPosition = new Rectangle(curActor.body.X - this.drawSpace.X,
                                                       curActor.body.Y - this.drawSpace.Y,
                                                       curActor.body.Width,
                                                       curActor.body.Height);


                    sourceRectangle = new Rectangle(curActor.curAnimFrame.X * curActor.body.Width,
                                                    curActor.curAnimFrame.Y * curActor.body.Height,
                                                    curActor.body.Width,
                                                    curActor.body.Height);

                    spriteBatch.Draw(
                        tex,
                        new Vector2(normalizedPosition.X + curActor.body.Width / 2, normalizedPosition.Y + curActor.body.Height/2),
                        sourceRectangle,
                        curActor.color,
                        curActor.bodyRotation,
                        new Vector2(normalizedPosition.Width / 2.0f, normalizedPosition.Height / 2.0f),
                        1,
                        SpriteEffects.None,
                        0
                    );

                    if (debug)
                    {
                        spriteBatch.Draw(
                            debugTex,
                            new Vector2(curActor.hitBox.X - this.drawSpace.X + normalizedPosition.Width / 2.0f, curActor.hitBox.Y - this.drawSpace.Y + normalizedPosition.Height / 2.0f),
                            new Rectangle(0, 0, curActor.hitBox.Width, curActor.hitBox.Height),
                            new Color(.5f, .5f, .5f, .5f),
                            0,
                            new Vector2(normalizedPosition.Width / 2.0f, normalizedPosition.Height / 2.0f),
                            1,
                            SpriteEffects.None,
                            0
                        );
                    }

                    if (debug && curActor.isAttacking)
                    {
                        spriteBatch.Draw(
                            debugTex,
                            new Vector2(curActor.attackBox.X - this.drawSpace.X, curActor.attackBox.Y - this.drawSpace.Y),
                            new Rectangle(0, 0, curActor.attackBox.Width, curActor.attackBox.Height),
                            new Color(.5f, .5f, .5f, .5f),
                            0,
                            new Vector2(curActor.attackBox.Width / 2.0f, curActor.attackBox.Height/2.0f),
                            1,
                            SpriteEffects.None,
                            0
                        );
                    }

                }
            }
        }

        public void DrawFloor()
        {
            Texture2D floorTileTex = curWorld.getFloorTileTex();
            int[,] tileMap = curWorld.getTileMap();
            int tileSize = curWorld.getTileSize();
            Rectangle screenRectangle;
            Rectangle sourceRectangle;
            


            for (int i = 0; i <= this.drawSpace.Height / tileSize + 1; i++)
            {
                for (int k = 0; k <= this.drawSpace.Width / tileSize + 1; k++)
                {
                    screenRectangle = new Rectangle((int)(k * tileSize - this.drawSpace.X % tileSize), (int)(i * tileSize - this.drawSpace.Y % tileSize), (int)(tileSize), (int)(tileSize));

                    if (i + (this.drawSpace.Y / tileSize) >= 0 && k + (this.drawSpace.X / tileSize) >= 0 && i + (this.drawSpace.Y / tileSize) < tileMap.GetLength(1) && k + (this.drawSpace.X / tileSize) < tileMap.GetLength(0))
                    //if (world.isInBounds(this.drawSpace.X + k * tileSize, this.drawSpace.Y + i * tileSize))
                    {
                        sourceRectangle = new Rectangle(tileMap[i + this.drawSpace.Y / tileSize, k + this.drawSpace.X / tileSize] * tileSize,
                                                        0,
                                                        tileSize,
                                                        tileSize);
                        spriteBatch.Draw(floorTileTex, screenRectangle, sourceRectangle, Color.White);
                    }
                }
            }
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

        public void update(Camera cam)
        {
            if (drawSpace.Center.X != protagonist.hitBox.Center.X || drawSpace.Center.Y != protagonist.hitBox.Center.Y)
                setFocus(protagonist.hitBox.Center.X, protagonist.hitBox.Center.Y);
        }

        public bool isInView(Rectangle hitbox)
        {
           return hitbox.Intersects(drawSpace);
        }

        private void World_Change(Object sender, EventArgs eventArgs)
        {

            WorldManager worldManager = (WorldManager)sender;
            this.curWorld = worldManager.curWorld;
            actors.Clear();
        }
    }
}
