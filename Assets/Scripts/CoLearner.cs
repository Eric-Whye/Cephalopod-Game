using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
using System;

public class CoLearner : MonoBehaviour
{
    private Vector3 MovementInput = new Vector3();
    public LinkedList<Vector3> TargetPositions{get;} = new LinkedList<Vector3>();
    [SerializeField] private Rigidbody RigidBody;
    [SerializeField] private GameObject CharacterModel;
    [SerializeField] private Transform FeetTransform;
    [Space]
	[SerializeField] private Animator Animator;
    [Space]
    public float TargetPositionRadius;
    public LayerMask JumpableLayerMask;
	[Space]
	public float WalkSpeed;
    public float RunSpeed;
	public float JumpForce;
	public float JumpCooldown;
    public GameObject AllowedMovementArea {get; set; }
    private bool allowedMovement = true;
    private Vector3 moveVector;
    public bool isWalking;
    private int count;
    void Awake(){
    }
    void Update(){
        count = TargetPositions.Count;
        /*************Rotation************/
        if (!Mathf.Approximately(MovementInput.x, 0) || !Mathf.Approximately(MovementInput.z, 0)){
            Quaternion rotation = Quaternion.LookRotation(new Vector3(RigidBody.velocity.x, 0, RigidBody.velocity.z));
            CharacterModel.transform.rotation = rotation;
        }
        /*************Animation************/
		Animator.SetBool("isRunning", IsRunning());
		Animator.SetBool("isJumping", IsJumping());
		Animator.SetBool("isFalling", IsFalling());
    }

    //For physics updates
    private void FixedUpdate() {
        if (TargetPositions.Count == 2)
            isWalking = false;
        else
            isWalking = true;
        /*************XZ Movements************/
        //If there are target positions to go to
        if (TargetPositions.Count > 0){
            Vector3 currentTarget = TargetPositions.First();
            //If CoLearner is relatively within range of target, stop moving
            if (Mathf.Abs(currentTarget.x - transform.position.x) > TargetPositionRadius || Mathf.Abs(currentTarget.z - transform.position.z) > TargetPositionRadius){
                if (allowedMovement && !IsFalling())
                    MovementInput = new Vector3(currentTarget.x - transform.position.x, 0, currentTarget.z - transform.position.z).normalized;
            } else{
                MovementInput = Vector3.zero;
                TargetPositions.RemoveFirst();
                Wait(new System.Random().Next(500, 2000));
            }
        }
        if (isWalking)
            moveVector = MovementInput.normalized * WalkSpeed;
        else 
            moveVector = MovementInput.normalized * RunSpeed;
        RigidBody.velocity = new Vector3(moveVector.x, RigidBody.velocity.y, moveVector.z);
    }
    public void Wait(int milliseconds){
        Thread thread = new Thread(new ThreadStart(wait));
        thread.Start();
        void wait(){
            allowedMovement = false;
            Thread.Sleep(milliseconds);
            allowedMovement = true;
        }
    }
    public void Stop(){
        TargetPositions.Clear();
        MovementInput = Vector3.zero;
    }

    public void Move(Vector3 position){
        TargetPositions.AddLast(position);
    }
    public void LookAt(Vector3 position){
        CharacterModel.transform.LookAt(position);
    }

    //Jump Handler
    private bool jumpOnCooldown = false;
    public void Jump(){
		if (!jumpOnCooldown){
			jumpOnCooldown = true;
			if (Physics.CheckSphere(FeetTransform.position, 0.1f, JumpableLayerMask))
			    RigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
			
			Thread thread = new Thread(new ThreadStart(wait));
			thread.Start();
		}

		void wait(){
			Thread.Sleep((int)(1000*JumpCooldown));
			jumpOnCooldown = false;
		}
    }

    void OnTriggerEnter(Collider col){
        if (col.tag == "DeathTrigger"){
            GameManager.Instance.KillCoLearner(gameObject);
        }
        //If CoLearner reaches the edge of a platform, jump
        if (IsRunning() && col.tag == "PlatformEdge")
            Jump();
    }

    private IEnumerator randomiseTargetPosition(){
        while (true){
            if (TargetPositions.Count==0){
                this.Move(GameManager.Instance.GetRandomPositionInsidePlane(AllowedMovementArea));
                yield return new WaitForSeconds(2f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private Coroutine currrentRunningCoroutine;
    public void startRandomMovementLoop(GameObject AllowedMovementArea){
        this.AllowedMovementArea = AllowedMovementArea;
        if (currrentRunningCoroutine != null) StopAllCoroutines();
        currrentRunningCoroutine = StartCoroutine(randomiseTargetPosition());
    }
    public void stopRandomMovementLoop(){
        if (currrentRunningCoroutine != null){
            StopAllCoroutines();
            StopCoroutine(currrentRunningCoroutine);
            currrentRunningCoroutine = null;
        }
    }
    public bool IsRunning(){return MovementInput.x != 0 || MovementInput.z != 0;}
	public bool IsJumping(){return RigidBody.velocity.y > 0.1;}
	public bool IsFalling(){return RigidBody.velocity.y < -1;}
}
