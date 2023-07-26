using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
public class FinalQuiz : MonoBehaviour{
    private AnswerPlatformsManager PlatformsManager;
    [SerializeField] private CoLearnerManager CoLearnerManager;
    [Space]
    [SerializeField] private Player Player;
    [Space]
    [SerializeField] private float GapBetweenPlatforms;
    [SerializeField] private float GapBetweenLayers;

    public List<GameConstants.Question> AvailableQuestions{get; private set;}
    private Random random = new Random();
    public int currLayerIndex{get; private set;} = -1;
    public int nextLayerIndex{get; private set;} = 0;
    public int prevLayerIndex{get; private set;} = -2;
    public int lastLayerIndex{get; private set;}
    public bool isStarted{get; private set;} = false;
    public bool isQuestionsStarted{get; private set;} = false;

    void Awake(){
        PlatformsManager = GetComponent<AnswerPlatformsManager>();
    }
    public void StartLevel(List<GameConstants.Question> questionsToUse){
        AvailableQuestions = questionsToUse;
        AvailableQuestions.RemoveAt(AvailableQuestions.Count-1);
        lastLayerIndex = AvailableQuestions.Count;
        StartCoroutine(startScriptedEvents());
        isStarted = true;
    }

    public IEnumerator startScriptedEvents(){
        GameManager.Instance.SetUITexts("Only two are Left", "");
            yield return new WaitForSeconds(GameConstants.Instance.TutorialMsgTime/2);
            GameManager.Instance.SetUITexts("Let the Final Quiz Begin", "");
            yield return new WaitForSeconds(GameConstants.Instance.TutorialMsgTime/2);
            GameManager.Instance.SetUITexts("", "");

        var playerSpawnPlatform = transform.Find("PlayerSpawnPlatform");
        var companionSpawnPlatform = transform.Find("CompanionSpawnPlatform");

        //Start spawning the Layers
        Vector3 platformEdge = playerSpawnPlatform.GetComponent<MeshRenderer>().bounds.extents;
        PlatformsManager.CreateLayers(new Vector3(0, 0, platformEdge.z) + playerSpawnPlatform.position, 
            GameConstants.Instance.FinalQuizNumIntialPlatforms, GameConstants.Instance.FinalQuizIntervalOfIncrease, GameConstants.Instance.FinalQuizPlatformIncreaseBy, GameConstants.Instance.FinalQuizMaxNumPlatforms,
            GapBetweenPlatforms, GapBetweenLayers, AvailableQuestions.Count);

        platformEdge = companionSpawnPlatform.GetComponent<MeshRenderer>().bounds.extents;
        PlatformsManager.CreateLayers(new Vector3(0, 0, platformEdge.z) + companionSpawnPlatform.position, 
            GameConstants.Instance.FinalQuizNumIntialPlatforms, GameConstants.Instance.FinalQuizIntervalOfIncrease, GameConstants.Instance.FinalQuizPlatformIncreaseBy, GameConstants.Instance.FinalQuizMaxNumPlatforms,
            GapBetweenPlatforms, GapBetweenLayers, AvailableQuestions.Count);

        Player.transform.position = playerSpawnPlatform.position + new Vector3(0, 1.5f, 0);
        CoLearnerManager.Companion.transform.position = companionSpawnPlatform.position + new Vector3(0, 1.5f, 0);
        CoLearnerManager.stopCompanionRandomMovements();
        CoLearnerManager.Companion.GetComponent<CoLearner>().Stop();
        CoLearnerManager.Companion.GetComponent<CoLearner>().LookAt(playerSpawnPlatform.position);
        
        for (int i = 0; i < GameConstants.Instance.CompanionFinalQuizMessages.Count; i++){
            var msg = GameConstants.Instance.CompanionFinalQuizMessages[i];
            GameAssets.Instance.ChatBubble.Setup(msg, Player.getFollowTargetTransform(), CoLearnerManager.Companion.transform, GameConstants.Instance.ChatBubbleTime);
            yield return new WaitForSeconds(GameConstants.Instance.ChatBubbleTime);
        }

        SetLayerTexts(nextLayerIndex);
        GameManager.Instance.SetUITexts(AvailableQuestions[nextLayerIndex].triviaQuestion, "");
        isQuestionsStarted = true;
    }

    private void SetLayerTexts(int layerIndex){
        PlatformsManager.setLayerTexts(layerIndex, AvailableQuestions[layerIndex].triviaAnswer, new string[]{"True", "False"});
    }

    public void AdvanceToNextLayer(){
        currLayerIndex = PlatformsManager.getCurrentLayerIndex();
        nextLayerIndex = currLayerIndex + 1;
        prevLayerIndex = currLayerIndex - 1;
        if (nextLayerIndex != lastLayerIndex){
            SetLayerTexts(nextLayerIndex);
            GameManager.Instance.SetUITexts(AvailableQuestions[nextLayerIndex].triviaQuestion, "");
        }

        CoLearnerManager.stopCompanionRandomMovements();

        //Current Layer Platforms
        List<GameObject> platforms = PlatformsManager.getLayerChildren(currLayerIndex+AvailableQuestions.Count, "Platform");//Layers Offset because AnswerPlatform Manager isn't design with two separate lines of layers
        Debug.Log(platforms[0].transform.parent.gameObject.name);
        Debug.Log(currLayerIndex+AvailableQuestions.Count);
        //Correct platform to be on
        int correctPlatformIndex = PlatformsManager.CorrectIndexesList[currLayerIndex+AvailableQuestions.Count];//Layers Offset because AnswerPlatform Manager isn't design with two separate lines of layers
        Vector3 correctPlatformPosition = platforms[correctPlatformIndex].transform.position;
        Debug.Log(correctPlatformIndex);
        if (nextLayerIndex != lastLayerIndex){
            CoLearnerManager.moveCompanion(GameManager.Instance.getNoisyVector(correctPlatformPosition, 1));
        } else
        {
            Debug.Log("Kill");
            int incorrectPlatformIndex = (correctPlatformIndex+1)%2;
            CoLearnerManager.moveCompanion(GameManager.Instance.getNoisyVector(platforms[incorrectPlatformIndex].transform.position, 1));
        }
    }
}
