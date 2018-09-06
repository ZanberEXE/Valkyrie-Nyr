using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Valkyrie_Nyr
{

    class Player : GameObject
    {

        private static Player nyr;

        private int[] animLength;
        
        private float frame;

        private Texture2D spriteSheet;

        public float speed;

        private float jumpHeight;

        public Player(string name, bool isStationary, bool isTrigger, int mass, int height, int width, Vector2 position) : base(name, isStationary, isTrigger, mass, height, width, position)
        {
            speed = 250;
            frame = 0;
            States.CurrentPlayerState = Playerstates.IDLE;
            animLength = new int[] { 3, 2, 2, 3, 4, 3 };
            jumpHeight = 10;
        }

        public void init()
        {
            spriteSheet = Game1.Ressources.Load<Texture2D>("test");
            States.CurrentPlayerState = Playerstates.IDLE;
        }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", false, false, 10, 30, 20, new Vector2(10, 10)); } return nyr; } }

        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Console.WriteLine("You died!");
        }

        //put here stuff that happens if you collect something
        public void collect(GameObject collectable)
        {

        }

        //moves the Player
        public void move(Vector2 moveValue)
        {
            Vector2 newPos = position + moveValue;

            GameObject[] collidedObjects = Collision(Level.Current.gameObjects.ToArray(), newPos);

            foreach (GameObject element in collidedObjects)
            {
                if (element.isTrigger)
                {
                    collect(element);
                    Level.Current.gameObjects.Remove(element);
                }
                else
                {
                    return;
                }
            }

            position = newPos;
        }

        //TODO: überarbeiten (nicht ganz funktionstauglich)
        public void render(SpriteBatch spriteBatch, GameTime gameTime)
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
        }

        //move the PLayer up, until it hit something or the gravition gets too strong and pulls him down
        public void jump(GameTime gameTime)
        {
            GameObject[] collidedObjects;
            collidedObjects = this.Collision(Level.Current.gameObjects.ToArray(), this.position - new Vector2(0, jumpHeight));

            foreach(GameObject element in collidedObjects)
            {
                if (!element.isTrigger)
                {
                    States.CurrentPlayerState = Playerstates.IDLE;
                    return;
                }
            }
            
            onGround = false;
            position.Y -= jumpHeight;
        }
    }
}
