using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour{
    private static PlayerInput instance;
    public static PlayerInput Instance{get{return instance;}}
    void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    public bool useInput{get; set;} = false;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        useInput = true;
    }

    public Vector3 GetMovementInput(){
        if (useInput == false) return new Vector3();
        Vector3 movementVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetKey("space"))
            movementVector.y = 1;
        else 
            movementVector.y = 0;
        return movementVector;
    }
    private Vector3 lastMouseVector;
    public Vector3 GetMouseInput(){
        if (useInput == false) return new Vector3();
        return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
    }
    public string lifeLineKey{get;} = "f";
    public bool getKeyPress(string key){
        return Input.GetKey(key);
    }
}
