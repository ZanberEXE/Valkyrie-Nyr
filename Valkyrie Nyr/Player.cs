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

        public Texture2D animTex { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        private int currentFrame;
        private int totalFrames;
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame = 5;

        public Player(string name, bool isTrigger, int mass, int height, int width, Vector2 position, int hp, int dmg, Texture2D texture, int rows, int columns) : base(name, isTrigger, mass, height, width, position, hp, dmg)
        {
            speed = 500;
            States.CurrentPlayerState = Playerstates.IDLE;
            jumpHeight = 15;

            animTex = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        public void init()
        {
            //spriteSheet = Game1.Ressources.Load<Texture2D>("test");
            States.CurrentPlayerState = Playerstates.IDLE;
        }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", false, 10, 180, 120, Vector2.Zero, 2000, 200, Game1.Ressources.Load<Texture2D>("Player/Idle"), 1, 25); } return nyr; } }


        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Console.WriteLine("You died!");
        }

        //put here stuff that happens if you collect something
        public void collect(GameObject collectable)
        {

        }

        public void activateTrigger()
        {
            GameObject[] collidedObjects = Collision(Level.Current.gameObjects.ToArray(), position);

            foreach (GameObject element in collidedObjects)
            {
                if (element.isTrigger)
                {
                    collect(element);
                    Level.Current.gameObjects.Remove(element);
                    for (int j = 0; j < Level.Current.triggerObjects.Count; j++)
                    {
                        if (Level.Current.triggerObjects[j] == element)
                        {
                            Level.Current.triggerObjects.RemoveAt(j);
                        }
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
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = animTex.Width / Columns;
            int height = animTex.Height / Rows;
            int row = (int)((float)currentFrame / Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            spriteBatch.Draw(animTex, destinationRectangle, sourceRectangle, Color.White);
        }
        //TODO: überarbeiten (nicht ganz funktionstauglich)
        /*public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {

            frame += (float)gameTime.ElapsedGameTime.TotalSeconds * 2; //gibt die geschwindigkeit der Animation an

            if ((int)frame > animLength[(int)States.CurrentPlayerState])
            {
                frame = 0;
                States.CurrentPlayerState = States.NextPlayerState;
            }

            if (States.CurrentPlayerState == Playerstates.IDLE)
            {
                spriteBatch.Draw(spriteSheet, position, new Rectangle(30 * (int) frame, 0, 20, 30), Color.White);
            }
            else if (States.CurrentPlayerState == Playerstates.WALK)
            {
                spriteBatch.Draw(spriteSheet, position, new Rectangle(30 * (int)frame, 20, 20, 30), Color.White);
            }
        }*/
    }
}
