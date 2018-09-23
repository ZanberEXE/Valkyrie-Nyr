using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Valkyrie_Nyr
{
    class SFX
    {
        public SoundEffect sfx;

        private static SFX currentSFX;

        public string name;

        public static SFX CurrentSFX { get { if(currentSFX == null) { currentSFX = new SFX(); } return currentSFX; } }

        public void loadSFX(string sfxName)
        {
            name = sfxName;
            this.sfx = Game1.Ressources.Load<SoundEffect>(sfxName);
            sfx.CreateInstance().Play();
        }
    }
}
