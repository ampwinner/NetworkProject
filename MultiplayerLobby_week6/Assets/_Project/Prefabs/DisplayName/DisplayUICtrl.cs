using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class DisplayUICtrl : MonoBehaviour
{
    public DisplayData prefebUI;
    public Transform transformHead;

    private DisplayData uiUse;

    public DisplayData displayData
    {
        get
        {
            return uiUse;
        }
    }

    private bool AvatarReceived;
    private GameObject objPlayer;
    private PlayerObjCtrl playerObj;

    [SerializeField]
    private Vector3 offset = new Vector3(0, .5f, 0);
    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        ImageLoaded = ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        playerObj = GetComponent<PlayerObjCtrl>();
    }

    public void SetPlayerValues()
    {
        uiUse = Instantiate(prefebUI, FindObjectOfType<Canvas>().transform) as DisplayData;
        uiUse.TextName = playerObj.PlayerName;
        objPlayer = GameObject.Find("LocalGamePlayer");
        //transformHead = transform.GetChild(0);

        if (!AvatarReceived) { GetPlayerIcon(); }
    }


    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerObj.PlayerSteamID);
        if (ImageID == -1) { return; }
        uiUse.SteamImage = GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }
    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == playerObj.PlayerSteamID) //us
        {
            uiUse.SteamImage = GetSteamImageAsTexture(callback.m_iImage);
        }
        else //another player
        {
            return;
        }
    }

    public void Update()
    {
        if (uiUse)
        {
            uiUse.transform.position = Camera.main.WorldToScreenPoint(transformHead.position + offset);

            //Get Distance from the player
            float dist = 1 / Vector3.Distance(transform.position, objPlayer.transform.position) * 2;

            //Set Alpha to CanvasGroup
            float alpha = dist - 1.5f;

            //Deform UI Size
            dist = Mathf.Clamp(dist, 0.5f, 1f);
            uiUse.transform.localScale = new Vector3(dist, dist, 0);
        }
    }

    private void OnEnable()
    {
        if (uiUse)
        {
            uiUse.canvasGroup.alpha = 1;
        }
    }

    private void OnDisable()
    {
        if (uiUse)
        {
            uiUse.canvasGroup.alpha = 0;
        }
    }
}
