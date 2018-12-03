using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour {

    public bool active;
    public int ownedIndex;

    private void Start() {
        active = false;
        ownedIndex = -1;
    }

    private void OnMouseDown() {
        if (active) {
            GameObject.Find("EventSystem").GetComponent<Game>().Place(this.gameObject);
                
        }
    }
}
