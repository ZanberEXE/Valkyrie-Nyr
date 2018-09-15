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

        private int[] animLength;
        
        private float frame;

        private Texture2D spriteSheet;

        public float speed;

        public float jumpHeight;

        

        public Player(string name, bool isTrigger, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, isTrigger, mass, height, width, position, hp, dmg)
        {
            speed = 500;
            frame = 0;
            States.CurrentPlayerState = Playerstates.IDLE;
            animLength = new int[] { 3, 2, 2, 3, 4, 3 };
            jumpHeight = 15;
            
        }

        public void init()
        {
            //spriteSheet = Game1.Ressources.Load<Texture2D>("test");
            States.CurrentPlayerState = Playerstates.IDLE;
        }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", false, 10, 180, 120, Vector2.Zero, 2000, 200); } return nyr; } }


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
            /*if(moveValue.X < 0)
            {
                nyrFacing = 1;
            }
            if (moveValue.X > 0)
            {
                nyrFacing = 2;
            }
                                                            //nyrFacing = Entity.facingDirection(position.X, newPos.X);*/
            position = newPos;
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
