using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_METRO && !UNITY_EDITOR  
        WindowsGateway.UnityLoaded();
#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
