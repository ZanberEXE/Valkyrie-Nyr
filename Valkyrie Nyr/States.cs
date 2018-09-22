using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    enum Playerstates { IDLE, WALK, JUMP, FIGHT, HIT, DANCE, FALL, LAND, STOP, CROUCH, DEAD };
    enum Enemystates { IDLE, WALK, ATTACK, AGGRO }
    enum Bossstates { IDLE, WALK, ATTACK1, ATTACK2, ATTACK3, ATTACK4, Special1 }

    enum BossElements { FIRE, ICE, EARTH, BOLT}

    enum GameStates { MAINMENU, PLAYING, EXIT, OPTIONS, CREDITS, PAUSE, LOSE, SPLASHSCREEN, CONVERSATION }

    enum BGMStates { MENU, PLAYING, BOSS, DEAD }

    //just saves the States, so they are accessable from everywhere
    class States
    {
        private static GameStates currentGameState;
        private static Playerstates currentPlayerState;
        private static Playerstates nextPlayerState;
        private static BGMStates currentBGMState;

        public static GameStates CurrentGameState { get { return currentGameState; } set { currentGameState = value; } }
        public static Playerstates CurrentPlayerState { get { return currentPlayerState; } set { currentPlayerState = value; Player.Nyr.currentFrame = 0; nextPlayerState = value; } }
        public static Playerstates NextPlayerState { get { return nextPlayerState; } set { nextPlayerState = value; } }
        public static BGMStates CurrentBGMState { get { return currentBGMState; } set { currentBGMState = value; } }
    }
}
