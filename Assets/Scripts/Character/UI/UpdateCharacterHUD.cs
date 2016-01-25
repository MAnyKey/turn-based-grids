using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateCharacterHUD : MonoBehaviour, CharacterStats.IObserver {

    public Text healthText;
    public Text armorText;
    public CharacterStats targetStats;

    void Start() {
        targetStats.AddObserver(this);
        UpdateValues(targetStats);
    }
	
    void OnDestroy() {
        targetStats.RemoveObserver(this);
    }

    public void StatsChanged(CharacterStats stats) {
        UpdateValues(stats);
    }

    private void UpdateValues(CharacterStats stats) {
        healthText.text = stats.Health.ToString();
        armorText.text = stats.Armor.ToString();
    }
}
