using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    enum Playerstates { IDLE, WALK, JUMP, FIGHT, HIT, DEAD };

    enum GameStates { MAINMENU, PLAYING, EXIT, OPTIONS, CREDITS, PAUSE, LOSE, SPLASHSCREEN }

    //just saves the States, so they are accessable from everywhere
    class States
    {
        private static GameStates currentGameState;
        private static Playerstates currentPlayerState;
        private static Playerstates nextPlayerState;

        public static GameStates CurrentGameState { get { return currentGameState; } set { currentGameState = value; } }
        public static Playerstates CurrentPlayerState { get { return currentPlayerState; } set { currentPlayerState = value; nextPlayerState = value; } }
        public static Playerstates NextPlayerState { get { return nextPlayerState; } set { nextPlayerState = value; } }

    }
}
