using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    static class Interface
    {
        private static Texture2D statsTexture;
        private static Texture2D hpBarTexture;
        private static Texture2D mpBarTexture;
        private static Texture2D moneyTexture;

        public static bool ShowMap = false;

        public static void Start()
        {
            statsTexture = Game1.Ressources.Load<Texture2D>("UI/stats");
            hpBarTexture = Game1.Ressources.Load<Texture2D>("UI/hp");
            mpBarTexture = Game1.Ressources.Load<Texture2D>("UI/mp");
            moneyTexture = Game1.Ressources.Load<Texture2D>("UI/money");
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(statsTexture, new Rectangle(0, 0, statsTexture.Width, statsTexture.Height), Color.White);

            spriteBatch.Draw(hpBarTexture, new Vector2(21, 24), new Rectangle(0, 0, hpBarTexture.Width, hpBarTexture.Height), Color.White, 0f, Vector2.Zero, new Vector2(Player.Nyr.health / (float)Player.Nyr.maxHealth, 1), SpriteEffects.None, 0f);
            spriteBatch.DrawString(Game1.Font, Player.Nyr.health.ToString() + " / " + Player.Nyr.maxHealth.ToString(), new Vector2(25, 21), Color.GhostWhite);

            spriteBatch.Draw(mpBarTexture, new Vector2(21, 88), new Rectangle(0, 0, mpBarTexture.Width, mpBarTexture.Height), Color.White, 0f, Vector2.Zero, new Vector2(Player.Nyr.mana / (float)Player.Nyr.maxMana, 1), SpriteEffects.None, 0f);
            spriteBatch.DrawString(Game1.Font, Player.Nyr.mana.ToString() + " / " + Player.Nyr.maxMana.ToString(), new Vector2(25, 85), Color.GhostWhite);


            spriteBatch.Draw(moneyTexture, new Rectangle(50, Game1.WindowSize.Y - moneyTexture.Height - 20, moneyTexture.Width, moneyTexture.Height), Color.White);
            spriteBatch.DrawString(Game1.Font, Player.Nyr.money.ToString(), new Vector2(50 + 80, Game1.WindowSize.Y - moneyTexture.Height - 20 + 5), Color.Gold);
        }
    }
}
