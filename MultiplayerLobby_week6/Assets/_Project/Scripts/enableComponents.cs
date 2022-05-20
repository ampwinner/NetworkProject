using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using StarterAssets;

public class enableComponents : NetworkBehaviour
{
    public Transform target;
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(true);
        if (isLocalPlayer)
        {
            CharacterController characterController = GetComponent<CharacterController>();
            characterController.enabled = true;
            ThirdPersonController thirdPersonController = GetComponent<ThirdPersonController>();
            thirdPersonController.enabled = true;
            thirdPersonController.SetMainCamara = GameObject.FindGameObjectWithTag("MainCamera");
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
            CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            GameObject playerFollowCamera = cinemachineVirtualCamera.gameObject;
            cinemachineVirtualCamera.Follow = target;
        }
        DisplayUICtrl displayUICtrl = GetComponent<DisplayUICtrl>();
        displayUICtrl.SetPlayerValues();
    }
}
