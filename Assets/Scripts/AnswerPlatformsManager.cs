using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using TMPro;

public class AnswerPlatformsManager : MonoBehaviour{
    public List<int> CorrectIndexesList{get;} = new List<int>();
    private List<GameObject> Layers = new List<GameObject>();
    private List<int> IDsList = new List<int>();

    private float GapBetweenLayers;
    public float platformExtentsSize{get; private set;}
    public float platformFullSize{get; private set;}
    public float interLayerExtentsSize{get; private set;}

    void Start()
    {
        platformExtentsSize = GameAssets.Instance.CorrectAnswerPlatformObject.GetComponent<MeshRenderer>().bounds.extents.z;
        platformFullSize = GameAssets.Instance.CorrectAnswerPlatformObject.GetComponent<MeshRenderer>().bounds.size.z;
        interLayerExtentsSize = GameAssets.Instance.InterLayerPlatform.GetComponent<MeshRenderer>().bounds.extents.z;
    }

    public void CreateLayers(Vector3 layerStartPosition, 
    int numInitialPlatforms, int intervalOfIncrease, int platformIncreaseBy, int maxNumPlatforms,
    float gapBetweenPlatforms, float gapBetweenLayers, int numLayers){

        GapBetweenLayers = gapBetweenLayers;
        
        /*************Layers***********/
        int currentNumPlatforms = numInitialPlatforms;
        int nextNumPlatforms = numInitialPlatforms;
        //Each layer corresponds to one question
        for (int i = 0 ; i < numLayers;i++){
            if (intervalOfIncrease > 0){
                if ((i+1) % intervalOfIncrease == 0)
                    currentNumPlatforms += platformIncreaseBy;
                if ((i+2) % intervalOfIncrease == 0)
                    nextNumPlatforms += platformIncreaseBy;
            }
            if (currentNumPlatforms >= maxNumPlatforms)
                currentNumPlatforms = maxNumPlatforms;


            /*************Layer***********/
            GameObject layer = new GameObject();
            layer.name = "layer " + i;
            layer.transform.SetParent(transform);
            layer.transform.position = layerStartPosition;
            //Calculating correct Layer positioning
            layer.transform.position += new Vector3(0,0, gapBetweenLayers + platformExtentsSize);
            layerStartPosition.z += gapBetweenLayers + platformFullSize;
            Debug.Log(platformFullSize + ", " + platformExtentsSize);
            Layers.Add(layer);

            /*************InterLayerPlatforms between layers************/
            if (nextNumPlatforms > 2){
                //Positioning
                layerStartPosition.z += gapBetweenLayers + interLayerExtentsSize;
                GameObject interPlatform = Instantiate(GameAssets.Instance.InterLayerPlatform, layerStartPosition, new Quaternion(), layer.transform);
                layerStartPosition.z += interLayerExtentsSize;

                //Sizing
                float targetSize = (float)((platformFullSize*nextNumPlatforms) +((gapBetweenPlatforms)*(nextNumPlatforms-1)));
                float xScale = targetSize/interPlatform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                interPlatform.transform.localScale = new Vector3(xScale, interPlatform.transform.localScale.y, interPlatform.transform.localScale.z);
            }

            //Deciding which platform for current layer is the correct one
            CorrectIndexesList.Add(new Random().Next(0, currentNumPlatforms));

            /*************Platforms************/
            //position for first platform   
            Vector3 platformStartPosition = new Vector3(layer.transform.position.x + ((-(gapBetweenPlatforms+platformFullSize)/2f) * (currentNumPlatforms-1)), layer.transform.position.y, layer.transform.position.z);
            for (int j = 0; j < currentNumPlatforms; j++){
                GameObject platformObject;
                if (j == CorrectIndexesList[CorrectIndexesList.Count-1])platformObject = GameAssets.Instance.CorrectAnswerPlatformObject;
                else platformObject = GameAssets.Instance.WrongAnswerPlatformObject;

                GameObject platform = Instantiate(platformObject, platformStartPosition, new Quaternion(), transform);

                //Add Listener for when player is detected on corerct platform
                if (j == CorrectIndexesList[CorrectIndexesList.Count-1])
                    platform.GetComponent<FractureObject>().PlayerDetectedEvent.AddListener(playerDetected);
                //Set each platform text to nothing
                platform.GetComponentInChildren<TextMeshPro>().SetText("");
                //
                platformStartPosition += new Vector3(gapBetweenPlatforms + platformFullSize, 0, 0);
                //Assign given parent
                platform.transform.parent = layer.transform;
            }
        }
    }

    private void playerDetected(int id){
        //If player is detecteed on a unique platform
        if (!IDsList.Contains(id)){
            IDsList.Add(id);
            GameManager.Instance.isPlayerOnCorrectPlatform = true;
        }
    }


    public void setLayerTexts(int layerIndex, string correctText, string[] fodderTexts){
        //Getting text Meshes
        TextMeshPro[] textsMeshes = Layers[layerIndex].GetComponentsInChildren<TextMeshPro>();

        List<string> usedTexts = new List<string>();
        //Changing answers' texts
        for (int i = 0 ; i < textsMeshes.Length;i++){
            if (i == CorrectIndexesList[layerIndex])//If current index corresponds with current layer's correct platform index, then set the text to the correct answer
                textsMeshes[i].SetText(correctText);
            else{
                string randomText = fodderTexts[new Random().Next(0, fodderTexts.Length)];
                //Making sure to use unique texts for incorrect textMeshes
                while (usedTexts.Contains(randomText) || randomText == correctText)
                    randomText = fodderTexts[new Random().Next(0, fodderTexts.Length)];
                usedTexts.Add(randomText);
                textsMeshes[i].SetText(randomText);
            }
        }
    }

    public int getCurrentLayerIndex(){return IDsList.Count-1;}
    public GameObject getCurrentInterLayerPlatform(){
        var temp = getCurrentLayerChildren("InterLayer");
        if (temp == null) return null;
        else return temp[0];
    }
    public List<GameObject> getCurrentLayerChildren(string tag){
        return getLayerChildren(getCurrentLayerIndex(), tag);
    }
    public List<GameObject> getLayerChildren(int layerIndex, string tag){
        if (layerIndex < 0) return null;
        return getChildren(Layers[layerIndex], tag);
    }
    public List<GameObject> getChildren(GameObject obj, string tag){
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < obj.transform.childCount; i++){
            var child = obj.transform.GetChild(i);
            if (child.tag == tag)
                children.Add(child.gameObject);
            else if (tag == "")
                children.Add(child.gameObject);
            
        } 
        if (children.Count == 0) return null;
        return children;
    }
    public Vector3 getDistanceToNextPlatform(){
        return new Vector3(0, 0, GapBetweenLayers+interLayerExtentsSize+platformExtentsSize);
    }
}
