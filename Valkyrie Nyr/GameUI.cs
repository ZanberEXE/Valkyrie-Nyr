using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    //Use this to update ingame Interface like playerstats and textboxes or the map
    class GameUI
    {
        private static GameUI instance;

        public static GameUI Handeler
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameUI();
                }
                return instance;
            }
        }

        public bool ShowMap = false;

        private GameUI()
        {

        }
    }
}
