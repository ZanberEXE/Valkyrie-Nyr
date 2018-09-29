using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Valkyrie_Nyr
{
    

    class Player : Entity
    {
        private static Player nyr;
       

        public float speed;
        public float jumpHeight;
        public float inactivityTime = 0;
        public int slide;
        public bool inHub;
        public bool interact;
        public bool inJump;
        public bool inStomp;
        public bool onIce;
        public bool isCrouching;
        public bool inConversation;
        public NSC conversationPartner;
        public int money;
        public int maxHealth = 3000;
        public int maxMana = 500;
        public int mana = 500;

        public int fAttackCheck;
        public bool inFireAoe = false;

        // Feature bools
        public bool hasHeadband = true;
        public bool hasFireArmor = false;
        public bool hasBoots = false;
        public bool hasBracer = false;

        

        public Player(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, int _attackBoxWidth, int _attackBoxHeight, bool _animationFlip) : base(name, triggerType, mass, height, width, position, hp, dmg, _attackBoxWidth, _attackBoxHeight, _animationFlip)
        {
            speed = 700;
            jumpHeight = 15;
            inHub = false;
            interact = false;
            inJump = false;

            animTex = new animation[]
            {
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Idle"), 10, 3, 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Running"), 10, 3, 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Jump"), 10, 3, 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Attack"), 10, 3, 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Hurt"), 10, 2, 18),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Dance"), 10, 50, 500),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Falling"), 10, 2, 12),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Landing"), 10, 3, 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Stop"), 10, 4, 31),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Crouch"), 10, 3, 25)
            };
            slide = 0;
            onIce = false;
            maxHealth = hp;
            health = maxHealth;
            damage = dmg;

            attackBox.X = 0;
            attackBox.Y = 0;
            attackBox.Width = _attackBoxWidth;
            attackBox.Height = _attackBoxHeight;

            hurtBox.X = (int)position.X;
            hurtBox.Y = (int)position.Y;
            hurtBox.Width = width / 2;
            hurtBox.Height = height;
    }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", null, 10, 180, 120, Vector2.Zero, 1000, 30, 140, 20, false); } return nyr; } }
        
        //put here stuff that happens if you collect something
        public void trigger(GameObject activatedTrigger, GameTime gameTime)
        {
            switch(activatedTrigger.triggerType)
            {
                case "collectable":
                    collect(activatedTrigger.name);
                    Level.Current.gameObjects.Remove(activatedTrigger);
                    break;
                case "area":
                    areaTrigger(activatedTrigger.name);
                    break;
                case "loader":
                    if (interact)
                    {
                        loader(activatedTrigger.name);
                    }
                    else
                    {
                        Level.Current.textboxText = "Press \"F\" to interact";
                    }
                    break;
                case "totem":
                    if (interact)
                    {
                        totem(activatedTrigger);
                    }
                    else
                    {
                        Level.Current.textboxText = "Press \"F\" to interact";
                    }
                    break;
                case "nsc":
                    if (interact)
                    {
                        ((NSC)Convert.ChangeType(activatedTrigger, typeof(NSC)))?.startConversation(gameTime);
                    }
                    else
                    {
                        Level.Current.textboxText = "Press \"F\" to interact";
                    }
                    break;
                case "ryn":
                    if (Antagonist.Ryn.health <= 0)
                    {
                        if (interact)
                        {
                            Antagonist.Ryn.Kill();
                        }
                        else
                        {
                            Level.Current.textboxText = "Press \"F\" to interact";
                        }
                    }
                    break;
            }
        }

        public void CastFireAOE()
        {
            Rectangle fireAOE = new Rectangle((int) position.X - 100, (int) position.Y - 70, 200 + width, 70 + height);
            for (int i = 0; i < Level.Current.gameObjects.Count; i++)
            {
                GameObject element = Level.Current.gameObjects[i];

                if(element.name == "ice")
                {
                    if(CollisionAABB(fireAOE, new Rectangle((int)element.position.X, (int)element.position.Y, element.width, element.height)))
                    {
                        element.name = "ground";
                    }
                }
            }
        }
        
        public void Attack(GameTime gameTime)
        {
           
            attackBox.X = (int)position.X;
            attackBox.Y = (int)position.Y;
            if (entityFacing == 1)
            {
                attackBox.Location += new Point(width / 2, height / 2);
            }
            else
            {
                attackBox.Location += new Point(width / 2 - attackBox.Width, height / 2);
            }

            
            
            for (int i = 0; i < Level.Current.enemyObjects.Count; i++)
            {
                
                Rectangle hurtbox = Level.Current.enemyObjects[i].hurtBox;
                if (CollisionAABB(attackBox, hurtbox))
                {
                    DamageEnemies(Level.Current.enemyObjects[i], gameTime);
                    Level.Current.enemyObjects[i].enemyHit = true;
                    Level.Current.enemyObjects[i].hitTimer = 10;
                }
            }
        }
        public void DamageEnemies(Enemy victim, GameTime gameTime)
        {
            victim.health -= damage - victim.armor;

            if (victim.health <= 0)
            {
                if (victim.name == "Ina" || victim.name == "Yinyin" || victim.name == "Aiye" || victim.name == "Monomono")
                {
                    Level.Current.nscObjects[0].startConversation(gameTime);
                    switch (victim.name)
                    {
                        case "Ina":
                            Level.soulsRescued[0] = true;
                            break;
                        case "Yinyin":
                            Level.soulsRescued[1] = true;
                            break;
                        case "Aiye":
                            Level.soulsRescued[2] = true;
                            break;
                        case "Monomono":
                            Level.soulsRescued[3] = true;
                            break;
                    }
                    Level.Current.loadLevel("Hub");
                }
            }
        }
        private void collect(string item)
        {
            switch (item)
            {
                case "Coin":
                    money += 100;
                    break;
                case "HPFlower":
                    if (health + 1000 <= maxHealth)
                    {
                        health += 1000;
                    }
                    else
                    {
                        health = maxHealth;
                    }
                    break;
                case "MPFlower":
                    if (mana + 250 <= maxMana)
                    {
                        mana += 250;
                    }
                    else
                    {
                        mana = maxMana;
                    }
                    break;
            }
        }

        private void areaTrigger(string activatedArea)
        {
            switch (activatedArea)
            {
                case "lava":
                    gameOver();
                    break;
                case "ice":
                    onIce = true;
                    break;
            }
        }

        public float slideValue(GameTime gameTime)
        {
            float slideAmount = speed * (float)gameTime.ElapsedGameTime.TotalSeconds * ((float)slide / 1000.0f);
            slide -= gameTime.ElapsedGameTime.Milliseconds;
            return slideAmount;
        }

        private void loader(string newLevel)
        {
            switch (newLevel)
            {
                case "BossstageLoader":
                    Level.Current.loadLevel("Bossstage");
                    States.CurrentBGMState = BGMStates.LEVEL;
                    break;
                case "ErdLevelLoader":
                    Level.Current.loadLevel("ErdLevel");
                    States.CurrentBGMState = BGMStates.LEVEL;
                    break;
                case "EisLevelLoader":
                    Level.Current.loadLevel("EisLevel");
                    States.CurrentBGMState = BGMStates.LEVEL;
                    break;
                case "FeuerLevelLoader":
                    Level.Current.loadLevel("FeuerLevel");
                    States.CurrentBGMState = BGMStates.LEVEL;
                    break;
                case "BlitzLevelLoader":
                    Level.Current.loadLevel("BlitzLevel");
                    States.CurrentBGMState = BGMStates.LEVEL;
                    break;
                case "OverworldLoader":
                    Level.Current.loadLevel("Overworld");
                    States.CurrentBGMState = BGMStates.HUB;
                    break;
                case "HubLoader":
                    Level.Current.loadLevel("Hub");
                    States.CurrentBGMState = BGMStates.HUB;
                    break;
            }
        }
        private void totem(GameObject activatedTrigger)
        {

            Antagonist.Ryn.health -= 5000;
            Antagonist.Ryn.HurtRyn();
            Level.Current.gameObjects.Remove(activatedTrigger);

        }

        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Level.Current.loadLevel("Hub");
            health = maxHealth;
        }

        public void activateTrigger(GameTime gameTime)
        {
            GameObject[] collidedObjects = Collision<GameObject>(Level.Current.gameObjects.ToArray(), position);
            NSC[] collidedNSCs = Collision<NSC>(Level.Current.gameObjects.ToArray(), position);

            //add all collided NSCs to GameObjects
            collidedObjects = collidedObjects.Concat(collidedNSCs).ToArray();

            foreach (GameObject element in collidedObjects)
            {
                if (element.triggerType != null)
                {
                    trigger(element, gameTime);
                }
            }

            trigger(Antagonist.Ryn, gameTime);

            interact = false;
        }

        
    }
}
