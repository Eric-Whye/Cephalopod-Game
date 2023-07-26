
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Level1 : MonoBehaviour{
    private AnswerPlatformsManager PlatformsManager;
    [SerializeField] private CoLearnerManager CoLearnerManager;
    [Space]
    [SerializeField] private Player Player;
    [Space]
    [SerializeField] private int NumLayers;
    [SerializeField] private float GapBetweenPlatforms;
    [SerializeField] private float GapBetweenLayers;

    public List<GameConstants.QuestionType> AvailableQuestionTypes{get;} = new List<GameConstants.QuestionType>();
    public List<GameConstants.Question> UsedQuestions{get;} = new List<GameConstants.Question>();
    private Dictionary<GameConstants.QuestionType, GameConstants.Question[]> qTypeToQuestionMap{get;} = new Dictionary<GameConstants.QuestionType, GameConstants.Question[]>();
    private Random random = new Random();
    public int currLayerIndex{get; private set;} = -1;
    public int nextLayerIndex{get; private set;} = 0;
    public int prevLayerIndex{get; private set;} = -2;
    void Awake(){
        PlatformsManager = GetComponent<AnswerPlatformsManager>();

        AvailableQuestionTypes.Add(GameConstants.QuestionType.ChemistrySymbols);
        AvailableQuestionTypes.Add(GameConstants.QuestionType.EnglishAnimalGroups);
        AvailableQuestionTypes.Add(GameConstants.QuestionType.RockTypes);
        for (int i = 0 ; i < AvailableQuestionTypes.Count; i++){
            qTypeToQuestionMap.Add(AvailableQuestionTypes[i], GameConstants.Instance.GetQuestions(AvailableQuestionTypes[i]));
        }
    }
    public void StartLevel(bool useWelcomeMsgs){
        Debug.Log("Level1 Started");
        StartCoroutine(WelcomeMessages(useWelcomeMsgs));
    }
    public IEnumerator WelcomeMessages(bool useWelcomeMsgs){
        if (useWelcomeMsgs){
            for (int i = 0; i < GameConstants.Instance.CompanionWelcomeMessages.Count; i++){
                //Scripted events
                if (i == 1) CoLearnerManager.spawnCoLearners(0.01f);
                if (i == 2) UI.Instance.SetLifeLineCounterActive(true);
                var msg = GameConstants.Instance.CompanionWelcomeMessages[i];
                GameAssets.Instance.ChatBubble.Setup(msg, Player.getFollowTargetTransform(), CoLearnerManager.Companion.transform, GameConstants.Instance.ChatBubbleTime);
                yield return new WaitForSeconds(GameConstants.Instance.ChatBubbleTime);
            }
        }else{
            CoLearnerManager.spawnCoLearners(0.01f);
            UI.Instance.SetLifeLineCounterActive(true);
        }
        yield return new WaitForSeconds(1f);
        CoLearnerManager.startCompanionRandomMovements();

        var spawnPlatform = transform.Find("SpawnPlatform");
        //Start spawning the Layers
        Vector3 platformEdge = spawnPlatform.GetComponent<MeshRenderer>().bounds.extents;
        PlatformsManager.CreateLayers(new Vector3(0, 0, platformEdge.z) + spawnPlatform.position, 
            GameConstants.Instance.Level1NumIntialPlatforms, GameConstants.Instance.Level1IntervalOfIncrease, GameConstants.Instance.Level1PlatformIncreaseBy, GameConstants.Instance.Level1MaxNumPlatforms,
            GapBetweenPlatforms, GapBetweenLayers, NumLayers);

        SetLayerTexts(nextLayerIndex);
        GameManager.Instance.SetUITexts(UsedQuestions[nextLayerIndex].question, "");
    }


    private void SetLayerTexts(int layerIndex){
        GameConstants.QuestionType randomQType = AvailableQuestionTypes[new Random().Next(0, AvailableQuestionTypes.Count)];
        GameConstants.Question randomQuestion = qTypeToQuestionMap[randomQType][new Random().Next(0, qTypeToQuestionMap[randomQType].Length)];
        //Ensuring unique random questions
        while (UsedQuestions.Contains(randomQuestion))
            randomQuestion = qTypeToQuestionMap[randomQType][new Random().Next(0, qTypeToQuestionMap[randomQType].Length)];
        UsedQuestions.Add(randomQuestion);

        string[] answers = GameConstants.Instance.GetAnswers(randomQType);
        PlatformsManager.setLayerTexts(layerIndex, randomQuestion.answer, answers);
    }



    public void AdvanceToNextLayer(){
        currLayerIndex = PlatformsManager.getCurrentLayerIndex();
        nextLayerIndex = currLayerIndex + 1;
        prevLayerIndex = currLayerIndex - 1;
        SetLayerTexts(nextLayerIndex);
        GameManager.Instance.SetUITexts(UsedQuestions[nextLayerIndex].question, UsedQuestions[currLayerIndex].trivia);

        CoLearnerManager.stopCoLearnersRandomMovements();
        CoLearnerManager.stopCompanionRandomMovements();



        Vector3 newCompanionPosition = new Vector3();
        //List of new positions for every CoLearner to be on
        List<Vector3> newCoLearnerPositions = new List<Vector3>();
        //Current Layer Platforms
        List<GameObject> platforms = PlatformsManager.getCurrentLayerChildren("Platform");

        /*************Gives CoLearners positions to either the correct platform, or a random incorrect one****************/
        //Correct platform to be on
        int correctPlatformIndex = PlatformsManager.CorrectIndexesList[currLayerIndex];
        Vector3 correctPlatformPosition = platforms[correctPlatformIndex].transform.position;
        //Reassigning random movement to the current platform that surviving CoLearners are standing on
        if (PlatformsManager.getCurrentInterLayerPlatform() != null)
            CoLearnerManager.CoLearnerMovementArea = PlatformsManager.getChildren(PlatformsManager.getCurrentInterLayerPlatform(), "PlatformMovementArea")[0];
        else
            CoLearnerManager.CoLearnerMovementArea = PlatformsManager.getChildren(platforms[correctPlatformIndex], "PlatformMovementArea")[0];

        //Adds a platform index to every element of list to be given to all CoLearners
        for (int i = 0; i < CoLearnerManager.getNumCoLearners(); i++){
            Vector3 position;
            //If skew rate is passed, then go to correct position and start random movements on correct platform
            if ((random.Next(99) + 1) <= (GameConstants.Instance.CoLearnerChanceToBeCorrect * 100)){
                position = correctPlatformPosition;
                CoLearnerManager.startCoLearnerRandomMovements(i);
            }else{//Else pick a random position to go to
                int rand = random.Next(platforms.Count);
                while (rand == correctPlatformIndex)
                    rand = random.Next(platforms.Count);
                position = platforms[rand].transform.position;
            }
            newCompanionPosition = GameManager.Instance.getNoisyVector(correctPlatformPosition, 1);
            newCoLearnerPositions.Add(GameManager.Instance.getNoisyVector(position, 1));
        }

        /**************Pre-Routes CoLearners behind Their given platform***************/
        //If the previous Layer had an interlayer. i.e current platform is an interlayer,
        //then move all coLearners behind their assigned platform
        if (PlatformsManager.getLayerChildren(prevLayerIndex, "InterLayer") != null){
            List<Vector3> preRoutePositions = new List<Vector3>();
            for (int i = 0; i < CoLearnerManager.getNumCoLearners(); i++){
                Vector3 position = newCoLearnerPositions[i] - PlatformsManager.getDistanceToNextPlatform();
                preRoutePositions.Add(GameManager.Instance.getNoisyVector(position, 1));
            }
            CoLearnerManager.moveCompanion(newCompanionPosition - PlatformsManager.getDistanceToNextPlatform());
            CoLearnerManager.moveCoLearners(preRoutePositions);
        }
        CoLearnerManager.moveCompanion(newCompanionPosition);
        CoLearnerManager.moveCoLearners(newCoLearnerPositions);


        /******If the next platform is an interlayer, move to the interlayer***************/
        GameObject interLayerPlatform = PlatformsManager.getCurrentInterLayerPlatform();
        if (interLayerPlatform != null){
            List<Vector3> nextPositions = new List<Vector3>();
            for (int i = 0; i < CoLearnerManager.getNumCoLearners(); i++){
                Vector3 nextPosition = newCoLearnerPositions[i] + PlatformsManager.getDistanceToNextPlatform();
                nextPositions.Add(GameManager.Instance.getNoisyVector(nextPosition, 1));
            }
            CoLearnerManager.moveCompanion(correctPlatformPosition + PlatformsManager.getDistanceToNextPlatform());
            CoLearnerManager.moveCoLearners(nextPositions);
        }
    
        CoLearnerManager.startCompanionRandomMovements();
    }
}
