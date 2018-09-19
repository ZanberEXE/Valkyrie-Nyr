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
        /// <summary>
        /// ////////////////// DIE UPDATE FUNKTION WIRD NIE AUFGERUFEN ABER ICH HABE KEINE AHNUNG WIE ICH DIE AUFRUFE!!!!! :'/
        /// </summary>
        public void update()
        {

            

            if (nyrInReach(200))
            {
                currentFrame = 0;
                entitystates = (int)Enemystates.AGGRO;
           
            }
            if (nyrInReach(50))
            {
                entitystates = (int)Enemystates.ATTACK;
                currentFrame = 0;
            }
        }
        private bool nyrInReach(int senseRadius)
        {
            if(Vector2.Distance(this.position, Player.Nyr.position) <= senseRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void React()
        {

        }
    }
}
