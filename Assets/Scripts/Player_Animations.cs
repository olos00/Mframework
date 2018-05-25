using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.Networking;

namespace MFramework
{

	public class Player_Animations : NetworkBehaviour 
	{

		public Animator playerAnimator;
		
		void Update ()
		{
			CheckForPlayerInput();
		}

		void CheckForPlayerInput()
		{
			if (!isLocalPlayer)
			{
				return;
			}

			if (Mathf.Abs( Input.GetAxis("Vertical")) > 0
			|| Mathf.Abs( Input.GetAxis("Horizontal")) > 0)
			{
				playerAnimator.SetBool("Moving", true);
			}

			else {
				playerAnimator.SetBool("Moving", false);
			}
		}
	}	
}
