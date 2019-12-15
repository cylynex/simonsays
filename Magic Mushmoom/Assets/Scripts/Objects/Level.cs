using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Objects", menuName = "Objects/Add Level")]
public class Level : ScriptableObject {

    [Header("Board Setup")]
    public string levelName;
    public string levelNumber;
    public int numCrystals;
    public int numSlots;
    public int maxTurns;
    public Sprite levelBackground;

    
    [Header("Reference Only")]
    //public Item[] startingItems;
    //[TextArea(5, 5)]
    public string dummy;

}