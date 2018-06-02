using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.Networking;

namespace MFramework
{
	public class Player_Shoot : NetworkBehaviour 
	{

		public GameObject[] hitEffects;
		public Transform firstPersonCharacter;
		public RaycastHit hit;
		private int damage = 20;


		// Update is called once per frame
		void Update () {
			if (isLocalPlayer && Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
		}
	
		void Shoot()
		{
			if (Physics.Raycast(firstPersonCharacter.transform.position, firstPersonCharacter.transform.forward, out hit))
			{
				Quaternion hitAngle = Quaternion.LookRotation(hit.normal);
				CmdSpawnHitPrefab(hit.point, hitAngle);

				if (hit.transform.CompareTag("Player"))
				{
					CmdApplyDamageOnServer(hit.transform.GetComponent<NetworkIdentity>().netId);
				}
			}
		}
		[Command]
		void CmdSpawnHitPrefab(Vector3 pos, Quaternion rot)
		{
			foreach (GameObject hitEffect in hitEffects)
			{
				GameObject go = Instantiate(hitEffect, pos, rot);
				NetworkServer.Spawn(go);
			}
		}

		[Command]
		void CmdApplyDamageOnServer (NetworkInstanceId networkID)
		{
			GameObject hitPlayerGO = NetworkServer.FindLocalObject(networkID);
			hitPlayerGO.GetComponent<Player_Health>().DeductHealth(damage);
		}
	}
}