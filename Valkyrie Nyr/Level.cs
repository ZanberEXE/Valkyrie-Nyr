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
        public string name;
        public string textboxText = "";
        //public Enemy ryn;

        public List<GameObject> gameObjects;
        //public List<Entity> entityObjects;
        public List<Enemy> enemyObjects;
        public List<NSC> nscObjects;
        public List<Projectile> projectileObjects;

        public int height;
        public int width;

        int atkCooldown;

        int dashtimer;
        int fireAoeTimer;

        bool hasDashed = false;
        bool drawMap = false;
        Vector2 tempposition;

        public Vector2 positionBGSprite;

        private static Level currentLevel;

        Texture2D levelBGSprite;
        Texture2D map;

        Projectile tempEffekt;

        Keys[] lastPressedKeys;

        //get current Level from everywhere
        public static Level Current { get { if (currentLevel == null) { currentLevel = new Level(); } return currentLevel; } }

        //
        //all beaten bosses in this order: Ina (Fire), Yinyin (Ice), Aiye(Earth), Monomono (Blitz)
        public static bool[] soulsRescued = new bool[] { false, false, false, false };
        //all enhanced Armor in this order: Torso (Fire), Guntlet (Ice), Shoes(Earth), Headband (Blitz)
        public static bool[] armorEnhanced = new bool[] { false, false, false, false };

        //loads the level
        public void loadLevel(string levelName)
        {
            //ryn = new Enemy("ryn", null, 5, 100, 60, new Vector2(300, 0), 300, 20);
            map = Game1.Ressources.Load<Texture2D>("Map");
            Interface.Start();
            name = levelName;

            Point startPosition;
            Player.Nyr.inHub = false;

            nscObjects = JsonConvert.DeserializeObject<List<NSC>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_nscObjects.json"));


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
                    //delete souls in Hub, if not rescued yet
                    for (int i = 0; i < nscObjects.Count; i++)
                    {
                        if (nscObjects[i].name == "inaSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.FIRE])
                            {
                                nscObjects.RemoveAt(i);
                                i--;
                            }
                            else if (Level.armorEnhanced[(int)BossElements.FIRE])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "yinyinSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.ICE])
                            {
                                nscObjects.RemoveAt(i);
                                i--;
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
                                i--;
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
                                i--;
                            }
                            else if (Level.armorEnhanced[(int)BossElements.BOLT])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        } 
                    }
                    Player.Nyr.inJump = false;
                    Player.Nyr.health = Player.Nyr.maxHealth;
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
                    //startPosition = new Point(-13000, -(height - Game1.WindowSize.Y));
                    startPosition = new Point(-500, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                    //TODO: evtl lötschn
                /*case "FeuerLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(-14000 + Game1.WindowSize.X, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;*/
                case "ErdLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                case "EisLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
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

            switch (levelName)
            {
                case "ErdLevel":
                    new Projectile("Earthspike", 200, 200, new Vector2(1500, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(2500, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(4000, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6800, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6600, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(5050, 3050 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9500, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10000, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11200, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10600, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    break;
                case "FeuerLevel":
                    new Projectile("Earthspike", 200, 200, new Vector2(1500, 4650 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(2500, 4650 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(2121, 4650 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(2720, 4650 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6600, 4670 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(7700, 4670 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(8800, 4670 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9900, 4670 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    break;
            }

            for ( int i = 0; i < enemyObjects.Count; i++) 
            {
                Enemy element = enemyObjects[i];
                gameObjects.Add(element);
                element.Initialize();

                //remove boss, if you already rescued it
                switch(levelName)
                {
                    case "EisLevel":
                        if (element.name == "Yinyin" && Level.soulsRescued[(int)BossElements.ICE])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                    case "FeuerLevel":
                        if (element.name == "Ina" && Level.soulsRescued[(int)BossElements.FIRE])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                    case "ErdLevel":
                        if (element.name == "Aiye" && Level.soulsRescued[(int)BossElements.EARTH])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                    case "BlitzLevel":
                        if (element.name == "Monomono" && Level.soulsRescued[(int)BossElements.BOLT])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                }
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

            Player.Nyr.currentEntityState = (int)Playerstates.IDLE;
            Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
            Player.Nyr.currentFrame = 0;

            lastPressedKeys = Keyboard.GetState().GetPressedKeys();

            Antagonist.Ryn.Reset();
            
        }

        private void UpdateTraps(GameTime gameTime)
        {
            for (int i = 0; i < projectileObjects.Count; i++)
            {
                switch (projectileObjects[i].name)
                {
                    case "Earthspike":
                        if (projectileObjects[i].currentFrame > 90 && projectileObjects[i].currentFrame <= 95)
                        {
                            projectileObjects[i].attackBoxOffset.Y = 150 - (projectileObjects[i].currentFrame - 90) * (150f / 5f) - 50;
                            projectileObjects[i].attackbox.Height = (int)((projectileObjects[i].currentFrame - 90) * (150 / 5f));
                        }
                        else if (projectileObjects[i].currentFrame > 170 && projectileObjects[i].currentFrame <= 187)
                        {
                            projectileObjects[i].attackBoxOffset.Y = (projectileObjects[i].currentFrame - 170) * (150 / 17f) - 40;
                            projectileObjects[i].attackbox.Height = (int)((187 - projectileObjects[i].currentFrame) * (150 / 17f));
                        }
                        else if (projectileObjects[i].currentFrame == 0)
                        {
                            projectileObjects[i].attackbox.Height = 0;
                        }
                        break;
                }
            }
        }

        //get input and update the elements inside the level
        public void update(GameTime gameTime)
        {
            for (int i = 0; i < projectileObjects.Count; i++)
            {
                projectileObjects[i].Update(gameTime);
            }

            UpdateTraps(gameTime);

            //Resetting Values
            Vector2 moveValue = Vector2.Zero;
            textboxText = "";


            //Let PLayer fall and save the moveValue in overall Movement
            if (!Player.Nyr.inHub)
            {
                if (Player.Nyr.inStomp)
                {
                    moveValue += Player.Nyr.Fall(gameTime, gameObjects.ToArray()) - Player.Nyr.position;
                    moveValue += Player.Nyr.Fall(gameTime, gameObjects.ToArray()) - Player.Nyr.position;
                }
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
                                SFX.CurrentSFX.loadSFX("sfx/sfx_jump");
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
                        else if (Player.Nyr.currentEntityState == (int)Playerstates.FALL && Level.armorEnhanced[(int) BossElements.EARTH] && Player.Nyr.mana >= 40)
                        {
                            Player.Nyr.mana -= 40;
                            Player.Nyr.inStomp = true;
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
                    case Keys.M:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            drawMap = !drawMap;
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
                                SFX.CurrentSFX.loadSFX("sfx/sfx_attack");

                            }
                        }
                        break;
                    case Keys.E:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            if (atkCooldown == 0 && !Player.Nyr.inHub && Level.armorEnhanced[(int)BossElements.ICE] && Player.Nyr.mana >= 30)
                            {
                                Player.Nyr.mana -= 30;
                                atkCooldown = 60;
                                if (Player.Nyr.entityFacing == -1)
                                {
                                    new Projectile("IceShot", 30, 10, Player.Nyr.position - new Vector2(35, -50), new Vector2(-1, 0), 2400, false, new Rectangle(-10, -10, 25, 10), false, 0, 0, 0);
                                }
                                else
                                {
                                    new Projectile("IceShot", 30, 10, Player.Nyr.position + new Vector2(Player.Nyr.width, 40), new Vector2(1, 0), 2400, false, new Rectangle(-10, 0, 25, 10), false, 0, 0, 0);
                                }
                                SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                            }
                        }
                        break;
                    case Keys.Q:
                        if (!newPressedKeys.SequenceEqual(lastPressedKeys))
                        {
                            if (atkCooldown == 0 && !Player.Nyr.inHub && Level.armorEnhanced[(int)BossElements.FIRE] && Player.Nyr.mana >= 80)
                            {
                                Player.Nyr.mana -= 80;
                                atkCooldown = 60;
                                Player.Nyr.CastFireAOE();
                                SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                                fireAoeTimer = 40;
                                Player.Nyr.MakeInvulnerable(40);
                            }
                        }
                        break;
                    case Keys.LeftControl:
                        if (Level.armorEnhanced[(int)BossElements.BOLT] && hasDashed == false && Player.Nyr.mana >= 30)
                        {
                            Player.Nyr.mana -= 30;
                            Player.Nyr.MakeInvulnerable();
                            dashtimer = 30;
                            tempposition = Player.Nyr.position;
                            hasDashed = true;
                            SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                        }
                        break;
                    

                }
            }
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
            if (fireAoeTimer >= 0)
            {
                if (fireAoeTimer == 39)
                {
                    tempEffekt = new Projectile("NyrFireAoe", 300, 300, new Vector2(Player.Nyr.position.X + Player.Nyr.width / 2, Player.Nyr.position.Y + Player.Nyr.height / 4), new Vector2(0, 0), 800, true, new Rectangle(-150, -90, 300, 200), true, 25, 10, Player.Nyr.damage * 2);
                }
                if (fireAoeTimer <= 49 && fireAoeTimer >= 2 && !Player.Nyr.inFireAoe)
                {
                    for (int i = 0; i < Level.Current.enemyObjects.Count; i++)
                    {
                        
                        Rectangle hurtbox = Level.Current.enemyObjects[i].hurtBox;
                        if (tempEffekt != null)
                        {
                            if (Player.Nyr.CollisionAABB(hurtbox, tempEffekt.attackbox))
                            {
                                Level.Current.enemyObjects[i].enemyHit = true;
                                Level.Current.enemyObjects[i].hitTimer = 20;
                                Player.Nyr.inFireAoe = true;
                                Level.currentLevel.enemyObjects[i].health -= tempEffekt.damage;
                                if (Level.currentLevel.enemyObjects[i].health <= 0)
                                {
                                    Level.Current.enemyObjects[i].SpawnLoot();
                                    Level.Current.enemyObjects.RemoveAt(i);
                                    i--;
                                    

                                }
                            }
                            
                        }
                        

                    }

                }
                if (fireAoeTimer == 2)
                {
                    Player.Nyr.inFireAoe = false;
                }
                if (fireAoeTimer <= 1 && tempEffekt != null)
                {
                    tempEffekt.Destroy();
                    tempEffekt = null;
                }
                fireAoeTimer--;
            }
            if (Player.Nyr.slide > 0 && Player.Nyr.onIce)
            {
                moveValue.X += Player.Nyr.slideValue(gameTime) * Player.Nyr.entityFacing;
            }

            //Let all movingPlatforms move and if Nyr stands on it, then move her too
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject element = gameObjects[i];
            
                if (element.moving != Vector2.Zero)
                {
                    if (Player.Nyr.Collision<GameObject>(new GameObject[] { element }, Player.Nyr.position + moveValue).Length > 0)
                    {
                        if (element.name == "AiyeWall")
                        {
                            float plattFormMovement = element.move(gameTime).X;
                            if ((plattFormMovement < 0 && moveValue.X > 0) || (plattFormMovement > 0 && moveValue.X < 0))
                            {
                                moveValue.X = plattFormMovement;
                            }
                            else
                            {
                                moveValue.X += plattFormMovement;
                            }
                            
                        }
                        else
                        {
                            moveValue += element.move(gameTime);
                        }
                       
                    }
                    else
                    {
                        element.move(gameTime);
                    }

                }
            }
       

            Antagonist.Ryn.Update(gameTime);

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

            for(int i = 0; i < collidedObjects.Length; i++)
            {
                GameObject element = collidedObjects[i];
                if (element.triggerType != null)
                {
                    continue;
                }
               
                if (element.position.X + element.width > newPos.X && element.position.X + element.width < Player.Nyr.position.X && !(element.name == "platform" || element.name == "cloud" || element.name == "cloud2"))
                {
                    collidedLeft = true;
                }
                else if (element.position.X < newPos.X + Player.Nyr.width && element.position.X > Player.Nyr.position.X + Player.Nyr.width && !(element.name == "platform" || element.name == "cloud" || element.name == "cloud2"))
                {
                    collidedRight = true;
                }
                else if (element.position.Y + element.height >= newPos.Y && element.position.Y + element.height <= Player.Nyr.position.Y && !(element.name == "platform" || element.name == "cloud" || element.name == "cloud2"))
                {
                    collidedTop = true;
                }
                else if (element.position.Y <= newPos.Y + Player.Nyr.height && element.position.Y > newPos.Y)
                {
                    if (Player.Nyr.inStomp && element.name == "breakableGround")
                    {
                        gameObjects.Remove(element);
                        continue;
                    }
                    collidedBottom = true;
                }
                Player.Nyr.inStomp = false;
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
            Antagonist.Ryn.position -= moveValue;
            positionBGSprite -= moveValue;
            foreach (GameObject gameObject in gameObjects)
            {
                if(gameObject.name == "Nyr" || gameObject.name == "Earthspike")
                {
                    continue;
                }

                gameObject.position -= moveValue;
                
                gameObject.startPosition -= moveValue;
            }
            foreach (Enemy element in enemyObjects)
            {
                element.heightReset -= moveValue.Y;
                //element.tempPosition -= moveValue;
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
            output.WriteLine("");
            output.WriteLine(Player.Nyr.maxHealth.ToString());

            output.Close();
        }

        private void DrawMap(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(map, new Rectangle(0, 0, map.Width, map.Height), Color.White * 0.8f);
            Texture2D nyrLocationIcon = Game1.Ressources.Load<Texture2D>("NyrMapIcon");
            switch (name)
            {
                case "Hub":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(310, 415, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "Overworld":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(820, 410, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "Bossstage":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(1335, 400, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "EisLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(550, 140, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "BlitzLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(1075, 130, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "FeuerLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(565, 690, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "ErdLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(1100, 680, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
            }
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


                if (!element.enemyHit)
                {
                    
                    element.EntityRender(gameTime, spriteBatch);
                }
                if (element.hitTimer < 0 && element.hitTimer >= 20)
                {
                    if (element.enemyHit && element.hitTimer % 2 == 0)
                    {
                        element.EntityRender(gameTime, spriteBatch);
                    }
                }
                if (element.hitTimer == 0)
                {
                    element.enemyHit = false;
                }
                element.hitTimer--;
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.hurtBox.Location.X, element.hurtBox.Location.Y - 100), Color.Black);
            }
            
            Player.Nyr.EntityRender(gameTime, spriteBatch);
            Antagonist.Ryn.EntityRender(gameTime, spriteBatch);

            for (int i = 0; i < projectileObjects.Count; i++)
            {
                projectileObjects[i].Draw(gameTime, spriteBatch);
                //spriteBatch.Draw(Game1.pxl, new Rectangle((int)projectileObjects[i].position.X, (int)projectileObjects[i].position.Y, projectileObjects[i].width, projectileObjects[i].height), Color.LightGreen * 0.5f);
                //spriteBatch.Draw(Game1.pxl, new Rectangle(projectileObjects[i].attackbox.X, projectileObjects[i].attackbox.Y, projectileObjects[i].attackbox.Width, projectileObjects[i].attackbox.Height), Color.BlueViolet * 0.5f);
            }

            if (textboxText.Length > 0)
            {
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(100, 200), Color.Black);

            }
            Interface.Draw(spriteBatch);
            if (drawMap)
            {
                DrawMap(spriteBatch);
            }
        }
    }
}