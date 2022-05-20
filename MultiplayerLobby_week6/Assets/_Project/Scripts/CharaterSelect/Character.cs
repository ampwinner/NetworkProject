using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character",menuName = "Character Selection/Character")]

public class Character : ScriptableObject
{
    [SerializeField] private string characterName = default;
    [SerializeField] private GameObject chatacterPreviewPrefab = default;
    [SerializeField] private GameObject gameplayCharacterPrefab = default;

    public string CharacterName => characterName;
    public GameObject ChatacterPreviewPrefab => chatacterPreviewPrefab;
    public GameObject GameplayCharacterPrefab => gameplayCharacterPrefab;
}
