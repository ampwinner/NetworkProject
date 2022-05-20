using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class CutomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjCtrl GameplayerPrefeb;
    public List<PlayerObjCtrl> GamePlayer { get; } = new List<PlayerObjCtrl>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjCtrl GamePlayerInstance = Instantiate(GameplayerPrefeb);
            GamePlayerInstance.ConnetionID = conn.connectionId;
            GamePlayerInstance.PlayerIDNumber = GamePlayer.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayer.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void StartGame(string ScenceName)
    {
        ServerChangeScene(ScenceName);
    }
}
