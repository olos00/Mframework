using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.NetworkSystem;

namespace MFramework
{
	public class NetworkManager_Custom : NetworkManager {
	
		private string ipAddress;
		private int port = 7777;
	
		public Text textConnectionInfo;
		public Text ipAddressTextField;
	
		public string menuSceneName = "Menu";
		private Scene currentScene;
		public GameObject[] uiPanels;
		private MatchInfo hostInfo;
		public Text matchRoomNameText;
	
		public Transform contentRoomList;
		public GameObject roomButtonPrefab;
	
		public Text playerNameText;
		private string playerName;

		public Text selectedMapTextComponent;
		private string mapSelectedString = "Map 1";

		private int characterSelected = 0;
		public Text characterSelectedTextComponent;
		public GameObject[] characterPrefabs;
		private short playerControllerID = 0;
		
		#region Unity Methods
		private void OnEnable()
		{
			RegisterCharacterPrefabs();
			SceneManager.sceneLoaded += OnMySceneLoaded;
		}

		public void OnClickSelectCharacter(int charNum)
		{
			characterSelected = charNum;
			characterSelectedTextComponent.text = "Character Selected: " + charNum;
		}
		
		void OnDisable()
		{
			SceneManager.sceneLoaded -= OnMySceneLoaded;
		}
	
		public override void OnClientDisconnect(NetworkConnection conn)
		{
			base.OnClientDisconnect(conn);
	
			if (textConnectionInfo.text != null)
			{
				textConnectionInfo.text = "Disconnected or timed out";
				ActivatePanel("PanelMainMenu");
			}
		}

		public override void OnClientConnect(NetworkConnection conn)
		{
			//Added just to override base instruction
			//since there are more than 1 maps
		}

		public override void OnClientSceneChanged(NetworkConnection conn)
		{
			IntegerMessage msg = new IntegerMessage(characterSelected);
			ClientScene.AddPlayer(conn, playerControllerID, msg);
		}

		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerID,
			NetworkReader extraMSGReader)
		{
			int id = 0;
			if (extraMSGReader != null)
			{
				var i = extraMSGReader.ReadMessage<IntegerMessage>();
				id = i.value;
			}

			Transform chosenSpawnPoint = NetworkManager.singleton.startPositions[Random.Range(0, NetworkManager.singleton.startPositions.Count)];
			GameObject player = Instantiate(characterPrefabs[id], chosenSpawnPoint.position, chosenSpawnPoint.rotation) as GameObject;
			NetworkServer.AddPlayerForConnection(conn, player, playerControllerID);
		}
		#endregion
	
		#region Custom Methods

		void RegisterCharacterPrefabs()
		{
			foreach (GameObject character in characterPrefabs)
			{
				ClientScene.RegisterPrefab(character);
			}	
		}
		
		public void OnClickSelectMap(string mapName)
		{
			mapSelectedString = mapName;
			selectedMapTextComponent.text = mapName + " selected";
		}

		public void LoadMap()
		{
			NetworkManager.singleton.ServerChangeScene(mapSelectedString);
		}
		
		public void OnClickCapturePlayerName()
		{
			if (playerNameText.text == string.Empty)
			{
				playerName = "Player";
				PlayerPrefs.SetString("PlayerName", playerName);
			}
	
			else
			{
				playerName = playerNameText.text;
				PlayerPrefs.SetString("PlayerName", playerName);
			}
		}
		
		void OnMySceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SetInitialReferences();
		}
	
		void SetInitialReferences()
		{
			currentScene = SceneManager.GetActiveScene();
	
			if (currentScene.name == menuSceneName)
			{
				ActivatePanel("PanelMainMenu");
			}
	
			else 
			{
				ActivatePanel("PanelInGame");
				OnClickClearConnectionTextInfo();
			}
		}
	
		public void ActivatePanel(string panelName)
		{
			foreach(GameObject panel in uiPanels)
			{
				if (panel.name.Equals(panelName))
				{
					panel.SetActive(true);
				}
	
				else panel.SetActive(false);
			}
		}
	
		void GetIPAddress()
		{
			ipAddress = ipAddressTextField.text;
		}
	
		void SetPort()
		{
			NetworkManager.singleton.networkPort = port;
		}
	
		void SetIPAddress()
		{
			NetworkManager.singleton.networkAddress = ipAddress;
		}
	
		public void OnClickClearConnectionTextInfo()
		{
			textConnectionInfo.text = string.Empty;
		}
	
		public void OnClickStartLANHost()
		{
			SetPort();
			NetworkManager.singleton.StartHost();
			LoadMap();
		}
	
		public void OnClickStartServerOnly()
		{
			SetPort();
			NetworkManager.singleton.StartServer();
			LoadMap();
		}
	
		public void OnClickJoinLANGame()
		{
			SetPort();
			GetIPAddress();
			SetIPAddress();
			NetworkManager.singleton.StartClient();
		}
	
		public void OnClickDisconnectFromNetwork()
		{
			//Only the applicable ones will be activated
			NetworkManager.singleton.StopHost();
			NetworkManager.singleton.StopServer();
			NetworkManager.singleton.StopClient();
		}
	
		public void OnClickExitGame()
		{
			Application.Quit();
		}
	
		public void OnClickDisableMatchMaker()
		{
			NetworkManager.singleton.StopMatchMaker();
		}
		
		public void OnClickEnableMatchMaker()
		{
			OnClickDisableMatchMaker();
			SetPort();
			NetworkManager.singleton.StartMatchMaker();
		}
	
		public void OnClickCreateMatch()
		{
			NetworkManager.singleton.matchMaker.CreateMatch(matchRoomNameText.text, 4, true, "", "", "", 0, 0,
				OnInternetCreateMatch);
		}
	
		void OnInternetCreateMatch(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			if (success)
			{
				textConnectionInfo.text = "Match Created!";
				hostInfo = matchInfo;
				NetworkServer.Listen(hostInfo, NetworkManager.singleton.matchPort);
				NetworkManager.singleton.StartHost(hostInfo);
				LoadMap();
			}
	
			else textConnectionInfo.text = "Match Creation Failed!";
		}
	
		
		public void JoinInternetMatch(NetworkID netID, string password, string publicClientAddr,
			string privateClientAddr, int eloScore, int reqDomain, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
		{
			NetworkManager.singleton.matchMaker.JoinMatch(netID, password, publicClientAddr, privateClientAddr,
				eloScore, reqDomain, OnJoinInternetMatch);
		}
		
		void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			if (success)
			{
				hostInfo = matchInfo;
				NetworkManager.singleton.StartClient(hostInfo);
			}
	
			else
			{
				textConnectionInfo.text = "Joining Failed!";
			}
		}
	
		void ClearContentRoomList()
		{
			foreach (Transform child in contentRoomList)
			{
				Destroy(child.gameObject);
			}
		}
	
		public void OnClickFindInternetMatch()
		{
			ClearContentRoomList();
			NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnInternetMatchList);
		}
	
		void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
		{
			if (success)
			{
				if (matches.Count != 0)
				{
					foreach (MatchInfoSnapshot availableMatch in matches)
					{
						GameObject roomButton = Instantiate(roomButtonPrefab) as GameObject;
						roomButton.GetComponentInChildren<Text>().text = availableMatch.name;
						
						roomButton.GetComponent<Button>().onClick.AddListener( delegate {
							JoinInternetMatch(availableMatch.networkId, "", "", "", 0, 0, OnJoinInternetMatch);
						});
						
						roomButton.GetComponent<Button>().onClick.AddListener( delegate {
							ActivatePanel("PanelLoader");
						});
						
						roomButton.transform.SetParent(contentRoomList, false);
					}
				}
	
				else textConnectionInfo.text = "No matches available!";
			}
			
			else textConnectionInfo.text = "Failed to connect to match maker!";
		}
		#endregion
	}
}

