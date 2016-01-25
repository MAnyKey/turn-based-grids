using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject obj;

    private GameObject val;

    // Use this for initialization
    void Start() {
        val = Instantiate(obj);
    }
}
