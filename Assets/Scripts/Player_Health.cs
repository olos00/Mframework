using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace MFramework
{
	public class Player_Health : NetworkBehaviour 
	{
		[SyncVar(hook = "UpdateHealthBar")]
		public int playerHealth = 100;
		public GameObject[] characterModel;
		public float timeToRespawn = 5;
		public GameObject defeatText;
		public GameObject healthBarGO;
		public RectTransform healthBarRect;
		public Text healthText;


		// Use this for initialization
		void Start ()
		{
			UpdateHealthBar(playerHealth);
		}

		void UpdateHealthBar(int pHealth)
		{
			healthBarRect.sizeDelta = new Vector2(pHealth, healthBarRect.sizeDelta.y);
			healthText.text = pHealth.ToString();
		}

		public void DeductHealth(int damage)
		{
			if (!isServer)
			{
				return;
			}

			playerHealth -= damage;

			if (playerHealth <= 0)
			{
				playerHealth = 0;
				RpcDeactivatePlayer();
			}
		}

		[ClientRpc]
		void RpcDeactivatePlayer()
		{
			GetComponent<FirstPersonController>().enabled = false;
			GetComponent<Player_Shoot>().enabled = false;
			GetComponent<NetworkTransform>().enabled = false;
			GetComponent<CharacterController>().enabled = false;
			healthBarGO.SetActive(false);

			foreach (GameObject go in characterModel)
			{
				go.SetActive(false);
			}

			if (isLocalPlayer)
			{
				defeatText.SetActive(true);
			}

			StartCoroutine(Respawn());
		}

		void SelectSpawnPoint()
		{
			Transform chosenSpawnPoint = NetworkManager.singleton.startPositions[Random.Range(0, NetworkManager.singleton.startPositions.Count)];
			transform.position = chosenSpawnPoint.position;
			transform.rotation = chosenSpawnPoint.rotation;
		}

		void ReactivatePlayer()
		{
			playerHealth = 100;
			GetComponent<NetworkTransform>().enabled = true;
			GetComponent<Player_Shoot>().enabled = true;
			GetComponent<CharacterController>().enabled = true;

			if (isLocalPlayer)
			{
				GetComponent<FirstPersonController>().enabled = true;
				defeatText.SetActive(false);
				SelectSpawnPoint();
			}

			else StartCoroutine(MakePlayerModelVisible());
		}

		IEnumerator MakePlayerModelVisible()
		{
			yield return new WaitForSeconds(1.5f);

			foreach(GameObject go in characterModel)
			{
				go.SetActive(true);
			}

			healthBarGO.SetActive(true);
		}
		
		IEnumerator Respawn()
		{
			yield return new WaitForSeconds(timeToRespawn);
			ReactivatePlayer();	
		}
	}
}
