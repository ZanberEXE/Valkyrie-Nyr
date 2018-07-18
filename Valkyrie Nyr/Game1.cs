using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Valkyrie_Nyr
{

    enum GameStates { MAINMENU, PLAYING, EXIT}


    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D test;
        Texture2D pxl;

        GamePadCapabilities capabilities;

        GameStates state;
        
        Level level;
        
        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferHeight = 900;
            this.graphics.PreferredBackBufferWidth = 1920;
            //this.graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.state = GameStates.PLAYING;
        }

        protected override void Initialize()
        {
            this.test = Content.Load<Texture2D>("test");
            this.pxl = Content.Load<Texture2D>("index");
            Texture2D levelSprite = Content.Load<Texture2D>("overworld");

            this.level = new Level(2700, 900, "", levelSprite);

            this.level.player = new Player("Nyr", false, false, 10, 30, 20, new Vector2(10, 10), this.spriteBatch, this.test);
            this.level.gameObjects.Add(this.level.player);



            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.capabilities = GamePad.GetCapabilities(PlayerIndex.One);
        }
        
        protected override void Update(GameTime gameTime)
        {
            //press escape to close
            //TODO:delete
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                state = GameStates.EXIT;
            }

            switch (this.state)
            {
                case GameStates.MAINMENU:
                    //TODO: Fill with Content
                    break;

                case GameStates.PLAYING:
                    //Use one Update-Menthod for keyboard and one for controller
                    if (this.capabilities.IsConnected)
                    {
                        this.level.update(gameTime, ref this.state, this.capabilities);
                    }
                    else
                    {
                        this.level.update(gameTime, ref this.state);
                    }
                    break;

                //Exits the Game
                case GameStates.EXIT:
                    this.Exit();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();

            this.level.render(spriteBatch, gameTime);

            //draw Colliders for Debugging
            //TODO: Delete
            foreach (GameObject collider in level.gameObjects)
            {
                this.spriteBatch.Draw(pxl, new Rectangle((int)collider.position.X, (int)collider.position.Y, collider.width, collider.height), Color.LightGreen * 0.5f);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
