using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedGamesLibrary.GameArchitecture
{
    public enum LobbyState
    {
        waitingForPlayers=0,
        InGame=1
    }
    class Lobby
    {
        public int CurrentPlayerCount;
        public uint TimeToStart;
        public LobbyState state;

        public Lobby() { }

    }
}
