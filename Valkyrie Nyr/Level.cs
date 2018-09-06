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
        public List<GameObject> gameObjects;
        public List<Trigger> triggerObjects;

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
            width = levelBorders.X;
            height = levelBorders.Y;
            gameObjects = JsonConvert.DeserializeObject<List<GameObject>>(File.ReadAllText(levelName + "_gameObjects.json"));
            triggerObjects = JsonConvert.DeserializeObject<List<Trigger>>(File.ReadAllText(levelName + "_triggerObjects.json"));
            foreach(Trigger element in triggerObjects)
            {
                gameObjects.Add(element);
            }
            levelBGSprite = Game1.Ressources.Load<Texture2D>(levelName);
            positionBGSprite = startPosition.ToVector2();

            gameObjects.Add(Player.Nyr);
        }

        //put here things like setup from enemies
        public void startLevel()
        {
            Player.Nyr.position = Vector2.Zero;
        }

        //get input and update the elements inside the level
        public void update(GameTime gameTime)
        {
            if (States.CurrentPlayerState == Playerstates.JUMP)
            {
                if(!Player.Nyr.onGround)
                {
                    Player.Nyr.jump(gameTime); 
                }
                else
                {
                    States.CurrentPlayerState = Playerstates.IDLE;
                }
            }

            //get Input from Keyboard
            foreach (Keys element in Keyboard.GetState().GetPressedKeys())
            {
                Vector2 moveValue = Vector2.Zero;

                switch (element)
                {
                    case Keys.A:
                        moveValue = new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        break;
                    case Keys.D:
                        moveValue = new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        break;
                    case Keys.Space:
                        if (Player.Nyr.onGround && States.CurrentPlayerState != Playerstates.JUMP)
                        {
                            States.CurrentPlayerState = Playerstates.JUMP;
                            Player.Nyr.jump(gameTime);
                        }
                        break;
                }
                //if moving, then check if you can move there
                if (moveValue != Vector2.Zero)
                {
                    if (checkCollision(moveValue))
                    {
                        Camera.Main.move(moveValue);
                    }
                }
            }
            useGrav(gameTime);
        }

        //theorethical move and seeing what happens
        bool checkCollision(Vector2 moveValue)
        {
            GameObject[] collidedObjects = Player.Nyr.Collision(gameObjects.ToArray(), Player.Nyr.position + moveValue);

            foreach (GameObject element in collidedObjects)
            {
                if (element.isTrigger && triggerObjects.Contains(element))
                {
                    foreach(Trigger triggerElement in triggerObjects)
                    {
                        if(triggerElement == element)
                        {
                            activateTrigger(triggerElement);
                            break;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void activateTrigger(Trigger element)
        {
            gameObjects.Remove(element);
            triggerObjects.Remove(element);
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
            foreach (GameObject element in gameObjects)
            {
                if (!element.isStationary)
                {
                    element.Fall(gameTime, gameObjects.ToArray());
                }
            }

            for (int i = 0; i < gameObjects.Count;)
            {
                //fällt aus der Welt und wird aus gelöscht
                if (gameObjects[i].position.Y > height)
                {
                    //und wenn es der Player ist, wird das Spiel beendet
                    if (gameObjects[i].name == "player")
                    {
                        States.CurrentPlayerState = Playerstates.DEAD;

                        States.CurrentGameState = GameStates.EXIT;
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

        //the typical render method
        public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(levelBGSprite, new Rectangle(positionBGSprite.ToPoint(), new Point(width, height)), Color.White);

            //Draw all GameObjects such as Enemys
            Player.Nyr.render(spriteBatch, gameTime);
        }
    }
}