using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Valkyrie_Nyr
{
    class Level
    {
        public Enemy ryn;

        public List<GameObject> gameObjects;
        public List<Trigger> triggerObjects;
        public List<Entity> entityObjects;

        public int height;
        public int width;

        public Vector2 positionBGSprite;

        private static Level currentLevel;

        Texture2D levelBGSprite;

        //get current Level from everywhere
        public static Level Current { get { if (currentLevel == null) { currentLevel = new Level(); } return currentLevel; } }

        //loads the level
        public void loadLevel(Point startPosition, Point levelBorders, string levelName)
        {
            ryn = new Enemy("ryn", false, 5, 100, 60, new Vector2(300, 0), 300, 20);

            width = levelBorders.X;
            height = levelBorders.Y;
            
            gameObjects = JsonConvert.DeserializeObject<List<GameObject>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_gameObjects.json"));
            
            triggerObjects = JsonConvert.DeserializeObject<List<Trigger>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_triggerObjects.json"));

            entityObjects = JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_entityObjects.json"));

          

            foreach (Trigger element in triggerObjects)
            {
                gameObjects.Add(element);
            }
            foreach (Entity element in entityObjects)
            {
                gameObjects.Add(element);
            }

            foreach (GameObject element in gameObjects)
            {
                element.position += startPosition.ToVector2();
            }
            
            levelBGSprite = Game1.Ressources.Load<Texture2D>(levelName);
            positionBGSprite = new Vector2(startPosition.X, startPosition.Y);

            entityObjects.Add(Player.Nyr);
            gameObjects.Add(Player.Nyr);

            Camera.Main.levelBounds = new Rectangle(startPosition, new Point(width, height));
        }

        //put here things like setup from enemies
        public void startLevel()
        {
            Player.Nyr.position = Vector2.Zero;
        }

        //get input and update the elements inside the level
        public void update(GameTime gameTime)
        {
            Vector2 moveValue = Vector2.Zero;

            if (States.CurrentPlayerState == Playerstates.JUMP)
            {
                if(!Player.Nyr.onGround)
                {
                    moveValue.Y -= Player.Nyr.jumpHeight;
                }
                else
                {
                    States.CurrentPlayerState = Playerstates.IDLE;
                }
            }

            //get Input from Keyboard
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {

                switch (element)
                {
                    case Keys.A:
                        moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        break;
                    case Keys.D:
                        moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        break;
                    case Keys.Space:
                        if (Player.Nyr.onGround && States.CurrentPlayerState != Playerstates.JUMP)
                        {
                            States.CurrentPlayerState = Playerstates.JUMP;
                            moveValue.Y -= Player.Nyr.jumpHeight;
                        }
                        break;
                    case Keys.Enter:
                        Player.Nyr.attack();
                        break;
                }
            }

            //Let PLayer fall and save the moveValue in overall Movement
            moveValue += Player.Nyr.Fall(gameTime, gameObjects.ToArray()) - Player.Nyr.position;

            //let em move, after all collisions have manipulated the movement
            Vector2 newMoveValue = checkCollision(moveValue);

            if (newMoveValue != Vector2.Zero)
            {
                Camera.Main.move(newMoveValue);
            }

            //Let all other gameObjects fall to gravitation
            useGrav(gameTime);

            //trigger all triggers, that have been triggered
            Player.Nyr.activateTrigger();
        }

        //theoretical move and seeing what happens
        public Vector2 checkCollision(Vector2 moveValue)
        {
            Vector2 newPos = Player.Nyr.position + moveValue;

            GameObject[] collidedObjects = Player.Nyr.Collision(gameObjects.ToArray(), newPos);

            bool collidedLeft = false;
            bool collidedRight = false;
            bool collidedTop = false;

            foreach (GameObject element in collidedObjects)
            {
                if (element.position.X + element.width > newPos.X && element.position.X + element.width < Player.Nyr.position.X && element.name != "platform")
                {
                    collidedLeft = true;
                }
                if (element.position.X < newPos.X + Player.Nyr.width && element.position.X > Player.Nyr.position.X + Player.Nyr.width && element.name != "platform")
                {
                    collidedRight = true;
                }
                if (element.position.Y + element.height >= newPos.Y && element.position.Y + element.height <= Player.Nyr.position.Y && element.name != "platform")
                {
                    collidedTop = true;
                }
            }

            if (collidedLeft || collidedRight)
            {
                moveValue.X = 0;
            }
            if (collidedTop)
            {
                moveValue.Y = 0;
            }
            return moveValue;
        }

        //move all Objects in this Level
        public void moveGameObjects(Vector2 moveValue)
        {
            positionBGSprite -= moveValue;
            foreach (GameObject gameObject in gameObjects)
            {
                if(gameObject.name == "Nyr")
                {
                    continue;
                }

                gameObject.position -= moveValue;
            }
        }

        //Lässt alle Objekte fallen, wenn sie nicht schon auf dem Boden sind und überprüft, ob sie aus der Welt gefallen sind
        private void useGrav(GameTime gameTime)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].mass > 0 && !gameObjects[i].onGround)
                {
                    if (gameObjects[i].name == "Nyr")
                    {
                        continue;
                    }
                    else
                    {
                        gameObjects[i].position = gameObjects[i].Fall(gameTime, gameObjects.ToArray());
                    }
                }
            }

            for (int i = 0; i < gameObjects.Count;)
            {
                //fällt aus der Welt und wird gelöscht
                if (gameObjects[i].position.Y > height)
                {
                    for (int j = 0; j < triggerObjects.Count(); j++)
                    {
                        if (triggerObjects[j] == gameObjects[i])
                        {
                            triggerObjects.RemoveAt(j);
                        }
                    }
                    gameObjects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        //the typical render method
        public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(levelBGSprite, new Rectangle(positionBGSprite.ToPoint(), new Point(width, height)), Color.White);

            //Draw all GameObjects such as Enemys
            foreach (Entity element in entityObjects)
            {
                element.entityRender(gameTime, spriteBatch);
            }
        }
    }
}