using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Valkyrie_Nyr
{
    struct animation
    {
        public Texture2D texture;
        public int Columns;
        public int Rows;
        public int Width;
        public int Height;
        public int maxFrames;
        public animation(Texture2D _texture, int _columns, int _rows, int _maxFrames)
        {
            texture = _texture;
            Columns = _columns;
            Rows = _rows;
            Width = _texture.Width / _columns;
            Height = _texture.Height / _rows;
            maxFrames = _maxFrames;
        }
    }

    class Player : Entity
    {

        private static Player nyr;
        public int nyrFacing;

        public float speed;
        public float jumpHeight;
        public bool onIce;
        public bool inHub;
        public bool interact;
        public bool inJump;

        public animation[] animTex;
       // public Texture2D[] animTex { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        public int currentFrame;
        private int totalFrames = 0;
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame = 5;

        public Player(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, string textureType, int rows, int columns) : base(name, triggerType, mass, height, width, position, hp, dmg)
        {
            speed = 700;
            jumpHeight = 15;
            inHub = false;
            interact = false;
            inJump = false;

            animTex = new animation[]
            {
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Idle"), 10 , 3 , 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Running"), 10 , 3 , 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Jump"), 10 , 3 , 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Attack"), 10 , 3 , 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Hurt"), 10 , 2 , 18),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Dance"), 10 , 63 , 625),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Falling"), 10 , 2 , 12),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Landing"), 10 , 3 , 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Stop"), 10 , 4 , 31),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Crouch"), 10 , 3 , 25),
            };

        

            
            currentFrame = 0;
            onIce = false;
        }

        public void init()
        {
            States.CurrentPlayerState = Playerstates.IDLE;
        }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", null, 10, 180, 120, Vector2.Zero, 2000, 200, "Player/Idle", 1, 25); } return nyr; } }


       

        //put here stuff that happens if you collect something
        public void trigger(GameObject activatedTrigger)
        {
            switch(activatedTrigger.triggerType)
            {
                case "collectable":
                    collect(activatedTrigger.name);
                    Level.Current.gameObjects.Remove(activatedTrigger);
                    break;
                case "area":
                    areaTrigger(activatedTrigger.name);
                    break;
                case "loader":
                    if (interact)
                    {
                        loader(activatedTrigger.name);
                    }
                    break;
            }
        }

        private void collect(string item)
        {
            switch (item)
            {
                case "health":
                    this.health += 10;
                    break;
            }
        }

        private void areaTrigger(string activatedArea)
        {
            switch (activatedArea)
            {
                case "lava":
                    States.CurrentPlayerState = Playerstates.DEAD;
                    break;
                case "ice":
                    onIce = true;
                    break;
            }
        }

        private void loader(string newLevel)
        {
            switch (newLevel)
            {
                case "BossstageLoader":
                    Level.Current.loadLevel("Bossstage");
                    break;
                case "ErdLevelLoader":
                    Level.Current.loadLevel("ErdLevel");
                    break;
                case "EisLevelLoader":
                    Level.Current.loadLevel("EisLevel");
                    break;
                case "FeuerLevelLoader":
                    Level.Current.loadLevel("FeuerLevel");
                    break;
                case "BlitzLevelLoader":
                    Level.Current.loadLevel("BlitzLevel");
                    break;
                case "OverworldLoader":
                    Level.Current.loadLevel("Overworld");
                    break;
                case "HubLoader":
                    Level.Current.loadLevel("Hub");
                    break;
            }
        }

        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Level.Current.loadLevel("Hub");
        }

        public void activateTrigger()
        {
            GameObject[] collidedObjects = Collision<GameObject>(Level.Current.gameObjects.ToArray(), position);

            foreach (GameObject element in collidedObjects)
            {
                if (element.triggerType != null)
                {
                    trigger(element);
                }
            }
            interact = false;
        }

        //moves the Player
        public void move(Vector2 moveValue)
        {
            Vector2 newPos = position + moveValue;

            position = newPos;
        }

        public void Update(GameTime gameTime)
        {
            totalFrames = animTex[(int)States.CurrentPlayerState].maxFrames; // * animTex[(int)States.CurrentPlayerState].Columns;
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;

                currentFrame++;

                timeSinceLastFrame = 0;

                if (currentFrame == totalFrames)
                {
                    currentFrame = 0;
                    States.CurrentPlayerState = States.NextPlayerState;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            int animWidth = animTex[(int)States.CurrentPlayerState].Width;
            int animHeight = animTex[(int)States.CurrentPlayerState].Height;
            int row = (int)((float)currentFrame / animTex[(int)States.CurrentPlayerState].Columns);
            int column = currentFrame % animTex[(int)States.CurrentPlayerState].Columns;

            Rectangle sourceRectangle = new Rectangle(animWidth * column, animHeight * row, animWidth, animHeight);
            Rectangle destinationRectangle = new Rectangle((int)position.X - (animWidth / 2) + (width / 2), (int)position.Y - (animWidth / 2) + 32, animWidth, animHeight);

            if (nyrFacing == 1)
            {
                spriteBatch.Draw(animTex[(int)States.CurrentPlayerState].texture, destinationRectangle, sourceRectangle, Color.White);
            }
            else
            {
                //destinationRectangle.X = destinationRectangle.X - (sourceRectangle.Width - this.width);
                spriteBatch.Draw(animTex[(int)States.CurrentPlayerState].texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f );
            }
        }
    }
}
