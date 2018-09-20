using System;
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
        SpriteFont font;

        private int oldGameTime = 0;

        public NSC (string name, string triggerType, int mass, int height, int width, Vector2 position, int hp, int dmg) : base(name, triggerType, mass, height, width, position)
        {
        }

        public void startConversation(GameTime newGameTime)
        {
            if(dialogueState > dialogues.Length)
            {
                dialogueState--;
            }
            font = Game1.Ressources.Load<SpriteFont>("File");
            Player.Nyr.conversationPartner = this;
            States.CurrentGameState = GameStates.CONVERSATION;
            currentSpeech = 0;
            oldGameTime = (int)newGameTime.TotalGameTime.TotalSeconds;
        }

        public void continueConversation(GameTime newGameTime)
        {
            if((int)newGameTime.TotalGameTime.TotalSeconds - oldGameTime == 0)
            {
                return;
            }
            oldGameTime = (int)newGameTime.TotalGameTime.TotalSeconds;
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
            //Draw Text
            spriteBatch.DrawString(font, dialogues[dialogueState].spokesman[currentSpeech], new Vector2(60, 610), Color.LightBlue);
            spriteBatch.DrawString(font, dialogues[dialogueState].speeches[currentSpeech], new Vector2(60, 700), Color.LightBlue);

        }

        
    }
}
