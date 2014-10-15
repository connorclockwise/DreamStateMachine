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

        public event EventHandler<EventArgs> NewGame;

        public Rectangle drawSpace;
        public List<IDrawable> actors;
        public List<IDrawable> menuItems;
        public Dictionary<IDrawable,IDrawable> healthBars;
        public Dictionary<String,Texture2D> guiTextures;
        Actor protagonist;
        SpriteBatch spriteBatch;
        bool debug;
        public bool menuEnabled;
        IDrawable curWorld;
        public UIComponent rootGUIElement;
        UIComponent focusedElement;
        MouseState mouseState;
        MouseState lastMouseState;
        KeyboardState keyboardState;

        //public static event EventHandler<EventArgs> EnableMenu;

        public Camera(SpriteBatch sB, Rectangle dS)
        {
            spriteBatch = sB;
            drawSpace = dS;
            //actors = new List<Actor>();
            actors = new List<IDrawable>();
            menuItems = new List<IDrawable>();
            
            healthBars = new Dictionary<IDrawable, IDrawable>();
            guiTextures = new Dictionary<string,Texture2D>();
            debug = false;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);

        }

        public void loadGuiTextures(ContentManager content){
            guiTextures["whiteSquare"] = content.Load<Texture2D>("whiteSquare");
            guiTextures["debugSquare"] = content.Load<Texture2D>("debugSquare");
            guiTextures["healthBar"] = content.Load<Texture2D>("debugSquare");
            guiTextures["menuPanel"] = content.Load<Texture2D>("nightSky");
            guiTextures["logo"] = content.Load<Texture2D>("DSMLogo");
            guiTextures["newGameButton"] = content.Load<Texture2D>("newGameButton");
            guiTextures["newGameButtonFocused"] = content.Load<Texture2D>("newGameButtonFocused");
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
                healthBars[hurtActor] = new HealthBar(hurtActor, guiTextures["healthBar"]);
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
                    curActor.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
                }
            }
        }

        public void drawFloor()
        {
            curWorld.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
        }

        public void drawGUI()
        {
            if (menuEnabled)
            {
                foreach (IDrawable menuItem in menuItems)
                {
                    menuItem.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
                }
            }
            foreach( KeyValuePair<IDrawable,  IDrawable> entry in healthBars){
                entry.Value.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
            }
            //healthBar.draw();
        }

        public void enterStartMenu()
        {
            Panel bg = new Panel(guiTextures["menuPanel"], new Color(255, 255, 255));
            bg.dimensions.X = drawSpace.X;
            bg.dimensions.Y = drawSpace.Y;
            bg.dimensions.Width = drawSpace.Width;
            bg.dimensions.Height = drawSpace.Height;
            Panel logo = new Panel(guiTextures["logo"], new Color(255, 255, 255));
            logo.dimensions.X = drawSpace.Width / 2 - guiTextures["logo"].Width /2;
            logo.dimensions.Y = 50;
            logo.dimensions.Width = guiTextures["logo"].Width;
            logo.dimensions.Height = guiTextures["logo"].Height * 4 / 5;
            Button newGameButton = new Button(guiTextures["newGameButton"]);
            newGameButton.dimensions.X = drawSpace.Width / 2 - guiTextures["newGameButton"].Width / 2;
            newGameButton.dimensions.Y = 400;
            newGameButton.dimensions.Width = guiTextures["newGameButton"].Width;
            newGameButton.dimensions.Height = guiTextures["newGameButton"].Height * 4 / 5;
            newGameButton.onClick = newGameClicked;
            bg.addChild(newGameButton);
            bg.addChild(logo);
            menuItems.Add(bg);
            rootGUIElement = bg;
            menuEnabled = true;
            //EnableMenu(this, EventArgs.Empty);
        }

        public void newGameClicked()
        {
            NewGame(this, EventArgs.Empty);
        }

        public void handleGuiControls()
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            if(lastMouseState != null){
                if (lastMouseState.X != mouseState.X || lastMouseState.Y != mouseState.Y)
                    mouseMoved(mouseState.X, mouseState.Y);
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                focusedElement.onClick();
            }


            if(keyboardState.IsKeyDown(Keys.Down)  || keyboardState.IsKeyDown(Keys.S)){
                
            }else if(keyboardState.IsKeyDown(Keys.Up)  || keyboardState.IsKeyDown(Keys.W)){
            }

            lastMouseState = mouseState;
        }

        private void mouseMoved(int x, int y)
        {
            Rectangle mouseRect = new Rectangle(x, y, 0, 0);
            if (focusedElement != null && !focusedElement.dimensions.Intersects(mouseRect))
            {
                focusedElement.takeFocus();
                focusedElement = null;
            }
            foreach (UIComponent child in rootGUIElement.children)
            {
                if (child.dimensions.Intersects(mouseRect) && child.canHaveFocus)
                {
                    
                    child.giveFocus();
                    focusedElement = child;
                }
            }
        }

        public void setDrawSpace(Rectangle rect)
        {
            drawSpace = rect;
        }

        public void setFocus(int x, int y)
        {
            //drawSpace.X = (int)MathHelper.Max((float)(x - (drawSpace.Width / 2)), 0);
            //drawSpace.Y = (int)MathHelper.Max((float)(y - (drawSpace.Height / 2)), 0);   
            drawSpace.X = x - (drawSpace.Width / 2);
            drawSpace.Y = y - (drawSpace.Height / 2);   
        }

        public void startMenuUpdate(float dt)
        {
            
        }

        public void gameUpdate(float dt)
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
