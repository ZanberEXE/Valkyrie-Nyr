using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Valkyrie_Nyr
{



    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //white 1x1 texture
        Texture2D pxl;

        public static ContentManager Ressources;

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

            //load Level
            Level.Current.loadLevel(Point.Zero, new Point(2890, 900), "test.json", "overworld");

            //set the borders of the Level to the Camera
            Camera.Main.levelBounds = new Rectangle(0, 0, Level.Current.width, Level.Current.height);
            
            States.CurrentGameState = GameStates.PLAYING;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }
        
        protected override void Update(GameTime gameTime)
        {
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

            Level.Current.render(spriteBatch, gameTime);

            //draw Colliders for Debugging
            //TODO: Delete
            foreach (GameObject collider in Level.Current.gameObjects)
            {
                spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), Color.LightGreen * 0.5f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
