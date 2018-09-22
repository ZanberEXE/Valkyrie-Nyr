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

        //white 1x1 texture
        

        public static ContentManager Ressources;
        public static SpriteBatch Renderer;
        public static Point WindowSize;
        public static SpriteFont Font;
        public static Texture2D pxl;

        //Sound
        public Song menuSong, playingSong;
        public List<SoundEffect> sfx;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //set windowsize
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1920;
            //graphics.IsFullScreen = true;

            WindowSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Content.RootDirectory = "Content";


            //Now you can load content from everywhere
            Ressources = Content;


            IsMouseVisible = true;

            //sound stuff
            sfx = new List<SoundEffect>();

            
        }

        protected override void Initialize()
        {
            //TODO:delete
            pxl = Game1.Ressources.Load<Texture2D>("index");
            Font = Game1.Ressources.Load<SpriteFont>("Font");

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Renderer = spriteBatch;


            States.CurrentGameState = GameStates.MAINMENU;

            //sound stuff
            States.CurrentBGMState = BGMStates.MENU;

            

            this.menuSong = Game1.Ressources.Load<Song>("music/Test");
            this.playingSong = Game1.Ressources.Load<Song>("music/Test2");
            MediaPlayer.Play(menuSong);

            //sfx stuff
            sfx.Add(Game1.Ressources.Load<SoundEffect>("sfx/sfx_collide"));
            sfx.Add(Game1.Ressources.Load<SoundEffect>("sfx/sfx_jump"));
            sfx.Add(Game1.Ressources.Load<SoundEffect>("sfx/sfx_thud"));
            
            //sfx[0].Play();
            //var instance = sfx[0].CreateInstance();
            //instance.Play();

            base.Initialize();
        }

        void testSFX()
        {
            if (States.CurrentPlayerState == Playerstates.JUMP)
            {
                sfx[1].CreateInstance().Play();
            }
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.J:
                        sfx[0].CreateInstance().Play();
                        return;
                    case Keys.K:
                        sfx[1].CreateInstance().Play();
                        return;
                    case Keys.L:
                        sfx[2].CreateInstance().Play();
                        return;
                }
            }
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
        }

        //TODO: proper content
        void mMainMenu()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.D1:
                        LoadSaveGame();
                        Level.Current.loadLevel("Overworld");
                        States.CurrentGameState = GameStates.PLAYING;
                        MediaPlayer.Play(playingSong);
                        return;
                    case Keys.D2:
                        States.CurrentGameState = GameStates.OPTIONS;
                        return;
                    case Keys.D3:
                        States.CurrentGameState = GameStates.CREDITS;
                        return;
                    case Keys.D4:
                        States.CurrentGameState = GameStates.EXIT;
                        return;
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
                return;
            }
        }

        void mPause()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.D1:
                        States.CurrentGameState = GameStates.OPTIONS;
                        break;
                    case Keys.D2:
                        States.CurrentGameState = GameStates.PLAYING;
                        break;
                    case Keys.D3:
                        States.CurrentGameState = GameStates.MAINMENU;
                        break;
                }
            }
        }

        void mLose()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.D1:
                        States.CurrentGameState = GameStates.PLAYING;
                        break;
                    case Keys.D2:
                        States.CurrentGameState = GameStates.MAINMENU;
                        break;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {

            //TODO:delete
            //TODO:Lose Screen when dead and respawn?

            testSFX();

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
                case GameStates.MAINMENU:
                    mMainMenu();
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
                    mPause();
                    break;

                case GameStates.LOSE:
                    mLose();
                    break;

                //Exits the Game
                case GameStates.EXIT:
                    Exit();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
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
                    spriteBatch.Draw(Ressources.Load<Texture2D>("MainMenu"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.OPTIONS:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Options"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.CREDITS:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Credits"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.PAUSE:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Pause"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
                case GameStates.LOSE:
                    spriteBatch.Draw(Ressources.Load<Texture2D>("Lose"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                    break;
            }

            //draw Colliders for Debugging
            //TODO: Delete
            if (States.CurrentGameState == GameStates.PLAYING)
            {
                foreach (GameObject collider in Level.Current.gameObjects)
                {
                    spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), new Color(Color.LightGreen, 150));
                }
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
