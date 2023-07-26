using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;

public class ChatBubble : MonoBehaviour
{
    private Transform followTargetTransform;
    private SpriteRenderer backgroundSpriteRenderer;
    private TextMeshPro textMeshPro;

    private void Awake() {
        backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }
    void Update(){
        transform.rotation = followTargetTransform.rotation;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    public GameObject Setup(string text, Transform followTargetTransform, Transform parent, float seconds) {
        var obj = Setup(text, followTargetTransform, parent);
        Destroy(obj, seconds);
        return obj;
    }
    public GameObject Setup(string text, Transform followTargetTransform, Transform parent) {
        GameObject obj = Instantiate(gameObject, parent);
        var script = obj.GetComponent<ChatBubble>();
        script.followTargetTransform = followTargetTransform;
        script.textMeshPro.SetText(text);
        script.textMeshPro.ForceMeshUpdate();

        //Text Padding
        Vector2 padding = new Vector2(1f, 0.5f);
        Vector2 textSize = script.textMeshPro.GetRenderedValues(false);
        script.backgroundSpriteRenderer.size = textSize + padding;

        //Correct positioning of speech bubble
        script.transform.localPosition = new Vector3(0, 2);

        return obj;
    }
}
