using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // UI Stuff
    [Header("UI Stuff")]
    public Text levelNumber;
    public Text levelName;
    public Text currentTurnLabel;
    public Text maxTurnLabel;
    public GameObject levelBackground;

    // Levels
    [Header("Levels")]
    public List<Level> allLevels = new List<Level>();
    public Level currentLevel; // TODO Serialize
    public int currentLevelID; // TODO Serialize
    public Transform[] crystalHolders; // TODO this should come from the object
    //public List<Transform> crystalHolders = new List<Transform>();

    //public List<GameObject> holders = new List<GameObject>();
    GameObject[] holders;

    // Board
    [Header("Board")]
    List<int> slotsFree = new List<int>();
    public List<int> slotsInUse = new List<int>();
    public List<GameObject> crystals = new List<GameObject>();
    bool roundFinished;

    [Header("Prefabs")]
    public GameObject crystalPrefab;
    
    // The pattern for this level (Random for now)
    public List<int> slotPattern = new List<int>();

    // Internal
    GameObject thisCrystal;

    // Player Stuff
    [Header("Player")]
    int playerMove = 0;
    public List<int> playerMoves = new List<int>();
    int currentTurn;

    void Start() {

        // Temp - Level Initialization - Set Level to 1st TODO see above
        currentLevelID = 0;
        currentLevel = allLevels[currentLevelID];

        // Setup Round
        roundFinished = false;
        currentTurn = 1;

        // Setup UI
        levelName.text = currentLevel.levelName;
        levelNumber.text = currentLevel.levelNumber.ToString();
        maxTurnLabel.text = currentLevel.maxTurns.ToString();
        Debug.Log("Background is: " + currentLevel.levelBackground);
        //levelBackground.GetComponent<Image>().sprite = currentLevel.levelBackground;
        UpdateUI();

        // Find the holders
        //holders = GameObject.FindGameObjectsWithTag("crystalHolder");
        /*
        holders = GameObject.FindGameObjectsWithTag("crystalHolder");
        for (int i = 0; i < holders.Length; i++) {
            Debug.Log(holders[i]);
            crystalHolders[i] = holders[i].GetComponent<Transform>();
        }
        */

        InitializeFreeSlots();
        SetupCrystalsRandom();

        CreatePattern(10);
        PlayPattern();

    }


    public void PlayerTurn(int crystalID) {

        if (slotPattern[playerMove] == crystalID) {
            Debug.Log("Player picked correct, advance");
            playerMove++;

            // TODO move this to a max turn situation
            if (playerMove == currentTurn) {
                Debug.Log("We reached the end of current NPC knowledge, advance turn");

                // End Check
                if (currentTurn == currentLevel.maxTurns) {
                    roundFinished = true;
                    SceneManager.LoadScene("Win");
                } else {
                    currentTurn++;
                    UpdateUI();
                    PlayPattern();
                }                
            }
        } else {
            roundFinished = true;
            SceneManager.LoadScene("Lose");

        }        
    }


    // Update the UI between Turns
    void UpdateUI() {
        currentTurnLabel.text = currentTurn.ToString();
    }
       
    
    // Setup the list with slots free from level SO
    private void InitializeFreeSlots() {
         for (int i = 0; i <= currentLevel.numSlots; i++) {
            slotsFree.Add(i);
        }
    }

    // Put the crystals in random slots
    private void SetupCrystalsRandom() {
        // Grab some random free slots for crystals
        Debug.Log("TO USE: " + currentLevel.numCrystals);

        for (int i = 0; i < currentLevel.numCrystals; i++) {

            // Pick a random slot to add a crystal to then add it
            int holderToUse = Random.Range(0, slotsFree.Count);
            Debug.Log("Max you can use: " + slotsFree.Count);
            int holderValue = slotsFree[holderToUse];

            // Add the Slot in use to the List
            slotsInUse.Add(holderValue);

            // Notify the holder that its taken for later use
            crystalHolders[holderValue].GetComponent<CrystalHolder>().isTaken = true;

            // Remove that option from the list of free slots so it can't be reused
            slotsFree.RemoveAt(holderToUse);

            // Instantiate the Crystal
            GameObject thisCrystal = (GameObject)Instantiate(crystalPrefab, crystalHolders[holderValue].position, Quaternion.identity);
            thisCrystal.transform.SetParent(crystalHolders[holderValue], true);
            thisCrystal.GetComponent<SpriteRenderer>().color = thisCrystal.GetComponent<Crystal>().normalColor;
            thisCrystal.GetComponent<Crystal>().crystalID = i;
            crystals.Add(thisCrystal);

        }
    }

    // Create the NPC pattern based on the level.  TODO make this not hardcoded
    void CreatePattern(int numberOfTicks) {

        // Create the patterns
        for (int i = 0; i < numberOfTicks; i++) {

            // Pick a random Square to be the target
            int crystalNextSpot = Random.Range(0, slotsInUse.Count);
            
            // Add entry to the memory list
            slotPattern.Add(crystalNextSpot);

        }
    }


    // Play the pattern for the PC
    void PlayPattern() {

        // Make crystals untouchable
        ChangeCrystalState(false);

        StartCoroutine(Blink());

    }

    IEnumerator Blink() {

        for (int i = 0; i < currentTurn; i++) {
            yield return new WaitForSeconds(1);
            crystals[slotPattern[i]].GetComponent<Crystal>().BlinkCrystal();
        }

        InitializePlayerTurn();
        ChangeCrystalState(true);
    }


    // Handles setting up the game to compare player input to the NPC pattern
    void InitializePlayerTurn() {
        Debug.Log("NPC has taken its turn - player goes now (new turn).  Setting up data.");
        playerMoves.Clear();
        playerMove = 0;

    }

    void ChangeCrystalState(bool setValue) {
        for (int i = 0; i < crystals.Count; i++) {
            crystals[i].GetComponent<Crystal>().isClickable = setValue;
        }
    }

}
