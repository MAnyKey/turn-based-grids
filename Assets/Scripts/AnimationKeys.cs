using UnityEngine;
using System.Collections;

public class AnimationKeys : MonoBehaviour {

    private Animation animation_;

	// Use this for initialization
	void Start () {
        animation_ = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("1")) {
            animation_.CrossFade("run");
        } else if (Input.GetKey("2")) {
            animation_.CrossFade("attack");
        } else if (Input.GetKey("3")) {
            animation_.CrossFade("walk");
        } else if (Input.GetKey("4")) {
            animation_.CrossFade("jump");
        } else {
            animation_.CrossFade("idle");
        }
    }
}
