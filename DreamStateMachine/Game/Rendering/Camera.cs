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
        public List<IDrawable> tutorialGui;
        Actor protagonist;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
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
            tutorialGui = new List<IDrawable>();
            debug = false;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);

        }

        public void loadGuiTextures(ContentManager content){
            guiTextures["whiteSquare"] = content.Load<Texture2D>("whiteSquare");
            guiTextures["debugSquare"] = content.Load<Texture2D>("debugSquare");
            guiTextures["healthBar"] = content.Load<Texture2D>("playerHealthBarTransparency");
            guiTextures["enemyHealthBar"] = content.Load<Texture2D>("enemyHealthBarTransparency");
            guiTextures["menuPanel"] = content.Load<Texture2D>("splashScreen");
            guiTextures["logo"] = content.Load<Texture2D>("titlePanel");
            guiTextures["newGameButton"] = content.Load<Texture2D>("newGameBtnUnfocused");
            guiTextures["newGameButtonFocused"] = content.Load<Texture2D>("newGameBtnFocused");
            spriteFont = content.Load<SpriteFont>("SpriteFont1");
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
                if (hurtActor.className == "player")
                    healthBars[hurtActor] = new HealthBar(hurtActor, guiTextures["healthBar"]);
                else
                    healthBars[hurtActor] = new HealthBar(hurtActor, guiTextures["enemyHealthBar"]);
            }
            
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            Actor deadActor = (Actor)sender;
            healthBars.Remove(deadActor);
            actors.Remove(deadActor);
            if (deadActor.className == "player")
            {
                Label deadLabel = new Label(spriteFont, "YEAH YOU ARE PRETTY MUCH DEAD HOMBRE");
                deadLabel.color = Color.Red;
                deadLabel.dimensions.X = drawSpace.Width / 2 - (int)spriteFont.MeasureString("YEAH YOU ARE PRETTY MUCH DEAD HOMBRE").X / 2;
                deadLabel.dimensions.Y = drawSpace.Height / 2 - (int)spriteFont.MeasureString("YEAH YOU ARE PRETTY MUCH DEAD HOMBRE").Y / 2;
                tutorialGui.Add(deadLabel);
                Label helpLabel = new Label(spriteFont, "Press e to start over ya dangus");
                //label.color = Color.Red;
                helpLabel.dimensions.X = drawSpace.Width / 2 - (int)spriteFont.MeasureString("Press e to start over ya dangus").X / 2;
                helpLabel.dimensions.Y = drawSpace.Height / 2 - (int)spriteFont.MeasureString("Press e to start over ya dangus").Y / 2 + 50;
                tutorialGui.Add(helpLabel);
            }
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
            foreach( IDrawable entry in tutorialGui)
            {
                entry.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
            }
            //healthBar.draw();
        }

        public void enterStartMenu()
        {
            menuItems.Clear();
            drawSpace.X = 0;
            drawSpace.Y = 0;
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

            if (focusedElement != null && mouseState.LeftButton == ButtonState.Pressed)
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
            healthBars.Clear();
            actors.Clear();
            tutorialGui.Clear();
            if (worldManager.curWorld != null)
            {
                this.curWorld = worldManager.curWorld;
                if (worldManager.curLevel == 1)
                {
                    WorldLabel walkUpLabel = new WorldLabel(spriteFont, "press w to walk up");
                    walkUpLabel.dimensions.X = 500;
                    walkUpLabel.dimensions.Y = 500;
                    tutorialGui.Add(walkUpLabel);
                    WorldLabel walkRightLabel = new WorldLabel(spriteFont, "press d to walk right");
                    walkRightLabel.dimensions.X = 1100;
                    walkRightLabel.dimensions.Y = 700;
                    tutorialGui.Add(walkRightLabel);
                    WorldLabel walkDownLabel = new WorldLabel(spriteFont, "press s to walk down");
                    walkDownLabel.dimensions.X = 1800;
                    walkDownLabel.dimensions.Y = 400;
                    tutorialGui.Add(walkDownLabel);
                    WorldLabel walkLeftLabel = new WorldLabel(spriteFont, "press a to walk left");
                    walkLeftLabel.dimensions.X = 1100;
                    walkLeftLabel.dimensions.Y = 1200;
                    tutorialGui.Add(walkLeftLabel);
                    WorldLabel followLabel = new WorldLabel(spriteFont, "Your character faces the mouse cursor");
                    followLabel.dimensions.X = 400;
                    followLabel.dimensions.Y = 1800;
                    tutorialGui.Add(followLabel);
                    WorldLabel attackLabel = new WorldLabel(spriteFont, "click the left mouse button to attack");
                    attackLabel.dimensions.X = -450;
                    attackLabel.dimensions.Y = 1500;
                    tutorialGui.Add(attackLabel);
                    WorldLabel useLabel = new WorldLabel(spriteFont, "press e when you are standing over stairs to go down a floor");
                    useLabel.dimensions.X = -200;
                    useLabel.dimensions.Y = 50;
                    tutorialGui.Add(useLabel);
                }
            }
        }
    }
}
