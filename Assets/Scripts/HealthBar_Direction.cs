using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
	public class HealthBar_Direction : MonoBehaviour 
	{

		// Update is called once per frame
		void Update () {
			transform.LookAt(Camera.main.transform);		
		}
	}
}

