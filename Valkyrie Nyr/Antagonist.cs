using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    class Antagonist : Enemy
    {
        private static Antagonist rynPlaceholder;

        public static Antagonist Ryn { get { if (rynPlaceholder == null) { rynPlaceholder = new Antagonist(); } return rynPlaceholder; } }

        int currentLocation = 0;
        Vector2[] locations;

        Vector2[] sizeInAnim;

        NSC Dialogues;

        public Antagonist() : base("Ryn", "ryn", 7, 200, 200, new Vector2(0, 0), 20000, 0, 200, 0, 0, 0, 0, false)
        {
            animTex = new animation[]
            {
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynIdle"), 10, 5, 50),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynMove"), 10, 5, 50),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynMoveCry"), 10, 5, 50),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynCry"), 10, 4, 37),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynIdleAura"), 10, 4, 37),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynDying"), 10, 7, 62),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynIsDead"), 10, 2, 12)
            };

            sizeInAnim = new Vector2[]
             {
                new Vector2(800, 2375) * 4,
                new Vector2(3000, 2450) * 4,
                new Vector2(2400, 1350) * 4,
                new Vector2(3580, 670) * 4,
                new Vector2(5450, 670) * 4,
                new Vector2(6250, 2100) * 4,
                new Vector2(6250, 2100) * 4
             };

            Dialogues = new NSC(name, "none", mass, height, width, position, health, damage);
            Dialogues.dialogues = new Conversation[]
            {
                new Conversation(new string[]{"Ryn" }, new string[]{"" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"Where am I...?" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"Aaaaah! Don't come near me!" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"Go away! Go away!" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"Why won't you go away?... Stay away!" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"... Uuh... No!" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"Don't come... please... *sobs* I... Sis... *sobs*" }),
                new Conversation(new string[]{"Ryn" }, new string[]{"..." })
            };

            locations = new Vector2[]
            {
                new Vector2(800, 2325) * 4,
                new Vector2(3000, 2200) * 4,
                new Vector2(2400, 1300) * 4,
                new Vector2(3580, 620) * 4,
                new Vector2(5450, 620) * 4,
                new Vector2(6250, 2050) * 4,
                new Vector2(6250, 2050) * 4
            };

            currentLocation = 0;
            position = locations[currentLocation] - Camera.Main.position;

            hurtBox.X = (int)position.X;
            hurtBox.Y = (int)position.Y;
            hurtBox.Width = width;
            hurtBox.Height = height;

            Level.Current.enemyObjects.Add(this);
        }

        public void Reset ()
        {
            rynPlaceholder = new Antagonist();
        }

        new public void Update(GameTime gameTime)
        {
            base.EntityUpdate(gameTime);

            position = Fall(gameTime, Level.Current.gameObjects.ToArray());
            
            if (NyrBy(aggroRange))
            {
                SetNewPosition();
            }
        }

        public void Kill()
        {
            currentEntityState = 5;
            nextEntityState = 6;
        }

        public void HurtRyn()
        {

            return;
            Dialogues.dialogueState = new Random().Next(Dialogues.dialogues.Length - 1);
            Dialogues.startConversation();
        }

        public void SetNewPosition()
        {
            if(currentLocation == locations.Length - 1)
            {
                return;
            }

            currentLocation++;
            Dialogues.dialogueState = currentLocation;
            Dialogues.startConversation();
            position = locations[currentLocation] - Camera.Main.position;
            
            setNewAnim();
        }

        private void setNewAnim()
        {
            if (currentLocation < 4)
            {
                currentEntityState = currentLocation;
                nextEntityState = currentLocation;
            }
            else
            {
                currentEntityState = 4;
                nextEntityState = 4;
            }
        }
    }
}
