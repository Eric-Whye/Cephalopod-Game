using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour{
    public static bool isRestarted{get; private set;}
    public void Setup(){
        Cursor.lockState = CursorLockMode.None;
        PlayerInput.Instance.useInput = false;
        gameObject.SetActive(true);
    }
    public void RestartButton(){
        isRestarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void MainMenuButton(){
        SceneManager.LoadScene("Main Menu");
    }
}
