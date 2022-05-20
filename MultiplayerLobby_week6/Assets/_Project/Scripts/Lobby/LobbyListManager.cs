using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Linq;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager instance;

    //Lobby
    public GameObject lobbyListName;
    public GameObject lobbyEntryPrefab;
    public GameObject scrollViewContent;

    public GameObject lobbyButton, hostButton;
    public List<LobbyDataEntry> listOfLobby = new List<LobbyDataEntry>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void DisplayLobby(List<CSteamID> lobbyIds, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIds.Count; i++)
        {
            if (lobbyIds[i].m_SteamID == result.m_ulSteamIDLobby && 
                !listOfLobby.Any(lobby => lobby.LobbySteamID==(CSteamID)lobbyIds[i].m_SteamID))
            {
                GameObject createdLobbyItem = Instantiate(lobbyEntryPrefab);
                LobbyDataEntry createdlobbyDataEntry = createdLobbyItem.GetComponent<LobbyDataEntry>();
                createdlobbyDataEntry.LobbySteamID = (CSteamID)lobbyIds[i].m_SteamID;
                createdlobbyDataEntry.lobbyName =
                    SteamMatchmaking.GetLobbyData((CSteamID)lobbyIds[i].m_SteamID, "name");
                createdlobbyDataEntry.SetLobbyName();

                createdLobbyItem.transform.SetParent(scrollViewContent.transform);
                createdLobbyItem.transform.localScale = Vector3.one;

                listOfLobby.Add(createdlobbyDataEntry);
            }
        }
    }

    public void GetListOfLobby()
    {
        lobbyButton.SetActive(false);
        hostButton.SetActive(false);
        lobbyListName.SetActive(true);

        SteamLobby.Instance.GetListOfLobby();
    }

    public void DestroyLobby()
    {
        foreach (LobbyDataEntry lobbyItem in listOfLobby)
        {
            Destroy(lobbyItem.gameObject);
        }
        listOfLobby.Clear();
    }
}
