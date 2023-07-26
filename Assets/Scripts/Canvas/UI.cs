using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class UI : MonoBehaviour{
    private static UI instance;
    public static UI Instance{get{return instance;}}
    private void Awake(){
        instance = this;
    }
    [SerializeField] private TextMeshProUGUI QuestionField;
    [SerializeField] private TextMeshProUGUI InfoField;
    [SerializeField] private GameObject LifelineCounter;
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject tutorial2;
    private bool useTutorial = !GameOverMenu.isRestarted;
    public void SetQuestion(string text){QuestionField.SetText(text);}
    public void SetInfo(string text){InfoField.SetText(text);}

    private TextMeshProUGUI LifelineField;
    public void SetLifeLineCounterActive(bool active){LifelineCounter.SetActive(active);}
    public void SetLifelineCounter(string text){LifelineField.SetText(text);}
    public void DecrementLifelineCounter(){
        int temp = Int32.Parse(LifelineField.text);
        LifelineField.SetText((temp-1).ToString());
    }
    void Start(){
        LifelineField = LifelineCounter.GetComponentInChildren<TextMeshProUGUI>();
        if (useTutorial == false){
            GameManager.Instance.StartGame();
            tutorial1.SetActive(false);
            tutorial2.SetActive(false);
        }
    }
    void Update() {
        if (useTutorial && (PlayerInput.Instance.GetMovementInput() != Vector3.zero || PlayerInput.Instance.GetMouseInput() != Vector3.zero)){
            StartCoroutine(setInactiveWithDelay(GameConstants.Instance.TutorialMsgTime));
            useTutorial = false;
        }
    }

    private IEnumerator setInactiveWithDelay(float seconds){
        yield return new WaitForSeconds(seconds);
        tutorial1.SetActive(false);
        tutorial2.SetActive(true);
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.StartGame();
        tutorial1.SetActive(false);
        tutorial2.SetActive(false);
    }
}
