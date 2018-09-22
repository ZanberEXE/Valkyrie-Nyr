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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Valkyrie_Nyr
{
    class Level
    {
        //my shit
        public SoundEffect jump, attack, thud;


        public Enemy ryn;

        public List<GameObject> gameObjects;
        public List<Entity> entityObjects;

        public int height;
        public int width;

        int atkCooldown;

        public Vector2 positionBGSprite;

        private static Level currentLevel;

        Texture2D levelBGSprite;

        Keys[] lastPressedKeys;

        //get current Level from everywhere
        public static Level Current { get { if (currentLevel == null) { currentLevel = new Level(); } return currentLevel; } }

        //loads the level
        public void loadLevel(string levelName)
        {
            ryn = new Enemy("ryn", null, 5, 100, 60, new Vector2(300, 0), 300, 20);

            Point startPosition;
            Player.Nyr.inHub = false;

            switch (levelName)
            {
                case "Bossstage":
                    width = 7500 * Camera.Main.zoom;
                    height = 2500 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y - 100));
                    Player.Nyr.position = new Vector2(600, Game1.WindowSize.Y / 2);
                    break;
                case "Hub":
                    width = 1125 * Camera.Main.zoom;
                    height = 625 * Camera.Main.zoom;
                    startPosition = new Point(-(width - Game1.WindowSize.X), -(height - Game1.WindowSize.Y - 50));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X - Player.Nyr.width, Game1.WindowSize.Y - Player.Nyr.height);
                    Player.Nyr.inHub = true;
                    break;
                case "Overworld":
                    width = 3000 * Camera.Main.zoom;
                    height = 1000 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(0, Game1.WindowSize.Y / 2);
                    break;
                case "BlitzLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(-500, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                default:
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(0, Game1.WindowSize.Y / 2);
                    break;
            }
            
            gameObjects = JsonConvert.DeserializeObject<List<GameObject>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_gameObjects.json"));
            
            entityObjects = JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_entityObjects.json"));
            
            foreach (Entity element in entityObjects)
            {
                gameObjects.Add(element);
            }

            foreach (GameObject element in gameObjects)
            {
                element.position += startPosition.ToVector2();
                element.init();
            }
            
            levelBGSprite = Game1.Ressources.Load<Texture2D>(levelName);
            positionBGSprite = new Vector2(startPosition.X, startPosition.Y);

            entityObjects.Add(Player.Nyr);
            gameObjects.Add(Player.Nyr);

            Camera.Main.levelBounds = new Rectangle(startPosition, new Point(width, height));

            Camera.Main.position = Vector2.Zero;

            States.CurrentPlayerState = Playerstates.IDLE;


            //sound test
            jump = Game1.Ressources.Load<SoundEffect>("sfx/sfx_jump");
            attack = Game1.Ressources.Load<SoundEffect>("sfx/sfx_collide");
            thud = Game1.Ressources.Load<SoundEffect>("sfx/sfx_thud");
        }

        //get input and update the elements inside the level
        public void update(GameTime gameTime)
        {
            Vector2 moveValue = Vector2.Zero;

            Player.Nyr.Update(gameTime);

            //Let PLayer fall and save the moveValue in overall Movement
            if (!Player.Nyr.inHub)
            {
                moveValue += Player.Nyr.Fall(gameTime, gameObjects.ToArray()) - Player.Nyr.position;
            }

            if (Player.Nyr.inJump)
            {
                if (!Player.Nyr.onGround)
                {
                    moveValue.Y -= Player.Nyr.jumpHeight;
                }
                else
                {
                    Player.Nyr.inJump = false;
                    States.CurrentPlayerState = Playerstates.LAND;
                    States.NextPlayerState = Playerstates.IDLE;
                }
            }

            if (atkCooldown > 0)
            {
                atkCooldown--;
            }

            //get Input from Keyboard
            bool anyKeyPressed = false;

            Keys[] newPressedKeys = Keyboard.GetState().GetPressedKeys();

            foreach (Keys element in newPressedKeys)
            {
                anyKeyPressed = true;

                switch (element)
                {
                    case Keys.A:
                        moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        Player.Nyr.nyrFacing = -1;
                        if (States.CurrentPlayerState == Playerstates.IDLE)
                        {
                            States.CurrentPlayerState = Playerstates.WALK;
                        }
                        break;
                    case Keys.D:
                        moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        Player.Nyr.nyrFacing = 1;
                        if (States.CurrentPlayerState == Playerstates.IDLE)
                        {
                            States.CurrentPlayerState = Playerstates.WALK;
                        }
                        break;
                    case Keys.Space:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            if (Player.Nyr.onGround && !Player.Nyr.inJump && !Player.Nyr.inHub)
                            {
                                States.CurrentPlayerState = Playerstates.JUMP;
                                States.NextPlayerState = Playerstates.FALL;
                                Player.Nyr.inJump = true;
                                Player.Nyr.onGround = false;
                                moveValue.Y -= Player.Nyr.jumpHeight;

                                
                                attack.CreateInstance().Play();
                                
                            }
                        }
                        break;
                    case Keys.W:
                        if (Player.Nyr.inHub)
                        {
                            moveValue += new Vector2(0, -1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            if (States.CurrentPlayerState != Playerstates.WALK)
                            {
                                States.CurrentPlayerState = Playerstates.WALK;
                            }
                        }
                        break;
                    case Keys.S:
                        if (Player.Nyr.inHub)
                        {
                            moveValue += new Vector2(0, 1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            if (States.CurrentPlayerState != Playerstates.WALK)
                            {
                                States.CurrentPlayerState = Playerstates.WALK;
                            }
                        }
                        break;
                    case Keys.F:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            Player.Nyr.interact = true;
                        }
                        break;
                    case Keys.LeftShift:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            if (atkCooldown == 0 && !Player.Nyr.inHub)
                            {
                                States.CurrentPlayerState = Playerstates.FIGHT;
                                States.NextPlayerState = Playerstates.IDLE;
                                Player.Nyr.attack(Player.Nyr.nyrFacing);
                                atkCooldown = 60;

                                jump.CreateInstance().Play();
                            }
                        }
                        break;
                }
            }

            

            //let em move, after all collisions have manipulated the movement
            Vector2 newMoveValue = checkCollision(moveValue);

            if (newMoveValue != Vector2.Zero)
            {
                if (States.CurrentPlayerState == Playerstates.FALL && Player.Nyr.onGround)
                {
                    States.CurrentPlayerState = Playerstates.LAND;
                    States.NextPlayerState = Playerstates.IDLE;
                }

                Camera.Main.move(newMoveValue);
            }


            //Let all other gameObjects fall to gravitation
            useGrav(gameTime);

            //trigger all triggers, that have been triggered
            Player.Nyr.activateTrigger();

            if (!anyKeyPressed)
            {
                if (!(States.CurrentPlayerState == Playerstates.JUMP || States.CurrentPlayerState == Playerstates.FALL || States.CurrentPlayerState == Playerstates.LAND))
                {
                    if (States.CurrentPlayerState == Playerstates.WALK && Player.Nyr.onGround)
                    {
                        States.CurrentPlayerState = Playerstates.STOP;
                    }
                    States.NextPlayerState = Playerstates.IDLE;
                }
            }

            lastPressedKeys = newPressedKeys;
        }

        //theoretical move and seeing what happens
        public Vector2 checkCollision(Vector2 moveValue)
        {
            Vector2 newPos = Player.Nyr.position + moveValue;

            GameObject[] collidedObjects = Player.Nyr.Collision<GameObject>(gameObjects.ToArray(), newPos);

            bool collidedLeft = false;
            bool collidedRight = false;
            bool collidedTop = false;
            bool collidedBottom = false;

            foreach (GameObject element in collidedObjects)
            {
                if (element.triggerType != null)
                {
                    continue;
                }

                if (element.position.X + element.width > newPos.X && element.position.X + element.width < Player.Nyr.position.X && element.name != "platform")
                {
                    collidedLeft = true;
                }
                else if (element.position.X < newPos.X + Player.Nyr.width && element.position.X > Player.Nyr.position.X + Player.Nyr.width && element.name != "platform")
                {
                    collidedRight = true;
                }
                else if (element.position.Y + element.height >= newPos.Y && element.position.Y + element.height <= Player.Nyr.position.Y && element.name != "platform")
                {
                    collidedTop = true;
                }
                else if (element.position.Y <= newPos.Y + Player.Nyr.height && element.position.Y > newPos.Y)
                {
                    collidedBottom = true;
                }
            }

            if (collidedBottom)
            {
                Player.Nyr.onGround = true;
            }
            if (moveValue.Y == 0)
            {
                Player.Nyr.onGround = true;
            }
            if (moveValue.Y < 0)
            {
                Player.Nyr.onGround = false;
            }

            if (collidedLeft || collidedRight)
            {
                moveValue.X = 0;
            }
            if ((collidedTop && moveValue.Y < 0) || (collidedBottom && moveValue.Y > 0))
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
            foreach (GameObject element in gameObjects)
            {
                element.Draw(gameTime, spriteBatch);
            }
            Player.Nyr.Draw(spriteBatch);
        }
    }
}