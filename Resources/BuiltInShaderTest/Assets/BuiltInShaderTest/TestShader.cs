using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TestShader : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.skin.label.fontSize = 9;
		var p = Camera.main.WorldToScreenPoint(this.transform.position);
		GUI.Label (new Rect(p.x, Screen.height - p.y, 200, 50), this.name);
	}
}
