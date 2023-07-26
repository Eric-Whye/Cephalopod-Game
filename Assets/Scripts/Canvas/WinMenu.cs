using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour{
    public void Setup(){
        Cursor.lockState = CursorLockMode.None;
        PlayerInput.Instance.useInput = false;
        gameObject.SetActive(true);
    }
    public void PlayAgainButton(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void MainMenuButton(){
        SceneManager.LoadScene("Main Menu");
    }
}
