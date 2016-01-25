using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public float distanceToParent;
    public GameObject healthUi;

    private GameObject healthUi_;
    private Transform healthUiTransform_;

    // Use this for initialization
    void Start() {
        healthUi_ = Instantiate(healthUi);
        healthUiTransform_ = healthUi_.transform;
        healthUiTransform_.parent = transform;
    }
	
    // Update is called once per frame
    void Update() {
        var toCamera = -Camera.main.transform.forward;
        healthUiTransform_.position = transform.position + toCamera * distanceToParent;
        healthUiTransform_.rotation = Quaternion.LookRotation(toCamera);
//        transform.position = parentTransform_.position + toCamera * distanceToParent;
//        transform.rotation = Quaternion.LookRotation(toCamera);
    }
}
