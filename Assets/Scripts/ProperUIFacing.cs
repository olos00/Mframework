using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
	public class ProperUIFacing : MonoBehaviour 
	{

		// Update is called once per frame
		void Update () {
			transform.LookAt(Camera.main.transform);		
		}
	}
}

