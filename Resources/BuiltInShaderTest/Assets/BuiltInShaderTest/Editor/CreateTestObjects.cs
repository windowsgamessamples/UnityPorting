using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateTestObjects
{
	[MenuItem("ShaderTests/Create from selection")]
	public static void CreateTestObjectsNow()
	{
		GameObject prefabObj = Resources.Load<GameObject>("TestShaderPrefab");
		if (null == prefabObj){
			Debug.LogError ("no prefab");
			return;
		}

		var parentObj = new GameObject("_CATEGORY "+Selection.activeObject.name);
				
		int count = 0;

		foreach (var obj in Selection.GetFiltered(typeof(Material),SelectionMode.DeepAssets)) {
			Debug.Log (obj.name);		

			GameObject newChild = (GameObject)Object.Instantiate(prefabObj);

			newChild.transform.parent = parentObj.transform;
			newChild.transform.localPosition = -Vector3.up*count*1.0f;
			newChild.name = obj.name.Remove (0,"Testmat ".Length);
			newChild.renderer.material = (Material)obj;
			count++;
		}
	}
}
