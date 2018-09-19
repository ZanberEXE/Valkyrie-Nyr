using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valkyrie_Nyr
{
    class GameObject
    {
        //Constants
        public string name;
        public string triggerType;
        public int mass;
        public int height;
        public int width;
        public int gravitation;

        //Variables
        public Vector2 position;
        public float gravValue;
        public bool onGround;
        public Texture2D sprite;

        
        //Constructor
        public GameObject(string _name, string _triggerType, int _mass, int _height, int _width, Vector2 _position)
        {
            onGround = false;
            gravValue = 1;
            gravitation = 1;
            name = _name;
            triggerType = _triggerType;
            mass = _mass;
            height = _height;
            width = _width;
            position = _position;
            
        }

        public void init()
        {
            string[] allAssets = Directory.GetFiles(Game1.Ressources.RootDirectory);
            if (Array.IndexOf(allAssets, "Content\\" + this.name + ".xnb") > -1)
            {
                sprite = Game1.Ressources.Load<Texture2D>(this.name);
            }
        }

        //Check Collision of Objects and returns Array with all collided Objects or null, if none has collided
        //public GameObject[] Collision(GameObject[] gameObjects, Vector2 newPos)
        public T[] Collision<T>(GameObject[] gameObjects, Vector2 newPos)
        {
            List<T> result = new List<T>();

            foreach (GameObject element in gameObjects)
            {
                if(element.GetType() != typeof(T))
                {
                    continue;
                }

                //TODO: not collided with top if platform

                //Create two Rectangles and 4 Corners for each
                //Then test, if any of the Corners is inside the other Rectangle
                Rectangle newPosRect = new Rectangle((int)newPos.X, (int)newPos.Y, width, height);
                Point newPosCorner1 = new Point((int)newPos.X, (int)newPos.Y);                                  //Top-Left Corner
                Point newPosCorner2 = new Point((int)newPos.X + width, (int)newPos.Y);                     //Top-Right Corner
                Point newPosCorner3 = new Point((int)newPos.X + width, (int)newPos.Y + height);       //Bottom-Right Corner
                Point newPosCorner4 = new Point((int)newPos.X, (int)newPos.Y + height);                    //Bottom-Left Corner

                Rectangle collider = new Rectangle((int)element.position.X, (int)element.position.Y, element.width, element.height);
                Point colliderCorner1 = new Point((int)element.position.X, (int)element.position.Y);                                  //Top-Left Corner
                Point colliderCorner2 = new Point((int)element.position.X + element.width, (int)element.position.Y);                     //Top-Right Corner
                Point colliderCorner3 = new Point((int)element.position.X + element.width, (int)element.position.Y + element.height);       //Bottom-Right Corner
                Point colliderCorner4 = new Point((int)element.position.X, (int)element.position.Y + element.height);                    //Bottom-Left Corner
                
                int i = collider.Left;
                
                //Not collided with itself
                if (element != this)
                {
                    if (collider.Contains(newPosCorner1) || collider.Contains(newPosCorner2) || collider.Contains(newPosCorner3) || collider.Contains(newPosCorner4))
                    {
                        result.Add((T) Convert.ChangeType(element, typeof(T)));
                    }
                    else if(newPosRect.Contains(colliderCorner1) || newPosRect.Contains(colliderCorner2) || newPosRect.Contains(colliderCorner3) || newPosRect.Contains(colliderCorner4))
                    {
                        result.Add((T) Convert.ChangeType(element, typeof(T)));
                    }
                }
            }

            return result.ToArray();
        }

        //let obejct fall an return position, that is actually accessable
        public Vector2 Fall(GameTime gameTime, GameObject[] gameObjects)
        {
            Vector2 newPosition = new Vector2(position.X, position.Y + mass * gravitation * gravValue * (float)gameTime.ElapsedGameTime.TotalSeconds);
            GameObject[] collidedObjects = Collision<GameObject>(gameObjects, newPosition); //all Objects, that you would Collide with if you'd fall

            bool thisIsInsideElementX;
            bool elementIsInsideThisX;
            bool thisIsOnTopOfElement;

            //List of grounds, so you always land at the highest Platform, when you fall
            List<int> grounds = new List<int>();

            foreach (GameObject element in collidedObjects)
            {
                if (element.name == "ground" || element.name == "platform")
                {
                    thisIsInsideElementX = this.position.X + this.width > element.position.X && this.position.X < element.position.X + element.width;
                    elementIsInsideThisX = this.position.X + this.width > element.position.X + element.width && this.position.X < element.position.X;
                    thisIsOnTopOfElement = this.position.Y + this.height < element.position.Y + 1;

                    if ((thisIsInsideElementX || elementIsInsideThisX) && thisIsOnTopOfElement)
                    {
                        gravValue = 1;
                        onGround = true;
                        grounds.Add((int)element.position.Y);
                    }
                }
            }

            if (grounds.Count > 0)
            {
                return new Vector2(this.position.X, grounds.Min() - this.height);
            }

            onGround = false;
            gravValue += 2.5f;
            return newPosition;
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            if(sprite == null)
            {
                return;
            }

            spriteBatch.Draw(sprite, new Rectangle(position.ToPoint(), new Point(width, height)), Color.White);
        }
    }
}
