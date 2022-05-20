using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SetPlayerActive : NetworkBehaviour
{
    [Command(requiresAuthority = false)]
    public void CmdSetAuthority(NetworkIdentity thing, NetworkIdentity PlayerID)
    {
        if (thing.connectionToClient != null)
        {
            thing.RemoveClientAuthority();
        }
        //Debug.Log(thing.hasAuthority);
        thing.AssignClientAuthority(PlayerID.connectionToClient);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayerActive(bool active)
    {
        RpcPlayerActive(active);
        gameObject.SetActive(active);
    }

    [ClientRpc]
    public void RpcPlayerActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
