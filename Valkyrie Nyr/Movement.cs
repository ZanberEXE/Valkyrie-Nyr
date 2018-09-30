using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    static class Movement
    {
        static public List<GameObject> allCollidedGameObjects;

        static private Rectangle collidedGameObjectTop;
        static private Rectangle collidedGameObjectRight;
        static private Rectangle collidedGameObjectBottom;
        static private Rectangle collidedGameObjectLeft;

        static public Vector2 Update(Vector2 moveValue)
        {
            //Set Variables Null
            collidedGameObjectTop = Rectangle.Empty;
            collidedGameObjectRight = Rectangle.Empty;
            collidedGameObjectBottom = Rectangle.Empty;
            collidedGameObjectLeft = Rectangle.Empty;
            Vector2 newPlayerPosition = Player.Nyr.position + moveValue;
            allCollidedGameObjects = new List<GameObject>();

            Collision(newPlayerPosition);

            //Set new MoveValue to collidedObject
            if (collidedGameObjectTop != Rectangle.Empty)
            {
                moveValue.Y = collidedGameObjectTop.Bottom - Player.Nyr.position.Y;
            }
            if (collidedGameObjectRight != Rectangle.Empty)
            {
                moveValue.X = collidedGameObjectRight.Left - (Player.Nyr.position.X + Player.Nyr.width);
            }
            if (collidedGameObjectBottom != Rectangle.Empty)
            {
                moveValue.Y = collidedGameObjectBottom.Top - (Player.Nyr.position.Y + Player.Nyr.height);
            }
            if (collidedGameObjectLeft != Rectangle.Empty)
            {
                moveValue.X = collidedGameObjectLeft.Right - Player.Nyr.position.X;
            }

            return moveValue;

        }

        static private void Collision(Vector2 newPlayerPos)
        {
            List<Rectangle> collidedTop = new List<Rectangle>();
            List<Rectangle> collidedRight = new List<Rectangle>();
            List<Rectangle> collidedBottom = new List<Rectangle>();
            List<Rectangle> collidedLeft = new List<Rectangle>();

            Rectangle playerRect = new Rectangle((int)newPlayerPos.X, (int)newPlayerPos.Y, Player.Nyr.width, Player.Nyr.height);

            for (int i = 0; i < Level.Current.gameObjects.Count; i++)
            {
                GameObject element = Level.Current.gameObjects[i];
                Rectangle elementRect = new Rectangle((int)element.position.X, (int)element.position.Y, element.width, element.height);

                if (Entity.CollisionAABB(playerRect, elementRect))
                {
                    allCollidedGameObjects.Add(element);

                    if (element.triggerType == null)
                    {
                        //check where the collision is
                        //Top
                        if (elementRect.Bottom >= playerRect.Top && elementRect.Bottom <= (int)Player.Nyr.position.Y)
                        {
                            if (element.name != "platform")
                            {
                                collidedTop.Add(elementRect);
                            }
                        }
                        //Right
                        if (elementRect.Left <= playerRect.Right && elementRect.Left >= (int)Player.Nyr.position.X + Player.Nyr.width)
                        {
                            if (element.name != "platform")
                            {
                                collidedRight.Add(elementRect);
                            }
                        }
                        //Bottom
                        if (elementRect.Top <= playerRect.Bottom && elementRect.Top >= (int)Player.Nyr.position.Y + Player.Nyr.height)
                        {
                            if (Player.Nyr.inStomp)
                            {
                                if (element.name == "breakableGround")
                                {
                                    Level.Current.gameObjects.Remove(element);
                                    i--;
                                    continue;
                                }
                                else
                                {
                                    Player.Nyr.inStomp = false;
                                }
                            }
                            collidedBottom.Add(elementRect);
                            Player.Nyr.gravValue = 1;
                            Player.Nyr.onGround = true;
                        }
                        //Left
                        if (elementRect.Right >= playerRect.Left && elementRect.Right <= (int)Player.Nyr.position.X)
                        {
                            if (element.name != "platform")
                            {
                                collidedLeft.Add(elementRect);
                            }
                        }
                    }
                }
            }

            GetNearestCollision(collidedTop.ToArray(), collidedRight.ToArray(), collidedBottom.ToArray(), collidedLeft.ToArray());
        }

        static private void GetNearestCollision(Rectangle[] allCollidedTop, Rectangle[] allCollidedRight, Rectangle[] allCollidedBottom, Rectangle[] allCollidedLeft)
        {

            //get nearest TopCollision
            for (int i = 0; i < allCollidedTop.Length; i++)
            {
                if (collidedGameObjectTop == Rectangle.Empty || collidedGameObjectTop.Bottom < allCollidedTop[i].Bottom)
                {
                    collidedGameObjectTop = allCollidedTop[i];
                }
            }

            //get nearest RightCollision
            for (int i = 0; i < allCollidedRight.Length; i++)
            {
                if (collidedGameObjectRight == Rectangle.Empty || collidedGameObjectRight.Left > allCollidedRight[i].Left)
                {
                    collidedGameObjectRight = allCollidedRight[i];
                }
            }

            //get nearest BottomCollision
            for (int i = 0; i < allCollidedBottom.Length; i++)
            {
                if (collidedGameObjectBottom == Rectangle.Empty || collidedGameObjectBottom.Top > allCollidedBottom[i].Top)
                {
                    collidedGameObjectBottom = allCollidedBottom[i];
                }
            }

            //get nearest LeftCollision
            for (int i = 0; i < allCollidedLeft.Length; i++)
            {
                if (collidedGameObjectLeft == Rectangle.Empty || collidedGameObjectLeft.Right < allCollidedLeft[i].Right)
                {
                    collidedGameObjectLeft = allCollidedLeft[i];
                }
            }
        }

        static public float PlayerFall(GameTime gameTime)
        {
            float value = Player.Nyr.mass * (float)gameTime.ElapsedGameTime.TotalSeconds * Player.Nyr.gravValue * Player.Nyr.gravitation;
            Player.Nyr.gravValue += 2.5f;
            Player.Nyr.onGround = false;
            return value;
        }
    }
}
