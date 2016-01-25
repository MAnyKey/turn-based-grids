using UnityEngine;
using System.Collections;

public class HealthCharacterFollow : MonoBehaviour {

    public Transform target;


    // Update is called once per frame
    void Update() {
        var pos = Camera.main.WorldToScreenPoint(target.position);
        transform.position = pos;
    }
}
