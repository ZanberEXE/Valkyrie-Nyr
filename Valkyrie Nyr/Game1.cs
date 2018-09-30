using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;

namespace Valkyrie_Nyr
{

    public class Game1 : Game
    {        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static GameTime randomTime;
        public static ContentManager Ressources;
        public static SpriteBatch Renderer;
        public static Point WindowSize;
        public static SpriteFont Font;
        //white 1x1 texture
        public static Texture2D pxl;

        //bnutton stuff
        private List<Button> _mainMenuButtons;
        private List<Button> _pauseButtons;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //set windowsize
            //graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1920;
            //graphics.IsFullScreen = true;

            WindowSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Content.RootDirectory = "Content";


            //Now you can load content from everywhere
            Ressources = Content;

            

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //TODO:delete
            pxl = Game1.Ressources.Load<Texture2D>("index");
            Font = Game1.Ressources.Load<SpriteFont>("Font");

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Renderer = spriteBatch;


            States.CurrentGameState = GameStates.SPLASHSCREEN;

            //Music.CurrentSong.loadSong("music/bgm_menuV2");

            //Button Stuff
            var playButton = new Button(Game1.Ressources.Load<Texture2D>("Button/playButton2"))
            {
                Position = new Vector2(97, 242),
            };
            playButton.Click += PlayButton_Click;
            var controlsButton = new Button(Game1.Ressources.Load<Texture2D>("Button/controlsButton"))
            {
                Position = new Vector2(97, 481),
            };
            controlsButton.Click += ControlsButton_Click;
            var creditsButton = new Button(Game1.Ressources.Load<Texture2D>("Button/creditsButton"))
            {
                Position = new Vector2(97, 607),
            };
            creditsButton.Click += CreditsButton_Click;
            var exitButton = new Button(Game1.Ressources.Load<Texture2D>("Button/exitButton"))
            {
                Position = new Vector2(97, 728),
            };
            exitButton.Click += ExitButton_Click;

            _mainMenuButtons = new List<Button>()
            {
                playButton,
                controlsButton,
                creditsButton,
                exitButton,

            };

            //pause button stuff
            var returnButton = new Button(Game1.Ressources.Load<Texture2D>("Button/returnButton"))
            {
                Position = new Vector2(264, 683),
            };
            returnButton.Click += ReturnButton_Click;
            var mainMenuButton = new Button(Game1.Ressources.Load<Texture2D>("Button/mainMenuButton"))
            {
                Position = new Vector2(1095, 685),
            };
            mainMenuButton.Click += MainMenuButton_Click;
            _pauseButtons = new List<Button>()
            {
                returnButton,
                mainMenuButton,
            };

            base.Initialize();
        }
        //button stuff
        private void PlayButton_Click(object sender, System.EventArgs e)
        {
            //Release Start
            LoadSaveGame();
            Level.Current.loadLevel("Hub");
            States.CurrentGameState = GameStates.PLAYING;
            if (!(Level.soulsRescued[0] || Level.soulsRescued[1] || Level.soulsRescued[2] || Level.soulsRescued[3]))
            {
                Level.Current.nscObjects[0].startConversation();
            }
        }
        private void ControlsButton_Click(object sender, System.EventArgs e)
        {
            States.CurrentGameState = GameStates.OPTIONS;
        }
        private void CreditsButton_Click(object sender, System.EventArgs e)
        {
            States.CurrentGameState = GameStates.CREDITS;
        }
        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            Exit();
        }
            //pause button stuff
        private void ReturnButton_Click(object sender, System.EventArgs e)
        {
            States.CurrentGameState = GameStates.PLAYING;
        }
        private void MainMenuButton_Click(object sender, System.EventArgs e)
        {
            States.CurrentGameState = GameStates.MAINMENU;
        }

        void LoadSaveGame()
        {
            if (!File.Exists("SaveGame.txt"))
            {
                return;
            }
            string[] saveData = File.ReadAllLines("SaveGame.txt");
            for(int i = 0; i < 12; i += 3)
            {
                Level.soulsRescued[i / 3] = (saveData[i][0] == 'T');
                Level.armorEnhanced[i / 3] = (saveData[i + 1][0] == 'T');
            }
            Player.Nyr.money = System.Int32.Parse(saveData[12]);
            Player.Nyr.maxHealth = System.Int32.Parse(saveData[14]);
            Player.Nyr.health = System.Int32.Parse(saveData[14]);
        }

        //TODO: proper content
        void mMainMenu(GameTime gameTime)
        {
            //button test
            foreach (var element in _mainMenuButtons)
                element.Update(gameTime);

            //foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            //{ 
            //    switch (element)
            //    {
            //        case Keys.D1:
            //            //Release Start
            //            LoadSaveGame();
            //            Level.Current.loadLevel("Overworld");
            //            States.CurrentGameState = GameStates.PLAYING;
            //            if (!(Level.soulsRescued[0] || Level.soulsRescued[1] || Level.soulsRescued[2] || Level.soulsRescued[3]))
            //            {
            //                //Level.Current.nscObjects[0].startConversation(gameTime);
            //            }
            //            break;
            //        case Keys.D2:
            //            States.CurrentGameState = GameStates.OPTIONS;
            //            break;
            //        case Keys.D3:
            //            //States.CurrentGameState = GameStates.CREDITS;
            //            break;
            //        case Keys.D4:
            //            States.CurrentGameState = GameStates.EXIT;
            //            break;
            //    }
            //}
        }

        void mSplashScreen()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    default:
                        States.CurrentGameState = GameStates.MAINMENU;
                        break;
                }
            }
        }

        void mOptions()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.Escape:
                        States.CurrentGameState = GameStates.MAINMENU;
                        break;
                }
            }
        }

        void mCredits()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                States.CurrentGameState = GameStates.MAINMENU;
            }
        }

        void mPause(GameTime gameTime)
        {
            MediaPlayer.Pause();

            //button stuff
            foreach (var element in _pauseButtons)
                element.Update(gameTime);

            //foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            //{
            //    switch (element)
            //    {
            //        case Keys.D1:
            //            //States.CurrentGameState = GameStates.OPTIONS;
            //            break;
            //        case Keys.D2:
            //            States.CurrentGameState = GameStates.PLAYING;
            //            MediaPlayer.Resume();
            //            break;
            //        case Keys.D3:
            //            States.CurrentGameState = GameStates.MAINMENU;
            //            break;
            //    }
            //}
        }

        void mLose()
        {
            //foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            //{
            //    switch (element)
            //    {
            //        case Keys.D1:
            //            States.CurrentGameState = GameStates.PLAYING;
            //            break;
            //        case Keys.D2:
            //            States.CurrentGameState = GameStates.MAINMENU;
            //            break;
            //    }
            //}
        }


        protected override void Update(GameTime gameTime)
        {

            //TODO:delete
            //TODO:Lose Screen when dead and respawn?

            //key test


            //adjust BGM Volume
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (MediaPlayer.Volume < 1f)
                {
                    MediaPlayer.Volume += 0.1f;
                } 
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if(MediaPlayer.Volume > 0.2f)
                {
                    MediaPlayer.Volume -= 0.1f;
                }
            }
            switch(States.CurrentBGMState)
            {
                case BGMStates.MENU:
                    if (Music.CurrentSong.name != "music/bgm_menuV2")
                    {
                        Music.CurrentSong.loadSong("music/bgm_menuV2");
                    }
                    break;
                case BGMStates.LEVEL:
                    if (Music.CurrentSong.name != "music/bgm_levelV2")
                    {
                        Music.CurrentSong.loadSong("music/bgm_levelV2");
                    }
                    break;
                case BGMStates.HUB:
                    if (Music.CurrentSong.name != "music/bgm_hubV2")
                    {
                        Music.CurrentSong.loadSong("music/bgm_hubV2");
                    }
                    break;
                case BGMStates.BOSS:
                    if(Music.CurrentSong.name != "music/bgm_bossV2")
                    {
                        SFX.CurrentSFX.loadSFX("sfx/sfx_warning");
                        Music.CurrentSong.loadSong("music/bgm_bossV2");
                    }
                    break;
            }
         
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && States.CurrentGameState == GameStates.PLAYING)
            {
                States.CurrentGameState = GameStates.PAUSE;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F) && States.CurrentGameState == GameStates.CONVERSATION)
            {
                Player.Nyr.conversationPartner.continueConversation(gameTime);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape) && States.CurrentGameState == GameStates.CONVERSATION)
            {
                Player.Nyr.conversationPartner.endConversation();
            }

            switch (States.CurrentGameState)
            {
                case GameStates.SPLASHSCREEN:
                    mSplashScreen();
                    break;

                case GameStates.MAINMENU:
                    mMainMenu(gameTime);
                    break;

                case GameStates.PLAYING:
                    Level.Current.update(gameTime);
                    break;

                case GameStates.OPTIONS:
                    mOptions();
                    break;

                case GameStates.CREDITS:
                    mCredits();
                    break;

                case GameStates.PAUSE:
                    mPause(gameTime);
                    break;

                case GameStates.LOSE:
                    mLose();
                    break;

                //Exits the Game
                case GameStates.EXIT:
                    Exit();
                    break;

            }

            randomTime = gameTime;
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            if(Level.Current.timeStop > 0)
            {
                Level.Current.timeStop--;
                return;
            }
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            
            switch(States.CurrentGameState)
            {
                case GameStates.PLAYING:
                    Level.Current.render(spriteBatch, gameTime);
                    break;
                case GameStates.CONVERSATION:
                    Player.Nyr.conversationPartner.renderConversation(spriteBatch);
                    break;
                case GameStates.MAINMENU:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("MainMenuBG"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    //buttoon stuff
                    foreach (var element in _mainMenuButtons)
                        element.Draw(gameTime, spriteBatch);
                    break;
                case GameStates.OPTIONS:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Options"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.CREDITS:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Credits"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.PAUSE:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Pause2"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    //button stuff
                    foreach (var element in _pauseButtons)
                        element.Draw(gameTime, spriteBatch);
                    break;
                case GameStates.LOSE:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Lose"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.SPLASHSCREEN:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("SplashScreenV2"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
            }

            //draw Colliders for Debugging
            //TODO: Delete
            if (States.CurrentGameState == GameStates.PLAYING)
            {
                foreach (GameObject collider in Level.Current.gameObjects)
                {
                    if (collider.name == "ice")
                    {
                        //spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), new Color(Color.LightSkyBlue, 150));
                    }
                    else
                    {
                        //spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), new Color(Color.LightGreen, 150));
                    }
                }
                //spriteBatch.Draw(pxl, new Rectangle((int)Antagonist.Ryn.position.X, (int)Antagonist.Ryn.position.Y, Antagonist.Ryn.width, Antagonist.Ryn.height), new Color(Color.LightGreen, 150));
                //spriteBatch.Draw(pxl, new Rectangle((int)Player.Nyr.position.X, (int)Player.Nyr.position.Y, Player.Nyr.width, Player.Nyr.height), new Color(Color.LightGreen, 150));
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
