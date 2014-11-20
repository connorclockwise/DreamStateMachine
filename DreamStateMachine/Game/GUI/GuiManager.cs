using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using DreamStateMachine.Game.GUI;
using Microsoft.Xna.Framework.Input;
using DreamStateMachine;
using Microsoft.Xna.Framework;

namespace DreamStateMachine2.game.GUI
{
    class GuiManager
    {
        public event EventHandler<EventArgs> NewGame;
        public event EventHandler<EventArgs> Tutorial;
        public event EventHandler<EventArgs> Credits;
        public event EventHandler<EventArgs> CreditsExit;
        public event EventHandler<EventArgs> ExitPause;
        public event EventHandler<EventArgs> MenuExit;

        public List<UIComponent> guiItems;
        public List<UIComponent> menuItems;
        public Dictionary<Actor, UIComponent> healthBars;
        public Dictionary<String, Texture2D> guiTextures;
        public List<UIComponent> tutorialGui;
        List<List<MovingLabel>> credits;
        List<FadingLabel> notifications;

        Camera camera;
        
        Label helpLabel;
        public UIComponent rootGUIElement;
        UIComponent focusedElement;
        MouseState mouseState;
        MouseState lastMouseState;
        KeyboardState keyboardState;
        GamePadState gamepadState;
        float clickCoolDown;

        public bool menuEnabled;
        public bool creditsEnabled;
        Panel pausePanel;
        Label pauseLabel;
        Button pauseButton;
        Panel hudPanel;
        SpriteFont spriteFont;

        public GuiManager(Camera camera)
        {
            this.camera = camera;

            guiItems = new List<UIComponent>();

            //menuItems = new List<UIComponent>();
            healthBars = new Dictionary<Actor, UIComponent>();
            
            tutorialGui = new List<UIComponent>();
            //notifications = new List<FadingLabel>();
            credits = new List<List<MovingLabel>>();

            guiTextures = new Dictionary<string, Texture2D>();
            menuEnabled = false;
            creditsEnabled = false;
            clickCoolDown = 0;

            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Actor.Hurt += new EventHandler<AttackEventArgs>(Actor_Hurt);
            Actor.OnPickUp += new EventHandler<PickupEventArgs>(Actor_OnPickUp);
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
            
        }

        public void loadTextures(ContentManager content)
        {
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
            guiTextures["exitMenuButton"] = content.Load<Texture2D>("exitBtn");
            spriteFont = content.Load<SpriteFont>("SpriteFont1");
        }

        public void enterStartMenu()
        {
            foreach (UIComponent component in guiItems)
            {
                component.remove();
            }
            foreach (UIComponent component in tutorialGui)
            {
                component.remove();
            }
            foreach (KeyValuePair<Actor, UIComponent> entry in healthBars)
            {
                entry.Value.remove();
            }
            healthBars.Clear();
            tutorialGui.Clear();
            guiItems.Clear();

            camera.drawSpace.X = 0;
            camera.drawSpace.Y = 0;
            //menuItems.Clear();
            //tutorialGui.Clear();
            //healthBars.Clear();
            //credits.Clear();

            Panel bg = new Panel(guiTextures["menuPanel"], Color.White);
            bg.dimensions.X = camera.drawSpace.X;
            bg.dimensions.Y = camera.drawSpace.Y;
            bg.dimensions.Width = camera.drawSpace.Width;
            bg.dimensions.Height = camera.drawSpace.Height;

            Panel logo = new Panel(guiTextures["logo"], new Color(255, 255, 255));
            logo.dimensions.Width = camera.drawSpace.Width * 4 / 5;
            logo.dimensions.Height = camera.drawSpace.Height * 2 / 10;
            logo.dimensions.X = camera.drawSpace.Width / 2 - logo.dimensions.Width / 2;
            logo.dimensions.Y = 50;
            bg.addChild(logo);

            Button newGameButton = new Button(guiTextures["newGameButton"]);
            newGameButton.dimensions.Width = camera.drawSpace.Width * 2 / 5;
            newGameButton.dimensions.Height = camera.drawSpace.Height / 6;
            newGameButton.dimensions.X = camera.drawSpace.Width / 4 - newGameButton.dimensions.Width / 2;
            newGameButton.dimensions.Y = camera.drawSpace.Height * 3 / 10;
            newGameButton.onClick = newGameClicked;
            bg.addChild(newGameButton);

            Button tutorialButton = new Button(guiTextures["tutorialButton"]);
            tutorialButton.dimensions.Width = camera.drawSpace.Width * 2 / 5;
            tutorialButton.dimensions.Height = camera.drawSpace.Height / 6;
            tutorialButton.dimensions.X = camera.drawSpace.Width / 4 - tutorialButton.dimensions.Width / 2;
            tutorialButton.dimensions.Y = camera.drawSpace.Height * 5 / 10;
            tutorialButton.onClick = tutorialClicked;
            bg.addChild(tutorialButton);

            Button creditsButton = new Button(guiTextures["creditsButton"]);
            creditsButton.dimensions.Width = camera.drawSpace.Width * 2 / 5;
            creditsButton.dimensions.Height = camera.drawSpace.Height / 6;
            creditsButton.dimensions.X = camera.drawSpace.Width * 3 / 4 - creditsButton.dimensions.Width / 2;
            creditsButton.dimensions.Y = camera.drawSpace.Height * 3 / 10;
            creditsButton.onClick = creditsClicked;
            bg.addChild(creditsButton);

            Button exitButton = new Button(guiTextures["exitMenuButton"]);
            exitButton.dimensions.Width = camera.drawSpace.Width * 2 / 5;
            exitButton.dimensions.Height = camera.drawSpace.Height / 6;
            exitButton.dimensions.X = camera.drawSpace.Width * 3 / 4 - exitButton.dimensions.Width / 2;
            exitButton.dimensions.Y = camera.drawSpace.Height * 5 / 10;
            exitButton.onClick = exitMenuClicked;
            bg.addChild(exitButton);

            guiItems.Add(bg);
            bg.initialize();
            //menuItems.Add(bg);
            rootGUIElement = bg;
            menuEnabled = true;
            //EnableMenu(this, EventArgs.Empty);
        }

        public void enterGame()
        {
            Panel hudPanel = new Panel(guiTextures["whiteSquare"], Color.Black);
            hudPanel.dimensions.Width = camera.drawSpace.Width;
            hudPanel.dimensions.Height = camera.drawSpace.Height / 5;
            hudPanel.dimensions.X = 0;
            hudPanel.dimensions.Y = camera.drawSpace.Height / 5;

            hudPanel.initialize();
            guiItems.Add(hudPanel);
        }

        public void enterPauseMenu()
        {
            //menuItems.Clear();

            Panel pauseMenu = new Panel(guiTextures["whiteSquare"], Color.Black);
            pauseMenu.dimensions.Width = camera.drawSpace.Width / 4;
            pauseMenu.dimensions.Height = camera.drawSpace.Height / 4;
            pauseMenu.dimensions.X = camera.drawSpace.Width / 2 - pauseMenu.dimensions.Width / 2;
            pauseMenu.dimensions.Y = camera.drawSpace.Height / 2 - pauseMenu.dimensions.Height / 2;

            pauseLabel = new Label(spriteFont, "Paused");
            pauseLabel.dimensions.X = camera.drawSpace.Width / 2 - (int)spriteFont.MeasureString("Paused").X / 2;
            pauseLabel.dimensions.Y = camera.drawSpace.Height / 2 - (int)spriteFont.MeasureString("Paused").Y / 2 - 60;

            pauseButton = new Button(guiTextures["exitButton"]);
            pauseButton.dimensions.Width = camera.drawSpace.Width / 6;
            pauseButton.dimensions.Height = camera.drawSpace.Height / 8;
            pauseButton.dimensions.X = camera.drawSpace.Width / 2 - pauseButton.dimensions.Width / 2;
            pauseButton.dimensions.Y = camera.drawSpace.Height / 2 - pauseButton.dimensions.Height / 2 + 30;
            pauseButton.onClick = exitClicked;


            pauseMenu.children.Add(pauseLabel);
            pauseMenu.children.Add(pauseButton);
            pauseMenu.initialize();
            guiItems.Add(pauseMenu);

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

        private void exitMenuClicked()
        {
            MenuExit(this, EventArgs.Empty);
        }

        private void creditsExit()
        {
            CreditsExit(this, EventArgs.Empty);
        }

        public void handleGuiControls()
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            gamepadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            if (lastMouseState != null)
            {
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


            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {

            }
            else if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
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

        public void rollCredits(bool controller = false)
        {
            foreach (UIComponent component in guiItems)
            {
                component.remove();
            }
            foreach (UIComponent component in tutorialGui)
            {
                component.remove();
            }
            foreach (KeyValuePair<Actor, UIComponent> entry in healthBars)
            {
                entry.Value.remove();
            }
            healthBars.Clear();
            tutorialGui.Clear();
            guiItems.Clear();
            

            if (!controller)
            {
                helpLabel = new MovingLabel(spriteFont, "Press E to go back to the menu.");
                helpLabel.dimensions.X = camera.drawSpace.Width / 2 + 200;
                helpLabel.dimensions.Y = 20;
            }
            else
            {
                helpLabel = new MovingLabel(spriteFont, "Press X to go back to the menu.");
                helpLabel.dimensions.X = camera.drawSpace.Width / 2 + 200;
                helpLabel.dimensions.Y = 20;
                
            }
            guiItems.Add(helpLabel);
            helpLabel.initialize();


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




            int startingY = camera.drawSpace.Height + 100;
            int additionalOffsetY = 0;
            foreach (List<MovingLabel> creditList in credits)
            {
                foreach (MovingLabel mention in creditList)
                {
                    mention.dimensions.X = 30;
                    mention.dimensions.Y = startingY + additionalOffsetY;
                    mention.velocity.Y = -1;
                    additionalOffsetY += 30;
                    guiItems.Add(mention);
                    mention.initialize();
                    
                }
                additionalOffsetY += 50;
            }
            //header.dimensions.X = 550;
        }

        public void startTutorialGui(bool controller = false)
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
            
            foreach (UIComponent component in tutorialGui)
            {
                component.initialize();
            }
            guiItems.Union(tutorialGui);
        }

        public void update(float dt)
        {
            if(clickCoolDown > 0)
                clickCoolDown -= dt;
            foreach (UIComponent component in guiItems)
            {
                component.update(dt);
            }
        }

        private void Actor_Spawn(object sender, EventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            if (!healthBars.ContainsKey(spawnedActor) && spawnedActor.health < spawnedActor.maxHealth)
            {
                if (spawnedActor.className == "player")
                    healthBars[spawnedActor] = new HealthBar(spawnedActor, guiTextures["healthBar"]);
                else
                    healthBars[spawnedActor] = new HealthBar(spawnedActor, guiTextures["enemyHealthBar"]);
            }
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
                healthBars[hurtActor].initialize();
            }
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            Actor deadActor = (Actor)sender;
            healthBars[deadActor].remove();
            healthBars.Remove(deadActor);
            if (deadActor.className == "player")
            {
                Panel panel = new Panel(guiTextures["whiteSquare"], new Color(Color.TransparentBlack, .5f));
                panel.dimensions.Width = (int)(camera.drawSpace.Width * 2.5 / 10);
                panel.dimensions.Height = camera.drawSpace.Width * 1 / 10;
                panel.dimensions.X = camera.drawSpace.Width / 2 - panel.dimensions.Width / 2;
                panel.dimensions.Y = camera.drawSpace.Height / 2 - panel.dimensions.Height / 2;
                guiItems.Add(panel);
                Label deadLabel = new Label(spriteFont, "YOU ARE DEAD");
                deadLabel.color = Color.Red;
                deadLabel.dimensions.X = camera.drawSpace.Width / 2 - (int)spriteFont.MeasureString(deadLabel.contents).X / 2;
                deadLabel.dimensions.Y = camera.drawSpace.Height / 2 - (int)spriteFont.MeasureString(deadLabel.contents).Y / 2 - 25;
                panel.addChild(deadLabel);
                Label helpLabel = new Label(spriteFont, "Press use to restart.");
                helpLabel.dimensions.X = camera.drawSpace.Width / 2 - (int)spriteFont.MeasureString(helpLabel.contents).X / 2;
                helpLabel.dimensions.Y = camera.drawSpace.Height / 2 - (int)spriteFont.MeasureString(helpLabel.contents).Y / 2 + 25;
                panel.addChild(helpLabel);
                guiItems.Add(panel);
                //tutorialGui.Add(helpLabel);
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
            notification.initialize();
            guiItems.Add(notification);
            //notifications.Add(notification);
        }

        private void World_Change(Object sender, EventArgs eventArgs)
        {
            foreach (UIComponent component in guiItems)
            {
                component.remove();
            }
            foreach (UIComponent component in tutorialGui)
            {
                component.remove();
            }
            foreach (KeyValuePair<Actor, UIComponent> entry in healthBars)
            {
                entry.Value.remove();
            }
            healthBars.Clear();
            tutorialGui.Clear();
            guiItems.Clear();
        }

    }
}
