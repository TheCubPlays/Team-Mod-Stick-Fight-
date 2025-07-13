// Credits to Monky for all of this

using System.Collections.Generic;

namespace TMOD;

public static class PlayerUtils
{
    public static readonly List<string> PlayerColorsParams = new() { "y", "yellow", "b", "blue", "r", "red", "g", "green" };

    public static bool IsPlayerInLobby(int targetID)
    {
        var connectedClients = GameManager.Instance.mMultiplayerManager.ConnectedClients;
        return connectedClients[targetID] != null && connectedClients[targetID].PlayerObject;
    }    
}