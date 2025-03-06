using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/MinigleMinigame/DialogueBatch")]
public class DialogueBatch : ScriptableObject
{
    public string batchName;
    public string[] lines;
    public int linesBeforeRepeats;
}
