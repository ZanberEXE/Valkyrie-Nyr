using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    enum Playerstates { IDLE, WALK, JUMP, FIGHT, HIT, DEAD, DANCE, FALL, LAND, STOP, CROUCH };

    enum GameStates { MAINMENU, PLAYING, EXIT }

    //just saves the States, so they are accessable from everywhere
    class States
    {
        private static GameStates currentGameState;
        private static Playerstates currentPlayerState;
        private static Playerstates nextPlayerState;

        public static GameStates CurrentGameState { get { return currentGameState; } set { currentGameState = value; } }
        public static Playerstates CurrentPlayerState { get { return currentPlayerState; } set { currentPlayerState = value; Player.Nyr.currentFrame = 0; } }
        public static Playerstates NextPlayerState { get { return nextPlayerState; } set { nextPlayerState = value; } }

    }
}
