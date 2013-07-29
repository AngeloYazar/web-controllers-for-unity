using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ControllerServer : MonoBehaviour {

	public int port = 8081;
	public GameObject PlayerPrefab;
	public static GameObject _PlayerPrefab;
	private WebSocketServiceHost<Controller> server;
	private static List<IEnumerator> pendingRoutines = new List<IEnumerator>();
	
	public class PlayerConnection {
		public bool connected = true;
		public int playerNumber = 0;
		public GameObject playerObject;
		public Player playerScript;
		
		public PlayerConnection( int number, GameObject PlayerPrefab ) {
			playerNumber = number;
		}
		
		public void ForwardMessage( string message ) {
			ControllerServer.QueueCoroutine( OnMessage(message) );
		}
		
		public IEnumerator OnMessage(string message) {
			if( playerObject != null ) {
				playerScript.OnMessage(message);
			}
			yield return null;
		}
		
		public bool IsConnected() {
			return connected;
		}
		
		public void Connect() {
			connected = true;
			ControllerServer.QueueCoroutine( OnConnect() );
		}
		
		public IEnumerator OnConnect() {
			if( playerObject == null ) {
				playerObject = (GameObject)GameObject.Instantiate(ControllerServer._PlayerPrefab, Vector3.zero, Quaternion.identity);
				playerScript = playerObject.GetComponent<Player>();
			}
			if( playerObject != null ) {
				playerScript.OnConnect(playerNumber);
			}	
			yield return null;
		}
		
		public void Disconnect() {
			connected = false;
			ControllerServer.QueueCoroutine( OnDisconnect() );
		}
		
		public IEnumerator OnDisconnect() {
			if( playerObject != null ) {
				playerScript.OnDisconnect();
			}
			yield return null;
		}
	}
	
	public class Controller : WebSocketService {
		public static int playerCount = 0;
		private static int lowestAvailablePlayerNumber = 0;
		public static List<PlayerConnection> players = new List<PlayerConnection>();
		private PlayerConnection self;
		
		public static PlayerConnection GetNextAvailablePlayer() {
			if( lowestAvailablePlayerNumber >= players.Count ) {
				PlayerConnection newPlayer = new PlayerConnection( lowestAvailablePlayerNumber, ControllerServer._PlayerPrefab );
				players.Add ( newPlayer );
				lowestAvailablePlayerNumber++;
				newPlayer.Connect();
				return newPlayer;
			}
			else {
				PlayerConnection availablePlayer = players[lowestAvailablePlayerNumber];
				availablePlayer.Connect();
				lowestAvailablePlayerNumber = players.Count;
				foreach( PlayerConnection player in players ) {
					if( !player.IsConnected() ) { 
						lowestAvailablePlayerNumber = player.playerNumber;
						break;
					}
				}
				return availablePlayer;
			}
		}
		
		protected override void OnMessage(MessageEventArgs e) {
			self.ForwardMessage(e.Data);
		}
		
		protected override void OnOpen() {
			Debug.Log ("Opened!"); 
			self = GetNextAvailablePlayer();
			playerCount++;
			Send("Player: " + self.playerNumber);
		}
		
		protected override void OnClose(CloseEventArgs e) {
			if( self.playerNumber < lowestAvailablePlayerNumber ) {
				lowestAvailablePlayerNumber = self.playerNumber;
			}
			self.Disconnect();
			playerCount--;
			Debug.Log ("Closed!");
		}
		
	}
	
	public static int GetPlayerCount() {
		return Controller.playerCount;
	}
	
	private static void QueueCoroutine( IEnumerator co ) {
		pendingRoutines.Add( co );
	}
	
	void LateUpdate() {
		try {
			if( pendingRoutines.Count > 0 ) {
				foreach( IEnumerator co in pendingRoutines ) {
					StartCoroutine( co );
				}
				pendingRoutines.Clear();
			}
		}
		catch( InvalidOperationException e ) { 
			/* This happens, I'm guessing because the collection
			 * is sometimes modified on another thread while
			 * this is looping. Not sure though...
			 */
			e = null; // hides the annoying warning
		}
	}
	
	void Start() {
		_PlayerPrefab = PlayerPrefab;
		string url = "ws://"+Network.player.ipAddress+":"+port;
		server = new WebSocketServiceHost<Controller>(url);
		server.Start();
	}
	
	void OnDestroy() {
		server.Stop();
	}
}