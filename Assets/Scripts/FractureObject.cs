using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class FractureObject : MonoBehaviour
{
    [SerializeField] private GameObject fracturedObject;
    [SerializeField] private bool useCollisionBreaking = true;
    [SerializeField] private float destroyFracturedObjectTime;
    [SerializeField] private float destroyPhysicsTime;
    [SerializeField] private float breakingForce;
    [SerializeField] private AudioClip breakingSound;
    //To notify the platformManager of the local event being triggered
    public UnityEvent<int> PlayerDetectedEvent{get;} = new UnityEvent<int>();

    public void Break(){
        GameObject frac = Instantiate(fracturedObject, transform.position, transform.rotation, transform.parent);

        foreach(Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>()){
            Vector3 force = (rb.transform.position - transform.position).normalized * breakingForce;
            rb.AddForce(force);
        }

        //Audio
        if (breakingSound != null){
            GetComponent<AudioSource>().clip = breakingSound;
            GetComponent<AudioSource>().Play();
        }

        //Destroy physics of fractured pieces after a time
        foreach(Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
            Destroy(frac, destroyPhysicsTime);
        foreach(Collider col in frac.GetComponentsInChildren<Collider>())
            Destroy(col, destroyPhysicsTime);

        //Destroy Fractured Pieces after a certain amount of time
        Destroy(frac, destroyFracturedObjectTime);

        //Destroy the current (non-fractured) object
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());
        Destroy(gameObject, breakingSound.length);
    }

    
    void OnCollisionEnter(Collision col){
        if (useCollisionBreaking == true)
            Break();
        
        if (col.gameObject.name.Equals("Player")){
            PlayerDetectedEvent.Invoke(gameObject.GetInstanceID());
        }
    }
    void OnTriggerEnter(Collider col){
        if (useCollisionBreaking == true)
            Break();
    }
}