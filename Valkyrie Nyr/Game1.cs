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

        //TODO: delete
        void mainMenu()
        {
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    case Keys.NumPad1:
                        Level.Current.loadLevel(new Point(0, -9000), new Point(7500 * Camera.Main.zoom, 2500 * Camera.Main.zoom), "Overworld");
                        States.CurrentGameState = GameStates.PLAYING;
                        return;
                    case Keys.NumPad2:
                        Level.Current.loadLevel(Point.Zero, new Point(3750 * Camera.Main.zoom, 1250 * Camera.Main.zoom), "ErdLevel");
                        States.CurrentGameState = GameStates.PLAYING;
                        return;
                    case Keys.NumPad3:
                        //Level.Current.loadLevel(Point.Zero, new Point(15000 * Camera.Main.zoom, 5000 * Camera.Main.zoom), "Level_Eis_halfed");
                        //States.CurrentGameState = GameStates.PLAYING;
                        return;
                    case Keys.NumPad4:
                        Level.Current.loadLevel(Point.Zero, new Point(3750 * Camera.Main.zoom, 1250 * Camera.Main.zoom), "FeuerLevel");
                        States.CurrentGameState = GameStates.PLAYING;
                        return;
                    case Keys.NumPad5:
                        Level.Current.loadLevel(Point.Zero, new Point(3750 * Camera.Main.zoom, 1250 * Camera.Main.zoom), "BlitzLevel");
                        States.CurrentGameState = GameStates.PLAYING;
                        return;
                    case Keys.Escape:
                        States.CurrentGameState = GameStates.EXIT;
                        return;
                }
            }
        }
        
        protected override void Update(GameTime gameTime)
        {

            //TODO:deleted
            if (States.CurrentPlayerState == Playerstates.DEAD)
            {
                Exit();
            }

            //press escape to close
            //TODO:delete
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                States.CurrentGameState = GameStates.EXIT;
            }

            switch (States.CurrentGameState)
            {
                case GameStates.MAINMENU:
                    //TODO: Fill with Content
                    mainMenu();
                    break;

                case GameStates.PLAYING:
                    Level.Current.update(gameTime);
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
            

            if (States.CurrentGameState == GameStates.PLAYING)
            {
                Level.Current.render(spriteBatch, gameTime);

                //draw Colliders for Debugging
                //TODO: Delete
                int r = 0;
                int g = 0;
                int b = 0;
                foreach (GameObject collider in Level.Current.gameObjects)
                {
                    spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), new Color(r, g, b, 255));
                    if(b == 0 && r != 256)
                    {
                        r += 8;
                    }
                    if (r == 256 && g != 256)
                    {
                        g += 8;
                    }
                    if (g == 256 && b != 256)
                    {
                        b += 8;
                    }
                    if(b == 256 && r != 0)
                    {
                        r -= 8;
                    }
                    if(r == 0 && g != 0)
                    {
                        g -= 8;
                    }
                    if(g == 0 && b != 0)
                    {
                        b -= 8;
                    }
                }
            }
            else if (States.CurrentGameState == GameStates.MAINMENU)
            {
                spriteBatch.Draw(Ressources.Load<Texture2D>("mainMenu"), new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
