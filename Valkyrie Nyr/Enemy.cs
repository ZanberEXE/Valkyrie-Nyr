using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Valkyrie_Nyr
{
    class Enemy : Entity
    {

        public int aggroRange;
        int defaultAttackRange;
        public int attackRange;
        public int speed;
        //TODO: DEBUG Höhe resetten
        public float heightReset;

        int stateTimer;
        int aiyeWalking = 56;
        bool aiyeDoIt = false;
        bool bossWalked = false;

        bool repairAnimation = false;
        bool isRepaired = false;
        public Vector2 tempPosition;
        bool animationStart = false;
        bool animationEnd = false;

        int effektTimer;
        bool yinyinSpikes = false;
        bool monomonoLightning = false;
        bool aiyeSpikes = false;
        bool aiyeProjektiles = false;
        bool aiyeWallToRight = false;
        bool aiyeWallToLeft = false;

        bool beginFight = false;
        bool fightStarted = false;
        bool hasAttacked = false;
        bool waitAttack = false;

        int nextAttack;
        int bufferValue;

        Projectile[] tempEffekt = new Projectile[6]; 

        Rectangle defaultHurtBox;
        Rectangle defaultAttackBox;

        GameObject earthWall;

        List<GameObject> aiyePlattform = new List<GameObject>();

        public Enemy(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, int _aggroRange, int _attackRange, int _speed, int _attackBoxWidth, int _attackBoxHeight, bool _animationFlip) : base(name, triggerType, mass, height, width, position, hp, dmg, _attackBoxWidth, _attackBoxHeight, _animationFlip)
        {
            aggroRange = _aggroRange;
            attackRange = _attackRange;
            speed = _speed;
            attackBoxHeight = _attackBoxHeight;
            attackBoxWidth = _attackBoxWidth;
            animationFlip = _animationFlip;
        }
        void HurtNyr(int _damage)
        {
            if (Player.Nyr.isInvulnerable == false)
            {
                Player.Nyr.health -= _damage;
                Player.Nyr.MakeInvulnerable();
                
            }
            
        }
      

        public void Update(GameTime gameTime)
        {

            Vector2 nextPosition = new Vector2((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds), 0);

            GameObject[] collideObjekts = this.Collision<GameObject>(Level.Current.gameObjects.ToArray(), nextPosition + position);



            if (collideObjekts.Length > 0)
            {
                foreach (GameObject element in collideObjekts)
                {
                    if (position.Y + height <= element.position.Y || element.triggerType != null)
                    {
                        continue;
                    }
                    entityFacing *= -1;
                    break;
                }

            }

            base.EntityUpdate(gameTime);





            // SMALL ENEMIES
            if (name != "Ina" && name != "Yinyin" && name != "Aiye" && name != "Monomono")
            {
                if (NyrBy(aggroRange))
                {
                    beginFight = true;
                    // aggroRange = 0;
                }
                if (beginFight)
                {
                    beginFight = false;
                    fightStarted = true;
                    
                }
                if (fightStarted)
                {

                    if (currentEntityState == (int)Enemystates.IDLE)
                    {

                        nextEntityState = (int)Enemystates.AGGRO;

                    }
                    if (currentEntityState == (int)Enemystates.AGGRO)
                    {
                        nextEntityState = (int)Enemystates.WALK;
                    }

                    if (currentEntityState == (int)Enemystates.WALK)
                    {
                        if (Player.Nyr.position.X + 40 < position.X)
                        {
                            entityFacing = 1;
                            position.X -= nextPosition.X * entityFacing;

                        }
                        if (Player.Nyr.position.X + 40 > position.X)
                        {
                            entityFacing = -1;
                            position.X -= nextPosition.X * entityFacing;

                        }
                        if (name == "Banshee")
                        {

                            if (Player.Nyr.position.Y + 20 > position.Y)
                            {
                                position.Y += ((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                            }
                            if (Player.Nyr.position.Y + 20 < position.Y)
                            {
                                position.Y -= ((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                            }

                        }

                    }
                    if (NyrBy(attackRange))
                    {

                        currentEntityState = (int)Enemystates.ATTACK;
                        if (currentEntityState == (int)Enemystates.ATTACK)
                        {
                            if (name == "Banshee")
                            {
                                stateTimer = 80;
                                defaultAttackRange = attackRange;
                                attackRange = 0;
                            }
                        }

                    }
                    if (currentEntityState == (int)Enemystates.ATTACK)
                    {
                        if (name == "Banshee" && stateTimer <= 30 && stateTimer >= 2)
                        {
                            if (entityFacing == -1)
                            {
                                attackBox.X = (int)position.X - 140;
                            }
                            if (entityFacing == 1)
                            {
                                attackBox.X = (int)position.X - 180;
                            }
                            attackBox.Y = (int)position.Y - 120;
                            attackBox.Width = 350;
                            attackBox.Height = 300;
                            if (CollisionAABB(attackBox, Player.Nyr.hurtBox))
                            {
                                HurtNyr(damage);
                            }
                        }

                        if (name == "Banshee" && stateTimer <= 1)
                        {

                            hasAttacked = true;

                            //Banshee stuff
                            position = new Vector2(20000, 20000);
                            fightStarted = false;
                        }
                    }
                    if (hasAttacked == true)
                    {
                        attackRange = defaultAttackRange;
                    }
                }

                stateTimer--;
            }




            //BOSS ENEMIES
            if (name == "Ina" || name == "Yinyin" || name == "Aiye" || name == "Monomono")
            {
                
                if (NyrBy(aggroRange))
                {
                    beginFight = true;
                    aggroRange = 0;
                }
                if (beginFight)
                {
                    Level.Current.nscObjects[0].startConversation(gameTime);
                    stateTimer = 50;
                    defaultHurtBox = hurtBox;
                    defaultAttackBox = attackBox;
                    beginFight = false;
                    fightStarted = true;

                    States.CurrentBGMState = BGMStates.BOSS;
                    // TODO: Höhe resetten
                    //heightReset = position.Y + Camera.Main.position.Y;
                }





                if (fightStarted)
                {

                    if (currentEntityState == (int)Bossstates.IDLE)
                    {
                        aiyeDoIt = false;

                        // TODO: Höhe resetten //position.Y = heightReset - Camera.Main.position.Y;

                        hurtBox.Width = defaultHurtBox.Width;
                        hurtBox.Height = defaultHurtBox.Height;
                        attackBox.Width = defaultAttackBox.Width;
                        attackBox.Height = defaultAttackBox.Height;

                        if (animationEnd)
                        {
                            repairAnimation = true;
                            isRepaired = false;
                        }
                        if (stateTimer <= 1)
                        {
                            nextEntityState = (int)Bossstates.WALK;
                            if (name == "Yinyin")
                            {
                                stateTimer = GenerateNumber(10, 200);
                            }
                            else
                            {
                                stateTimer = GenerateNumber(100, 400);
                            }
                            
                        }
                    }
                    if (currentEntityState == (int)Bossstates.WALK && waitAttack == false)
                    {
                        if (name == "Aiye")
                        {
                            if (aiyeWalking % 93 == 0)
                            {
                                if (Player.Nyr.onGround)
                                {
                                    Player.Nyr.position.Y -= 50;
                                }

                            }
                            aiyeWalking++;
                        }

                        position.X += nextPosition.X * entityFacing;

                        if (stateTimer <= 1)
                        {

                            if (name == "Ina")
                            {
                                nextAttack = GenerateNumber(4);
                                
                                if (nextAttack == 0)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                }
                                if (nextAttack == 1)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                }
                                if (nextAttack == 2)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK3;
                                }
                                if (nextAttack == 3)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                }
                            }
                            if (name == "Yinyin")
                            {
                                nextAttack = GenerateNumber(2);

                                if (nextAttack == 0)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                    stateTimer = 90;
                                }
                                if (nextAttack == 1)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                }
                            }
                            if (name == "Aiye")
                            {
                                nextAttack = GenerateNumber(4);

                                if (nextAttack == 0)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                }
                                if (nextAttack == 1)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                }
                                if (nextAttack == 2)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK3;
                                }
                                if (nextAttack == 3)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                }
                            }
                            if (name == "Monomono")
                            {
                                {
                                    nextAttack =  GenerateNumber(4);

                                    if (nextAttack == 0)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK1;
                                    }
                                    if (nextAttack == 1)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK2;
                                    }
                                    if (nextAttack == 2)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK3;
                                    }
                                    if (nextAttack == 3)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK1;
                                        stateTimer = 50;
                                    }
                                }
                            }

                            bossWalked = true;
                        }
                        if (bossWalked == true)
                        {
                            if (name == "Ina")
                            {
                                if (nextEntityState == (int)Bossstates.ATTACK1)
                                {
                                    stateTimer = 50;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK2)
                                {
                                    stateTimer = 350;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK3)
                                {
                                    stateTimer = 190;
                                }
                            }
                            if (name == "Yinyin")
                            {
                                if (nextEntityState == (int)Bossstates.ATTACK1)
                                {
                                    stateTimer = 60;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK2)
                                {
                                    stateTimer = 120;
                                }
                            }
                            if (name == "Aiye")
                            {
                                aiyeWalking++;
                                if (nextEntityState == (int)Bossstates.ATTACK1)
                                {
                                    stateTimer = 111;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK2)
                                {
                                    stateTimer = 50;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK3)
                                {
                                    stateTimer = 200;
                                }
                            }
                            if (name == "Monomono")
                            {
                                if (nextEntityState == (int)Bossstates.ATTACK1)
                                {
                                    stateTimer = 100;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK2)
                                {
                                    stateTimer = 60;
                                }
                                if (nextEntityState == (int)Bossstates.ATTACK3)
                                {

                                }
                            }
                            if (name != "Yinyin" && name != "Ina" && name != "Aiye")
                            {
                                //waitAttack = true;
                            }
                            
                        }

                        
                    
                    }
                    if (currentEntityState == (int)Bossstates.ATTACK1)
                    {
                        if (name != "Aiye")
                        {
                            bossWalked = false;
                        }
                        

                        if (name == "Ina")      // FireBall
                        {
                            if (Player.Nyr.position.X + 40 < position.X)
                            {
                                entityFacing = -1;
                            }
                            if (Player.Nyr.position.X + 40 > position.X)
                            {
                                entityFacing = 1;
                            }
                            if (stateTimer == 10)
                            {
                                if (entityFacing == 1)
                                {
                                    new Projectile("Fireball", 65 * 2, 26 * 2, new Vector2(position.X + 20, position.Y + 40), new Vector2(1, 0), 800, false, new Rectangle(-110, -0, 60 * 2, 30 * 2), false, 0, 0, damage * 2);
                                }
                                else
                                {
                                    new Projectile("Fireball", 65 * 2, 26 * 2, new Vector2(position.X + 20, position.Y + 40), new Vector2(-1, 0), 800, false, new Rectangle(-10, -60, 60 * 2, 30 * 2), false, 0, 0, damage * 2);
                                }
                                Player.Nyr.position.Y -= 10;
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 10;
                            }
                        }
                        if (name == "Yinyin")   // Ice Spit
                        {
                            if (Player.Nyr.position.X + 40 < position.X)
                            {
                                entityFacing = -1;


                            }
                            if (Player.Nyr.position.X + 40 > position.X)
                            {
                                entityFacing = 1;


                            }
                            if (stateTimer == 20)
                            {
                                if (entityFacing == 1)
                                {
                                    new Projectile("IceArrow", 30 * 2, 10 * 2, new Vector2(position.X + 350, position.Y - 90), new Vector2(1, 0), 1200, false, new Rectangle(-350, 0, 30 * 2, 10 * 2), false, 0, 0, damage * 2);
                                    
                                }
                                else
                                {
                                    new Projectile("IceArrow", 30 * 2, 10 * 2, new Vector2(position.X - 320, position.Y - 90), new Vector2(-1, 0), 1200, false, new Rectangle(+280, -10, 30 * 2, 10 * 2), false, 0, 0, damage * 2);
                                }
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 10;
                            }
                        }
                        if (name == "Aiye")         // Jump Attack                                    
                        {
                            if (aiyeDoIt == false && stateTimer == 110)
                            {
                                aiyeDoIt = true;
                                bossWalked = false;
                            }
                            aiyeWalking = 56;
                            if (aiyeDoIt)
                            {
                                if (stateTimer <= 75 && stateTimer >= 35)
                                {
                                    if (Player.Nyr.position.X < position.X)
                                    {
                                        entityFacing = -1;
                                        position.X -= nextPosition.X * entityFacing * -3;

                                    }
                                    if (Player.Nyr.position.X > position.X)
                                    {
                                        entityFacing = 1;
                                        position.X -= nextPosition.X * entityFacing * -3;

                                    }
                                    if (stateTimer >= 60)
                                    {
                                        position.Y -= 10;
                                    }
                                    if (stateTimer <= 50)
                                    {
                                        position.Y += 10;
                                    }
                                }


                                if (stateTimer == 50)
                                {
                                    aiyeSpikes = true;
                                    effektTimer = 150;
                                }
                                if (stateTimer == 40)
                                {
                                    tempPosition = position;
                                }
                                if (stateTimer <= 20)
                                {
                                    nextEntityState = (int)Bossstates.IDLE;
                                    stateTimer = 19;
                                }
                            }
                            
                        }
                        if (name == "Monomono")
                        {
                            if (Player.Nyr.position.X + 40 < position.X)
                            {
                                entityFacing = -1;
                            }
                            if (Player.Nyr.position.X + 40 > position.X)
                            {
                                entityFacing = 1;
                            }
                            if (stateTimer == 42 || stateTimer == 12)
                            {

                                if (entityFacing == 1)
                                {
                                    new Projectile("Blitzball2", 60 * 2, 60 * 2, new Vector2(position.X + 20, position.Y + 650), new Vector2(1, 0), 800, false, new Rectangle(-40, -0, 50, 50), false, 0, 0, damage * 2);
                                }
                                else
                                {
                                    new Projectile("blitzball2", 60 * 2, 62 *2, new Vector2(position.X - 20, position.Y + 710), new Vector2(-1, 0), 800, false, new Rectangle(-5, -50, 50, 50), false, 0, 0, damage * 2);
                                }
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 10;
                            }
                        }
                        waitAttack = false;
                    }
                    if (currentEntityState == (int)Bossstates.ATTACK2)
                    {
                        bossWalked = false;
                        if (name == "Ina")                                              // MeteorShower
                        {
                            if (isRepaired == false)
                            {
                                repairAnimation = true;
                                animationStart = true;
                            }
                            hurtBox.Y = hurtBox.Y + 400;

                            if (stateTimer <= 320 && stateTimer >= 2)
                            {
                                if (stateTimer % 10 == 0)
                                {
                                    int randomNumber = GenerateNumber(12000, 15000) - (int)Camera.Main.position.X;
                                    float randomFlightRotationNumber = GenerateNumber(-2, 3, randomNumber) / 10f;
                                    
                                    new Projectile("Fireball", 65 * 3, 26 * 3, new Vector2(randomNumber, -position.Y - Camera.Main.position.Y + 4000), new Vector2(randomFlightRotationNumber, 1), 600, false, new Rectangle(-110 - (int)(randomFlightRotationNumber * 200) , -180, 60, 70), false, 0, 0, damage * 2);
                                }

                                if (entityFacing == 1)
                                {
                                    attackBox.X = (int)position.X - 70;
                                    attackBox.Y = (int)position.Y + 30;
                                    attackBox.Width = 190;
                                    attackBox.Height = 300;
                                }

                                if (entityFacing == -1)
                                {
                                    attackBox.X = (int)position.X - 50;
                                    attackBox.Y = (int)position.Y + 30;
                                    attackBox.Width = 190;
                                    attackBox.Height = 300;
                                }
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 100;

                                animationEnd = true;
                            }
                        }
                        if (name == "Yinyin")         // FLIP                                    
                        {
                            if (stateTimer <= 55 && stateTimer >= 39)
                            {
                                animationFlip = true;
                                if (Player.Nyr.position.X + 40 < position.X)
                                {
                                    entityFacing = 1;
                                    position.X -= nextPosition.X * entityFacing * 4;

                                }
                                if (Player.Nyr.position.X + 40 > position.X)
                                {
                                    entityFacing = -1;
                                    position.X -= nextPosition.X * entityFacing * 4;

                                }
                                position.Y -= 10;
                                bufferValue -= 10;


                            }
                            if (stateTimer == 38)
                            {
                                position.Y += bufferValue * -1;
                                bufferValue = 0;
                            }
                            if (stateTimer == 35)
                            {
                                yinyinSpikes = true;
                                effektTimer = 100;
                                 
                            }
                            if (stateTimer <= 1)
                            {
                                animationFlip = false;
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 10;
                            }
                        }
                        if (name == "Aiye")         // create Earthprojectile
                        {
                            aiyeWalking = 56;
                            bossWalked = false;
                            if (stateTimer == 20)
                            {
                                aiyeProjektiles = true;
                                effektTimer = 400;
                            }
                            if (stateTimer <= 10)
                            {
                                nextEntityState = (int)Bossstates.WALK;
                                stateTimer = 400;
                            }

                        }
                        if (name == "Monomono")
                        {
                            if (stateTimer == 15)
                            {
                                monomonoLightning = true;
                                effektTimer = 160;
                            }
                            if (stateTimer <= 10)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 160;
                            }
                        }
                        waitAttack = false;
                    }
                    if (currentEntityState == (int)Bossstates.ATTACK3)
                    {
                        bossWalked = false;
                        if (name == "Ina")                                               // Explosion
                        {
                            if (stateTimer <= 55 && stateTimer >= 1)
                            {
                                if (entityFacing == 1)
                                {
                                    attackBox.X = (int)position.X - 260;
                                    attackBox.Y = (int)position.Y - 250;
                                    attackBox.Width = 550;
                                    attackBox.Height = 500;
                                }

                                if (entityFacing == -1)
                                {
                                    attackBox.X = (int)position.X - 220;
                                    attackBox.Y = (int)position.Y - 250;
                                    attackBox.Width = 550;
                                    attackBox.Height = 500;
                                }
                                if (CollisionAABB(attackBox, Player.Nyr.hurtBox))
                                {
                                    HurtNyr(500);

                                }
                            }
                            if (stateTimer <= 10)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 100;

                            }
                        }
                        if (name == "Aiye")
                        {
                            aiyeWalking = 56;
                            bossWalked = false;
                            if (stateTimer == 50)
                            {
                                if (Player.Nyr.position.X + Camera.Main.position.X <= 15000 && Player.Nyr.position.X + Camera.Main.position.X >= 12250)
                                {
                                    position.X = 14800 - Camera.Main.position.X;
                                    entityFacing = -1;

                                }
                                if (Player.Nyr.position.X + Camera.Main.position.X <= 15000 && Player.Nyr.position.X + Camera.Main.position.X >= 13625)
                                {
                                    entityFacing = 1;
                                    position.X = 12250 - Camera.Main.position.X;

                                }
                            }
                            if (stateTimer <= 30)
                            {
                                nextEntityState = (int)Bossstates.Special1;
                                stateTimer = 120;
                            }

                        }
                        if (name == "Monomono")
                        {
                            nextEntityState = (int)Bossstates.Special1;
                            stateTimer = 300;
                        }
                        waitAttack = false;
                    }
                    if (currentEntityState == (int)Bossstates.ATTACK4)
                    {

                    }
                    if (currentEntityState == (int)Bossstates.Special1)
                    {
                        if (name == "Aiye")
                        {
                            if (isRepaired == false)
                            {
                                repairAnimation = true;
                                animationStart = true;
                            }
                            attackBox.Y += 600;
                            hurtBox.Y += 600;
                            if (stateTimer == 35)
                            {
                                if (entityFacing == -1)
                                {
                                    aiyeWallToLeft = true;
                                    effektTimer = 600;
                                }
                                if (entityFacing == 1)
                                {
                                    aiyeWallToRight = true;
                                    effektTimer = 600;
                                }
                                
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 600;

                                animationEnd = true;
                            }
                        }
                        if (name == "Monomono")
                        {
                            position.X += nextPosition.X * entityFacing * 3;
                            if (CollisionAABB(Player.Nyr.hurtBox, attackBox))
                            {
                                HurtNyr(damage * 3);
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.Special2;
                            }
                        }
                    }
                    if (currentEntityState == (int)Bossstates.Special2)
                    {
                        nextEntityState = (int)Bossstates.IDLE;
                        stateTimer = 200;
                    }

                    if (CollisionAABB(Player.Nyr.hurtBox, attackBox))
                    {
                        if (name == "Ina")
                        {
                            if (currentEntityState != (int)Bossstates.ATTACK1 && currentEntityState != (int)Bossstates.ATTACK2 && currentEntityState != (int)Bossstates.ATTACK3)
                            {
                                HurtNyr(damage);
                            }
                            if (currentEntityState == (int)Bossstates.ATTACK2)
                            {
                                HurtNyr(damage * 3);
                            }
                        }
                        if (name == "Yinyin")
                        {
                            if (currentEntityState != (int)Bossstates.ATTACK1 && currentEntityState != (int)Bossstates.ATTACK2 && currentEntityState != (int)Bossstates.ATTACK3)
                            {
                                HurtNyr(damage);
                            }
                        }
                        if (name == "Aiye")
                        {
                            if (currentEntityState != (int)Bossstates.ATTACK1 && currentEntityState != (int)Bossstates.ATTACK2 && currentEntityState != (int)Bossstates.ATTACK3)
                            {
                                HurtNyr(damage);
                            }
                        }
                        if (name == "Monomono")
                        {
                            if (currentEntityState != (int)Bossstates.ATTACK1 && currentEntityState != (int)Bossstates.ATTACK2 && currentEntityState != (int)Bossstates.Special2)
                            {
                                HurtNyr(damage);
                            }
                        }
                    }
                }
                if (repairAnimation)
                {

                    if (animationStart)
                    {
                        if (name == "Ina")
                        {
                            position.Y -= 115;
                        }
                        if (name == "Aiye")
                        {
                            position.Y -= 600;

                        }
                        isRepaired = true;
                    }
                    if (animationEnd)
                    {
                        if (name == "Ina")
                        {
                            position.Y += 115;
                        }
                        if (name == "Aiye")
                        {
                            position.Y += 600;
                            hurtBox.Y += 600;
                        }
                    }

                    repairAnimation = false;
                    animationEnd = false;
                    animationStart = false;
                }
                stateTimer--;
            }


            ///// ANIMATIONSEFFEKTE!!!!!! //////

            if (yinyinSpikes)
            {
                if (effektTimer == 99)
                {
                    tempEffekt[0] = new Projectile("YinyinSpikesW400H200", 200, 400, new Vector2(position.X + 30, position.Y + 30), new Vector2(0, 0), 800, true, new Rectangle(-185,-40, 350, 40), true, 50, 10, damage * 3);

                }
                if (effektTimer <= 60 && tempEffekt[0] != null)
                {
                    tempEffekt[0].attackbox.Width -= 2;
                    tempEffekt[0].attackBoxOffset.X += 1;
                }
                if (effektTimer <= 24 && tempEffekt[0] != null)
                {
                    yinyinSpikes = false;
                    tempEffekt[0].Destroy();
                    tempEffekt[0] = null;
                }
            }
            if (monomonoLightning)
            {
                if (effektTimer == 120)
                {
                    tempEffekt[0] = new Projectile("LightningW200H800v2", 800, 200, new Vector2(position.X + 200, position.Y + 400), new Vector2(0, 0), 800, true, new Rectangle(-20, -800, 80, 1200), true, 12, 10, damage * 3);
                    tempEffekt[1] = new Projectile("LightningW200H800v2", 800, 200, new Vector2(position.X - 200, position.Y + 400), new Vector2(0, 0), 800, true, new Rectangle(-20, -800, 80, 1200), true, 12, 10, damage * 3);

                }
                if (effektTimer == 80)
                {
                    tempEffekt[2] = new Projectile("LightningW200H800v2", 800, 200, new Vector2(position.X + 400, position.Y + 400), new Vector2(0, 0), 800, true, new Rectangle(-20, -800, 80, 1200), true, 12, 10, damage * 3);
                    tempEffekt[3] = new Projectile("LightningW200H800v2", 800, 200, new Vector2(position.X - 400, position.Y + 400), new Vector2(0, 0), 800, true, new Rectangle(-20, -800, 80, 1200), true, 12, 10, damage * 3);

                }
                if (effektTimer == 40)
                {
                    tempEffekt[4] = new Projectile("LightningW200H800v2", 800, 200, new Vector2(position.X + 600, position.Y + 400), new Vector2(0, 0), 800, true, new Rectangle(-20, -800, 80, 1200), true, 12, 10, damage * 3);
                    tempEffekt[5] = new Projectile("LightningW200H800v2", 800, 200, new Vector2(position.X - 600, position.Y + 400), new Vector2(0, 0), 800, true, new Rectangle(-20, -800, 80, 1200), true, 12, 10, damage * 3);

                }
                if (effektTimer <= 100 && tempEffekt[0] != null && tempEffekt[1] != null)
                {
                    
                    tempEffekt[0].Destroy();
                    tempEffekt[1].Destroy();
                    tempEffekt[0] = null;
                    tempEffekt[1] = null;
                }
                if (effektTimer <= 60 && tempEffekt[2] != null && tempEffekt[3] != null)
                {

                    tempEffekt[2].Destroy();
                    tempEffekt[3].Destroy();
                    tempEffekt[2] = null;
                    tempEffekt[3] = null;
                }
                if (effektTimer <= 20 && tempEffekt[4] != null && tempEffekt[5] != null)
                {

                    tempEffekt[4].Destroy();
                    tempEffekt[5].Destroy();
                    tempEffekt[4] = null;
                    tempEffekt[5] = null;
                }
                if (effektTimer == 10)
                {
                    monomonoLightning = false;
                }
            }
            if (aiyeSpikes)
            {
                if (effektTimer == 140)
                {
                    if (entityFacing == 1)
                    {
                        tempEffekt[0] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X + 200, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                   
                    if (entityFacing == -1)
                    {
                        tempEffekt[0] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X - 100, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                }
                if (effektTimer == 135)
                {
                    if (entityFacing == 1)
                    {
                        tempEffekt[1] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X + 250, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }

                    if (entityFacing == -1)
                    {
                        tempEffekt[1] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X - 150, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                }
                if (effektTimer == 130)
                {
                    if (entityFacing == 1)
                    {
                        tempEffekt[2] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X + 300, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }

                    if (entityFacing == -1)
                    {
                        tempEffekt[2] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X - 200, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                }
                if (effektTimer == 125)
                {
                    if (entityFacing == 1)
                    {
                        tempEffekt[3] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X + 350, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }

                    if (entityFacing == -1)
                    {
                        tempEffekt[3] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X - 250, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                }
                if (effektTimer == 120)
                {
                    if (entityFacing == 1)
                    {
                        tempEffekt[4] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X + 400, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }

                    if (entityFacing == -1)
                    {
                        tempEffekt[4] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X - 300, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                }
                if (effektTimer == 115)
                {
                    if (entityFacing == 1)
                    {
                        tempEffekt[5] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X + 450, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }

                    if (entityFacing == -1)
                    {
                        tempEffekt[5] = new Projectile("EarthSpikeW200H200", 200, 200, new Vector2(tempPosition.X - 350, tempPosition.Y + 180), new Vector2(0, 0), 800, true, new Rectangle(-10, -1000, 20, 50), true, 50, 10, damage * 3);
                    }
                }
                if (effektTimer == 100)
                {
                    tempEffekt[0].attackBoxOffset.Y = 0;
                }
                if (effektTimer == 95)
                {
                    tempEffekt[1].attackBoxOffset.Y = 0;
                }
                if (effektTimer == 90)
                {
                    tempEffekt[2].attackBoxOffset.Y = 0;
                }
                if (effektTimer == 85)
                {
                    tempEffekt[3].attackBoxOffset.Y = 0;
                }
                if (effektTimer == 80)
                {
                    tempEffekt[4].attackBoxOffset.Y = 0;
                }
                if (effektTimer == 75)
                {
                    tempEffekt[5].attackBoxOffset.Y = 0;
                }
                if (effektTimer <= 70 && tempEffekt[0] != null)
                {

                    tempEffekt[0].Destroy();
                    tempEffekt[0] = null;
                }
                if (effektTimer <= 65 && tempEffekt[1] != null)
                {

                    tempEffekt[1].Destroy();
                    tempEffekt[1] = null;
                }
                if (effektTimer <= 60 && tempEffekt[2] != null)
                {

                    tempEffekt[2].Destroy();
                    tempEffekt[2] = null;
                }
                if (effektTimer <= 55 && tempEffekt[3] != null)
                {

                    tempEffekt[3].Destroy();
                    tempEffekt[3] = null;
                }
                if (effektTimer <= 50 && tempEffekt[4] != null)
                {

                    tempEffekt[4].Destroy();
                    tempEffekt[4] = null;
                }
                if (effektTimer <= 45 && tempEffekt[5] != null)
                {

                    tempEffekt[5].Destroy();
                    tempEffekt[5] = null;
                }
                if (effektTimer <= 1)
                {
                    aiyeSpikes = false;
                }
            }
            if (aiyeProjektiles)
            {
                if (effektTimer == 400)
                {
                    tempEffekt[0] = new Projectile("AiyeEarthProjectile", 500, 400, new Vector2(tempPosition.X, tempPosition.Y), new Vector2(0, 0), 800, true, new Rectangle(-150, -90, 300, 200), true, 50, 10, damage * 2);
                }
                if (effektTimer <= 399 && effektTimer >= 100)
                {

                    tempEffekt[0].position.X = position.X + 50;
                    tempEffekt[0].position.Y = position.Y + 90;
                }
                if (effektTimer == 99)
                {
                    tempEffekt[0].Destroy();
                    tempEffekt[0] = null;
                    int tempNumber = GenerateNumber(3);
                    if (tempNumber == 0)
                    {
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(1, 1), 800, false, new Rectangle(-10, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(-1, 1), 800, false, new Rectangle(-20, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(1, -1), 800, false, new Rectangle(-15, -10, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(-1, -1), 800, false, new Rectangle(-15, -20, 30, 30), false, 0, 0, damage * 2);
                    }
                    else if (tempNumber == 1)
                    {
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(1, 0), 800, false, new Rectangle(-10, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(-1, 0), 800, false, new Rectangle(-20, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(0, 1), 800, false, new Rectangle(-10, -10, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(0, -1), 800, false, new Rectangle(-15, -20, 30, 30), false, 0, 0, damage * 2);
                    }
                    else
                    {
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(1, 1), 800, false, new Rectangle(-10, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(-1, 1), 800, false, new Rectangle(-20, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(1, -1), 800, false, new Rectangle(-15, -10, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(-1, -1), 800, false, new Rectangle(-15, -20, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(1, 0), 800, false, new Rectangle(-10, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(-1, 0), 800, false, new Rectangle(-20, -15, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(0, 1), 800, false, new Rectangle(-10, -10, 30, 30), false, 0, 0, damage * 2);
                        new Projectile("AiyeErdProjektil", 33, 35, new Vector2(position.X, position.Y + 100), new Vector2(0, -1), 800, false, new Rectangle(-15, -20, 30, 30), false, 0, 0, damage * 2);
                    }
                }
                if (effektTimer <= 1)
                {
                    aiyeProjektiles = false;
                }
            }





            if (aiyeWallToRight)
            {
                if (effektTimer == 599)
                {
                    aiyePlattform = new List<GameObject>();
                    earthWall = new GameObject("AiyeWall", null , 20, 244 * 3,47, new Vector2(+100, +80) + position);
                    earthWall.init();
                    earthWall.moving = new Vector2(2600, 0);
                    Level.Current.gameObjects.Add(earthWall);
                }
                if (effektTimer <= 580 && effektTimer >= 100)
                {
                    for (int i = 0; i < aiyePlattform.Count; i++)
                    {
                        if ((aiyePlattform[i].moving.X <= 0 && earthWall.moving.X > 0) || (aiyePlattform[i].moving.X >= 0 && earthWall.moving.X < 0))
                        {
                            Level.Current.gameObjects.Remove(aiyePlattform[i]);
                            aiyePlattform.RemoveAt(i);
                        }
                    }
                    if (effektTimer % 50 == 0)
                    {
                        GameObject newPlattform = new GameObject("AiyeWall2", null, 0, 30, 100, new Vector2(earthWall.position.X, GenerateNumber(100, 700) + earthWall.position.Y));
                        newPlattform.init();
                        newPlattform.name = "platform";
                        newPlattform.moving = new Vector2(2050, 0);
                        aiyePlattform.Add(newPlattform);
                        Level.Current.gameObjects.Add(newPlattform);
                    }
                    
                }
                if (effektTimer == 2)
                {
                    Rectangle omgItsABox = new Rectangle();
                    for (int i = 0; i < aiyePlattform.Count; i++)
                    {

                        Level.Current.gameObjects.Remove(aiyePlattform[i]);


                    }
                    for (int i = 0; i < Level.Current.gameObjects.Count; i++)
                    {
                        if (Level.Current.gameObjects[i].name == "AiyeWall")
                        {
                            omgItsABox.Location = Level.Current.gameObjects[i].position.ToPoint();
                            omgItsABox.Width = Level.Current.gameObjects[i].width;
                            omgItsABox.Height = Level.Current.gameObjects[i].height;

                            Level.Current.gameObjects.RemoveAt(i);

                            if (CollisionAABB(Player.Nyr.hurtBox, omgItsABox))
                            {
                                Player.Nyr.gameOver();
                            }

                        }
                    }
                }
                if (effektTimer <= 1)
                {
                    aiyePlattform = new List<GameObject>();
                    aiyeWallToRight = false;
                }
            }

            if (aiyeWallToLeft)
            {
                if (effektTimer == 599)
                {
                    aiyePlattform = new List<GameObject>();
                    earthWall = new GameObject("AiyeWall", null, 20, 244 * 3, 47, new Vector2(-100, +80) + position);
                    earthWall.init();
                    earthWall.moving = new Vector2(-2500, 0);
                    Level.Current.gameObjects.Add(earthWall);

                }
                if (effektTimer <= 580 && effektTimer >= 100)
                {
                    for (int i = 0; i < aiyePlattform.Count; i++)
                    {
                        if ((aiyePlattform[i].moving.X <= 0 && earthWall.moving.X > 0) || (aiyePlattform[i].moving.X >= 0 && earthWall.moving.X < 0))
                        {
                            Level.Current.gameObjects.Remove(aiyePlattform[i]);
                            aiyePlattform.RemoveAt(i);
                        }
                    }
                    if (effektTimer % 50 == 0)
                    {
                        GameObject newPlattform = new GameObject("AiyeWall2", null, 0, 30, 100, new Vector2(earthWall.position.X, GenerateNumber(100, 700) + earthWall.position.Y));
                        newPlattform.init();
                        newPlattform.name = "platform";
                        newPlattform.moving = new Vector2(-2000, 0);
                        aiyePlattform.Add(newPlattform);
                        Level.Current.gameObjects.Add(newPlattform);
                    }
                }
                if (effektTimer == 10)
                {
                    Rectangle omgItsABox = new Rectangle();
                    for (int i = 0; i < aiyePlattform.Count; i++)
                    {
                        
                         Level.Current.gameObjects.Remove(aiyePlattform[i]);
                        
                        
                    }
                    for (int i = 0; i < Level.Current.gameObjects.Count; i++)
                    {
                        if (Level.Current.gameObjects[i].name == "AiyeWall")
                        {
                            omgItsABox.Location = Level.Current.gameObjects[i].position.ToPoint();
                            omgItsABox.Width = Level.Current.gameObjects[i].width;
                            omgItsABox.Height = Level.Current.gameObjects[i].height;
                            
                            Level.Current.gameObjects.RemoveAt(i);

                            if (CollisionAABB(Player.Nyr.hurtBox, omgItsABox))
                            {
                                Player.Nyr.gameOver();
                            }
                        }
                    }
                }
                if (effektTimer <= 1)
                {
                    aiyePlattform = new List<GameObject>();
                    aiyeWallToLeft = false;
                }
            }

            if (effektTimer >= 0)
            {
                effektTimer--;
            }

        }


        public void SpawnLoot()
        {
            bool[] allowSpawn = new bool[4] { true, true, Player.Nyr.health < Player.Nyr.maxHealth, Player.Nyr.mana < Player.Nyr.maxMana };
            
            int chosenLoot;

            GameObject spawnedLoot = null;

            do
            {
                chosenLoot = GenerateNumber(allowSpawn.Length);
            }
            while (!(allowSpawn[chosenLoot]));

            switch (chosenLoot)
            {
                case 0:
                case 1:
                    spawnedLoot = new GameObject("Coin", "collectable", 5, 25, 25, position - new Vector2(10, 0));
                    break;
                case 2:
                    spawnedLoot = new GameObject("HPFlower", "collectable", 5, 64, 64, position - new Vector2(10,0));
                    break;
                case 3:
                    spawnedLoot = new GameObject("MPFlower", "collectable", 5, 64, 64, position - new Vector2(10, 0));
                    break;
            }
            if(spawnedLoot != null)
            {
                spawnedLoot.init();
                Level.Current.gameObjects.Add(spawnedLoot);
            }
        }

        private int GenerateNumber(int maxNumber)
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(0, maxNumber);
            return randomNumber;    
        }
        private int GenerateNumber(int minNumber, int maxNumber)
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(minNumber, maxNumber);
            return randomNumber;
        }
        private int GenerateNumber(int minNumber, int maxNumber, int seed)
        {
            Random rnd = new Random(seed);
            int randomNumber = rnd.Next(minNumber, maxNumber);
            return randomNumber;
        }
        /* private double GenerateNumber(double minNumber, double maxNumber)
         {
             Random rnd = new Random();
             double randomNumber = rnd.NextDouble();
             return randomNumber;
         }*/

        protected bool NyrBy(int senseRadius)
        {
            if(Vector2.Distance(this.position, Player.Nyr.position) <= senseRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
