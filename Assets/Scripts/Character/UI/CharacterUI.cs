using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterStats))]
public class CharacterUI : MonoBehaviour {

    public GameObject uiSample;
    public Canvas targetCanvas;

    private GameObject ui_;

    void Start() {
        ui_ = Instantiate(uiSample);    
        ui_.transform.SetParent(targetCanvas.transform, false);

        var follow = ui_.GetComponent<HealthCharacterFollow>();
        follow.target = gameObject.transform;
        var updater = ui_.GetComponent<UpdateCharacterHUD>();
        updater.targetStats = GetComponent<CharacterStats>();
    }
	
    void OnDestroy() {
        if (ui_) {
            Destroy(ui_);
        }
    }
}
