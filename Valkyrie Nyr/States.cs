﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valkyrie_Nyr
{
    enum Playerstates { IDLE, WALK, JUMP, FIGHT, HIT, DANCE, FALL, LAND, STOP, CROUCH, DEAD };
    enum Enemystates { IDLE, WALK, ATTACK, AGGRO }
    enum Bossstates { IDLE, WALK, ATTACK1, ATTACK2, ATTACK3, ATTACK4, Special1 }

    enum GameStates { MAINMENU, PLAYING, EXIT, OPTIONS, CREDITS, PAUSE, LOSE, SPLASHSCREEN }

    //just saves the States, so they are accessable from everywhere
    class States
    {
        private static GameStates currentGameState;
        private static Playerstates currentPlayerState;
        private static Playerstates nextPlayerState;

        public static GameStates CurrentGameState { get { return currentGameState; } set { currentGameState = value; } }
        public static Playerstates CurrentPlayerState { get { return currentPlayerState; } set { currentPlayerState = value; Player.Nyr.currentFrame = 0; nextPlayerState = value; Player.Nyr.changeState(); } }
        public static Playerstates NextPlayerState { get { return nextPlayerState; } set { nextPlayerState = value; } }

    }
}
