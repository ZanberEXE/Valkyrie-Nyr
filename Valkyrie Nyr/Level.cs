using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Valkyrie_Nyr
{
    class Level
    {
        //get current Level from everywhere
        public static Level Current { get { if (currentLevel == null) { currentLevel = new Level(); } return currentLevel; } }

        //all beaten bosses in this order: Ina (Fire), Yinyin (Ice), Aiye(Earth), Monomono (Blitz)
        public static bool[] soulsRescued = new bool[] { false, false, false, false };
        //all enhanced Armor in this order: Torso (Fire), Guntlet (Ice), Shoes(Earth), Headband (Blitz), WeaponUpgrade, ArmorUpgrade
        public static bool[] armorEnhanced = new bool[] { false, false, false, false, false, false };


        public string name;
        public string textboxText = "";

        public List<GameObject> gameObjects;
        //public List<Entity> entityObjects;
        public List<Enemy> enemyObjects;
        public List<NSC> nscObjects;
        public List<Projectile> projectileObjects;

        public int height;
        public int width;

        int atkCooldown;

        public int dashtimer;
        public int fireAoeTimer;

        public bool hasDashed = false;
        public bool anyKeyPressed = false;
        public Vector2 tempposition;

        public Vector2 positionBGSprite;
        public Vector2 moveValue;

        private static Level currentLevel;

        Texture2D levelBGSprite;
        Texture2D map;

        public Projectile tempEffekt;

        public Keys[] lastPressedKeys;
        public Keys[] newPressedKeys;

        public int timeStop;
        
        //loads the level
        public void loadLevel(string levelName)
        {
            map = Game1.Ressources.Load<Texture2D>("Map");
            Interface.Start();
            name = levelName;

            Point startPosition;
            Player.Nyr.inHub = false;

            nscObjects = JsonConvert.DeserializeObject<List<NSC>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_nscObjects.json"));


            switch (levelName)
            {
                case "Bossstage":
                    width = 7500 * Camera.Main.zoom;
                    height = 2500 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y - 100));
                    Player.Nyr.position = new Vector2(600, Game1.WindowSize.Y / 2);
                    break;
                case "Hub":
                    width = 1125 * Camera.Main.zoom;
                    height = 625 * Camera.Main.zoom;
                    startPosition = new Point(-(width - Game1.WindowSize.X), -(height - Game1.WindowSize.Y - 50));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X - Player.Nyr.width, Game1.WindowSize.Y - Player.Nyr.height);
                    Player.Nyr.inHub = true;
                    //delete souls in Hub, if not rescued yet
                    for (int i = 0; i < nscObjects.Count; i++)
                    {
                        if (nscObjects[i].name == "inaSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.FIRE])
                            {
                                nscObjects.RemoveAt(i);
                                i--;
                            }
                            else if (Level.armorEnhanced[(int)BossElements.FIRE])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "yinyinSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.ICE])
                            {
                                nscObjects.RemoveAt(i);
                                i--;
                            }
                            else if (Level.armorEnhanced[(int)BossElements.ICE])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "aiyeSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.EARTH])
                            {
                                nscObjects.RemoveAt(i);
                                i--;
                            }
                            else if (Level.armorEnhanced[(int)BossElements.EARTH])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        }
                        else if (nscObjects[i].name == "monomonoSoul")
                        {
                            if (!Level.soulsRescued[(int)BossElements.BOLT])
                            {
                                nscObjects.RemoveAt(i);
                                i--;
                            }
                            else if (Level.armorEnhanced[(int)BossElements.BOLT])
                            {
                                nscObjects[i].dialogueState++;
                            }
                        } 
                    }
                    Player.Nyr.inJump = false;
                    break;
                case "Overworld":
                    width = 3000 * Camera.Main.zoom;
                    height = 1000 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(0, Game1.WindowSize.Y / 2);
                    break;
                case "BlitzLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    //startPosition = new Point(-13000, -(height - Game1.WindowSize.Y));
                    startPosition = new Point(-500, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                    //TODO: evtl lötschn
                /*case "FeuerLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(-14000 + Game1.WindowSize.X, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;*/
                case "ErdLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                case "EisLevel":
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(Game1.WindowSize.X / 2, Game1.WindowSize.Y / 2);
                    break;
                default:
                    width = 3750 * Camera.Main.zoom;
                    height = 1250 * Camera.Main.zoom;
                    startPosition = new Point(0, -(height - Game1.WindowSize.Y));
                    Player.Nyr.position = new Vector2(0, Game1.WindowSize.Y / 2);
                    break;
            }
            
            gameObjects = JsonConvert.DeserializeObject<List<GameObject>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_gameObjects.json"));
            
            //entityObjects = JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_entityObjects.json"));
            enemyObjects = JsonConvert.DeserializeObject<List<Enemy>>(File.ReadAllText("Ressources\\json-files\\" + levelName + "_enemyObjects.json"));
            projectileObjects = new List<Projectile>();

            switch (levelName)
            {
                case "ErdLevel":
                    new Projectile("Earthspike", 200, 200, new Vector2(1500, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(2500, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(4000, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(5600, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(5700, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(5800, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(5900, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6000, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6100, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6200, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6300, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 150, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6400, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6500, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6600, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6700, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6800, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(6900, 4600 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(5050, 3050 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9100, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9200, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 178, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9300, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 165, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9400, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 155, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9500, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 160, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9600, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 185, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9700, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 170, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9800, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 152, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(9900, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 156, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10000, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 171, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10100, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10200, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 176, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10300, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 156, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10400, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 175, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10500, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 153, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10600, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 185, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10700, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 164, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10800, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 168, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10900, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 160, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11000, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 170, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11100, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 177, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11200, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11300, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 159, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11400, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 166, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11500, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 172, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11600, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 163, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11700, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 155, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11800, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 172, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11900, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 164, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12000, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 150, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(10600, 4200 + 130) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    break;
                case "FeuerLevel":
                    new Projectile("FirePillar", 1000, 200, new Vector2(4600, 3000 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 125, 10, 250);
                    new Projectile("FirePillar", 1000, 200, new Vector2(6070, 3000 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 125, 10, 250);
                    new Projectile("FirePillar", 1000, 200, new Vector2(8500, 3300 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 125, 10, 250);
                    new Projectile("FirePillar", 1000, 200, new Vector2(9000, 3300 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 111, 10, 250);
                    new Projectile("FirePillar", 1000, 200, new Vector2(9500, 3300 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 100, 10, 250);
                    gameObjects.Add(new GameObject("armorUpgrade", "collectable", 1, 42, 35, new Vector2(10000, 8500) + startPosition.ToVector2()));
                    break;
                case "BlitzLevel":
                    new Projectile("Lightning", 800, 200, new Vector2(7750, 1800 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 100, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(3000, 4500 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 100, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(3400, 4500 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 48, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(4000, 4500 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 100, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(3700, 4500 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 72, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(4500, 4500 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 90, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(7250, 3500 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 50, 10, 200);
                    gameObjects.Add(new GameObject("HPFlower", "collectable", 5, 64, 64, new Vector2(8000, 1500) + startPosition.ToVector2()));
                    gameObjects.Add(new GameObject("weaponUpgrade", "collectable", 1, 21, 96, new Vector2(1000, 5000) + startPosition.ToVector2()));
                    break;
                case "Overworld":
                    break;
                case "Bossstage":
                    new Projectile("Lightning", 800, 200, new Vector2(8200, 1000 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 50, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(8700, 1000 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 72, 10, 200);
                    new Projectile("Lightning", 800, 200, new Vector2(9200, 1000 - 300) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 100, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11700, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 176, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11800, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 156, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(11900, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 175, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12000, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 153, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12100, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 164, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12200, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 185, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12300, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 168, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12400, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 160, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12500, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 155, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12600, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 159, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12700, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 166, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12800, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 150, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(12900, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 177, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13000, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 163, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13100, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 155, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13200, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 156, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13300, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 160, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13400, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 159, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13500, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 182, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13600, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 187, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13700, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 178, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13800, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 165, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(13900, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 155, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(14000, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 160, 10, 200);
                    new Projectile("Earthspike", 200, 200, new Vector2(14100, 8500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, 200, 100, 0), true, 185, 10, 200);
                    new Projectile("FirePillar", 1000, 200, new Vector2(16360, 6900 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 125, 10, 250);
                    new Projectile("FirePillar", 1000, 200, new Vector2(17350, 6900 - 500) + startPosition.ToVector2(), Vector2.Zero, 0, true, new Rectangle(-50, -400, 100, 0), true, 111, 10, 250);
                    break;
            }

            for ( int i = 0; i < enemyObjects.Count; i++) 
            {
                Enemy element = enemyObjects[i];
                gameObjects.Add(element);
                element.Initialize();

                //remove boss, if you already rescued it
                switch(levelName)
                {
                    case "EisLevel":
                        if (element.name == "Yinyin" && Level.soulsRescued[(int)BossElements.ICE])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                    case "FeuerLevel":
                        if (element.name == "Ina" && Level.soulsRescued[(int)BossElements.FIRE])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                    case "ErdLevel":
                        if (element.name == "Aiye" && Level.soulsRescued[(int)BossElements.EARTH])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                    case "BlitzLevel":
                        if (element.name == "Monomono" && Level.soulsRescued[(int)BossElements.BOLT])
                        {
                            enemyObjects.Remove(element);
                            gameObjects.Remove(element);
                            i--;
                        }
                        break;
                }
                //element.hurtBox.Location += startPosition;
                //element.attackBox.Location += startPosition;
            }
            foreach (NSC element in nscObjects)
            {
                gameObjects.Add(element);
            }

            foreach (GameObject element in gameObjects)
            {
                element.position += startPosition.ToVector2();
                element.init();
            }
            
            levelBGSprite = Game1.Ressources.Load<Texture2D>(levelName);
            positionBGSprite = new Vector2(startPosition.X, startPosition.Y);
            

            Camera.Main.levelBounds = new Rectangle(Vector2.Zero.ToPoint(), new Point(width, height));

            Camera.Main.position = startPosition.ToVector2() * -1;

            States.CurrentPlayerState = Playerstates.IDLE;

            Player.Nyr.currentEntityState = (int)Playerstates.IDLE;
            Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
            Player.Nyr.currentFrame = 0;

            lastPressedKeys = Keyboard.GetState().GetPressedKeys();

            Antagonist.Ryn.Reset();
            
        }

        //Update LevelTraps
        private void UpdateTraps(GameTime gameTime)
        {
            for (int i = 0; i < projectileObjects.Count; i++)
            {
                switch (projectileObjects[i].name)
                {
                    case "Earthspike":
                        if (projectileObjects[i].currentFrame > 90 && projectileObjects[i].currentFrame <= 95)
                        {
                            projectileObjects[i].attackBoxOffset.Y = 150 - (projectileObjects[i].currentFrame - 90) * (150f / 5f) - 50;
                            projectileObjects[i].attackbox.Height = (int)((projectileObjects[i].currentFrame - 90) * (150 / 5f));
                        }
                        else
                        {
                            projectileObjects[i].attackBoxOffset.Y = 110;
                            projectileObjects[i].attackbox.Height = 0;
                        }
                        break;
                    case "Lightning":
                        if (projectileObjects[i].currentFrame > 0 && projectileObjects[i].currentFrame <= 12)
                        {
                            projectileObjects[i].attackbox.Height = (int)(projectileObjects[i].currentFrame * (750 / 12)) + 50;
                        }
                        else if(projectileObjects[i].currentFrame > 12)
                        {
                            projectileObjects[i].attackbox.Height = 0;
                        }
                        break;
                    case "FirePillar":
                       if (projectileObjects[i].currentFrame > 0 && projectileObjects[i].currentFrame <= 30)
                        {
                            projectileObjects[i].attackbox.Height = (int)(projectileObjects[i].currentFrame * (1000 / 30));
                            projectileObjects[i].attackBoxOffset.Y = (30 - projectileObjects[i].currentFrame) * (1000 / 30) - 500;
                        }
                        else if (projectileObjects[i].currentFrame > 90)
                        {
                            projectileObjects[i].attackbox.Height = 1000 - ((int)(projectileObjects[i].currentFrame - 90) * (1000 / 35));
                            projectileObjects[i].attackBoxOffset.Y = (projectileObjects[i].currentFrame - 90) * (1000 / 35) - 500;
                        }
                      
                        break;
                }
            }
        }

        //get input and update the elements inside the level
        public void update(GameTime gameTime)
        {
            //Resetting Values
            moveValue = Vector2.Zero;
            textboxText = "";
            anyKeyPressed = false;

            #region updateTraps
            for (int i = 0; i < projectileObjects.Count; i++)
            {
                projectileObjects[i].Update(gameTime);
            }

            UpdateTraps(gameTime);
            #endregion updateTraps

            #region updateEnemies
            for (int i = 0; i < enemyObjects.Count; i++)
            {
                enemyObjects[i].Update(gameTime);
            }
            #endregion updateEnemies

            #region updateOther
            //Let all movingPlatforms move and if Nyr stands on it, then move her too
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject element = gameObjects[i];

                if (element.moving != Vector2.Zero)
                {
                    if (Player.Nyr.Collision<GameObject>(new GameObject[] { element }, Player.Nyr.position + moveValue).Length > 0)
                    {
                        if (element.name == "AiyeWall")
                        {
                            float plattFormMovement = element.move(gameTime).X;
                            if ((plattFormMovement < 0 && moveValue.X > 0) || (plattFormMovement > 0 && moveValue.X < 0))
                            {
                                Camera.Main.move(new Vector2(plattFormMovement, moveValue.Y));
                            }
                            else
                            {
                                Camera.Main.move(new Vector2(moveValue.X + plattFormMovement, moveValue.Y));
                            }

                        }
                        else
                        {
                            Camera.Main.move(moveValue + element.move(gameTime));
                        }

                    }
                    else
                    {
                        element.move(gameTime);
                    }

                }
            }

            //Let all other gameObjects fall to gravitation
            useGrav(gameTime);

            Antagonist.Ryn.Update(gameTime);
            #endregion updateOther

            #region updatePlayer
            //get input from keyboard or controller
            if ( Player.Nyr.currentEntityState != (int)Playerstates.DYING)
            {
                Input.Handeler.Update(gameTime);
            }
            

            Player.Nyr.onIce = false;

            //Let PLayer fall and save the moveValue in overall Movement
            if (!Player.Nyr.inHub)
            {
                if (Player.Nyr.inStomp)
                {
                    //fall faster in Stomp
                    moveValue.Y += Movement.PlayerFall(gameTime) * 2;
                }
                moveValue.Y += moveValue.Y += Movement.PlayerFall(gameTime);
            }

            //AnimationUpdate
            Player.Nyr.EntityUpdate(gameTime);

            //let em move, after all collisions have manipulated the movement
            moveValue = Movement.Update(moveValue);
            
            if (moveValue != Vector2.Zero)
            {
                Camera.Main.move(moveValue);
            }

            //trigger all triggers, that have been triggered
            Player.Nyr.activateTrigger(gameTime);

            //some configuration stuff
            if(moveValue.Y > 0)
            {
                Player.Nyr.currentEntityState = (int)Playerstates.FALL;
                Player.Nyr.currentFrame = 0;
                Player.Nyr.nextEntityState = (int)Playerstates.FALL;
            }
            if (Player.Nyr.currentEntityState == (int)Playerstates.FALL && Player.Nyr.onGround)
            {
                Player.Nyr.currentEntityState = (int)((Player.Nyr.onIce && !armorEnhanced[(int)BossElements.FIRE]) ? Playerstates.SLIP : Playerstates.LAND);
                Player.Nyr.currentFrame = 0;
                Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                if (Player.Nyr.onIce)
                {
                    Player.Nyr.slide = 1000;
                }
            }

            


            //set right state
            if (!anyKeyPressed)
            {
                if (Player.Nyr.isCrouching)
                {
                    Player.Nyr.currentEntityState = (int)Playerstates.IDLE;
                    Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                    Player.Nyr.currentFrame = 0;
                    Player.Nyr.isCrouching = false;
                }

                Player.Nyr.inactivityTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (!(Player.Nyr.currentEntityState == (int)Playerstates.JUMP || Player.Nyr.currentEntityState == (int)Playerstates.FALL || Player.Nyr.currentEntityState == (int)Playerstates.LAND || Player.Nyr.currentEntityState == (int)Playerstates.DYING || Player.Nyr.currentEntityState == (int)Playerstates.ISDEAD))
                {
                    if (Player.Nyr.currentEntityState == (int)Playerstates.WALK && Player.Nyr.onGround)
                    {
                        Player.Nyr.currentEntityState = (int)Playerstates.STOP;
                        Player.Nyr.currentFrame = 0;
                        if (Player.Nyr.onIce)
                        {
                            Player.Nyr.slide = 1000;
                        }
                    }
                    Player.Nyr.nextEntityState = (int)Playerstates.IDLE;
                }
                if (Player.Nyr.inactivityTime > 30f && Player.Nyr.currentEntityState != (int)Playerstates.DANCE)
                {
                    Player.Nyr.currentEntityState = (int)Playerstates.DANCE;
                    Player.Nyr.nextEntityState = (int)Playerstates.DANCE;
                    Player.Nyr.currentFrame = 0;
                }
            }
            else
            {
                Player.Nyr.inactivityTime = 0;
            }

            lastPressedKeys = newPressedKeys;
            #endregion updatePlayer
            
        }

        //theoretical move and seeing what happens
        public Vector2 checkCollision(Vector2 moveValue)
        {
            Vector2 newPos = Player.Nyr.position + moveValue;

            GameObject[] collidedObjects = Player.Nyr.Collision<GameObject>(gameObjects.ToArray(), newPos);

            bool collidedLeft = false;
            bool collidedRight = false;
            bool collidedTop = false;
            bool collidedBottom = false;

            for(int i = 0; i < collidedObjects.Length; i++)
            {
                GameObject element = collidedObjects[i];
                if (element.triggerType != null)
                {
                    continue;
                }
               
                if (element.position.X + element.width > newPos.X && element.position.X + element.width < Player.Nyr.position.X && !(element.name == "platform" || element.name == "cloud" || element.name == "cloud2"))
                {
                    collidedLeft = true;
                }
                else if (element.position.X < newPos.X + Player.Nyr.width && element.position.X > Player.Nyr.position.X + Player.Nyr.width && !(element.name == "platform" || element.name == "cloud" || element.name == "cloud2"))
                {
                    collidedRight = true;
                }
                else if (element.position.Y + element.height >= newPos.Y && element.position.Y + element.height <= Player.Nyr.position.Y && !(element.name == "platform" || element.name == "cloud" || element.name == "cloud2"))
                {
                    collidedTop = true;
                }
                else if ((int)element.position.Y <= (int)(newPos.Y + Player.Nyr.height))
                {
                    if (Player.Nyr.inStomp && element.name == "breakableGround")
                    {
                        gameObjects.Remove(element);
                        continue;
                    }
                    collidedBottom = true;
                }
                Player.Nyr.inStomp = false;
            }

            if (collidedLeft || collidedRight)
            {
                moveValue.X = 0;
            }
            if ((collidedTop && moveValue.Y < 0) || (collidedBottom && moveValue.Y > 0))
            {
                moveValue.Y = 0;
            }

            //fall, if you fall
            if ((int) moveValue.Y > 0 && !Player.Nyr.inJump && !Player.Nyr.inHub && !collidedBottom && Player.Nyr.currentEntityState != (int)Playerstates.FALL)
            {
                Player.Nyr.currentEntityState = (int)Playerstates.FALL;
                Player.Nyr.currentFrame = 0;
                Player.Nyr.nextEntityState = (int)Playerstates.FALL;
            }
            return moveValue;
        }
        
        //move all Objects in this Level
        public void moveGameObjects(Vector2 moveValue)
        {
            Antagonist.Ryn.position -= moveValue;
            positionBGSprite -= moveValue;
            foreach (GameObject gameObject in gameObjects)
            {
                if(gameObject.name == "Nyr" || gameObject.name == "Earthspike")
                {
                    continue;
                }

                gameObject.position -= moveValue;
                
                gameObject.startPosition -= moveValue;
            }
            foreach (Projectile element in projectileObjects)
            {
                element.position -= moveValue;
            }
        }

        //Lässt alle Objekte fallen, wenn sie nicht schon auf dem Boden sind und überprüft, ob sie aus der Welt gefallen sind
        public void useGrav(GameTime gameTime)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].mass > 0 && !gameObjects[i].onGround)
                {
                    if (gameObjects[i].name == "Nyr")
                    {
                        continue;
                    }
                    else
                    {
                        gameObjects[i].position = gameObjects[i].Fall(gameTime, gameObjects.ToArray());
                    }
                }
            }

            for (int i = 0; i < gameObjects.Count;)
            {
                //fällt aus der Welt und wird gelöscht
                if (gameObjects[i].position.Y > height)
                {
                    gameObjects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            for (int i = 0; i < projectileObjects.Count;i++)
            {
                if (projectileObjects[i].mass > 0)
                {
                    projectileObjects[i].aim.Y = (projectileObjects[i].Fall(gameTime, projectileObjects.ToArray()).Y - projectileObjects[i].position.Y ) / 10;
                    //projectileObjects[i].aim.Y = projectileObjects[i].Fall(gameTime, projectileObjects.ToArray()).Y / 1000;
                }
            }
        }

        //Saves the Game
        public void SaveGame()
        {
            StreamWriter output = File.CreateText("SaveGame.txt");
            
            for(int i = 0; i < 4; i++)
            {
                output.WriteLine((Level.soulsRescued[i] ? "T" : "F"));
                output.WriteLine((Level.armorEnhanced[i] ? "T" : "F"));
                output.WriteLine("");
            }
            output.WriteLine(Player.Nyr.money.ToString());
            output.WriteLine("");
            output.WriteLine(Player.Nyr.maxHealth.ToString());

            output.Close();


            Player.Nyr.health = Player.Nyr.maxHealth;
            Player.Nyr.mana = Player.Nyr.maxMana;
        }

        //draws the map
        private void DrawMap(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(map, new Rectangle(0, 0, map.Width, map.Height), Color.White * 0.8f);
            Texture2D nyrLocationIcon = Game1.Ressources.Load<Texture2D>("NyrMapIcon");
            switch (name)
            {
                case "Hub":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(310, 415, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "Overworld":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(820, 410, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "Bossstage":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(1335, 400, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "EisLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(550, 140, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "BlitzLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(1075, 130, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "FeuerLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(565, 690, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
                case "ErdLevel":
                    spriteBatch.Draw(nyrLocationIcon, new Rectangle(1100, 680, nyrLocationIcon.Width, nyrLocationIcon.Height), Color.White * 0.8f);
                    break;
            }
        }

        //the typical render method
        public void render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(levelBGSprite, new Rectangle(positionBGSprite.ToPoint(), new Point(width, height)), Color.White);

            foreach (GameObject element in gameObjects)
            {
                element.Draw(gameTime, spriteBatch);
            }
            //Draw all GameObjects such as Enemys
            foreach (Enemy element in enemyObjects)
            {


                if (!element.enemyHit)
                {
                    
                    element.EntityRender(gameTime, spriteBatch);
                }
                if (element.hitTimer < 0 && element.hitTimer >= 20)
                {
                    if (element.enemyHit && element.hitTimer % 2 == 0)
                    {
                        element.EntityRender(gameTime, spriteBatch);
                    }
                }
                if (element.hitTimer == 0)
                {
                    element.enemyHit = false;
                }
                element.hitTimer--;
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.hurtBox.Location.X-2, element.hurtBox.Location.Y - 102), Color.Black);
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.hurtBox.Location.X-2, element.hurtBox.Location.Y - 98), Color.Black);
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.hurtBox.Location.X+2, element.hurtBox.Location.Y - 102), Color.Black);
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.hurtBox.Location.X+2, element.hurtBox.Location.Y - 98), Color.Black);
                spriteBatch.DrawString(Game1.Font, element.health.ToString(), new Vector2(element.hurtBox.Location.X, element.hurtBox.Location.Y - 100), Color.White);
            }
            
            Player.Nyr.EntityRender(gameTime, spriteBatch);
            Antagonist.Ryn.EntityRender(gameTime, spriteBatch);

            for (int i = 0; i < projectileObjects.Count; i++)
            {
                projectileObjects[i].Draw(gameTime, spriteBatch);
                //spriteBatch.Draw(Game1.pxl, new Rectangle((int)projectileObjects[i].position.X, (int)projectileObjects[i].position.Y, projectileObjects[i].width, projectileObjects[i].height), Color.LightGreen * 0.5f);
                spriteBatch.Draw(Game1.pxl, new Rectangle(projectileObjects[i].attackbox.X, projectileObjects[i].attackbox.Y, projectileObjects[i].attackbox.Width, projectileObjects[i].attackbox.Height), Color.BlueViolet * 0.5f);
            }

            if (textboxText.Length > 0)
            {
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(98, 202), Color.Black);
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(98, 198), Color.Black);
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(102, 202), Color.Black);
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(102, 198), Color.Black);
                spriteBatch.DrawString(Game1.Font, textboxText, new Vector2(100, 200), Color.White);

            }
            Interface.Draw(spriteBatch);
            if (Interface.ShowMap)
            {
                DrawMap(spriteBatch);
            }
        }
    }
}