using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Valkyrie_Nyr
{
    enum Playerstates { IDLE, WALK, IDLE_JUMP, WALK_JUMP, FIGHT, HIT, DEAD };

    class Player : GameObject
    {
        //only change State, if Animation is played completely
        public Playerstates newState;
        private Playerstates state;

        private int[] animLength;
        
        private float frame;

        private Texture2D spriteSheet;

        public float speed;

        private float jumpValue;
        private float jumpHeight;

        public Player(string name, bool isStationary, bool isTrigger, int mass, int height, int width, Vector2 position, SpriteBatch spriteBatch, Texture2D _spriteSheet) : base(name, isStationary, isTrigger, mass, height, width, position)
        {
            spriteSheet = _spriteSheet;
            speed = 250;
            frame = 0;
            state = Playerstates.IDLE;
            animLength = new int[] { 3, 2, 2, 3, 4, 3 };
            jumpValue = 0;
            jumpHeight = 1;
        }

        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Console.WriteLine("You died!");
        }

        public bool collect(GameObject collectable)
        {
            //just to make sure, if the gameObject is indeed a collectable
            if(collectable.type == "collectable")
            {
                return true;
            }
            return false;
        }

        public void updatePos(float direction, GameTime gameTime, ref List<GameObject> gameObjects)
        {
            Vector2 newPos = new Vector2(position.X + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, position.Y);

            GameObject[] collidedObjects = Collision(gameObjects.ToArray(), newPos);

            foreach (GameObject element in collidedObjects)
            {
                if (element.isTrigger)
                {
                    if(this.collect(element))
                    {
                        gameObjects.Remove(element);
                    }                    
                }
                else
                {
                    return;
                }
            }

            this.position = newPos;

            if(this.newState == Playerstates.IDLE_JUMP || this.newState == Playerstates.WALK_JUMP)
            {
                this.newState = Playerstates.WALK_JUMP;
            }
            else
            {
                this.newState = Playerstates.WALK;
            }
        }

        public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {

            frame += (float)gameTime.ElapsedGameTime.TotalSeconds * 2; //gibt die geschwindigkeit der Animation an

            if ((int)frame > animLength[(int) state])
            {
                frame = 0;
                state = newState;
            }

            if (state == Playerstates.IDLE)
            {
                spriteBatch.Draw(spriteSheet, position, new Rectangle(30 * (int) frame, 0, 20, 30), Color.White);
            }
            else if (state == Playerstates.WALK)
            {
                spriteBatch.Draw(spriteSheet, position, new Rectangle(30 * (int)frame, 20, 20, 30), Color.White);
            }
        }

        public void jump(GameTime gameTime, GameObject[] gameObjects)
        {
            if (jumpHeight < 150)
            {
                jumpValue += 0.125f;
                jumpHeight += 300 * (float)gameTime.ElapsedGameTime.TotalSeconds / jumpValue;
                float newPosY = -1 * 300 * (float)gameTime.ElapsedGameTime.TotalSeconds / jumpValue + this.position.Y;
                GameObject[] collidedObjects = Collision(gameObjects, new Vector2(this.position.X, newPosY));
                foreach (GameObject element in collidedObjects)
                {
                    if (!element.isTrigger)
                    {
                        return;
                    }
                }
                this.position.Y = newPosY;
            }
            else
            {
                Vector2 newPosition = new Vector2(this.position.X, this.position.Y + this.mass * this.gravitation * this.gravValue * (float)gameTime.ElapsedGameTime.TotalSeconds);
                GameObject[] collidedObjects = Collision(gameObjects, newPosition); //all Objects, that you would Collide with if you'd fall

                foreach (GameObject element in collidedObjects)
                {
                    if (!element.isTrigger)
                    {
                        this.gravValue = 1;
                        this.jumpValue = 0;
                        this.state = Playerstates.IDLE;
                        this.newState = Playerstates.IDLE;
                        jumpHeight = 0;
                        jumpValue = 0;
                        return;
                    }
                }
                this.position = newPosition; ;
                this.gravValue++;
            }
        }
    }
}
