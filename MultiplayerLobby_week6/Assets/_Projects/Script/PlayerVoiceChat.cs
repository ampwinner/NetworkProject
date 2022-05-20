using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerVoiceChat : NetworkBehaviour
{
    public AudioSource audioSource;

    public KeyCode keyTalk = KeyCode.T;
    public KeyCode keyMute = KeyCode.M;
    public bool isMute;
    private DisplayUICtrl displayUICtrl;

    [Header("Debug")]
    public bool isHearYourself;

    // Start is called before the first frame update
    void Start()
    {
        displayUICtrl = GetComponent<DisplayUICtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(keyTalk) && !isMute)
            {
                SteamUser.StartVoiceRecording();
                Debug.Log("Record Start");
                CmdChangeSpeakerStatus(Speaker.use);
            }

            else if (Input.GetKeyUp(keyTalk) && !isMute)
            {
                SteamUser.StopVoiceRecording();
                Debug.Log("Record Stop");
                CmdChangeSpeakerStatus(Speaker.idle);
            }

            if (Input.GetKeyDown(keyMute))
            {
                isMute = !isMute;
                CmdChangeSpeakerStatus(isMute ? Speaker.mute : Speaker.idle);
                if (isMute)
                    SteamUser.StartVoiceRecording();
            }

            GetVoice();
        }
    }

    void GetVoice()
    {
        uint compressed;
        EVoiceResult result = SteamUser.GetAvailableVoice(out compressed);
        if (result == EVoiceResult.k_EVoiceResultOK && compressed > 1024)
        {
            Debug.Log(compressed);
            byte[] destBuffer = new byte[1024];
            uint bytesWritten;
            result = SteamUser.GetVoice(true, destBuffer, 1024, out bytesWritten);
            if (result == EVoiceResult.k_EVoiceResultOK && bytesWritten > 0)
            {
                CmdSendData(destBuffer, bytesWritten);
            }
        }
    }

    [Command(channel = 2)]
    void CmdSendData(byte[] data,uint size)
    {
        Debug.Log("Command");
        PlayerVoiceChat[] players = FindObjectsOfType<PlayerVoiceChat>();

        for (int i = 0; i < players.Length; i++)
        {
            TargetPlayerSound(players[i].GetComponent<NetworkIdentity>().connectionToClient, data, size);
        }
    }

    [TargetRpc(channel = 2)]
    void TargetPlayerSound(NetworkConnection conn,byte[] destBuffer,uint bytesWritten)
    {
        if (!isHearYourself)
        {
            if (isLocalPlayer) return;
        }
        Debug.Log("Target");
        byte[] destBuffer2 = new byte[22050 * 2];
        uint bytesWritten2;
        EVoiceResult result = SteamUser.DecompressVoice(destBuffer, bytesWritten
            , destBuffer2, (uint)destBuffer2.Length, out bytesWritten2, 22050);

        if (result == EVoiceResult.k_EVoiceResultOK && bytesWritten2 > 0)
        {
            audioSource.clip = AudioClip.Create(Random.Range(100, 1000000).ToString()
                , 22050, 1, 22050, false);
            float[] sample = new float[22050];
            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = (short)(destBuffer2[i * 2] | destBuffer2[i * 2 + 1] << 8) / 32768.0f;
            }

            audioSource.clip.SetData(sample, 0);
            audioSource.Play();
        }
    }

    [Command]
    void CmdChangeSpeakerStatus(Speaker status)
    {
        displayUICtrl.displayData.StatusSpeaker = status;
        RpcChangeSpeakerStatus(status);
    }

    [ClientRpc]
    void RpcChangeSpeakerStatus(Speaker status)
    {
        displayUICtrl.displayData.statusSpeaker = status;
    }
}
