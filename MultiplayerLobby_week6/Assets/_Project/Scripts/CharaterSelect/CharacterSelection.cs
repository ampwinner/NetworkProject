using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CharacterSelection : NetworkBehaviour
{
    [SerializeField] private GameObject characterSelectDisplay = default;
    [SerializeField] private Transform characterPreviewParent = default;
    [SerializeField] private TMP_Text characterNameText = default;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private Character[] characters = default;

    private int currentCharacterIndex;
    private List<GameObject> characterInstances = new List<GameObject>();

    public override void OnStartClient()
    {
        if (characterPreviewParent.childCount == 0)
        {
            foreach (var character in characters)
            {
                GameObject characterInstance =
                    Instantiate(character.ChatacterPreviewPrefab, characterPreviewParent);
                characterInstance.SetActive(false);
                characterInstances.Add(characterInstance);
            }
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;

        characterSelectDisplay.SetActive(true);
    }

    public void RightSelect()
    {
        characterInstances[currentCharacterIndex].SetActive(false);
        currentCharacterIndex = (currentCharacterIndex + 1) % characterInstances.Count;

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }

    public void LiftSelect()
    {
        characterInstances[currentCharacterIndex].SetActive(false);
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex += characterInstances.Count;
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }

    public void Select()
    {
        CmdSelect(currentCharacterIndex);
        characterSelectDisplay.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void CmdSelect(int characterIndex, NetworkConnectionToClient sender = null)
    {
        GameObject characterInstance = Instantiate(characters[characterIndex].GameplayCharacterPrefab);
        NetworkServer.Spawn(characterInstance, sender);
    }
}
