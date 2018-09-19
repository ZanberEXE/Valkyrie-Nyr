using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Valkyrie_Nyr
{
    

    class Player : Entity
    {

        private static Player nyr;
       

        public float speed;
        public float jumpHeight;
        public bool onIce;
        public bool inHub;
        public bool interact;
        public bool inJump;

        

        public Player(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, triggerType, mass, height, width, position, hp, dmg)
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
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Dance"), 10, 63, 625),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Falling"), 10, 2, 12),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Landing"), 10, 3, 25),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Stop"), 10, 4, 31),
                new animation(Game1.Ressources.Load<Texture2D>("newPlayer/Crouch"), 10, 3, 25)
            };
            onIce = false;
            health = hp;
            damage = dmg;
            hitbox = new GameObject(name, "hitbox", 0, 20, 100, new Vector2(60, 100));
        }

        //get Nyr from everywhere
        public static Player Nyr { get { if (nyr == null) { nyr = new Player("Nyr", null, 10, 180, 120, Vector2.Zero, 2000, 200); } return nyr; } }


        public void changeState()
        {
            entitystates = (int)States.CurrentPlayerState;
        }

        //put here stuff that happens if you collect something
        public void trigger(GameObject activatedTrigger)
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
                    break;
            }
        }

        private void collect(string item)
        {
            switch (item)
            {
                case "health":
                    this.health += 10;
                    break;
            }
        }

        private void areaTrigger(string activatedArea)
        {
            switch (activatedArea)
            {
                case "lava":
                    States.CurrentPlayerState = Playerstates.DEAD;
                    break;
                case "ice":
                    onIce = true;
                    break;
            }
        }

        private void loader(string newLevel)
        {
            switch (newLevel)
            {
                case "BossstageLoader":
                    Level.Current.loadLevel("Bossstage");
                    break;
                case "ErdLevelLoader":
                    Level.Current.loadLevel("ErdLevel");
                    break;
                case "EisLevelLoader":
                    Level.Current.loadLevel("EisLevel");
                    break;
                case "FeuerLevelLoader":
                    Level.Current.loadLevel("FeuerLevel");
                    break;
                case "BlitzLevelLoader":
                    Level.Current.loadLevel("BlitzLevel");
                    break;
                case "OverworldLoader":
                    Level.Current.loadLevel("Overworld");
                    break;
                case "HubLoader":
                    Level.Current.loadLevel("Hub");
                    break;
            }
        }

        //this method is called, if the Player dies/falls out of the world
        public void gameOver()
        {
            Level.Current.loadLevel("Hub");
        }

        public void activateTrigger()
        {
            GameObject[] collidedObjects = Collision<GameObject>(Level.Current.gameObjects.ToArray(), position);

            foreach (GameObject element in collidedObjects)
            {
                if (element.triggerType != null)
                {
                    trigger(element);
                }
            }
            interact = false;
        }

        //moves the Player
        public void move(Vector2 moveValue)
        {
            Vector2 newPos = position + moveValue;

            position = newPos;
        }

        

        public void Draw(SpriteBatch spriteBatch)
        {
            
           
        }
    }
}
