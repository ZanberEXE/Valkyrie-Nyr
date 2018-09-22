﻿using Microsoft.Xna.Framework;
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
        public SoundEffect attack, collect, hurt, jump, warning;

        public string textboxText = "";
        public Enemy ryn;

        public List<GameObject> gameObjects;
        //public List<Entity> entityObjects;
        public List<Enemy> enemyObjects;
        public List<NSC> nscObjects;
        public List<Projectile> projectileObjects;

        public int height;
        public int width;

        int atkCooldown;

        int dashtimer;
        bool hasDashed = false;
        Vector2 tempposition;

        public Vector2 positionBGSprite;

        private static Level currentLevel;

        Texture2D levelBGSprite;

        Keys[] lastPressedKeys;

        //get current Level from everywhere
        public static Level Current { get { if (currentLevel == null) { currentLevel = new Level(); } return currentLevel; } }

        //
        //all beaten bosses in this order: Ina (Fire), Yinyin (Ice), Aiye(Earth), Monomono (Blitz)
        public static bool[] soulsRescued = new bool[] { true, true, true, true };
        //all enhanced Armor in this order: Torso (Fire), Guntlet (Ice), Shoes(Earth), Headband (Blitz)
        public static bool[] armorEnhanced = new bool[] { false, false, false, false };

        //loads the level
        public void loadLevel(string levelName)
        {
            //ryn = new Enemy("ryn", null, 5, 100, 60, new Vector2(300, 0), 300, 20);

            Point startPosition;
            Player.Nyr.inHub = false;

            nscObjects = new List<NSC>();

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
                    nscObjects = JsonConvert.DeserializeObject<List<NSC>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_nscObjects.json"));
                    //delete souls in Hub, if not rescued yet
                    for (int i = 0; i < nscObjects.Count; i++)
                    {
                        if (nscObjects[i].name == "inaSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.FIRE])
                            {
                                nscObjects.RemoveAt(i);
                            }
                            else if (Level.armorEnhanced[(int)BossElements.FIRE])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "yinyinSoul")
                        {
                            if (!Level.soulsRescued[(int) BossElements.ICE])
                            {
                                nscObjects.RemoveAt(i);
                            }
                            else if (Level.armorEnhanced[(int)BossElements.ICE])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "aiyeSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.EARTH])
                            {
                                nscObjects.RemoveAt(i);
                            }
                            else if (Level.armorEnhanced[(int)BossElements.EARTH])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "monomonoSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.BOLT])
                            {
                                nscObjects.RemoveAt(i);
                            }
                            else if (Level.armorEnhanced[(int)BossElements.BOLT])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }  
                    }
                    Player.Nyr.inJump = false;
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
                    startPosition = new Point(-13000, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                    //TODO: evtl lötschn
                case "ErdLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(-500 + Game1.WindowSize.X, -(height - Game1.WindowSize.Y));
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
            
            //entityObjects = JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_entityObjects.json"));
            enemyObjects = JsonConvert.DeserializeObject<List<Enemy>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_enemyObjects.json"));
            projectileObjects = new List<Projectile>();

            foreach (Enemy element in enemyObjects)
            {
                gameObjects.Add(element);
                element.Initialize();
                //element.hurtBox.Location += startPosition;
                //element.attackBox.Location += startPosition;
            }
            foreach (NSC element in nscObjects)
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
            

            Camera.Main.levelBounds = new Rectangle(Vector2.Zero.ToPoint(), new Point(width, height));

            Camera.Main.position = startPosition.ToVector2() * -1;

            States.CurrentPlayerState = Playerstates.IDLE;


            //sound test
            //attack = Game1.Ressources.Load<SoundEffect>("sfx/sfx_collide");
            //thud = Game1.Ressources.Load<SoundEffect>("sfx/sfx_thud");

            //soundeffects: attack, collect, hurt, jump, warning
            attack = Game1.Ressources.Load<SoundEffect>("sfx/sfx_attack");
            collect = Game1.Ressources.Load<SoundEffect>("sfx/sfx_collect");
            hurt = Game1.Ressources.Load<SoundEffect>("sfx/sfx_hurt");
            jump = Game1.Ressources.Load<SoundEffect>("sfx/sfx_jump");
            //warning = Game1.Ressources.Load<SoundEffect>("sfx/sfx_warning");

            Player.Nyr.currentEntityState = (int)Playerstates.IDLE;
            Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
            Player.Nyr.currentFrame = 0;


            States.CurrentPlayerState = Playerstates.IDLE;


            //sound test
            //jump = Game1.Ressources.Load<SoundEffect>("sfx/sfx_jump");
            //attack = Game1.Ressources.Load<SoundEffect>("sfx/sfx_collide");
            //thud = Game1.Ressources.Load<SoundEffect>("sfx/sfx_thud");
            
        }

        //get input and update the elements inside the level
        public void update(GameTime gameTime)
        {   
            //Resetting Values
            Vector2 moveValue = Vector2.Zero;
            textboxText = "";

            // Player.Nyr.Update(gameTime);

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
                    Player.Nyr.currentEntityState = (int)Playerstates.LAND;
                    Player.Nyr.currentFrame = 0;
                    Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                }
            }

            //Let all movingPlatforms move and if Nyr stands on it, then move her too
            foreach (GameObject element in gameObjects)
            {
                if (element.moving != Vector2.Zero)
                {
                    if (Player.Nyr.Collision<GameObject>(new GameObject[] { element }, Player.Nyr.position + moveValue).Length > 0)
                    {
                        moveValue += element.move(gameTime);
                    }
                    else
                    {
                        element.move(gameTime);
                    }
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
                        if (!Player.Nyr.isCrouching)
                        {
                            moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Player.Nyr.entityFacing = -1;
                            if (Player.Nyr.onIce)
                            {
                                Player.Nyr.slide = 1000;
                            }
                            if (Player.Nyr.currentEntityState == (int)Playerstates.IDLE || Player.Nyr.currentEntityState == (int)Playerstates.STOP || Player.Nyr.currentEntityState == (int)Playerstates.DANCE)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                        break;
                    case Keys.D:
                        if (!Player.Nyr.isCrouching)
                        {
                            moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Player.Nyr.entityFacing = 1;
                            if (Player.Nyr.onIce)
                            {
                                Player.Nyr.slide = 1000;
                            }
                            if (Player.Nyr.currentEntityState == (int)Playerstates.IDLE || Player.Nyr.currentEntityState == (int)Playerstates.STOP || Player.Nyr.currentEntityState == (int)Playerstates.DANCE)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                        break;
                    case Keys.Space:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            if (Player.Nyr.onGround && !Player.Nyr.inJump && !Player.Nyr.inHub)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.JUMP;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.FALL;
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
                            if (Player.Nyr.currentEntityState != (int) Playerstates.WALK)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                        break;
                    case Keys.S:
                        if (Player.Nyr.inHub)
                        {
                            moveValue += new Vector2(0, 1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            if (Player.Nyr.currentEntityState != (int)Playerstates.WALK)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                        else if (Player.Nyr.onGround)
                        {
                            Player.Nyr.isCrouching = true;
                            if (Player.Nyr.currentEntityState != (int)Playerstates.CROUCH)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.CROUCH;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.CROUCH;
                            }

                            //TODO: set collider to crouch position
                        }
                        else if (Player.Nyr.currentEntityState == (int)Playerstates.FALL && Level.armorEnhanced[(int) BossElements.EARTH])
                        {
                            //TODO: let Nyr fall
                            //Player.Nyr.currentEntityState = (int)Playerstates.STOMP;
                            //Player.Nyr.currentFrame = 0;
                            //Player.Nyr.nextEntityState = (int)Playerstates.STOMP;

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
                                Player.Nyr.currentEntityState = (int)Playerstates.FIGHT;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                                Player.Nyr.fAttackCheck = 20;
                                atkCooldown = 60;

                                jump.CreateInstance().Play();
                            }
                        }break;
                    case Keys.LeftControl:
                        if (Level.armorEnhanced[(int)BossElements.BOLT] && hasDashed == false)
                        {
                            
                            Player.Nyr.MakeInvulnerable();
                            dashtimer = 30;
                            tempposition = Player.Nyr.position;
                            hasDashed = true;

                            attack.CreateInstance().Play();
                        }
                        break;
                    

                }
            }
            //TODO: Höhe beibehalten 
            if (dashtimer >= 0)
            {
                if (dashtimer >= 20)
                {
                    if (Player.Nyr.entityFacing == -1)
                    {
                        moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * 4, 0);
                    }
                    if (Player.Nyr.entityFacing == 1)
                    {
                        moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * 4, 0);
                    }
                }
                if (dashtimer <= 1)
                {
                    hasDashed = false;
                }
                dashtimer--;
            }
            if(Player.Nyr.slide > 0 && Player.Nyr.onIce)
            {
                moveValue.X += Player.Nyr.slideValue(gameTime) * Player.Nyr.entityFacing;
            }
            

            //let em move, after all collisions have manipulated the movement
            Vector2 newMoveValue = checkCollision(moveValue);

            if (newMoveValue != Vector2.Zero)
            {
                if (Player.Nyr.currentEntityState == (int)Playerstates.FALL && Player.Nyr.onGround)
                {
                    Player.Nyr.currentEntityState = (int)Playerstates.LAND;
                    Player.Nyr.currentFrame = 0;
                    Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                }

                Camera.Main.move(newMoveValue);
            }


            //Let all other gameObjects fall to gravitation
            useGrav(gameTime);


            Player.Nyr.onIce = false;
            //trigger all triggers, that have been triggered
            Player.Nyr.activateTrigger(gameTime);

            Player.Nyr.EntityUpdate(gameTime);

            foreach (Enemy element in enemyObjects)
            {
                element.Update(gameTime);
            }
        

            if (!anyKeyPressed || (Player.Nyr.currentEntityState == (int) Playerstates.CROUCH && Keyboard.GetState().IsKeyUp(Keys.S)))
            {
                if (Player.Nyr.isCrouching)
                {
                    Player.Nyr.currentEntityState = (int)Playerstates.IDLE;
                    Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                    Player.Nyr.currentFrame = 0;
                    Player.Nyr.isCrouching = false;
                }

                Player.Nyr.inactivityTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (!(Player.Nyr.currentEntityState == (int)Playerstates.JUMP || Player.Nyr.currentEntityState == (int)Playerstates.FALL || Player.Nyr.currentEntityState == (int)Playerstates.LAND))
                {
                    if (Player.Nyr.currentEntityState == (int)Playerstates.WALK && Player.Nyr.onGround)
                    {
                        Player.Nyr.currentEntityState = (int)Playerstates.STOP;
                        Player.Nyr.currentFrame = 0;
                    }
                    Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                }
                if(Player.Nyr.inactivityTime > 30f && Player.Nyr.currentEntityState != (int)Playerstates.DANCE)
                {
                    Player.Nyr.currentEntityState = (int)Playerstates.DANCE;
                    Player.Nyr.nextEntityState = (int)Playerstates.DANCE;
                    Player.Nyr.currentFrame = 0;
                }
            }
            else
            {
                Player.Nyr.inactivityTime = 0;
            }

            lastPressedKeys = newPressedKeys;

            for (int i = 0; i < projectileObjects.Count; i++)
            {
                projectileObjects[i].Update(gameTime);
            }

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

                if (element.position.X + element.width > newPos.X && element.position.X + element.width < Player.Nyr.position.X && !(element.name == "platform" || element.name == "cloud"))
                {
                    collidedLeft = true;
                }
                else if (element.position.X < newPos.X + Player.Nyr.width && element.position.X > Player.Nyr.position.X + Player.Nyr.width && !(element.name == "platform" || element.name == "cloud"))
                {
                    collidedRight = true;
                }
                else if (element.position.Y + element.height >= newPos.Y && element.position.Y + element.height <= Player.Nyr.position.Y && !(element.name == "platform" || element.name == "cloud"))
                {
                    collidedTop = true;
                }
                else if (element.position.Y <= newPos.Y + Player.Nyr.height && element.position.Y > newPos.Y)
                {
                    collidedBottom = true;
                }
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
                
                gameObject.startPosition -= moveValue;
            }
            foreach (Enemy element in enemyObjects)
            {
                element.tempPosition -= moveValue;
            }
            foreach (Projectile element in projectileObjects)
            {
                element.position -= moveValue;
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

        //Saves the Game
        public void SaveGame()
        {
            StreamWriter output = File.CreateText("SaveGame.txt");
            
            for(int i = 0; i < 4; i++)
            {
                output.WriteLine((Level.soulsRescued[i] ? "T" : "F"));
                output.WriteLine((Level.armorEnhanced[i] ? "T" : "F"));
                output.WriteLine("");
            }
            output.WriteLine(Player.Nyr.money.ToString());

            output.Close();
        }

        //the typical render method
        public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(levelBGSprite, new Rectangle(positionBGSprite.ToPoint(), new Point(width, height)), Color.White);

            foreach (GameObject element in gameObjects)
            {
                element.Draw(gameTime, spriteBatch);
            }
            //Draw all GameObjects such as Enemys
            foreach (Enemy element in enemyObjects)
            {
                element.EntityRender(gameTime, spriteBatch);
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.position.X, element.position.Y - 100), Color.Black);
            }
            Player.Nyr.EntityRender(gameTime, spriteBatch);

            for (int i = 0; i < projectileObjects.Count; i++)
            {
                projectileObjects[i].Draw(gameTime, spriteBatch);
            }

            if (textboxText.Length > 0)
            {
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(100, 50), Color.Black);
            }
        }
    }
}