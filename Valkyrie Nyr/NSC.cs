using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Valkyrie_Nyr
{

    
    public struct Conversation
    {
        public string[] spokesman;
        public string[] speeches;

        public Conversation(string[] _spokesman, string[] _speeches)
        {
            spokesman = _spokesman;
            speeches = _speeches;
        }
    }

    class NSC : GameObject
    {
        public int dialogueState;
        private int currentSpeech;

        public Conversation[] dialogues;

        private double oldGameTime = 0;

        public NSC (string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, triggerType, mass, height, width, position)
        {
        }

        public void startConversation(GameTime newGameTime)
        {
            if (dialogueState > dialogues.Length)
            {
                dialogueState--;
            }
            Player.Nyr.conversationPartner = this;
            States.CurrentGameState = GameStates.CONVERSATION;
            currentSpeech = 0;
            oldGameTime = newGameTime.TotalGameTime.TotalSeconds;
        }
        public void startConversation()
        {
            if (dialogueState > dialogues.Length)
            {
                dialogueState--;
            }
            Player.Nyr.conversationPartner = this;
            States.CurrentGameState = GameStates.CONVERSATION;
            currentSpeech = 0;
            oldGameTime = 0;
        }

        public void continueConversation(GameTime newGameTime)
        {
            //to wait 0.2 seconds before you can continue with reading
            if(newGameTime.TotalGameTime.TotalSeconds - oldGameTime < 0.2)
            {
                return;
            }
            oldGameTime = newGameTime.TotalGameTime.TotalSeconds;
            if (currentSpeech + 1 >= dialogues[dialogueState].speeches.Length)
            {
                
                endConversation();
            }
            else
            {
                currentSpeech++;
            }
        }

        public void endConversation()
        {
            switch (this.name)
            {
                case "inaSoul":
                    if (!Level.armorEnhanced[(int)BossElements.FIRE])
                    {
                        Level.armorEnhanced[(int)BossElements.FIRE] = true;
                    }
                    break;
                case "yinyinSoul":
                    if (!Level.armorEnhanced[(int)BossElements.ICE])
                    {
                        Level.armorEnhanced[(int)BossElements.ICE] = true;
                    }
                    break;
                case "aiyeSoul":
                    if (!Level.armorEnhanced[(int)BossElements.EARTH])
                    {
                        Level.armorEnhanced[(int)BossElements.EARTH] = true;
                    }
                    break;
                case "monomonoSoul":
                    if (!Level.armorEnhanced[(int)BossElements.BOLT])
                    {
                        Level.armorEnhanced[(int)BossElements.BOLT] = true;
                    }
                    break;
                case "Statue":
                    Level.Current.SaveGame();
                    break;
                case "drWhich":
                    //TODO:Anpassen
                    if(Player.Nyr.money >= 500)
                    {
                        Player.Nyr.money -= 500;
                        Player.Nyr.health += 1000;
                        Player.Nyr.maxHealth += 1000;
                    }
                    break;
            }
            if (dialogueState + 1 < dialogues.Length)
            {
                dialogueState++;
            }
            currentSpeech = 0;
            States.CurrentGameState = GameStates.PLAYING;
            Player.Nyr.conversationPartner = null;

        }

        public void renderConversation(SpriteBatch spriteBatch)
        {
            string[] allFiles = Directory.GetFiles(Game1.Ressources.RootDirectory + "\\ConversationSprites");
            for(int i = 0; i < allFiles.Length; i++)
            {
                string currentSpokesman = dialogues[dialogueState].spokesman[currentSpeech];
                if (allFiles[i] == Game1.Ressources.RootDirectory + "\\ConversationSprites\\" + currentSpokesman + ".xnb")
                {
                    Texture2D spokemanTexture = Game1.Ressources.Load<Texture2D>("ConversationSprites/" + currentSpokesman);
                    spriteBatch.Draw(spokemanTexture, new Rectangle((currentSpokesman == "Nyr") ? 200 : Game1.WindowSize.X - spokemanTexture.Width, 0, spokemanTexture.Width, spokemanTexture.Height), Color.White);
                }
            }
            spriteBatch.Draw(Game1.pxl, new Rectangle(10, 700, 400, 80), new Color(0.05f, 0.05f, 0.1f, 0.8f));
            spriteBatch.Draw(Game1.pxl, new Rectangle(10, 800, Game1.WindowSize.X - 20, Game1.WindowSize.Y - 810), new Color(0.05f, 0.05f, 0.1f, 0.8f));

            //Draw Text
            spriteBatch.DrawString(Game1.Font, dialogues[dialogueState].spokesman[currentSpeech], new Vector2(30, 720), Color.GhostWhite);
            int lineCounter = 0;
            for (int i = 0; i < dialogues[dialogueState].speeches[currentSpeech].Length;)
            {
                string line = dialogues[dialogueState].speeches[currentSpeech].Remove(0, i);

                if(line.Length > 100)
                {
                    line = line.Remove(100);

                    int lineLenght = line.LastIndexOf(' ') + 1;

                    if (line.Length > lineLenght && lineLenght > 0)
                    {
                        line = line.Remove(lineLenght);
                    }
                }
                spriteBatch.DrawString(Game1.Font, line, new Vector2(30, 820 + 50 * lineCounter), Color.GhostWhite);
                i += line.Length;
                lineCounter++;
            }

        }

        
    }
}
