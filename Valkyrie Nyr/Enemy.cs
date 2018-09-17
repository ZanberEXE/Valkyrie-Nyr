using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valkyrie_Nyr
{
    class Enemy : Entity
    {
        
        public Enemy(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, triggerType, mass, height, width, position, hp, dmg)
        {
          
        }
    }
}
