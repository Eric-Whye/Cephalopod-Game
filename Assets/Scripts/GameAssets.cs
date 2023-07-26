using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour{
    private static GameAssets instance;
    public static GameAssets Instance{get{return instance;}}
    void Awake(){instance = this;}
    [field: Header("Platforms")]
    [field: SerializeField] public GameObject WrongAnswerPlatformObject{ get; private set; }
    [field: SerializeField] public GameObject CorrectAnswerPlatformObject{ get; private set; }
    [field: SerializeField] public GameObject InterLayerPlatform{ get; private set; }
    [field: Header("CoLearners")]
    [field: SerializeField] public List<GameObject> CoLearners{ get; private set; }
    [field: SerializeField] public GameObject initialSpawnArea{ get; private set; }
    [field: SerializeField] public GameObject initialMovementArea{ get; private set; }
    [field: Header("Misc")]
    [field: SerializeField] public ChatBubble ChatBubble{ get; private set; }

}
