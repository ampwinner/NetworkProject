using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LobbyCtrl : MonoBehaviour
{
    public static LobbyCtrl Instance;

    //UI Elements
    public TextMeshProUGUI LobbyNameText;

    //Player Data
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    //Other Data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjCtrl LocalplayerController;

    //Ready
    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;

    //Manager
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

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalplayerController.isReady)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void ChackIfAllReady()
    {
        bool isAllReady = false;

        foreach (PlayerObjCtrl player in Manager.GamePlayer)
        {
            if (player.isReady)
            {
                isAllReady = true;
            }
            else
            {
                isAllReady = false;
                break;
            }
        }

        if (isAllReady)
        {
            if (LocalplayerController.PlayerIDNumber == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) { CreateHostPlayerItem(); } //Host
        if (PlayerListItems.Count < Manager.GamePlayer.Count) { CreateClientPlayerItem(); }
        if (PlayerListItems.Count > Manager.GamePlayer.Count) { RemovePlayerItem(); }
        if (PlayerListItems.Count == Manager.GamePlayer.Count) { UpdatePlayerItem(); }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjCtrl>();
    }


    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjCtrl player in Manager.GamePlayer)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnetionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.isReady = player.isReady;
            NewPlayerItemScript.SetPlayerValues();


            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjCtrl player in Manager.GamePlayer)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnetionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnetionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.isReady = player.isReady;
                NewPlayerItemScript.SetPlayerValues();


                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjCtrl player in Manager.GamePlayer)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnetionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.isReady = player.isReady;
                    PlayerListItemScript.SetPlayerValues();
                    if (player == LocalplayerController)
                    {
                        UpdateButton();
                    }
                }
            }

        }
        ChackIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistItem in PlayerListItems)
        {
            if (!Manager.GamePlayer.Any(b => b.ConnetionID == playerlistItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }
        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                if (playerlistItemToRemove)
                {
                    GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                    PlayerListItems.Remove(playerlistItemToRemove);
                    Destroy(ObjectToRemove);
                    ObjectToRemove = null;
                }
            }
        }
    }

    public void StartGame(string ScenceName)
    {
        LocalplayerController.CanStartGame(ScenceName);
    }
}
