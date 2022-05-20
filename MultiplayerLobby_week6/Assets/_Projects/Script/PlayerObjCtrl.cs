using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjCtrl : NetworkBehaviour
{
    //player data
    [SyncVar] public int ConnetionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] 
    public string PlayerName;

    [SyncVar(hook = nameof(PlayeReadyUpdate))]
    public bool isReady;

    private CutomNetworkManager manager;

    private CutomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = CutomNetworkManager.singleton as CutomNetworkManager;
        }
    }

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyCtrl.Instance.FindLocalPlayer();
        LobbyCtrl.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayer.Add(this);
        LobbyCtrl.Instance.UpdateLobbyName();
        LobbyCtrl.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayer.Remove(this);
        LobbyCtrl.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string oldValue,string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }

        if (isClient)
        {
            LobbyCtrl.Instance.UpdatePlayerList();
        } 
    }

    public void PlayeReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.isReady = newValue;
        }

        if (isClient)
        {
            LobbyCtrl.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayeReadyUpdate(this.isReady,!this.isReady);
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady();
        }
    }

    //Start Game
    public void CanStartGame(string ScenceName)
    {
        if (hasAuthority)
        {
            CmdCanStartGame(ScenceName);
        }
    }

    [Command]
    public void CmdCanStartGame(string ScenceName)
    {
        Manager.StartGame(ScenceName);
    }
}
