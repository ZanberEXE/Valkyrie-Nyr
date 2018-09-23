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

        int stateTimer;
        int aiyeWalking = 56;
        bool bossWalked = false;

        bool repairAnimation = false;
        bool isRepaired = false;
        public Vector2 tempPosition;
        bool animationStart = false;
        bool animationEnd = false;

        bool beginFight = false;
        bool fightStarted = false;
        bool isAttacking = false;
        bool hasAttacked = false;

        int nextAttack;
        int bufferValue;

        Rectangle defaultHurtBox;
        Rectangle defaultAttackBox;

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

            Vector2 nextPosition = new Vector2((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds),0);

            GameObject[] collideObjekts = this.Collision<GameObject>(Level.Current.gameObjects.ToArray(), nextPosition + position);

            

            if (collideObjekts.Length > 0)
            {
                foreach(GameObject element in collideObjekts)
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
                        if(name == "Banshee" && stateTimer <= 30 && stateTimer >= 2)
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
                    stateTimer = 50;
                    defaultHurtBox = hurtBox;
                    defaultAttackBox = attackBox;
                    beginFight = false;
                    fightStarted = true;

                    States.CurrentBGMState = BGMStates.BOSS;
                }
                if (fightStarted)
                {

                    if (currentEntityState == (int)Bossstates.IDLE)
                    {
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
                            if (name == "Aiye")
                            {
                                stateTimer = 93 * 5;
                            }
                            else
                            {
                                stateTimer = 200;
                            }
                        }
                    }
                    if (currentEntityState == (int)Bossstates.WALK)
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
                        }
                       
                        position.X += nextPosition.X * entityFacing;

                        if (stateTimer <= 1)
                        {

                            if (name == "Ina")
                            {
                                nextAttack = GenerateAttack(4);

                                if (nextAttack == 0)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                    stateTimer = 50;
                                }
                                if (nextAttack == 1)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                    stateTimer = 350;
                                }
                                if (nextAttack == 2)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK3;
                                    stateTimer = 190;
                                }
                                if (nextAttack == 3)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                    stateTimer = 50;
                                }
                            }
                            if (name == "Yinyin")                           
                            {
                                nextAttack = GenerateAttack(2);

                                if (nextAttack == 0)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                    stateTimer = 90;
                                }
                                if (nextAttack == 1)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                    stateTimer = 155;
                                }
                            }
                            if (name == "Aiye")
                            {
                                nextAttack = GenerateAttack(4);

                                if (nextAttack == 0)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK1;
                                    bossWalked = true;
                                }
                                if (nextAttack == 1)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                    bossWalked = true;
                                }
                                if (nextAttack == 2)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK3;
                                    bossWalked = true;
                                }
                                if (nextAttack == 3)
                                {
                                    nextEntityState = (int)Bossstates.ATTACK2;
                                    stateTimer = 50;
                                }
                            }
                            if (name == "Monomono")
                            {
                                {
                                    nextAttack = 1;// GenerateAttack(4);

                                    if (nextAttack == 0)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK1;
                                        stateTimer = 100;
                                    }
                                    if (nextAttack == 1)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK2;
                                        bossWalked = true;
                                    }
                                    if (nextAttack == 2)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK3;
                                        stateTimer = 190;
                                    }
                                    if (nextAttack == 3)
                                    {
                                        nextEntityState = (int)Bossstates.ATTACK1;
                                        stateTimer = 50;
                                    }
                                }
                            }
                        }
                        if (name == "Aiye")
                        {
                            aiyeWalking++;
                            if (bossWalked == true && nextEntityState == (int)Bossstates.ATTACK1)
                            {
                                stateTimer = 110;
                            }
                            if (bossWalked == true && nextEntityState == (int)Bossstates.ATTACK2)
                            {
                                stateTimer = 50;
                            }
                            if (bossWalked == true && nextEntityState == (int)Bossstates.ATTACK3)
                            {
                                stateTimer = 200;
                            }
                        }
                        if (name == "Monomono")
                        {
                            if (bossWalked == true && nextEntityState == (int)Bossstates.ATTACK2)
                            {
                                stateTimer = 120;
                            }
                        }
                        

                    }
                    if (currentEntityState == (int)Bossstates.ATTACK1)
                    {
                        if (name == "Ina")      // FireBall
                        {
                            if (Player.Nyr.position.X + 40 < position.X)
                            {
                                entityFacing = 1;
                                

                            }
                            if (Player.Nyr.position.X + 40 > position.X)
                            {
                                entityFacing = -1;
                                

                            }
                            if (stateTimer <= 10)
                            {
                                                            //TODO: create Fireball
                            }
                            if  (stateTimer <=1)
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
                            if (stateTimer == 5)
                            {
                                //TODO: Create IceSpit
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 10;
                            }
                        }
                        if (name == "Aiye")         // Jump Attack                                    
                        {
                            aiyeWalking = 56;
                            bossWalked = false;
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
                            

                            if (stateTimer == 34)
                            {
                            //TODO: Create Earth Spikes
                            attackBox.Width += 500000;
                            }
                            if (stateTimer <= 10)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 19;
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
                            if (stateTimer == 30 || stateTimer == 11)
                            {
                                Player.Nyr.position.Y -= 10;
                                //TODO: create Fireball
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 10;
                            }
                        }
                    }
                    if (currentEntityState == (int)Bossstates.ATTACK2)
                    {
                        if (name == "Ina")                                              // MeteorShower
                        {
                            if (isRepaired == false)
                            {
                                repairAnimation = true;
                                animationStart = true;
                            }
                            
                            
                            if (stateTimer <= 320 && stateTimer >= 2)
                            {
                                //TODO: create Meteore
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
                                stateTimer = 60;
                                
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
                                //TODO: Create Ice Spikes
                                attackBox.Width += 500000;
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
                            bossWalked = false;
                            if (stateTimer == 20)
                            {
                                //TODO: Create Earthprojectiel
                                attackBox.Width += 500000;
                            }
                            if (stateTimer <= 10)
                            {
                                nextEntityState = (int)Bossstates.WALK;
                                stateTimer = 600;
                            }
                            
                        }
                        if (name == "Monomono")
                        {
                            if (stateTimer == 80 || stateTimer == 15)
                            {
                                // TODO: Create Lightning
                                Player.Nyr.position.Y -= 10;
                            }
                            if (stateTimer <= 10)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                            }
                        }
                    }
                    if (currentEntityState == (int)Bossstates.ATTACK3)
                    {
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
                            bossWalked = false;
                            if (stateTimer == 50)
                            {
                                if (Player.Nyr.position.X + Camera.Main.position.X <= 13624 && Player.Nyr.position.X + Camera.Main.position.X >= 12250)
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
                                //TODO: Create Wallstuff
                                attackBox.Width += 500000;
                            }
                            if (stateTimer <= 1)
                            {
                                nextEntityState = (int)Bossstates.IDLE;
                                stateTimer = 600;

                                animationEnd = true;
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





        }
        private int GenerateAttack(int maxAttacks)
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(0, maxAttacks);
            return randomNumber;
                
        }
        private bool NyrBy(int senseRadius)
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

        public virtual void React()
        {

        }
    }
}
