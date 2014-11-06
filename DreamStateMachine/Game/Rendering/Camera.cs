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
        public event EventHandler<EventArgs> Tutorial;
        public event EventHandler<EventArgs> Credits;
        public event EventHandler<EventArgs> CreditsExit;
        public event EventHandler<EventArgs> ExitPause;

        public Rectangle drawSpace;
        public List<IDrawable> actors;
        public List<IDrawable> props;
        public List<IDrawable> menuItems;
        public Dictionary<IDrawable,IDrawable> healthBars;
        public Dictionary<String,Texture2D> guiTextures;
        public List<IDrawable> tutorialGui;
        List<List<MovingLabel>> credits;
        List<FadingLabel> notifications;
        Actor protagonist;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        bool debug;
        public bool menuEnabled;
        public bool creditsEnabled;
        IDrawable curWorld;
        Panel pausePanel;
        Label pauseLabel;
        Button pauseButton;

        Label helpLabel;
        public UIComponent rootGUIElement;
        UIComponent focusedElement;
        MouseState mouseState;
        MouseState lastMouseState;
        KeyboardState keyboardState;
        GamePadState gamepadState;
        float clickCoolDown;

        public Camera(SpriteBatch sB, Rectangle dS)
        {
            spriteBatch = sB;
            drawSpace = dS;
            actors = new List<IDrawable>();
            props = new List<IDrawable>();
            menuItems = new List<IDrawable>();
            
            healthBars = new Dictionary<IDrawable, IDrawable>();
            guiTextures = new Dictionary<string,Texture2D>();
            tutorialGui = new List<IDrawable>();
            notifications = new List<FadingLabel>();
            credits = new List<List<MovingLabel>>();
            debug = false;
            menuEnabled = false;
            creditsEnabled = false;
            clickCoolDown = 0;

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Prop.Spawn += new EventHandler<SpawnEventArgs>(Prop_Spawn);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.OnPickUp += new EventHandler<PickupEventArgs>(Actor_OnPickUp);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Prop.Remove += new EventHandler<EventArgs>(Prop_Remove);
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
            guiTextures["tutorialButton"] = content.Load<Texture2D>("tutorialBtnUnfocused");
            guiTextures["tutorialButtonFocused"] = content.Load<Texture2D>("tutorialBtnFocused");
            guiTextures["creditsButton"] = content.Load<Texture2D>("creditsBtnUnfocused");
            guiTextures["creditsButtonFocused"] = content.Load<Texture2D>("creditsBtnFocused");
            guiTextures["exitButton"] = content.Load<Texture2D>("exitGameButton");
            spriteFont = content.Load<SpriteFont>("SpriteFont1");
        }

        private void Actor_Spawn(object sender, EventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            actors.Add(spawnedActor);
            if (spawnedActor.className == "player")
            {
                protagonist = spawnedActor;
            }
            if (!healthBars.ContainsKey(spawnedActor) && spawnedActor.health < spawnedActor.maxHealth)
            {
                if (spawnedActor.className == "player")
                    healthBars[spawnedActor] = new HealthBar(spawnedActor, guiTextures["healthBar"]);
                else
                    healthBars[spawnedActor] = new HealthBar(spawnedActor, guiTextures["enemyHealthBar"]);
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

        private void Actor_Hurt(object sender, AttackEventArgs e)
        {
            Actor hurtActor = (Actor)sender;
            if (!healthBars.ContainsKey(hurtActor))
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
                Panel panel = new Panel(guiTextures["whiteSquare"], new Color(Color.TransparentBlack, .5f));
                panel.dimensions.Width = (int)( drawSpace.Width * 2.5 / 10);
                panel.dimensions.Height = drawSpace.Width * 1 / 10;
                panel.dimensions.X = drawSpace.Width / 2 - panel.dimensions.Width/2;
                panel.dimensions.Y = drawSpace.Height / 2 - panel.dimensions.Height / 2;
                tutorialGui.Add(panel);
                Label deadLabel = new Label(spriteFont, "YOU ARE DEAD");
                deadLabel.color = Color.Red;
                deadLabel.dimensions.X = drawSpace.Width / 2 - (int)spriteFont.MeasureString(deadLabel.contents).X / 2;
                deadLabel.dimensions.Y = drawSpace.Height / 2 - (int)spriteFont.MeasureString(deadLabel.contents).Y / 2 - 25;
                tutorialGui.Add(deadLabel);
                Label helpLabel = new Label(spriteFont, "Press use to restart.");
                helpLabel.dimensions.X = drawSpace.Width / 2 - (int)spriteFont.MeasureString(helpLabel.contents).X / 2;
                helpLabel.dimensions.Y = drawSpace.Height / 2 - (int)spriteFont.MeasureString(helpLabel.contents).Y / 2 + 25;
                tutorialGui.Add(helpLabel);
            }
        }

        private void Actor_OnPickUp(object sender, PickupEventArgs e)
        {
            Actor pickingUpActor = (Actor)sender;
            String notificationString = "Picked up " + e.itemClassName;
            FadingLabel notification = new FadingLabel(spriteFont, notificationString);
            notification.color = Color.LightGreen;
            notification.dimensions.X = pickingUpActor.hitBox.Center.X - (int)spriteFont.MeasureString(notificationString).X / 2;
            notification.dimensions.Y = pickingUpActor.hitBox.Center.Y - (int)spriteFont.MeasureString(notificationString).Y / 2 + 40;
            //notification.setPos(pickingUpActor.hitBox.Center);
            notifications.Add(notification);
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
                    curActor.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
                }
            }
        }

        public void drawCredits()
        {
            helpLabel.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], false);
            foreach (List<MovingLabel> creditList in credits)
            {
                foreach (MovingLabel mention in creditList)
                {
                    mention.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], false);
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
            foreach (IDrawable entry in notifications)
            {
                entry.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], debug);
            }
        }

        public void drawPauseMenu()
        {
            pausePanel.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], false);
            pauseLabel.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], false);
            pauseButton.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], false);
        }

        public void drawProps()
        {
            foreach (Prop prop in props)
            {
                prop.draw(spriteBatch, drawSpace, guiTextures["debugSquare"], false);
            }
        }

        public void enterStartMenu()
        {
            menuItems.Clear();
            tutorialGui.Clear();
            healthBars.Clear();
            actors.Clear();
            props.Clear();
            credits.Clear();
            drawSpace.X = 0;
            drawSpace.Y = 0;

            Panel bg = new Panel(guiTextures["menuPanel"], Color.White);
            bg.dimensions.X = drawSpace.X;
            bg.dimensions.Y = drawSpace.Y;
            bg.dimensions.Width = drawSpace.Width;
            bg.dimensions.Height = drawSpace.Height;

            Panel logo = new Panel(guiTextures["logo"], new Color(255, 255, 255));
            logo.dimensions.Width = drawSpace.Width * 4 / 5;
            logo.dimensions.Height = drawSpace.Height * 2 / 10;
            logo.dimensions.X = drawSpace.Width / 2 - logo.dimensions.Width / 2;
            logo.dimensions.Y = 50;
            
            bg.addChild(logo);

            Button newGameButton = new Button(guiTextures["newGameButton"]);
            newGameButton.dimensions.Width = drawSpace.Width * 2 / 5;
            newGameButton.dimensions.Height = drawSpace.Height / 6;
            newGameButton.dimensions.X = drawSpace.Width / 2 - newGameButton.dimensions.Width / 2;
            newGameButton.dimensions.Y = drawSpace.Height * 3 / 10;
            newGameButton.onClick = newGameClicked;
            bg.addChild(newGameButton);

            Button tutorialButton = new Button(guiTextures["tutorialButton"]);
            tutorialButton.dimensions.Width = drawSpace.Width * 2 / 5;
            tutorialButton.dimensions.Height = drawSpace.Height / 6;
            tutorialButton.dimensions.X = drawSpace.Width / 2 - tutorialButton.dimensions.Width / 2;
            tutorialButton.dimensions.Y = drawSpace.Height * 5 / 10;
            tutorialButton.onClick = tutorialClicked;
            bg.addChild(tutorialButton);

            Button creditsButton = new Button(guiTextures["creditsButton"]);
            creditsButton.dimensions.Width = drawSpace.Width * 2 / 5;
            creditsButton.dimensions.Height = drawSpace.Height / 6;
            creditsButton.dimensions.X = drawSpace.Width / 2 - creditsButton.dimensions.Width / 2;
            creditsButton.dimensions.Y = drawSpace.Height * 7 / 10;
            creditsButton.onClick = creditsClicked;
            bg.addChild(creditsButton);


            menuItems.Add(bg);
            rootGUIElement = bg;
            menuEnabled = true;
            //EnableMenu(this, EventArgs.Empty);
        }

        public void enterPauseMenu()
        {
            menuItems.Clear();

            Panel pauseMenu = new Panel(guiTextures["whiteSquare"], Color.Black);
            pauseMenu.dimensions.Width = drawSpace.Width / 4;
            pauseMenu.dimensions.Height = drawSpace.Height / 4;
            pauseMenu.dimensions.X = drawSpace.Width / 2 - pauseMenu.dimensions.Width/2;
            pauseMenu.dimensions.Y = drawSpace.Height / 2 - pauseMenu.dimensions.Height / 2;

            pauseLabel = new Label(spriteFont, "Paused");
            pauseLabel.dimensions.X = drawSpace.Width / 2 - (int)spriteFont.MeasureString("Paused").X /2;
            pauseLabel.dimensions.Y = drawSpace.Height / 2 - (int)spriteFont.MeasureString("Paused").Y / 2 - 60;

            pauseButton = new Button(guiTextures["exitButton"]);
            pauseButton.dimensions.Width = drawSpace.Width / 6;
            pauseButton.dimensions.Height = drawSpace.Height / 8;
            pauseButton.dimensions.X = drawSpace.Width / 2 - pauseButton.dimensions.Width / 2;
            pauseButton.dimensions.Y = drawSpace.Height / 2 - pauseButton.dimensions.Height / 2 + 30;
            pauseButton.onClick = exitClicked;

            pausePanel = pauseMenu;
            menuItems.Add(pauseMenu);
            menuItems.Add(pauseLabel);
            menuItems.Add(pauseButton);
            pauseMenu.children.Add(pauseLabel);
            pauseMenu.children.Add(pauseButton);
            rootGUIElement = pauseMenu;
            menuEnabled = true;
            //EnableMenu(this, EventArgs.Empty);
        }

        private void newGameClicked()
        {
            NewGame(this, EventArgs.Empty);
        }

        private void tutorialClicked()
        {
            Tutorial(this, EventArgs.Empty);
        }

        private void creditsClicked()
        {
            Credits(this, EventArgs.Empty);
        }

        private void exitClicked()
        {
            ExitPause(this, EventArgs.Empty);
        }

        private void creditsExit(){
            CreditsExit(this, EventArgs.Empty);
        }

        public void handleGuiControls()
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            gamepadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            if(lastMouseState != null){
                if (lastMouseState.X != mouseState.X || lastMouseState.Y != mouseState.Y)
                    mouseMoved(mouseState.X, mouseState.Y);
            }

            if (focusedElement != null && mouseState.LeftButton == ButtonState.Pressed && menuEnabled && clickCoolDown <= 0)
            {
                focusedElement.onClick();
                clickCoolDown = .25f;
            }

            if ((keyboardState.IsKeyDown(Keys.E) || gamepadState.IsButtonDown(Buttons.X)) && creditsEnabled)
            {
                creditsExit();
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
            clickCoolDown -= dt;
        }

        public void creditsUpdate(float dt)
        {
            foreach (List<MovingLabel> creditList in credits)
            {
                foreach (MovingLabel mention in creditList)
                {
                    mention.update(dt);
                }
            }
        }

        public void gameUpdate(float dt)
        {
            if (drawSpace.Center.X != protagonist.hitBox.Center.X || drawSpace.Center.Y != protagonist.hitBox.Center.Y)
                setFocus(protagonist.hitBox.Center.X, protagonist.hitBox.Center.Y);
        }

        public void notificationsUpdate(float dt)
        {
            foreach (FadingLabel notification in notifications.ToList())
            {
                notification.update(dt);
                if (notification.curTime > notification.timeToFade)
                    notifications.Remove(notification);
            }
        }

        public void rollCredits(bool controller = false)
        {
            if (!controller)
            {
                helpLabel = new MovingLabel(spriteFont, "Press E to go back to the menu.");
                helpLabel.dimensions.X = drawSpace.Width / 2 + 200;
                helpLabel.dimensions.Y = 20;
            }
            else
            {
                helpLabel = new MovingLabel(spriteFont, "Press X to go back to the menu.");
                helpLabel.dimensions.X = drawSpace.Width / 2 + 200;
                helpLabel.dimensions.Y = 20;
            }


            MovingLabel header = new MovingLabel(spriteFont, "DREAM STATE MACHINE");
            List<MovingLabel> gameHeaderList = new List<MovingLabel>();
            gameHeaderList.Add(header);
            credits.Add(gameHeaderList);
            MovingLabel projectLeadHeader = new MovingLabel(spriteFont, "Connor Brinkmann");
            MovingLabel projectLead = new MovingLabel(spriteFont, "Project lead, Game Architect, Programming, Animation, World design");
            List<MovingLabel> projectLeadList = new List<MovingLabel>();
            projectLeadList.Add(projectLeadHeader);
            projectLeadList.Add(projectLead);
            credits.Add(projectLeadList);

            MovingLabel MitchProgrammerHeader = new MovingLabel(spriteFont, "Mitchell McClellan");
            MovingLabel MitchProgrammer = new MovingLabel(spriteFont, "Sound programmer, Item programmer/designer, Character design/animation");
            List<MovingLabel> MitchProgrammerList = new List<MovingLabel>();
            MitchProgrammerList.Add(MitchProgrammerHeader);
            MitchProgrammerList.Add(MitchProgrammer);
            credits.Add(MitchProgrammerList);

            MovingLabel JeremyProgrammerHeader = new MovingLabel(spriteFont, "Jeremy Feltracco");
            MovingLabel JeremyProgrammer = new MovingLabel(spriteFont, "Game input programming, Controller support programming");
            List<MovingLabel> JeremyProgrammerList = new List<MovingLabel>();
            JeremyProgrammerList.Add(JeremyProgrammerHeader);
            JeremyProgrammerList.Add(JeremyProgrammer);
            credits.Add(JeremyProgrammerList);

            MovingLabel AaronProgrammerHeader = new MovingLabel(spriteFont, "Aaron Andrews");
            MovingLabel AaronProgrammer = new MovingLabel(spriteFont, "Prop programming");
            List<MovingLabel> AaronProgrammerList = new List<MovingLabel>();
            AaronProgrammerList.Add(AaronProgrammerHeader);
            AaronProgrammerList.Add(AaronProgrammer);
            credits.Add(AaronProgrammerList);

            MovingLabel MattProgrammerHeader = new MovingLabel(spriteFont, "Matt Schmidt");
            MovingLabel MattProgrammer = new MovingLabel(spriteFont, "Linux assistance");
            List<MovingLabel> MattProgrammerList = new List<MovingLabel>();
            MattProgrammerList.Add(MattProgrammerHeader);
            MattProgrammerList.Add(MattProgrammer);
            credits.Add(MattProgrammerList);

            MovingLabel HoKeunProgrammerHeader = new MovingLabel(spriteFont, "Ho Keun Kim");
            MovingLabel HoKeunProgrammer = new MovingLabel(spriteFont, "Tutorial World programming/design");
            List<MovingLabel> HoKeunProgrammerList = new List<MovingLabel>();
            HoKeunProgrammerList.Add(HoKeunProgrammerHeader);
            HoKeunProgrammerList.Add(HoKeunProgrammer);
            credits.Add(HoKeunProgrammerList);

            MovingLabel OjanProgrammerHeader = new MovingLabel(spriteFont, "Ojan Thornycroft");
            MovingLabel OjanProgrammer = new MovingLabel(spriteFont, "Rendering programming, world transition programming");
            List<MovingLabel> OjanProgrammerList = new List<MovingLabel>();
            OjanProgrammerList.Add(OjanProgrammerHeader);
            OjanProgrammerList.Add(OjanProgrammer);
            credits.Add(OjanProgrammerList);

            MovingLabel JonathanProgrammerHeader = new MovingLabel(spriteFont, "Jonathan Hunter");
            MovingLabel JonathanProgrammer = new MovingLabel(spriteFont, "Additional Enemy programming");
            List<MovingLabel> JonathanProgrammerList = new List<MovingLabel>();
            JonathanProgrammerList.Add(JonathanProgrammerHeader);
            JonathanProgrammerList.Add(JonathanProgrammer);
            credits.Add(JonathanProgrammerList);

            MovingLabel PatrickArtistHeader = new MovingLabel(spriteFont, "Patrick Sewell");
            MovingLabel PatrickArtist = new MovingLabel(spriteFont, "Menu design/art, additional Sound effects, Health bar design");
            List<MovingLabel> PatrickArtistList = new List<MovingLabel>();
            PatrickArtistList.Add(PatrickArtistHeader);
            PatrickArtistList.Add(PatrickArtist);
            credits.Add(PatrickArtistList);

            MovingLabel LarryArtistHeader = new MovingLabel(spriteFont, "Larry Smith");
            MovingLabel LarryArtist = new MovingLabel(spriteFont, "Character design/animation, Item design");
            List<MovingLabel> LarryArtistList = new List<MovingLabel>();
            LarryArtistList.Add(LarryArtistHeader);
            LarryArtistList.Add(LarryArtist);
            credits.Add(LarryArtistList);

            MovingLabel NickMusicianHeader = new MovingLabel(spriteFont, "Nicholas Shooter");
            MovingLabel NickMusician = new MovingLabel(spriteFont, "Composer for Ice World, Temple world, Grass world");
            List<MovingLabel> NickMusicianList = new List<MovingLabel>();
            NickMusicianList.Add(NickMusicianHeader);
            NickMusicianList.Add(NickMusician);
            credits.Add(NickMusicianList);

            MovingLabel XenaMusicianHeader = new MovingLabel(spriteFont, "Xena Grant");
            MovingLabel XenaMusician = new MovingLabel(spriteFont, "Composer for Nightmare world");
            List<MovingLabel> XenaMusicianList = new List<MovingLabel>();
            XenaMusicianList.Add(XenaMusicianHeader);
            XenaMusicianList.Add(XenaMusician);
            credits.Add(XenaMusicianList);

            MovingLabel CaylenaMusicianHeader = new MovingLabel(spriteFont, "Caylen Lee");
            MovingLabel CaylenaMusician = new MovingLabel(spriteFont, "Composer for Credits");
            List<MovingLabel> CaylenaMusicianList = new List<MovingLabel>();
            CaylenaMusicianList.Add(CaylenaMusicianHeader);
            CaylenaMusicianList.Add(CaylenaMusician);
            credits.Add(CaylenaMusicianList);

            MovingLabel userTestingHeader = new MovingLabel(spriteFont, "USER TESTING:");
            MovingLabel RobbieTest = new MovingLabel(spriteFont, "Robbie Thomas");
            MovingLabel ChaseTest = new MovingLabel(spriteFont, "Chase Melton");
            MovingLabel IdeanTest = new MovingLabel(spriteFont, "Idean Behforouz");
            MovingLabel MattTest = new MovingLabel(spriteFont, "Mathew Guzdial");
            MovingLabel ElliotTest = new MovingLabel(spriteFont, "Elliot Outland");
            List<MovingLabel> UserTestingList = new List<MovingLabel>();
            UserTestingList.Add(userTestingHeader);
            UserTestingList.Add(RobbieTest);
            UserTestingList.Add(ChaseTest);
            UserTestingList.Add(IdeanTest);
            UserTestingList.Add(MattTest);
            UserTestingList.Add(ElliotTest);
            credits.Add(UserTestingList);


            int startingY = drawSpace.Height + 100;
            int additionalOffsetY = 0;
            foreach (List<MovingLabel> creditList in credits)
            {
                foreach (MovingLabel mention in creditList)
                {
                    mention.dimensions.X = 30;
                    mention.dimensions.Y = startingY + additionalOffsetY;
                    mention.velocity.Y = -1;
                    additionalOffsetY += 30;
                }
                additionalOffsetY += 50;
            }
            //header.dimensions.X = 550;
        }

        public void startTutorialGui(bool controller =false)
        {
            if (!controller)
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
                WorldLabel escapeLabel = new WorldLabel(spriteFont, "Press escape to pause");
                escapeLabel.dimensions.X = 0;
                escapeLabel.dimensions.Y = 1800;
                tutorialGui.Add(escapeLabel);
                WorldLabel pickUpLabel = new WorldLabel(spriteFont, "press e to pickup weapons, keys, and health potions");
                pickUpLabel.dimensions.X = -450;
                pickUpLabel.dimensions.Y = 1000;
                tutorialGui.Add(pickUpLabel);
                WorldLabel weaponLabel = new WorldLabel(spriteFont, "press q to switch between two picked up weapons");
                weaponLabel.dimensions.X = -450;
                weaponLabel.dimensions.Y = 800;
                tutorialGui.Add(weaponLabel);
                WorldLabel doorLabel = new WorldLabel(spriteFont, "press e to unlock doors with keys");
                doorLabel.dimensions.X = -450;
                doorLabel.dimensions.Y = 600;
                tutorialGui.Add(doorLabel);
                WorldLabel attackLabel = new WorldLabel(spriteFont, "click the left mouse button to attack");
                attackLabel.dimensions.X = -450;
                attackLabel.dimensions.Y = 1500;
                tutorialGui.Add(attackLabel);
                WorldLabel useLabel = new WorldLabel(spriteFont, "press e when you are standing over stairs to go down a floor");
                useLabel.dimensions.X = -200;
                useLabel.dimensions.Y = 50;
                tutorialGui.Add(useLabel);
            }
            else
            {
                WorldLabel walkRightLabel = new WorldLabel(spriteFont, "Use the left thumbstick to move");
                walkRightLabel.dimensions.X = 1100;
                walkRightLabel.dimensions.Y = 700;
                tutorialGui.Add(walkRightLabel);
                WorldLabel followLabel = new WorldLabel(spriteFont, "Your character faces where you point the right thumbstick");
                followLabel.dimensions.X = 400;
                followLabel.dimensions.Y = 1800;
                tutorialGui.Add(followLabel);
                WorldLabel escapeLabel = new WorldLabel(spriteFont, "Press start to pause");
                escapeLabel.dimensions.X = 0;
                escapeLabel.dimensions.Y = 1800;
                tutorialGui.Add(escapeLabel);
                WorldLabel pickUpLabel = new WorldLabel(spriteFont, "Press X to pickup weapons, keys, and health potions");
                pickUpLabel.dimensions.X = -450;
                pickUpLabel.dimensions.Y = 1000;
                tutorialGui.Add(pickUpLabel);
                WorldLabel weaponLabel = new WorldLabel(spriteFont, "Press Y to switch between two picked up weapons");
                weaponLabel.dimensions.X = -450;
                weaponLabel.dimensions.Y = 800;
                tutorialGui.Add(weaponLabel);
                WorldLabel doorLabel = new WorldLabel(spriteFont, "Press X to unlock doors with keys");
                doorLabel.dimensions.X = -450;
                doorLabel.dimensions.Y = 600;
                tutorialGui.Add(doorLabel);
                WorldLabel attackLabel = new WorldLabel(spriteFont, "Press the right bumper button to attack");
                attackLabel.dimensions.X = -450;
                attackLabel.dimensions.Y = 1500;
                tutorialGui.Add(attackLabel);
                WorldLabel useLabel = new WorldLabel(spriteFont, "Press X when you are standing over stairs to go down a floor");
                useLabel.dimensions.X = -200;
                useLabel.dimensions.Y = 50;
                tutorialGui.Add(useLabel);
            }
        }

        private void World_Change(Object sender, EventArgs eventArgs)
        {

            WorldManager worldManager = (WorldManager)sender;
            curWorld = worldManager.curWorld;
            healthBars.Clear();
            actors.Clear();
            tutorialGui.Clear();
        }
    }
}
