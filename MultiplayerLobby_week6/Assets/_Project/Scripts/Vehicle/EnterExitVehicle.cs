using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnterExitVehicle : NetworkBehaviour
{
    private MSVehicleControllerFree vehicleController;
    private MSSceneControllerFree sceneController;

    [SyncVar]
    public bool isControlled = false;

    // Start is called before the first frame update
    void Start()
    {
        vehicleController = GetComponent<MSVehicleControllerFree>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            sceneController = FindObjectOfType<MSSceneControllerFree>();
            sceneController.player = other.gameObject;
            sceneController.enabled = true;
            SetPlayerActive setPlayerActive = other.gameObject.GetComponent<SetPlayerActive>();
            NetworkIdentity playerID = other.gameObject.GetComponent<NetworkIdentity>();
            NetworkIdentity networkID = this.GetComponent<NetworkIdentity>();
            setPlayerActive.CmdSetAuthority(networkID, playerID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            sceneController.enabled = false;
        }
    }
}
