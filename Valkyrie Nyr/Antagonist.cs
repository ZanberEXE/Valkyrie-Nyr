using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        Enemy[] enemyTemplates;

        int stageEnemiesAtBeginning = 0;
        
        private double timeOfLastEnemySpawn;

        bool gameFinisched = false;
        public bool falseEnding = false;
        public int endingTimer = 0;

        public Antagonist() : base("Ryn", "ryn", 7, 150, 200, new Vector2(0, 0), 20000, 0, 200, 0, 0, 0, 0, false)
        {
            animTex = new animation[]
            {
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynIdle"), 10, 5, 50),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynMove"), 10, 4, 31),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynCry"), 10, 7, 62),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynCryAura"), 10, 7, 62),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynStandCry"), 3, 1, 3),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynDying"), 10, 8, 75),
                new animation(Game1.Ressources.Load<Texture2D>("Bosses/Ryn/RynIsDead"), 10, 2, 12)
            };

            currentEntityState = 2;
            nextEntityState = 2;

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

            //Banshee, BeeShocking, FireRocky, IceCollossus, Skeleton
            enemyTemplates = JsonConvert.DeserializeObject<List<Enemy>>(File.ReadAllText("Ressources\\json-files\\enemyTemplates.json")).ToArray();

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
                new Vector2(800, 2385) * 4,
                new Vector2(3000, 2445) * 4,
                new Vector2(2400, 1370) * 4,
                new Vector2(3580, 660) * 4,
                new Vector2(5450, 665) * 4,
                new Vector2(6250, 2120) * 4,
                new Vector2(6250, 2123) * 4
            };

            currentLocation = 5;
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
            
            int tmp = currentEntityState;

            base.EntityUpdate(gameTime);

            //position = Fall(gameTime, Level.Current.gameObjects.ToArray());
            if ( currentEntityState <= 5)
            {
                currentEntityState = tmp;
            }
            
            if (NyrBy(aggroRange))
            {
                SetNewPosition();
            }
           
            if (health <= 0 && currentEntityState < 4)
            {
                GameObject escape = new GameObject("FeuerLevelLoader", "loader", 0, 300, 300, Antagonist.Ryn.position + new Vector2(1000, -150));
                escape.init();
                escape.name = "escape";
                Level.Current.gameObjects.Add(escape);

                currentEntityState = 4;
                nextEntityState = 4;
                for (int i = 0; i < Level.Current.enemyObjects.Count();)
                {
                    if ( Level.Current.enemyObjects[i].name != "Ryn")
                    {
                        Level.Current.gameObjects.Remove(Level.Current.enemyObjects[i]);
                        Level.Current.enemyObjects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            else
            //EndFight
            if (currentLocation == locations.Length - 1 && Level.Current.enemyObjects.Count() - stageEnemiesAtBeginning < 10 && currentEntityState < 4)
            {
                if (timeOfLastEnemySpawn >= 25)
                {
                    SpawnEnemy();
                }
                else if (GenerateNumber(0, 300) == 0)
                {
                    SpawnEnemy();
                }
                else
                {
                    timeOfLastEnemySpawn += gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if (gameFinisched == true)
            {
                States.CurrentGameState = GameStates.MAINMENU;
            }
            if (currentEntityState == 6 && gameFinisched == false)
            {
                Dialogues.dialogueState = Dialogues.dialogues.Length - 1;
                Dialogues.startConversation();
                gameFinisched = true;

            }

            if (endingTimer == 1)
            {
                Player.Nyr.health -= 100000;
                Player.Nyr.finallyDead = true;
            }
            if ( endingTimer != 0)
            {
                endingTimer--;
            }
            

        }

        public void SpawnEnemy()
        {
            Enemy newEnemy = enemyTemplates[GenerateNumber(0, 5)].Clone() as Enemy;

            newEnemy.Initialize();

            newEnemy.position = this.position - new Vector2(GenerateNumber(-3000, 3000), newEnemy.height);

            Level.Current.enemyObjects.Add(newEnemy);
            Level.Current.gameObjects.Add(newEnemy);

            timeOfLastEnemySpawn = 0;
        }

       public void Kill()
        {
            if ( nextEntityState == 6)
            {
                return;
            }
            currentEntityState = 5;
            nextEntityState = 6;
        }

        public void HurtRyn()
        {
            return;
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
            
            if(currentLocation == locations.Length - 1)
            {
                currentEntityState = 3;
                nextEntityState = 3;
                stageEnemiesAtBeginning = Level.Current.enemyObjects.Count();
                SpawnEnemy();
            }
            
        }

       /* private void setNewAnim()
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
        }*/
    }
}
