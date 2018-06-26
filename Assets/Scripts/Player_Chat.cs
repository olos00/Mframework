using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.Networking;

namespace MFramework
{
	public class Player_Chat : NetworkBehaviour
	{

		[SyncVar(hook = "UpdateChat")]
		private string chat;
		private NetworkManger_Chat networkMangerChatScript;
		private Player_Name playerNameScript;

		private void Awake()
		{
			SetInitialReferences();
		}

		// Update is called once per frame
		void Update ()
		{
			if (!isLocalPlayer)
			{
				this.enabled = false;
			}

			ListenForChatInputRequest();
			ListenForChatSubmissionRequest();
		}

		void SetInitialReferences()
		{
			networkMangerChatScript = GameObject.Find("NetworkManager").GetComponent<NetworkManger_Chat>();
			playerNameScript = GetComponent<Player_Name>();
		}

		void ListenForChatInputRequest()
		{
			if (Input.GetKeyUp(KeyCode.T))
			{
				networkMangerChatScript.ActivateChatInputField();
			}
		}

		[Command]
		void CmdSendChat(string chatText)
		{
			chat = chatText;
		}

		void UpdateChat(string chatText)
		{
			chat = chatText;
			networkMangerChatScript.OutputChat(chat);
		}

		void ListenForChatSubmissionRequest()
		{
			if (Input.GetKeyUp(KeyCode.Return))
			{
				string chatText = networkMangerChatScript.RetrieveInputChat();
				
				if (chatText != string.Empty)
				{
					chatText = playerNameScript.playerName + ":" + chatText;
					CmdSendChat(chatText);
				}
				
				networkMangerChatScript.DisableChatInputField();
			}
		}
	}
}

