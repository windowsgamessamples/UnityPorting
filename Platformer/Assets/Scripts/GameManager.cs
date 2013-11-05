using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Tell our Windows Gateway class that Unity has loaded
        WindowsGateway.DoUnityLoaded();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
