using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Valkyrie_Nyr
{
    class GameObject
    {
        //Constants
        public readonly string type;
        public readonly bool isStationary;
        public readonly bool isTrigger;
        public readonly int mass;
        public readonly int height;
        public readonly int width;
        public readonly int gravitation;

        //Variables
        public Vector2 position;
        public int gravValue;

        //Constructor
        public GameObject(string _name, bool _isStationary, bool _isTrigger, int _mass, int _height, int _width, Vector2 _position)
        {
            type = _name;
            isStationary = _isStationary;
            isTrigger = _isTrigger;
            mass = _mass;
            gravitation = 1;
            gravValue = 3;
            height = _height;
            width = _width;
            position = _position;
        }

        //Check Collision of Objects and returns Array with all collided Objects or null, if none has collided
        public GameObject[] Collision(GameObject[] gameObjects, Vector2 newPos)
        {
            List<GameObject> result = new List<GameObject>();

            foreach (GameObject element in gameObjects)
            {

                //TODO: not collided with top if platform

                //Create two Rectangles and 4 Corners for each
                //Then test, if any of the Corners is inside the other Rectangle
                Rectangle newPosRect = new Rectangle((int)newPos.X, (int)newPos.Y, this.width, this.height);
                Point newPosCorner1 = new Point((int)newPos.X, (int)newPos.Y);                                  //Top-Left Corner
                Point newPosCorner2 = new Point((int)newPos.X + this.width, (int)newPos.Y);                     //Top-Right Corner
                Point newPosCorner3 = new Point((int)newPos.X + this.width, (int)newPos.Y + this.height);       //Bottom-Right Corner
                Point newPosCorner4 = new Point((int)newPos.X, (int)newPos.Y + this.height);                    //Bottom-Left Corner

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
                        result.Add(element);
                    }
                    else if(newPosRect.Contains(colliderCorner1) || newPosRect.Contains(colliderCorner2) || newPosRect.Contains(colliderCorner3) || newPosRect.Contains(colliderCorner4))
                    {
                        result.Add(element);
                    }
                }
            }

            return result.ToArray();
        }

        //let the gameObject fall or not, if collided with the ground
        public void Fall(GameTime gameTime, GameObject[] gameObjects)
        {
            if(!this.isStationary)
            {
                Vector2 newPosition = new Vector2(this.position.X, this.position.Y + this.mass * this.gravitation * this.gravValue * (float)gameTime.ElapsedGameTime.TotalSeconds);
                GameObject[] collidedObjects = Collision(gameObjects, newPosition); //all Objects, that you would Collide with if you'd fall
                
                foreach (GameObject element in collidedObjects)
                {
                    if (!element.isTrigger)
                    {
                        this.gravValue = 1;
                        return;
                    }
                }
                this.position = newPosition;
                this.gravValue++;
            }
        }
    }
}
