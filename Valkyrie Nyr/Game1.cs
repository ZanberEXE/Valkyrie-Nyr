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
        public static string CurrentRootDirectory;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //set windowsize
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1920;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            //Now you can load content from everywhere
            Ressources = Content;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            pxl = Game1.Ressources.Load<Texture2D>("index");
            

            //create Nyr
            Player.Nyr.init();

            spriteBatch = new SpriteBatch(GraphicsDevice);



            //load Level
            //Level.Current.loadLevel(Point.Zero, new Point(15000 * 2, 5000 * 2), "Level_Overworld_halfed");

            //set the borders of the Level to the Cameradd
            
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
                        Level.Current.loadLevel(new Point(0, -9000), new Point(7500 * Camera.Main.zoom, 2500 * Camera.Main.zoom), "Overworld");
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

            //TODO:deleted
            //TODO:Lose Screen when dead and respawn?
            if (States.CurrentPlayerState == Playerstates.DEAD)
            {
                Exit();
            }

            //press escape to close
            //TODO:delete
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //{
            //    States.CurrentGameState = GameStates.EXIT;
            //}
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && States.CurrentGameState == GameStates.PLAYING)
            {
                States.CurrentGameState = GameStates.PAUSE;
            }

            switch (States.CurrentGameState)
            {
                case GameStates.MAINMENU:
                    //TODO: Fill with Content
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

            //if (States.CurrentGameState == GameStates.PLAYING)
            //{
            //    Level.Current.render(spriteBatch, gameTime);

            //    //draw Colliders for Debugging
            //    //TODO: Delete
            //    /*  UM ANIMATION ZU PRÜFEN
            //    int r = 0;
            //    int g = 0;
            //    int b = 0;
            //    foreach (GameObject collider in Level.Current.gameObjects)
            //    {
            //        spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), new Color(r, g, b, 255));
            //        if(b == 0 && r != 256)
            //        {
            //            r += 8;
            //        }
            //        if (r == 256 && g != 256)
            //        {
            //            g += 8;
            //        }
            //        if (g == 256 && b != 256)
            //        {
            //            b += 8;
            //        }
            //        if(b == 256 && r != 0)
            //        {
            //            r -= 8;
            //        }
            //        if(r == 0 && g != 0)
            //        {
            //            g -= 8;
            //        }
            //        if(g == 0 && b != 0)
            //        {
            //            b -= 8;
            //        }
            //    }
            //    */// UM ANIMATION ZU PRÜFEN
            //}
            //else if (States.CurrentGameState == GameStates.MAINMENU)
            //{
            //    spriteBatch.Draw(Ressources.Load<Texture2D>("MainMenu"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            //}


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
