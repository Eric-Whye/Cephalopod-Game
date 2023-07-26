using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour{
    [SerializeField] private TMPro.TMP_Dropdown DifficultyDropdown;
    public void ChangeDifficulty(){
        GameConstants.Difficulty = DifficultyDropdown.value;
    }
}
