﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    class Projectile : GameObject
    {

        float currentFrame;
        Vector2 aim;
        int damage;
        Texture2D spritesheet;
        int speed;
        bool pierce;
        public Rectangle attackbox;
        bool hasAnimation;
        int maxFrames;
        int framesPerRow;
        int framesPerSecond;
        public Vector2 attackBoxOffset;

        public Projectile(string name, int height, int width, Vector2 position, Vector2 _aim, int _speed, bool _pierce, Rectangle _attackBox, bool _hasAnimation, int _maxFrames, int _framesPerRow, int _damage):base(name, "", 0, height, width, position)
        {
            damage = _damage;
            aim = _aim;
            speed = _speed;
            pierce = _pierce;
            attackbox = _attackBox;
            maxFrames = _maxFrames;
            framesPerRow = _framesPerRow;
            hasAnimation = _hasAnimation;
            framesPerSecond = 40;
            attackBoxOffset = _attackBox.Location.ToVector2();


            string pathToSpriteSheet = Game1.Ressources.RootDirectory + "\\Projectiles\\" + name + ".xnb";
            if (File.Exists(pathToSpriteSheet))
            {
                spritesheet = Game1.Ressources.Load<Texture2D>("Projectiles/" + name);
            }
            currentFrame = 0;

            Level.Current.projectileObjects.Add(this);
        }

        private void Move(float gameTimeInTotalSeconds)
        {
            position += aim * speed * gameTimeInTotalSeconds;
        }

        public void Update(GameTime gameTime)
        {
            attackbox.Location = (position + attackBoxOffset).ToPoint();
            //move it, move it
            if (speed > 0)
            {
                Move((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            //destroy if outside of the world
            if (OutOfworld() || (!pierce && collidedWithMap()))
            {
                Destroy();
                return;
            }

            //check if Nyr is hitted
            if(Player.Nyr.CollisionAABB(Player.Nyr.hurtBox, attackbox))
            {
                HurtNyr();

                if (!pierce)
                {
                    Destroy();
                    return;
                }
            }

            //only animationStuff
            if (!hasAnimation)
            {
                return;
            }
            if (currentFrame >= maxFrames)
            {
                currentFrame = 0;
            }
            else
            {
                currentFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * framesPerSecond;
            }
        }

        private bool collidedWithMap()
        {
            GameObject newAttackBox = new GameObject("", "", 0, attackbox.Height, attackbox.Width, attackbox.Location.ToVector2());
            GameObject[] hittedWalls = newAttackBox.Collision<GameObject>(Level.Current.gameObjects.ToArray(), newAttackBox.position);

            for(int i = 0; i < hittedWalls.Length; i++)
            {
                if(hittedWalls[i].triggerType == null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool OutOfworld()
        {
            Rectangle worldRect = new Rectangle((-Camera.Main.position).ToPoint(), Camera.Main.levelBounds.Size);

            if (worldRect.Contains(attackbox))
            {
                return false;
            }

            return true;
        }

        private void HurtNyr()
        {
            Player.Nyr.health -= damage - Player.Nyr.armor;
        }

        private void Destroy()
        {
            Level.Current.projectileObjects.Remove(this);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (hasAnimation)
            {
                int row = (int)currentFrame / framesPerRow;
                int column = (int)currentFrame - (row * framesPerRow);
                spriteBatch.Draw(spritesheet, new Rectangle(position.ToPoint(), new Point(width, height)), new Rectangle(width * column, height * row, width, height), Color.White, (float)System.Math.Atan2(aim.X, aim.Y), new Vector2(width / 2, height / 2), SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(spritesheet, new Rectangle(position.ToPoint(), new Point(width, height)), new Rectangle(0, 0, width, height), Color.White, (float)System.Math.Atan2(-aim.X, aim.Y), new Vector2(width / 2, height / 2), SpriteEffects.None, 0.0f);
            }
        }
    }
}