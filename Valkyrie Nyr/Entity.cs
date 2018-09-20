using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valkyrie_Nyr
{
    
    class Entity : GameObject
    {

        public struct animationPrefab
        {
            public string path;
            public int Columns;
            public int Rows;
            public int maxFrames;
            public animationPrefab(string _path, int _columns, int _rows, int _maxFrames)
            {
                path = _path;
                Columns = _columns;
                Rows = _rows;
                maxFrames = _maxFrames;
            }
        }
        protected struct animation
        {
            public Texture2D texture;
            public int Columns;
            public int Rows;
            public int Width;
            public int Height;
            public int maxFrames;
            public animation(Texture2D _texture, int _columns, int _rows, int _maxFrames)
            {
                texture = _texture;
                Columns = _columns;
                Rows = _rows;
                Width = _texture.Width / _columns;
                Height = _texture.Height / _rows;
                maxFrames = _maxFrames;
            }
        }



        protected animation[] animTex;
        public animationPrefab[] animationPrefabs;
        public string[] paths;

        public int Rows { get; set; }
        public int Columns { get; set; }

        public int entityFacing;
        public int currentFrame = 0;

        private int totalFrames = 0;
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame = 5;

        public int health;
        protected int damage;

        public int currentEntityState = 0;
        public int nextEntityState = 0;

        public Rectangle attackBox = new Rectangle();
        public Rectangle hurtBox = new Rectangle();

        public Entity(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, triggerType, mass, height, width, position)
        {
            
            
        }
            
        public void Initialize()
        {
            animTex = new animation[animationPrefabs.Length];
            for (int i = 0; i < animationPrefabs.Length; i++)
            {
                animTex[i].Rows = animationPrefabs[i].Rows;
                animTex[i].Columns = animationPrefabs[i].Columns;
                animTex[i].texture = Game1.Ressources.Load<Texture2D>(animationPrefabs[i].path);
                animTex[i].Width = animTex[i].texture.Width / animTex[i].Columns;
                animTex[i].Height = animTex[i].texture.Height / animTex[i].Rows;
                animTex[i].maxFrames = animationPrefabs[i].maxFrames;
            }
        }

        bool CollisionAABB(Rectangle recA, Rectangle recB)
        {
	        if (
                recA.X + recA.Width >= recB.X &&
		        recB.X + recB.Width >= recA.X &&
		        recA.Y + recA.Height >= recB.Y &&
		        recB.Y + recB.Height >= recA.Y
		    )
	        {
		        return true;
	    }
	
	        return false;
        }

    //moves the Player
    public void Move(Vector2 moveValue)
        {
            Vector2 newPos = position + moveValue;

            position = newPos;
        }
        public void Attack(int lookPos)
        {

            
            // turns the hitbox to the left of the character
            if(lookPos == -1 && attackBox.X > 0)
            {
                attackBox.X = attackBox.X * -1;
            }
            // turn the hitbos to the right of the character
            if (lookPos == 1 && attackBox.X < 0)
            {
                attackBox.X = attackBox.X * -1;
            }

            if (CollisionAABB(attackBox, hurtBox))
            {

            }

            /*
            GameObject[] hittetObjects = Collision<Enemy>(Level.Current.gameObjects.ToArray(), hitbox.position + this.position);
            List<Entity> hittetEntitys = new List<Entity>();

            if (hittetObjects.Length == 0)
            {
                return;
            }
            foreach(GameObject element in hittetObjects)
            {
                for (int i = 0; i < Level.Current.enemyObjects.Count(); i++)
                {
                    if(Level.Current.enemyObjects[i].Equals(element))
                    {
                        hittetEntitys.Add(Level.Current.enemyObjects[i]);
                    }
                }
            }
            foreach(Enemy element in hittetEntitys)
            {
                element.health -= this.damage;
                if(element.health <= 0)
                {
                    Level.Current.enemyObjects.Remove(element);
                    Level.Current.gameObjects.Remove(element);
                }
            }*/
        }
        /// <summary>
        /// UPDATE
        /// </summary>
        /// <param name="gameTime"></param>
        public void EntityUpdate(GameTime gameTime)
        {
            hurtBox.X = (int)position.X;
            hurtBox.Y = (int)position.Y;
            hurtBox.Width = width;
            hurtBox.Height = height;

            attackBox.X = 

            totalFrames = animTex[currentEntityState].maxFrames; // * animTex[(int)States.CurrentPlayerState].Columns;
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;

                currentFrame++;

                timeSinceLastFrame = 0;

                if (currentFrame >= totalFrames)
                {
                    currentFrame = 0;
                    currentEntityState = nextEntityState;
                }
            }
           
            if (health <= 0)
            {
                if (name == "Nyr")
                {
                    Player.Nyr.gameOver();
                }
            }
          
        }

        public void EntityRender(GameTime gametime, SpriteBatch spriteBatch)
        {
            int animWidth = animTex[currentEntityState].Width;
            int animHeight = animTex[currentEntityState].Height;
            int row = (int)((float)currentFrame / animTex[(int)currentEntityState].Columns);
            int column = currentFrame % animTex[(int)currentEntityState].Columns;

            Rectangle sourceRectangle = new Rectangle(animWidth * column, animHeight * row, animWidth, animHeight);
            Rectangle destinationRectangle = new Rectangle((int)position.X - (animWidth / 2) + (width / 2), (int)position.Y - (animWidth / 2) + 32, animWidth, animHeight);

            if (entityFacing == 1)
            {
                spriteBatch.Draw(animTex[(int)currentEntityState].texture, destinationRectangle, sourceRectangle, Color.White);
            }
            else
            {
                //destinationRectangle.X = destinationRectangle.X - (sourceRectangle.Width - this.width);
                spriteBatch.Draw(animTex[(int)currentEntityState].texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
            }
            
            Texture2D pxl = Game1.Ressources.Load<Texture2D>("index");
            //draw hitbox
            //spriteBatch.Draw(pxl, new Rectangle((int)hitbox.position.X + (int)this.position.X, (int)hitbox.position.Y + (int)this.position.Y, hitbox.width, hitbox.height), Color.BlueViolet * 0.5f);
            spriteBatch.Draw(pxl, new Rectangle(hurtBox.X, hurtBox.Y, hurtBox.Width, hurtBox.Height), Color.BlueViolet * 0.5f);
        }

      
    }
}
