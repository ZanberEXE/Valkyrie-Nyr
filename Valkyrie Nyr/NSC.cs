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
            if(dialogueState > dialogues.Length)
            {
                dialogueState--;
            }
            Player.Nyr.conversationPartner = this;
            States.CurrentGameState = GameStates.CONVERSATION;
            currentSpeech = 0;
            oldGameTime = newGameTime.TotalGameTime.TotalSeconds;
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
                if (dialogueState + 1 < dialogues.Length)
                {
                    dialogueState++;
                }
                endConversation();
            }
            else
            {
                currentSpeech++;
            }
        }

        public void endConversation()
        {
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
                    spriteBatch.Draw(spokemanTexture, new Rectangle((currentSpokesman == "Nyr") ? 200 : Game1.WindowSize.X - 200 - spokemanTexture.Width, 100, spokemanTexture.Width, spokemanTexture.Height), Color.White);
                }
            }
            spriteBatch.Draw(Game1.pxl, new Rectangle(0, 600, Game1.WindowSize.X, Game1.WindowSize.Y - 600), Color.Black * 0.7f);
            //Draw Text
            spriteBatch.DrawString(Game1.Font, dialogues[dialogueState].spokesman[currentSpeech], new Vector2(60, 610), Color.LightBlue);
            for (int i = 0; i <= (int)(dialogues[dialogueState].speeches[currentSpeech].Length / 100f); i++)
            {
                string line = dialogues[dialogueState].speeches[currentSpeech].Remove(0, 100 * i);
                if (line.Length > 100)
                {
                    line = line.Remove(100);
                }
                spriteBatch.DrawString(Game1.Font, line, new Vector2(60, 700 + 50 * i), Color.LightBlue);
            }

        }

        
    }
}
