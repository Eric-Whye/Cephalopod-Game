using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour{
    private static BGMusic instance;
    void Awake(){
         if (instance != null){
            instance.volumeSlider = volumeSlider;
            Destroy(gameObject);
            return;
        }
        instance = this;

        volumeSlider.value = instance.bgmusic.volume;

        if (!bgmusic.isPlaying)
            bgmusic.Play();

        DontDestroyOnLoad(gameObject);
    }
    [SerializeField] private AudioSource bgmusic;
    [SerializeField] private UnityEngine.UI.Slider volumeSlider;
    public void onVolumeChange(){
        bgmusic.volume = volumeSlider.value;
    }
}
