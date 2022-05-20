using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    //Callback
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //Lobby List Callback
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    List<CSteamID> lobbyIds = new List<CSteamID>();

    //Variable
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CutomNetworkManager manager;

    //GameObj;
    public GameObject HostButtom;
    public TextMeshProUGUI LobbyNameText;

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        if (Instance == null)
        {Instance = this; }

            manager = GetComponent<CutomNetworkManager>();
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby()
    {
        //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }

    public void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby created succesfully");
        // (!this) { return; }

        manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    public void OnJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Join Lobby");
        SteamMatchmaking.JoinLobby((callback.m_steamIDFriend));
    }

    public void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        //HostButtom.SetActive(false);
            CurrentLobbyID = callback.m_ulSteamIDLobby;
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        //Clients
        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        //if (!this) { return; }

        if (!string.IsNullOrWhiteSpace(manager.networkAddress))
            manager.StartClient();

    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetListOfLobby()
    {
        if(lobbyIds.Count > 0)
        {
            lobbyIds.Clear();
        }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(100);
        SteamMatchmaking.RequestLobbyList();
    }

    public void GetLobbyList()
    {
        if (lobbyIds.Count > 0)
        {
            lobbyIds.Clear();
        }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(100);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbyListManager.instance.DisplayLobby(lobbyIds, result);
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbyListManager.instance.listOfLobby.Count > 0)
        {
            LobbyListManager.instance.DestroyLobby();
        }

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIds.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnDestroy()
    {
        if (SteamManager.Initialized)
            SteamMatchmaking.LeaveLobby((CSteamID)CurrentLobbyID);
        LobbyCreated.Unregister();
        JoinRequested.Unregister();
        LobbyEntered.Unregister();
        LobbyList.Unregister();
        LobbyDataUpdated.Unregister();
    }
}
