using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class CoLearnerManager : MonoBehaviour{
    public GameObject Companion;
    [SerializeField] private float TargetPositionRadius;
    [SerializeField] private LayerMask JumpableLayerMask;
	[SerializeField] private float WalkSpeed;
    [SerializeField] private float RunSpeed;
	[SerializeField] private float JumpForce;
	[SerializeField] private float JumpCooldown;
    [Space]
    [SerializeField] private int initalNumberOfSpawns;
    public GameObject CoLearnerMovementArea{get; set;}
    private List<GameObject> CoLearnersList = new List<GameObject>();
    public int getNumCoLearners(){return CoLearnersList.Count;}
    private int count;
    void Awake(){
        CoLearnerMovementArea = GameAssets.Instance.initialMovementArea;
    }
    void Start(){
        setCoLearnerVariables(Companion);
    }

    // Update is called once per frame
    void Update(){
         count = CoLearnersList.Count;
    }

    public void spawnCoLearners(){
        StartCoroutine(spawnWithDelay(0));
    }
    public void spawnCoLearners(float delay){
        StartCoroutine(spawnWithDelay(delay));
    }
    private IEnumerator spawnWithDelay(float delay){
        for (int i = 0 ; i < initalNumberOfSpawns;i++){
            //Spawn new CoLearner
            Vector3 position = GameManager.Instance.GetRandomPositionInsidePlane(GameAssets.Instance.initialSpawnArea);
            GameObject mob = Instantiate(GameAssets.Instance.CoLearners[new Random().Next(GameAssets.Instance.CoLearners.Count)], position, new Quaternion(), transform);
            addCoLearner(mob);

            yield return new WaitForSeconds(delay);
            
        }
    }
    public void setCoLearnerVariables(GameObject coLearner){
        CoLearner script = coLearner.GetComponent<CoLearner>();
        script.TargetPositionRadius = TargetPositionRadius;
        script.JumpableLayerMask = JumpableLayerMask;
        script.WalkSpeed = WalkSpeed;
        script.RunSpeed = RunSpeed;
        script.JumpForce = JumpForce;
        script.JumpCooldown = JumpCooldown;
    }
    public void addCoLearner(GameObject CoLearner){
        setCoLearnerVariables(CoLearner);
        CoLearnersList.Add(CoLearner);
        CoLearner.GetComponent<CoLearner>().startRandomMovementLoop(CoLearnerMovementArea);
    }
    public void removeCoLearner(GameObject CoLearner){
        CoLearnersList.Remove(CoLearner);
        Destroy(CoLearner);
    }
    public void moveCoLearners(List<Vector3> newPositionList){
        if (newPositionList.Count != CoLearnersList.Count) throw new System.Exception("List sizes do not match");

        for (int i = 0 ; i < CoLearnersList.Count;i++){
            var coLearner = CoLearnersList[i];
           coLearner.GetComponent<CoLearner>().Move(newPositionList[i]);
        }
    }
    public void startCoLearnersRandomMovements(){
        for (int i = 0 ; i < CoLearnersList.Count;i++){
            var coLearner = CoLearnersList[i];
            coLearner.GetComponent<CoLearner>().startRandomMovementLoop(CoLearnerMovementArea);
        }
    }
    public void stopCoLearnersRandomMovements(){
        for (int i = 0 ; i < CoLearnersList.Count;i++){
            var coLearner = CoLearnersList[i];
            coLearner.GetComponent<CoLearner>().stopRandomMovementLoop();
        }
    }
    public void startCoLearnerRandomMovements(int CoLearnerIndex){
        var coLearner = CoLearnersList[CoLearnerIndex];
        coLearner.GetComponent<CoLearner>().startRandomMovementLoop(CoLearnerMovementArea);
    }
    public void stopCoLearnerRandomMovements(int CoLearnerIndex){
        var coLearner = CoLearnersList[CoLearnerIndex];
        coLearner.GetComponent<CoLearner>().stopRandomMovementLoop();
        
    }

    public void moveCompanion(Vector3 position){
        Companion.GetComponent<CoLearner>().Move(position);
    }
    public void startCompanionRandomMovements(){
        Companion.GetComponent<CoLearner>().startRandomMovementLoop(CoLearnerMovementArea);
    }
    public void stopCompanionRandomMovements(){
        Companion.GetComponent<CoLearner>().stopRandomMovementLoop();
    }
}
