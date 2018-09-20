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

        public int aggroRange;
        public int attackRange;
        
        public Enemy(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, int _aggroRange, int _attackRange) : base(name, triggerType, mass, height, width, position, hp, dmg)
        {
            aggroRange = _aggroRange;
            attackRange = _attackRange;
    }
        /// <summary>
        /// ////////////////// DIE UPDATE FUNKTION WIRD NIE AUFGERUFEN ABER ICH HABE KEINE AHNUNG WIE ICH DIE AUFRUFE!!!!! :'/
        /// 
        /// es müssen noch die json-dateien umbenannt werden (entityObjects -> enemyObjects)
        /// anschließend müssen sie dann noch in visual studio dem ressources ordner hinzugefügt werden und auf "immer kopieren" gestellt werden
        /// 
        /// Ich habe jetzt noch die Animationen von Nyr über die Entitystates laufen lassen
        /// 
        /// Außerdem ist der erkennungsradius der Gegner jetzt in der Json-Datei
        /// </summary>
        public void Update(GameTime gameTime)
        {
            base.entityUpdate(gameTime);            

            if (NyrBy(aggroRange))
            {
                currentFrame = 0;
                currentEntityState = (int)Enemystates.AGGRO;
                nextEntityState = (int)Enemystates.WALK;

            }
            if (NyrBy(attackRange))
            {
                currentEntityState = (int)Enemystates.ATTACK;
                nextEntityState = (int)Enemystates.IDLE;
                currentFrame = 0;
            }
        }
        private bool NyrBy(int senseRadius)
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
