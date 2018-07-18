using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Valkyrie_Nyr
{
    class Level
    {
        public List<GameObject> gameObjects;

        private int height;
        private int width;

        public Player player;
        public Vector2 position;

        Texture2D levelSprite;

        public Level(int _width, int _height, string _path, Texture2D _sprite)
        {
            //TODO: read file from path and store it to gameObjects and collider Array

            this.height = _height;
            this.width = _width;

            gameObjects = new List<GameObject>();

            gameObjects.Add(new GameObject("ground", true, false, 0, 30, 100, new Vector2(0, 200 )));
            gameObjects.Add(new GameObject("ground", true, false, 0, 30, 100, new Vector2(50, 100)));
            gameObjects.Add(new GameObject("ground", true, false, 0, 30, 3000, new Vector2(0, this.height - 30)));
            gameObjects.Add(new GameObject("collectable", false, true, 1, 10, 10, new Vector2(30, 50)));

            

            levelSprite = _sprite;

            position = new Vector2(0, 0);
        }

        private void move(GameTime gameTime, int direction)
        {
            float value = direction * player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            GameObject[] collidedObjects = player.Collision(gameObjects.ToArray(), player.position + new Vector2(value, 0));

            foreach (GameObject element in collidedObjects)
            {
                if (element.isTrigger)
                {
                    if (player.collect(element))
                    {
                       this.gameObjects.Remove(element);
                    }
                }
                else
                {
                    return;
                }
            }

            this.position.X -= value;

            foreach (GameObject gameObject in gameObjects)
            {
                if(gameObject.type == "Nyr")
                {
                    continue;
                }

                gameObject.position.X -= value;
            }
        }

        //Lässt alle Objekte fallen, wenn sie nicht schon auf dem Boden sind und überprüft, ob sie aus der Welt gefallen sind
        private void useGrav(GameTime gameTime, ref GameStates gameState)
        {

            if(player.newState == Playerstates.WALK)
            {
                player.newState = Playerstates.IDLE;
            }
            for (int i = 0; i < gameObjects.Count;)
            {
                foreach (GameObject element in gameObjects)
                {
                    if (!element.isStationary)
                    {
                        element.Fall(gameTime, gameObjects.ToArray());
                    }
                }

                //fällt aus der Welt und wird aus gelöscht
                if (gameObjects[i].position.Y > height)
                {
                    if (gameObjects[i].type == "player")
                    {
                        player.newState = Playerstates.DEAD;

                        gameState = GameStates.EXIT;
                    }
                    else
                    {
                        gameObjects.RemoveAt(i);
                    }
                }
                else
                {
                    i++;
                }
            }
        }

        public void update(GameTime gameTime, ref GameStates gameState)
        {
            //get Input from Keyboard
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                switch (element)
                {
                    //TODO: set windowSize variable
                    case Keys.A:
                        if ((int)player.position.X > 0)
                        {
                            if ((int)player.position.X <= 1920 / 2 && (int)this.position.X < 0)
                            {
                                this.move(gameTime, -1);
                            }
                            else if (this.position.X >= 0 || this.position.X <= -(this.width - 1920))
                            {
                                player.updatePos(-1, gameTime, ref gameObjects);
                            }
                        }
                            break;
                    case Keys.D:
                        if ((int)player.position.X < 1920 - player.width)
                        {
                            if ((int)player.position.X >= 1920 / 2 && (int)this.position.X > -(this.width - 1920))
                            {
                                this.move(gameTime, 1);
                            }
                            else if (this.position.X >= 0 || this.position.X <= -(this.width - 1920))
                            {
                                player.updatePos(1, gameTime, ref gameObjects);
                            }
                        }
                        break;
                    case Keys.Space:
                        if (player.newState == Playerstates.IDLE)
                        {
                            player.newState = Playerstates.IDLE_JUMP;
                            player.jump(gameTime, gameObjects.ToArray());
                        }
                        break;
                }
            }

            if (player.newState == Playerstates.IDLE_JUMP || player.newState == Playerstates.WALK_JUMP)
            {
                player.jump(gameTime, gameObjects.ToArray());
            }
            else
            {
                this.useGrav(gameTime, ref gameState);
            }

            if (player.position.Y > this.height)
            {
                gameState = GameStates.EXIT;
            }
        }

        public void update(GameTime gameTime, ref GameStates gameState, GamePadCapabilities capabilities)
        {
            //get Input from GamePad
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            if (state.ThumbSticks.Left.X != 0)
            {
                player.updatePos(state.ThumbSticks.Left.X, gameTime, ref gameObjects);
            }

            this.useGrav(gameTime, ref gameState);

            if (player.position.Y > 300)
            {
                gameState = GameStates.EXIT;
            }
        }

        public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(this.levelSprite, new Rectangle((int) this.position.X, (int) this.position.Y, this.width, this.height), Color.White);

            if (player != null)
            {
                player.render(spriteBatch, gameTime);
            }
        }
    }
}