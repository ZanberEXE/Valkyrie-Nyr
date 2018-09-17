using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Valkyrie_Nyr
{

    class Player : Entity
    {

        private static Player nyr;
        public int nyrFacing;

        public float speed;
        public float jumpHeight;
        public bool onIce;

        public Texture2D[] animTex { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        public int currentFrame;
        private int totalFrames;
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame = 5;

        public Player(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, string textureType, int rows, int columns) : base(name, triggerType, mass, height, width, position, hp, dmg)
        {
            speed = 500;
            jumpHeight = 15;

            animTex = new Texture2D[]
            {
                Game1.Ressources.Load<Texture2D>("Player/Idle"),
                Game1.Ressources.Load<Texture2D>("Player/Walking Side"),
                Game1.Ressources.Load<Texture2D>("Player/Jump"),
                Game1.Ressources.Load<Texture2D>("Player/Attack"),
                Game1.Ressources.Load<Texture2D>("Player/Hurt"),
                Game1.Ressources.Load<Texture2D>("Player/Dead"),
                Game1.Ressources.Load<Texture2D>("Player/Dance"),
                Game1.Ressources.Load<Texture2D>("Player/Falling"),
                Game1.Ressources.Load<Texture2D>("Player/Landing"),
                Game1.Ressources.Load<Texture2D>("Player/Stop Running"),
                Game1.Ressources.Load<Texture2D>("Player/Crouch")
            };
            
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
            onIce = false;
        }

        public void init()
        {
            States.CurrentPlayerState = Playerstates.IDLE;
        }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", "", 10, 180, 120, Vector2.Zero, 2000, 200, "Player/Idle", 1, 25); } return nyr; } }


        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Console.WriteLine("You died!");
        }

        //put here stuff that happens if you collect something
        public void trigger(GameObject activatedTrigger)
        {
            switch(activatedTrigger.triggerType)
            {
                case "collectable":
                    collect(activatedTrigger.name);
                    break;
                case "area":
                    areaTrigger(activatedTrigger.name);
                    break;
                case "loader":
                    loader(activatedTrigger.name);
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

        public void activateTrigger()
        {
            GameObject[] collidedObjects = Collision<GameObject>(Level.Current.gameObjects.ToArray(), position);

            foreach (GameObject element in collidedObjects)
            {
                if (element.triggerType == "")
                {
                    trigger(element);
                    Level.Current.gameObjects.Remove(element);
                    for (int j = 0; j < Level.Current.triggerObjects.Count; j++)
                    {
                        //if (Level.Current.triggerObjects[j] == element)
                        //{
                        //    Level.Current.triggerObjects.RemoveAt(j);
                        //}
                    }
                }
            }
        }

        //moves the Player
        public void move(Vector2 moveValue)
        {
            Vector2 newPos = position + moveValue;

            position = newPos;
        }

        public void Update(GameTime gameTime)
        {
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
            int width = animTex[(int)States.CurrentPlayerState].Width / Columns;
            int height = animTex[(int)States.CurrentPlayerState].Height / Rows;
            int row = (int)((float)currentFrame / Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            if (nyrFacing == 1)
            {
                spriteBatch.Draw(animTex[(int)States.CurrentPlayerState], destinationRectangle, sourceRectangle, Color.White);
            }
            else
            {
                destinationRectangle.X = destinationRectangle.X - (sourceRectangle.Width - this.width);
                spriteBatch.Draw(animTex[(int)States.CurrentPlayerState], destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f );
            }
        }
    }
}
