using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class Player : MonoBehaviour{
	private Vector3 PlayerMovementInput = new Vector3();
	private Vector2 PlayerMouseInput;
	[SerializeField] private Rigidbody PlayerBody;
	[SerializeField] private Transform FollowTargetTransform;
	[SerializeField] private Transform PlayerFeetTransform;
	[Space]
	[SerializeField] private LayerMask JumpableLayerMask;
	[Space]
	[SerializeField] private float MoveSpeed;
	[SerializeField] private float JumpForce;
	[SerializeField] private float JumpCooldown;
	[Space]
	[SerializeField] private float MouseSensitivity;
	[SerializeField] private float MinViewAngle;
	[SerializeField] private float MaxViewAngle;
	[Space]
	[SerializeField] private Animator Animator;

	private Vector3 moveVector;

	void Start(){
		//FollowTargetTransform.parent = null;
	}
	
	void Update(){
		/*************Translating Inputs to movement************/
		PlayerMovementInput = PlayerInput.Instance.GetMovementInput();//Acquire movement input from player
		PlayerMouseInput = PlayerInput.Instance.GetMouseInput();//Acquire Mouse input from player

		//XZ player Movement
		moveVector = transform.TransformDirection(PlayerMovementInput).normalized * MoveSpeed;

		//Jump Handler
		if (PlayerMovementInput.y > 0){
			Jump();
		}
		//Life line Handler
		if (PlayerInput.Instance.getKeyPress(PlayerInput.Instance.lifeLineKey)){
			DoLifeline();
		}

		/*************Animation************/
		Animator.SetBool("isRunning", IsRunning());
		Animator.SetBool("isJumping", IsJumping());
		Animator.SetBool("isFalling", IsFalling());
	}
	//For physics updates
	private void FixedUpdate() {
		MoveCamera();
		PlayerBody.velocity = new Vector3(moveVector.x, PlayerBody.velocity.y, moveVector.z);
	}

	public bool usinglifeLine{get; set;} = false;
	private void DoLifeline(){
		if (!usinglifeLine){
			usinglifeLine = true;
		}
	}
	private bool jumpOnCooldown = false;
	private void Jump(){
		if (!jumpOnCooldown){
			jumpOnCooldown = true;
			if (Physics.CheckSphere(PlayerFeetTransform.position, 0.1f, JumpableLayerMask)){
				PlayerBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
			}
			Thread thread = new Thread(new ThreadStart(wait));
			thread.Start();
		}

		void wait(){
			Thread.Sleep((int)(1000*JumpCooldown));
			jumpOnCooldown = false;
		}
	}

	private void MoveCamera(){
		/*************Translation************/
		//FollowTargetTransform.localPosition = transform.position;

		/*************Rotation************/
		//rotate camera horizontally around target
		FollowTargetTransform.rotation *= Quaternion.AngleAxis(PlayerMouseInput.x * MouseSensitivity, Vector3.up);
		//rotate camera vertically
		FollowTargetTransform.rotation *= Quaternion.AngleAxis(-PlayerMouseInput.y * MouseSensitivity, Vector3.right);
		
        //Limit camera up and down rotation
		var angles = FollowTargetTransform.localEulerAngles;
		angles.z = 0;
		var xAngle = angles.x;
		if (xAngle > 180 && xAngle < 360 + MinViewAngle)
			angles.x = 360 + MinViewAngle;
		if (xAngle < 180 && xAngle > MaxViewAngle)
			angles.x = MaxViewAngle;
		FollowTargetTransform.localEulerAngles = angles;

		if (PlayerMovementInput.x != 0 || PlayerMovementInput.z != 0){
			//Rotate Player and everything below it excluding follow target 
			transform.rotation = Quaternion.Euler(0f, FollowTargetTransform.rotation.eulerAngles.y, 0f);

			FollowTargetTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
		}
	}
	public bool IsRunning(){return PlayerMovementInput.x != 0 || PlayerMovementInput.z != 0;}
	public bool IsJumping(){return PlayerBody.velocity.y > 0.1;}
	public bool IsFalling(){return PlayerBody.velocity.y < -1;}

	public Transform getFollowTargetTransform(){
		return FollowTargetTransform;
	}
	private void OnTriggerEnter(Collider other) {
		if (other.tag=="DeathTrigger"){
			GameManager.Instance.GameOver();
		}
	}
}