using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public float speed = 0.1f;
	public TextMesh display;
	private float leftMove = 0;
	private float rightMove = 0;
	private float upMove = 0;
	private float downMove = 0;
	
	void Start () {
	
	}
	
	void LateUpdate () {
		transform.Translate( leftMove + rightMove, upMove + downMove, 0 );
	}
	
	public void OnConnect(int playerNumber) {
		transform.position = new Vector3((float)playerNumber,0.0f,0.0f);
		renderer.material.color = new Color( Random.value, Random.value, Random.value );
		display.text = ".";
	}
	
	public void OnDisconnect() {
		renderer.material.color = Color.gray;
		display.text = "Disconnected";
	}
	
	public void OnMessage( string message) {
		if( message == "pLEFT" ) { leftMove = -speed; }
		else if( message == "rLEFT" ) { leftMove = 0; }
		else if( message == "pRIGHT" ) { rightMove = speed; }
		else if( message == "rRIGHT" ) { rightMove = 0; }
		else if( message == "pUP" ) { upMove = speed; }
		else if( message == "rUP" ) { upMove = 0; }
		else if( message == "pDOWN" ) { downMove = -speed; }
		else if( message == "rDOWN" ) { downMove = 0; }
		display.text = message;
	}
}