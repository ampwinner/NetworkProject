using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Speaker { idle,use,mute}

public class DisplayData : MonoBehaviour
{
    public RawImage steamImage;
    public TextMeshProUGUI textName;
    public CanvasGroup canvasGroup;

    [Header("Speaker")]
    public Image speakerImage;
    public Sprite idleSpeaker, useSpeaker, muteSpeaker;
    public Speaker statusSpeaker;
    public Speaker StatusSpeaker
    {
        set
        {
            statusSpeaker = value;
            switch (statusSpeaker)
            {
                case Speaker.idle:
                    speakerImage.color = Color.white;
                    speakerImage.sprite = idleSpeaker;
                    break;
                case Speaker.use:
                    speakerImage.color = Color.green;
                    speakerImage.sprite = useSpeaker;
                    break;
                case Speaker.mute:
                    speakerImage.color = Color.red;
                    speakerImage.sprite = muteSpeaker;
                    break;
            }
        }
        get
        {
            return statusSpeaker;
        }
    }

    public Texture SteamImage
    {
        set
        {
            steamImage.texture = value;
        }
    }

    public string TextName
    {
        set
        {
            textName.text = value;
        }
    }
}
