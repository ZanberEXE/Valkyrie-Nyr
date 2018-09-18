using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Valkyrie_Nyr
{

    public class Game1 : Game
    {        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //white 1x1 texture
        Texture2D pxl;

        public static ContentManager Ressources;
        public static Point WindowSize;

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
        }

        protected override void Initialize()
        {
            //TODO:delete
            pxl = Game1.Ressources.Load<Texture2D>("index");
            
            //create Nyr
            Player.Nyr.init();

            spriteBatch = new SpriteBatch(GraphicsDevice);
                        
            States.CurrentGameState = GameStates.MAINMENU;

            base.Initialize();
        }

        //TODO: proper content
        void mMainMenu()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.D1:
                        Level.Current.loadLevel("Overworld");
                        States.CurrentGameState = GameStates.PLAYING;
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
            if (States.CurrentPlayerState == Playerstates.DEAD)
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && States.CurrentGameState == GameStates.PLAYING)
            {
                States.CurrentGameState = GameStates.PAUSE;
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
