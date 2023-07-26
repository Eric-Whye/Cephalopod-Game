using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Random = System.Random;

public class GameManager : MonoBehaviour{
    private static GameManager instance;
    public static GameManager Instance{get{return instance;}}
    private void Awake(){
        instance = this;
    }

    [SerializeField] private CoLearnerManager CoLearnerManager;
    [SerializeField] private Level1 Level1;
    [SerializeField] private FinalQuiz FinalQuiz;
    [SerializeField] private GameOverMenu GameOverMenu;
    [SerializeField] private WinMenu WinMenu;
    [Space]
    [SerializeField] private Player Player;
    private GameObject Companion;
    private Random random = new Random();
    public bool isPlayerOnCorrectPlatform{get; set;} = false;
    
    void Start(){
        LifelineCount = GameConstants.Instance.NumLifeLines;
        UI.Instance.SetLifelineCounter(LifelineCount.ToString());
        Companion = CoLearnerManager.Companion;
    }

    public void StartGame(){
        if (!GameOverMenu.isRestarted)
            Level1.StartLevel(true);
        else
            Level1.StartLevel(false);
    }
    public void GameOver(){
        GameOverMenu.Setup();
    }
    
    private GameObject currentChatBubble;
    private int LifelineCount;
    void Update(){
        //If player wants to use lifeline
        if (Player.GetComponent<Player>().usinglifeLine){
            if (LifelineCount != 0 && currentChatBubble == null){
                string latestCorrectAnswer;
                if (!FinalQuiz.isStarted){
                    latestCorrectAnswer = Level1.UsedQuestions[Level1.nextLayerIndex].answer;
                    var msg = GameConstants.Instance.getCompanionLifelineMessage(latestCorrectAnswer);
                    currentChatBubble = GameAssets.Instance.ChatBubble.Setup(msg, Player.getFollowTargetTransform(), CoLearnerManager.Companion.transform);
                    LifelineCount--;
                    UI.Instance.SetLifelineCounter(LifelineCount.ToString());
                }
                else if (FinalQuiz.isStarted && FinalQuiz.isQuestionsStarted){
                    latestCorrectAnswer = FinalQuiz.AvailableQuestions[FinalQuiz.nextLayerIndex].triviaAnswer;
                    var msg = GameConstants.Instance.getCompanionLifelineMessage(latestCorrectAnswer);
                    currentChatBubble = GameAssets.Instance.ChatBubble.Setup(msg, Player.getFollowTargetTransform(), CoLearnerManager.Companion.transform);
                    LifelineCount--;
                    UI.Instance.SetLifelineCounter(LifelineCount.ToString());
                }
            }
        }
        if (isPlayerOnCorrectPlatform){
            //ChatBubble
            if (currentChatBubble != null){
                Destroy(currentChatBubble);
                currentChatBubble = null;
                Player.GetComponent<Player>().usinglifeLine = false;
            }
        }
        //***********When player advances through the layers of platforms, CoLearners advance with him.*********/
        if (isPlayerOnCorrectPlatform && !FinalQuiz.isStarted){
            isPlayerOnCorrectPlatform = false;
            Level1.AdvanceToNextLayer();
        } else if(isPlayerOnCorrectPlatform && FinalQuiz.isStarted){
            isPlayerOnCorrectPlatform = false;
            FinalQuiz.AdvanceToNextLayer();
        }
        //Check for Final Quiz
        if (CoLearnerManager.getNumCoLearners() == 0 && !FinalQuiz.isStarted && Level1.currLayerIndex >= 0){
            if (currentChatBubble != null){
                Destroy(currentChatBubble);
                currentChatBubble = null;
                Player.GetComponent<Player>().usinglifeLine = false;
            }
            FinalQuiz.StartLevel(Level1.UsedQuestions);
        }
    }
    public void KillCoLearner(GameObject CoLearner){
        if (CoLearner == Companion){
            Destroy(CoLearner);
            WinMenu.Setup();
            return;
        }
        CoLearnerManager.removeCoLearner(CoLearner);
    }
    public void SetUITexts(string questionText, string infoText){
        UI.Instance.SetQuestion(questionText);
        UI.Instance.SetInfo(infoText);
    }
    public Vector3 getNoisyVector(Vector3 vector, float error){
        float getNoise(float error){
            if (random.Next(1) == 1)
                return (float)random.NextDouble()*error;
            else
                return (float)random.NextDouble()*-error;
        }
        Vector3 newVector = new Vector3();
        newVector.x = vector.x + getNoise(error);
        newVector.y = vector.y + getNoise(error);
        newVector.z = vector.z + getNoise(error);
        return newVector;
    }

    public List<Vector3> GetCornersFromPlane(GameObject plane){
        List<Vector3> corners = new List<Vector3>();
        Vector3[] vertices = plane.GetComponent<MeshFilter>().mesh.vertices;
        corners.Add(plane.transform.TransformPoint(vertices[0]));
        corners.Add(plane.transform.TransformPoint(vertices[10]));
        corners.Add(plane.transform.TransformPoint(vertices[110]));
        corners.Add(plane.transform.TransformPoint(vertices[120]));
        return corners;
    }
    public Vector3 GetRandomPositionInsidePlane(GameObject plane){
        List<Vector3> corners = GetCornersFromPlane(plane);
        float minX = (corners.Aggregate((v, next) => next.x < v.x ? next : v)).x;
        float maxX = (corners.Aggregate((v, next) => next.x > v.x ? next : v)).x;
        float minY = (corners.Aggregate((v, next) => next.y < v.y ? next : v)).y;
        float maxY = (corners.Aggregate((v, next) => next.y > v.y ? next : v)).y;
        float minZ = (corners.Aggregate((v, next) => next.z < v.z ? next : v)).z;
        float maxZ = (corners.Aggregate((v, next) => next.z > v.z ? next : v)).z;
        System.Random random = new System.Random();
        double randX = (random.NextDouble()*(maxX-minX))+(minX-0);
        double randY = (random.NextDouble()*(maxY-minY))+(minY-0);
        double randZ = (random.NextDouble()*(maxZ-minZ))+(minZ-0);
        return new Vector3((float)randX, (float)randY, (float)randZ);
    }
}