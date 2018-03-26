using UnityEngine;
using UnityEditor;

public class ChangeMaterial : MonoBehaviour
{

	[MenuItem("Tools/changeMaterial")]
	public static void change()
	{
		Object[] m_objects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);//选择的所以对象  

		if (m_objects.Length != 1)
		{
			Debug.Log("选择的材质不唯一");
			return;
		}
		foreach (GameObject go in Selection.gameObjects)
		{
			FindMater(go, m_objects[0] as Material);
		}

		Debug.Log("Complete! ");
	}


	public static void FindMater(GameObject go, Material m)
	{
		if (go.GetComponent<MeshRenderer>() != null)
		{
			go.GetComponent<MeshRenderer>().material = m;
		}

		foreach (Transform child in go.transform)
		{
			FindMater(child.gameObject, m);

			if (child.GetComponent<MeshRenderer>() != null)
			{
				child.GetComponent<MeshRenderer>().material = m;
			}
		}
	}

}
