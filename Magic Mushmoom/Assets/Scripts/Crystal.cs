using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour {

    public int crystalID;
    public Color normalColor;
    public Color highlightColor;
    public bool isClickable = false;
    public GameController gc;

    void Start() {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void OnMouseDown() {
        if (isClickable) {
            SetColor(highlightColor);
            StartCoroutine(UnsetColor());

            // Notify the GC that a crystal was clicked
            gc.PlayerTurn(crystalID);

        } else {
            Debug.Log("Crystals are not clickable");
        }
    }

    IEnumerator UnsetColor() {
        yield return new WaitForSeconds(0.5f);
        SetColor(normalColor);
    }


    void SetColor(Color colorToUse) {
        this.GetComponent<SpriteRenderer>().color = colorToUse;
    }


    public void BlinkCrystal() {
        SetColor(highlightColor);
        StartCoroutine(UnBlinkCrystal());
    }

    IEnumerator UnBlinkCrystal() {
        yield return new WaitForSeconds(0.5f);
        SetColor(normalColor);
    }

}