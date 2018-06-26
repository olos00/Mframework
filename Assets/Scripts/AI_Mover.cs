using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.Networking;
using UnityEngine.AI;

namespace MFramework
{
	public class AI_Mover : NetworkBehaviour
	{
		[SyncVar(hook = "UpdateTarget")]
		private NetworkInstanceId targetNetID;
		public LayerMask targetLayer;
		private float thoughtTime = 2;
		private float nextThoughtTime;
		private Transform targetTransform;
	
		// Use this for initialization
		void Start ()
		{
			WarpAIToNavMesh();

			if (isServer)
			{
				nextThoughtTime = Time.time + 2;
			}
			
			else UpdateTarget(targetNetID);
		}
	
		// Update is called once per frame
		void Update () 
		{
			if (isServer)
			{
				FindPlayer();
			}
			
			CheckIfDestinationReached();
		}

		void FindPlayer()
		{
			if (Time.time > nextThoughtTime)
			{
				Collider[] targetColliders = Physics.OverlapSphere(transform.position, 50, targetLayer);
				
				if (targetColliders.Length > 0)
				{
					targetNetID = targetColliders[0].GetComponent<NetworkIdentity>().netId;
					GetComponent<NavMeshAgent>().SetDestination(NetworkServer.FindLocalObject(targetNetID).transform.position);
				}

				nextThoughtTime = Time.time + thoughtTime;
			}
		}
		
		void CheckIfDestinationReached()
		{
			NavMeshAgent myNavMeshAgent = GetComponent<NavMeshAgent>();

			if (targetTransform != null)
			{
				myNavMeshAgent.SetDestination(targetTransform.position);
			}
			
			if (myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance
			    && !myNavMeshAgent.pathPending)
			{
				StopWalking();
			}
			
			else StartWalking();
		}

		void WarpAIToNavMesh()
		{
			NavMeshHit posOnNavMesh;
			NavMesh.SamplePosition(transform.position, out posOnNavMesh, 10, NavMesh.AllAreas);
			GetComponent<NavMeshAgent>().Warp(posOnNavMesh.position);
		}
		
		void StartWalking()
		{
			GetComponent<NavMeshAgent>().isStopped = false;
			GetComponent<Animator>().SetBool("Moving", true);
		}

		void StopWalking()
		{
			GetComponent<NavMeshAgent>().isStopped = true;
			GetComponent<Animator>().SetBool("Moving", false);
		}
		
		void UpdateTarget(NetworkInstanceId netID)
		{
			targetNetID = netID;
			targetTransform = ClientScene.FindLocalObject(targetNetID).transform;
		}
	}
}

