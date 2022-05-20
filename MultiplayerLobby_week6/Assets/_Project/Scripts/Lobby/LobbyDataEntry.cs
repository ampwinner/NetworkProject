using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class LobbyDataEntry : MonoBehaviour
{
    //Data
    public CSteamID LobbySteamID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;
    public Button JoinButton;

    public void SetLobbyName()
    {
        if (lobbyName == "")
        {
            lobbyNameText.text = "Empty";
            lobbyNameText.color = Color.gray;
            JoinButton.interactable = false;
        }
        else
        {
            lobbyNameText.text = lobbyName;

            if (lobbyName.Contains("'s Lobby"))
            {
                lobbyNameText.color = Color.yellow;
            }
            else
            {
                lobbyNameText.color = Color.gray;
                JoinButton.interactable = false;
            }
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(LobbySteamID);
    }
}
