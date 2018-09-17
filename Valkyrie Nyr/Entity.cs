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
        public int health;
        int damage;


        private GameObject hitbox;

        public Entity(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, triggerType, mass, height, width, position)
        {
            health = hp;
            damage = dmg;
            hitbox = new GameObject(name, "hitbox", 0, 20, 100, new Vector2(60, 100));
        }
            
        public void attack(int lookPos)
        {
            // turns the hitbox to the left of the character
            if(lookPos == -1 && hitbox.position.X > 0)
            {
                hitbox.position.X = hitbox.position.X * -1;
            }
            // turn the hitbos to the right of the character
            if (lookPos == 1 && hitbox.position.X < 0)
            {
                hitbox.position.X = hitbox.position.X * -1;
            }

            GameObject[] hittetObjects = Collision<GameObject>(Level.Current.gameObjects.ToArray(), hitbox.position + this.position);
            List<Entity> hittetEntitys = new List<Entity>();

            if (hittetObjects.Length == 0)
            {
                return;
            }
            foreach(GameObject element in hittetObjects)
            {
                for (int i = 0; i < Level.Current.entityObjects.Count(); i++)
                {
                    if(Level.Current.entityObjects[i].Equals(element))
                    {
                        hittetEntitys.Add(Level.Current.entityObjects[i]);
                    }
                }
            }
            foreach(Entity element in hittetEntitys)
            {
                element.health -= this.damage;
                if(element.health <= 0)
                {
                    Level.Current.entityObjects.Remove(element);
                    Level.Current.gameObjects.Remove(element);
                }
            }
        }
        
        public void entityRender(GameTime gametime, SpriteBatch spriteBatch)
        {
            /*
             * if (sprite == null)
            {
                    sprite = Game1.Ressources.Load<Texture2D>(name);
            }

            spriteBatch.Draw(sprite, new Rectangle(position.ToPoint(), new Point(width, height)), Color.White);
            */

            Texture2D pxl = Game1.Ressources.Load<Texture2D>("index");
            // draw hitbox
            spriteBatch.Draw(pxl, new Rectangle((int)hitbox.position.X + (int)this.position.X, (int)hitbox.position.Y + (int)this.position.Y, hitbox.width, hitbox.height), Color.BlueViolet * 0.5f);
        }

      
    }
}
