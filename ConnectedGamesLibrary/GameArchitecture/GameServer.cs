using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedGamesLibrary.GameArchitecture
{
    class GameServer
    {
        public IPEndPoint Address;

        public int PlayersMaxCount = 150;

        public GameServer(int playercounts)
        {


            PlayersMaxCount = playercounts;
        }
        public Lobby CreateNewLobby()
        {
            return new Lobby();



        }

    }
}
