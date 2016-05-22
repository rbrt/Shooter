using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum KeyMaps  {Forward,
					  Right,
					  Left,
					  Back,
					  Shoot,
					  Strafe}

public class PlayerController : MonoBehaviour {

	static List<PlayerController> allPlayers;
	public static List<PlayerController> GetAllPlayers(){
		return allPlayers;
	}

	Dictionary <KeyMaps, KeyCode> mappings;

	[SerializeField] protected int player = 0;

	[SerializeField] Weapon currentWeapon;

	bool movingRight,
		 movingLeft,
		 movingForward,
		 movingBack,
		 strafing,
		 shooting;

	float forwardSpeed = 10,
		  strafeSpeed = 12,
		  turnSpeed = 350,
		  gravity = 0;

	void Awake(){
		mappings = PlayerControlMappings.GetMappingsForPlayer(player);

		if (allPlayers == null){
			allPlayers = new List<PlayerController>();
		}

		if (!allPlayers.Contains(this)){
			allPlayers.Add(this);
		}
	}

	void Start () {

	}

	void Update () {
		HandleInput();
	}

	void HandleInput(){
		HandleInputDown();

		HandleInputUp();

		MovePlayer();

		if (shooting && currentWeapon != null){
			currentWeapon.Fire();
		}
	}

	void HandleInputDown(){
		if (Input.GetKeyDown(mappings[KeyMaps.Forward])){
			movingForward = true;
			movingBack = false;
		}
		else if (Input.GetKeyDown(mappings[KeyMaps.Right])){
			movingRight = true;
			movingLeft = false;
		}
		else if (Input.GetKeyDown(mappings[KeyMaps.Left])){
			movingLeft = true;
			movingRight = false;
		}
		else if (Input.GetKeyDown(mappings[KeyMaps.Back])){
			movingBack = true;
			movingForward = false;
		}

		if (Input.GetKeyDown(mappings[KeyMaps.Shoot])){
			shooting = true;
		}
		if (Input.GetKeyDown(mappings[KeyMaps.Strafe])){
			strafing = true;
		}
	}

	void HandleInputUp(){
		if (Input.GetKeyUp(mappings[KeyMaps.Forward])){
			movingForward = false;
		}
		else if (Input.GetKeyUp(mappings[KeyMaps.Right])){
			movingRight = false;
		}
		else if (Input.GetKeyUp(mappings[KeyMaps.Left])){
			movingLeft = false;
		}
		else if (Input.GetKeyUp(mappings[KeyMaps.Back])){
			movingBack = false;
		}

		if (Input.GetKeyUp(mappings[KeyMaps.Shoot])){
			shooting = false;
		}
		if (Input.GetKeyUp(mappings[KeyMaps.Strafe])){
			strafing = false;
		}
	}


	void MovePlayer(){
		Vector3 movement = Vector3.zero;
		if (movingForward){
			movement += transform.forward * forwardSpeed * Time.deltaTime;
		}
		else if (movingBack){
			movement -= transform.forward * forwardSpeed * Time.deltaTime;
		}

		if (strafing){
			HandleStrafe(ref movement);
		}
		else{
			HandleTurn();
		}

		movement.y -= gravity * Time.deltaTime;

		transform.position += movement;

	}

	void HandleStrafe(ref Vector3 movement){
		if (movingLeft){
			movement -= transform.right * strafeSpeed * Time.deltaTime;
		}
		else if (movingRight){
			movement += transform.right * strafeSpeed * Time.deltaTime;
		}
	}

	void HandleTurn(){
		if (movingLeft){
			transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
		}
		else if (movingRight){
			transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
		}
	}
}

public class PlayerControlMappings{
	public static Dictionary<KeyMaps, KeyCode> GetMappingsForPlayer(int player){
		if (player == 0){
			return new Dictionary<KeyMaps, KeyCode>{
				{KeyMaps.Forward, KeyCode.UpArrow},
				{KeyMaps.Right, KeyCode.RightArrow},
				{KeyMaps.Left, KeyCode.LeftArrow},
				{KeyMaps.Back, KeyCode.DownArrow},
				{KeyMaps.Shoot, KeyCode.Z},
				{KeyMaps.Strafe, KeyCode.X}
			};
		}
		else{
			return null;
		}
	}
}
