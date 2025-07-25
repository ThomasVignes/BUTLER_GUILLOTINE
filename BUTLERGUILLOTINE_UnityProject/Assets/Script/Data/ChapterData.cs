using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chapter")]
public class ChapterData : ScriptableObject
{
    [Header("General")]
    public string Name;
    public int Number;
    public CharacterData StartCharacter;
    public CharacterData[] OtherCharacters = new CharacterData[0];

    [Header("Specific")]
    public List<Conditions> conditions = new List<Conditions>();

    [Header("Inventory")]
    public Item[] items;

    [Header("Comments")]
    public CommentData CommentData;
}
