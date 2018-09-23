using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Valkyrie_Nyr
{
    class Music
    {
        public Song song;

        private static Music currentSong;

        public string name;

        public static Music CurrentSong { get { if (currentSong == null) { currentSong = new Music(); } return currentSong; } }

        public void loadSong(string songName)
        {
            name = songName;
            this.song = Game1.Ressources.Load<Song>(songName);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
        }
    }
}
