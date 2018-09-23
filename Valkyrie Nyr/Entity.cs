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
        public bool animationFlip = false;
        public int currentFrame = 0;

        private int totalFrames = 0;
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame = 5;

        public int health;
        public int damage;
        public int armor;

        public int currentEntityState = 0;
        public int nextEntityState = 0;

        public Rectangle attackBox = new Rectangle();
        public Rectangle hurtBox = new Rectangle();

        public int attackBoxWidth;
        public int attackBoxHeight;

        public bool isInvulnerable;
        public int invulnerableTimer;

        public Entity(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, int _attackBoxWidth, int _attackBoxHeight, bool _animationFlip) : base(name, triggerType, mass, height, width, position)
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
            hurtBox.X = (int)position.X;
            hurtBox.Y = (int)position.Y;
            hurtBox.Width = width;
            hurtBox.Height = height;

            attackBox.X = (int)position.X;
            attackBox.Y = (int)position.Y;
            attackBox.Width = attackBoxWidth;
            attackBox.Height = attackBoxHeight;
            
        }
        //moves the Player
        public void Move(Vector2 moveValue)
        {
            Vector2 newPos = position + moveValue;

            position = newPos;
        }
       
        public bool CollisionAABB(Rectangle recA, Rectangle recB)
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
       
        public void MakeInvulnerable()
        {
            isInvulnerable = true;
            invulnerableTimer = 50;
        }
        public void MakeInvulnerable(int timer)
        {
            isInvulnerable = true;
            invulnerableTimer = timer;
        }
        /// <summary>
        /// UPDATE                                                                                                                                          (/(/(/`/p9uanlkdhlfansdlkfha
        /// </summary>
        /// <param name="gameTime"></param>
        public void EntityUpdate(GameTime gameTime)
        {
            if (isInvulnerable)
            {
                invulnerableTimer--;
            }
            if (invulnerableTimer <= 0)
            {
                isInvulnerable = false;
            }
            
            
            
            if (name == "Nyr" && Player.Nyr.isCrouching)
            {
                hurtBox.Y = (int)position.Y + (height /2);
                hurtBox.Height = height /2;
            }
            else
            {
                hurtBox.Height = height;
                hurtBox.Y = (int)position.Y;
            }

            if (name == "Nyr" && Player.Nyr.isCrouching == false)
            {
                hurtBox.Height = height - 20;
                hurtBox.Y = (int)position.Y + 20;
            }

            if (name == "Nyr")
            {
                hurtBox.X = (int)position.X + width / 4;
            }
            else
            {
                hurtBox.X = (int)position.X;
            }

            //hurtBox.Y = (int)position.Y;

            if (name != "Nyr")
            {
                attackBox.X = (int)position.X;
                attackBox.Y = (int)position.Y;
            }
            

            if (name == "Banshee")
            {
                attackBox.X = (int)position.X;
                attackBox.Y = (int)position.Y;
                hurtBox.X += 20;
            }
            if (name == "Yinyin")
            {
                hurtBox.Y = (int)position.Y - 120;
                hurtBox.X = (int)position.X;
                hurtBox.Height = height + 120;

                attackBox.Y = (int)position.Y - 120;
            }
            if (name == "Aiye")
            {
                hurtBox.Y = (int)position.Y + 40;
                hurtBox.X = (int)position.X;
                hurtBox.Height = height - 20;
               
                if (entityFacing == -1)
                {
                    attackBox.Y = (int)position.Y + 100;
                    attackBox.X = (int)position.X - 30;
                }
                if (entityFacing == 1)
                {
                    attackBox.Y = (int)position.Y + 100;
                    attackBox.X = (int)position.X + 90;
                }
            }
            if (name == "Monomono")
            {
                hurtBox.Y = (int)position.Y + 630;
                hurtBox.Height = height - 630;
                if (entityFacing == -1)
                {
                    attackBox.Y = (int)position.Y + 650;
                    attackBox.X = (int)position.X - 30;
                }
                if (entityFacing == 1)
                {
                    attackBox.Y = (int)position.Y + 650;
                    attackBox.X = (int)position.X + 30;
                }
                
            }



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
                else
                {
                    
                }
            }
            if (name == "Nyr")
            {
                if (Player.Nyr.fAttackCheck == 13)
                {
                    Player.Nyr.Attack();
                }
                Player.Nyr.fAttackCheck--;
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

            
            if (animationFlip)
            {
                if (entityFacing == -1)
                {
                    spriteBatch.Draw(animTex[(int)currentEntityState].texture, destinationRectangle, sourceRectangle, Color.White);
                }
                else
                {
                    spriteBatch.Draw(animTex[(int)currentEntityState].texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
                }
            }
            else
            {
                if (entityFacing == 1)
                {
                    spriteBatch.Draw(animTex[(int)currentEntityState].texture, destinationRectangle, sourceRectangle, Color.White);
                }
                else
                {
                    spriteBatch.Draw(animTex[(int)currentEntityState].texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
                }
            }

            Texture2D pxl = Game1.Ressources.Load<Texture2D>("index");
            //draw hitbox
            //spriteBatch.Draw(pxl, new Rectangle((int)hitbox.position.X + (int)this.position.X, (int)hitbox.position.Y + (int)this.position.Y, hitbox.width, hitbox.height), Color.BlueViolet * 0.5f);
         
            
            spriteBatch.Draw(pxl, new Rectangle(hurtBox.X, hurtBox.Y, hurtBox.Width, hurtBox.Height), Color.Red * 0.5f);
            spriteBatch.Draw(pxl, new Rectangle(attackBox.X, attackBox.Y, attackBox.Width, attackBox.Height), Color.BlueViolet * 0.5f);
        }

      
    }
}
