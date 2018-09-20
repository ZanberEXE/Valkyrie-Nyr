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
        public int speed;

        bool wasInAggroRange = false;
        bool wasInAttackRange = false;
        bool isAttacking = false;
        bool hasAttacked = false;
        
        public Enemy(string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg, int _aggroRange, int _attackRange, int _speed) : base(name, triggerType, mass, height, width, position, hp, dmg)
        {
            aggroRange = _aggroRange;
            attackRange = _attackRange;
            speed = _speed;
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

            base.EntityUpdate(gameTime);


            if (NyrBy(attackRange))
            {
                isAttacking = true;

            }

            if (isAttacking && currentEntityState != (int)Enemystates.IDLE)
            {
                if (wasInAggroRange == false)
                {
                    currentFrame = 0;
                    wasInAggroRange = true;
                }
                currentEntityState = (int)Enemystates.ATTACK;
                Attack(entityFacing);
                nextEntityState = (int)Enemystates.IDLE;

                hasAttacked = true;

            }
            if (hasAttacked && currentEntityState == (int)Enemystates.IDLE)
            {
                if(name == "Banshee")
                {
                    position = new Vector2(20000, 20000);
                }
                
            }

            if (NyrBy(aggroRange) && !NyrBy(attackRange) && currentEntityState != (int)Enemystates.ATTACK)
            {
                if (wasInAggroRange == false)
                {
                    currentFrame = 0;
                    wasInAggroRange = true;
                }

                if (currentEntityState != (int)Enemystates.WALK)
                {
                    currentEntityState = (int)Enemystates.AGGRO;
                    nextEntityState = (int)Enemystates.WALK;
                }
                
                if (currentEntityState == (int)Enemystates.WALK)
                {

                    if (Player.Nyr.position.X + 40 < this.position.X)
                    {
                        position.X -= ((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        entityFacing = 1;
                    }
                    if (Player.Nyr.position.X + 40 > this.position.X)
                    {
                        position.X += ((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        entityFacing = 0;
                    }
                    if (name == "Banshee")
                    {

                        if (Player.Nyr.position.Y + 40 > this.position.Y)
                        {
                            position.Y += ((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        }
                        if (Player.Nyr.position.Y + 40 < this.position.Y)
                        {
                            position.Y -= ((1 * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        }

                    }
                }
                
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
