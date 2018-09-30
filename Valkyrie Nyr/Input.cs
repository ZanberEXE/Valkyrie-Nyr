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
    class Input
    {
        //Singleton
        private static Input instance;

        public static Input Handeler
        {
            get
            {
                if (instance == null)
                {
                    instance = new Input();
                }
                return instance;
            }
        }

        bool controllerUsed = false;

        Buttons[] allGamepadButtons = new Buttons[] { Buttons.A, Buttons.B, Buttons.Back, Buttons.BigButton, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp, Buttons.LeftShoulder, Buttons.LeftStick, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftTrigger, Buttons.RightShoulder, Buttons.RightStick, Buttons.RightThumbstickDown, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp, Buttons.RightTrigger, Buttons.Start, Buttons.X, Buttons.Y };
        bool[] lastPressedGamePadButtons;
        bool[] currentPressedGamePadButtons;
        bool anyInput = false;
        Keys[] currentInput;
        Keys[] lastInput;

        private Input()
        {

        }

        public void Update(GameTime gameTime)
        {
            //reset VAriables
            currentPressedGamePadButtons = new bool[allGamepadButtons.Length];

            //Let PLayer fall and save the moveValue in overall Movement
            if (!Player.Nyr.inHub)
            {
                if (Player.Nyr.inStomp)
                {
                    Level.Current.moveValue += Player.Nyr.Fall(gameTime, Level.Current.gameObjects.ToArray()) - Player.Nyr.position;
                    Level.Current.moveValue += Player.Nyr.Fall(gameTime, Level.Current.gameObjects.ToArray()) - Player.Nyr.position;
                }
                Level.Current.moveValue += Player.Nyr.Fall(gameTime, Level.Current.gameObjects.ToArray()) - Player.Nyr.position;
            }

            if (Player.Nyr.inJump)
            {
                if (!Player.Nyr.onGround)
                {
                    Level.Current.moveValue.Y -= Player.Nyr.jumpHeight;
                }
                else
                {
                    Player.Nyr.inJump = false;
                    //Player.Nyr.currentEntityState = (int)Playerstates.LAND;
                    //Player.Nyr.currentFrame = 0;
                    //Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                }
            }
            if (Player.Nyr.attackCooldown <= 29 && Player.Nyr.attackCooldown >= 15)
            {
                if (Player.Nyr.entityFacing == -1)
                {
                    Camera.Main.move(new Vector2(-2, 0));
                    
                }
                else
                {
                    Camera.Main.move(new Vector2(2, 0));
                }
            }
            if (Player.Nyr.attackCooldown > 0)
            {
                Player.Nyr.attackCooldown--;
            }
            if (Player.Nyr.skillCooldown > 0)
            {
                Player.Nyr.skillCooldown--;
            }


            if (GamePad.GetCapabilities(0).IsConnected)
            {
                SetDevice();
            }

            if (controllerUsed)
            {
                ControllerUpdate(gameTime);
            }
            else
            {
                KeyboardUpdate(gameTime);
            }

            if (Level.Current.dashtimer >= 0)
            {
                if (Level.Current.dashtimer >= 20)
                {
                    if (Player.Nyr.entityFacing == -1)
                    {
                        Level.Current.moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * 4, 0);
                    }
                    if (Player.Nyr.entityFacing == 1)
                    {
                        Level.Current.moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * 4, 0);
                    }
                }
                if (Level.Current.dashtimer <= 1)
                {
                    Level.Current.hasDashed = false;
                }
                Level.Current.dashtimer--;
            }
            if (Level.Current.fireAoeTimer >= 0)
            {
                if (Level.Current.fireAoeTimer == 39)
                {
                    Level.Current.tempEffekt = new Projectile("NyrFireAoe", 300, 300, new Vector2(Player.Nyr.position.X + Player.Nyr.width / 2, Player.Nyr.position.Y + Player.Nyr.height / 4), new Vector2(0, 0), 800, true, new Rectangle(-150, -90, 300, 200), true, 25, 10, Player.Nyr.damage * 2);
                }
                if (Level.Current.fireAoeTimer <= 49 && Level.Current.fireAoeTimer >= 2 && !Player.Nyr.inFireAoe)
                {
                    for (int i = 0; i < Level.Current.enemyObjects.Count; i++)
                    {

                        Rectangle hurtbox = Level.Current.enemyObjects[i].hurtBox;
                        if (Level.Current.tempEffekt != null)
                        {
                            if (Player.Nyr.CollisionAABB(hurtbox, Level.Current.tempEffekt.attackbox))
                            {
                                Level.Current.enemyObjects[i].enemyHit = true;
                                Level.Current.enemyObjects[i].hitTimer = 20;
                                Player.Nyr.inFireAoe = true;
                                Level.Current.enemyObjects[i].health -= Level.Current.tempEffekt.damage;
                                if (Level.Current.enemyObjects[i].health <= 0)
                                {
                                    Level.Current.enemyObjects[i].SpawnLoot();
                                    Level.Current.enemyObjects.RemoveAt(i);
                                    i--;


                                }
                            }
                        }
                    }
                }
                if (Level.Current.fireAoeTimer == 2)
                {
                    Player.Nyr.inFireAoe = false;
                }
                if (Level.Current.fireAoeTimer <= 1 && Level.Current.tempEffekt != null)
                {
                    Level.Current.tempEffekt.Destroy();
                    Level.Current.tempEffekt = null;
                }
                Level.Current.fireAoeTimer--;
            }
            if (Player.Nyr.slide > 0 && Player.Nyr.onIce && Player.Nyr.currentEntityState != (int) Playerstates.WALK)
            {
                Level.Current.moveValue.X += Player.Nyr.slideValue(gameTime) * Player.Nyr.entityFacing;
            }

            lastPressedGamePadButtons = currentPressedGamePadButtons;
        }

        private void KeyboardUpdate(GameTime gameTime)
        {
            //get Input from Keyboard

            Level.Current.newPressedKeys = Keyboard.GetState().GetPressedKeys();

            foreach (Keys element in Level.Current.newPressedKeys)
            {
                Level.Current.anyKeyPressed = true;

                switch (element)
                {
                    case Keys.A:
                        if (!Player.Nyr.isCrouching)
                        {
                            Level.Current.moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Player.Nyr.entityFacing = -1;
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
                            Level.Current.moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Player.Nyr.entityFacing = 1;
                            if (Player.Nyr.currentEntityState == (int)Playerstates.IDLE || Player.Nyr.currentEntityState == (int)Playerstates.STOP || Player.Nyr.currentEntityState == (int)Playerstates.DANCE)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                        break;
                    case Keys.Space:
                        if (!Level.Current.newPressedKeys.SequenceEqual(Level.Current.lastPressedKeys))
                        {
                            if (Player.Nyr.onGround && !Player.Nyr.inJump && !Player.Nyr.inHub && Player.Nyr.currentEntityState != (int)Playerstates.SLIP)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.JUMP;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.FALL;
                                Player.Nyr.inJump = true;
                                Player.Nyr.onGround = false;
                                Level.Current.moveValue.Y -= Player.Nyr.jumpHeight;
                                SFX.CurrentSFX.loadSFX("sfx/sfx_jump");
                            }
                        }
                        break;
                    case Keys.W:
                        if (Player.Nyr.inHub)
                        {
                            Level.Current.moveValue += new Vector2(0, -1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            if (Player.Nyr.currentEntityState != (int)Playerstates.WALK)
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
                            Level.Current.moveValue += new Vector2(0, 1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
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
                        }
                        else if (!Player.Nyr.inStomp && Player.Nyr.currentEntityState == (int)Playerstates.FALL && Level.armorEnhanced[(int)BossElements.EARTH] && Player.Nyr.mana >= 40)
                        {
                            Player.Nyr.mana -= 40;
                            Player.Nyr.inStomp = true;
                            //Player.Nyr.currentEntityState = (int)Playerstates.STOMP;
                            //Player.Nyr.currentFrame = 0;
                            //Player.Nyr.nextEntityState = (int)Playerstates.STOMP;

                        }
                        break;
                    case Keys.F:
                        if (!Level.Current.newPressedKeys.SequenceEqual(Level.Current.lastPressedKeys))
                        {
                            Player.Nyr.interact = true;
                        }
                        break;
                    case Keys.M:
                        if (!Level.Current.newPressedKeys.SequenceEqual(Level.Current.lastPressedKeys))
                        {
                            GameUI.Handeler.ShowMap = !GameUI.Handeler.ShowMap;
                        }
                        break;
                    case Keys.LeftShift:
                        if (!Level.Current.newPressedKeys.SequenceEqual(Level.Current.lastPressedKeys))
                        {
                            if (Player.Nyr.attackCooldown == 0 && !Player.Nyr.inHub)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.FIGHT;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                                Player.Nyr.fAttackCheck = 20;
                                Player.Nyr.attackCooldown = 30;
                                SFX.CurrentSFX.loadSFX("sfx/sfx_attack");

                            }
                        }
                        break;
                    case Keys.E:
                        if (!Level.Current.newPressedKeys.SequenceEqual(Level.Current.lastPressedKeys))
                        {
                            if (Player.Nyr.skillCooldown == 0 && !Player.Nyr.inHub && Level.armorEnhanced[(int)BossElements.ICE] && Player.Nyr.mana >= 30)
                            {
                                Player.Nyr.mana -= 30;
                                Player.Nyr.skillCooldown = 60;
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
                        if (!Level.Current.newPressedKeys.SequenceEqual(Level.Current.lastPressedKeys))
                        {
                            if (Player.Nyr.skillCooldown == 0 && !Player.Nyr.inHub && Level.armorEnhanced[(int)BossElements.FIRE] && Player.Nyr.mana >= 80)
                            {
                                Player.Nyr.mana -= 80;
                                Player.Nyr.skillCooldown = 60;
                                Player.Nyr.CastFireAOE();
                                SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                                Level.Current.fireAoeTimer = 40;
                                Player.Nyr.MakeInvulnerable(40);
                            }
                        }
                        break;
                    case Keys.LeftControl:
                        if (Level.armorEnhanced[(int)BossElements.BOLT] && Level.Current.hasDashed == false && Player.Nyr.mana >= 30)
                        {
                            Player.Nyr.currentEntityState = (int)Playerstates.EVASION;
                            Player.Nyr.currentFrame = 0;
                            Player.Nyr.mana -= 30;
                            Player.Nyr.MakeInvulnerable();
                            Level.Current.dashtimer = 30;
                            Level.Current.tempposition = Player.Nyr.position;
                            Level.Current.hasDashed = true;
                            SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                        }
                        break;
                }
            }
        }

        private void ControllerUpdate(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(0);
            {
                //AnalogStockMovement
                if (state.ThumbSticks.Left != Vector2.Zero)
                {
                    if (state.ThumbSticks.Left.X > 0.2f || state.ThumbSticks.Left.X < -0.2f)
                    {
                        Level.Current.anyKeyPressed = true;
                        if (!Player.Nyr.isCrouching)
                        {
                            Level.Current.moveValue += new Vector2(state.ThumbSticks.Left.X * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Player.Nyr.entityFacing = (int)state.ThumbSticks.Left.X;
                            if (Player.Nyr.currentEntityState == (int)Playerstates.IDLE || Player.Nyr.currentEntityState == (int)Playerstates.STOP || Player.Nyr.currentEntityState == (int)Playerstates.DANCE)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                    }
                    if (Player.Nyr.inHub && (state.ThumbSticks.Left.Y > 0.2f || state.ThumbSticks.Left.Y < -0.2f))
                    {
                        Level.Current.anyKeyPressed = true;
                        Level.Current.moveValue += new Vector2(0, -state.ThumbSticks.Left.Y * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        if (Player.Nyr.currentEntityState != (int)Playerstates.WALK)
                        {
                            Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                            Player.Nyr.currentFrame = 0;
                            Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                        }
                    }
                    if (state.ThumbSticks.Left.Y == -1)
                    {
                        Level.Current.anyKeyPressed = true;
                        if (Player.Nyr.onGround)
                        {
                            Player.Nyr.isCrouching = true;
                            if (Player.Nyr.currentEntityState != (int)Playerstates.CROUCH)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.CROUCH;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.CROUCH;
                            }
                        }
                        else if (Player.Nyr.currentEntityState == (int)Playerstates.FALL && Level.armorEnhanced[(int)BossElements.EARTH] && Player.Nyr.mana >= 40)
                        {
                            Player.Nyr.mana -= 40;
                            Player.Nyr.inStomp = true;
                            //Player.Nyr.currentEntityState = (int)Playerstates.STOMP;
                            //Player.Nyr.currentFrame = 0;
                            //Player.Nyr.nextEntityState = (int)Playerstates.STOMP;
                        }
                    }
                }
                //DigiPadMovement
                else
                {
                    if (state.IsButtonDown(Buttons.DPadRight))
                    {
                        Level.Current.anyKeyPressed = true;
                        Level.Current.moveValue += new Vector2(1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                        Player.Nyr.entityFacing = 1;
                        if (Player.Nyr.currentEntityState == (int)Playerstates.IDLE || Player.Nyr.currentEntityState == (int)Playerstates.STOP || Player.Nyr.currentEntityState == (int)Playerstates.DANCE)
                        {
                            Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                            Player.Nyr.currentFrame = 0;
                            Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                        }
                    }
                    if (state.IsButtonDown(Buttons.DPadLeft))
                    {
                        Level.Current.anyKeyPressed = true;
                        if (!Player.Nyr.isCrouching)
                        {
                            Level.Current.moveValue += new Vector2(-1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Player.Nyr.entityFacing = -1;
                            if (Player.Nyr.currentEntityState == (int)Playerstates.IDLE || Player.Nyr.currentEntityState == (int)Playerstates.STOP || Player.Nyr.currentEntityState == (int)Playerstates.DANCE)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                    }
                    if (state.IsButtonDown(Buttons.DPadUp))
                    {
                        Level.Current.anyKeyPressed = true;
                        if (Player.Nyr.inHub)
                        {
                            Level.Current.moveValue += new Vector2(0, -1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            if (Player.Nyr.currentEntityState != (int)Playerstates.WALK)
                            {
                                Player.Nyr.currentEntityState = (int)Playerstates.WALK;
                                Player.Nyr.currentFrame = 0;
                                Player.Nyr.nextEntityState = (int)Playerstates.WALK;
                            }
                        }
                    }
                    if (state.IsButtonDown(Buttons.DPadDown))
                    {
                        Level.Current.anyKeyPressed = true;

                        if (Player.Nyr.inHub)
                        {
                            Level.Current.moveValue += new Vector2(0, 1 * Player.Nyr.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
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
                        }
                        else if (!Player.Nyr.inStomp && Player.Nyr.currentEntityState == (int)Playerstates.FALL && Level.armorEnhanced[(int)BossElements.EARTH] && Player.Nyr.mana >= 40)
                        {
                            Player.Nyr.mana -= 40;
                            Player.Nyr.inStomp = true;
                            //Player.Nyr.currentEntityState = (int)Playerstates.STOMP;
                            //Player.Nyr.currentFrame = 0;
                            //Player.Nyr.nextEntityState = (int)Playerstates.STOMP;

                        }

                    }
                }

                //Buttons
                //Shoulders
                if (state.IsButtonDown(Buttons.A) && !lastPressedGamePadButtons[0])
                {
                    Level.Current.anyKeyPressed = true;
                    if (Player.Nyr.onGround && !Player.Nyr.inJump && !Player.Nyr.inHub && Player.Nyr.currentEntityState != (int)Playerstates.SLIP)
                    {
                        Player.Nyr.currentEntityState = (int)Playerstates.JUMP;
                        Player.Nyr.currentFrame = 0;
                        Player.Nyr.nextEntityState = (int)Playerstates.FALL;
                        Player.Nyr.inJump = true;
                        Player.Nyr.onGround = false;
                        Level.Current.moveValue.Y -= Player.Nyr.jumpHeight;
                        SFX.CurrentSFX.loadSFX("sfx/sfx_jump");
                    }

                }
                if (state.IsButtonDown(Buttons.Y) && !lastPressedGamePadButtons[24])
                {
                    Level.Current.anyKeyPressed = true;
                    Player.Nyr.interact = true;
                }
                if (state.IsButtonDown(Buttons.Back) && !lastPressedGamePadButtons[2])
                {
                    Level.Current.anyKeyPressed = true;
                    GameUI.Handeler.ShowMap = !GameUI.Handeler.ShowMap;
                }
                if (state.IsButtonDown(Buttons.X) && !lastPressedGamePadButtons[23])
                {
                    Level.Current.anyKeyPressed = true;
                    if (Player.Nyr.attackCooldown == 0 && !Player.Nyr.inHub)
                    {
                        Player.Nyr.currentEntityState = (int)Playerstates.FIGHT;
                        Player.Nyr.currentFrame = 0;
                        Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                        Player.Nyr.fAttackCheck = 20;
                        Player.Nyr.attackCooldown = 30;
                        SFX.CurrentSFX.loadSFX("sfx/sfx_attack");

                    }
                }
                if (state.IsButtonDown(Buttons.RightShoulder) && !lastPressedGamePadButtons[15])
                {
                    Level.Current.anyKeyPressed = true;
                    if (Player.Nyr.skillCooldown == 0 && !Player.Nyr.inHub && Level.armorEnhanced[(int)BossElements.ICE] && Player.Nyr.mana >= 30)
                    {
                        Player.Nyr.mana -= 30;
                        Player.Nyr.skillCooldown = 60;
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
                if (state.IsButtonDown(Buttons.LeftShoulder) && !lastPressedGamePadButtons[8])
                {
                    Level.Current.anyKeyPressed = true;
                    if (Player.Nyr.skillCooldown == 0 && !Player.Nyr.inHub && Level.armorEnhanced[(int)BossElements.FIRE] && Player.Nyr.mana >= 80)
                    {
                        Player.Nyr.mana -= 80;
                        Player.Nyr.skillCooldown = 60;
                        Player.Nyr.CastFireAOE();
                        SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                        Level.Current.fireAoeTimer = 40;
                        Player.Nyr.MakeInvulnerable(40);
                    }
                }
                if (state.IsButtonDown(Buttons.B) && !lastPressedGamePadButtons[1])
                {
                    Level.Current.anyKeyPressed = true;
                    if (Level.armorEnhanced[(int)BossElements.BOLT] && Level.Current.hasDashed == false && Player.Nyr.mana >= 30)
                    {
                        Player.Nyr.currentEntityState = (int) Playerstates.EVASION;
                        Player.Nyr.currentFrame = 0;
                        Player.Nyr.mana -= 30;
                        Player.Nyr.MakeInvulnerable();
                        Level.Current.dashtimer = 30;
                        Level.Current.tempposition = Player.Nyr.position;
                        Level.Current.hasDashed = true;
                        SFX.CurrentSFX.loadSFX("sfx/sfx_attack");
                    }
                }
            }
        }
        
        private void SetDevice()
        {
            if(controllerUsed && Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                controllerUsed = false;
                return;
            }

            for(int i = 0; i < allGamepadButtons.Length; i++)
            {
                if (GamePad.GetState(0).IsButtonDown(allGamepadButtons[i]))
                {
                    currentPressedGamePadButtons[i] = true;
                    controllerUsed = true;
                    return;
                }
            }
        }
    }
}
