using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class CombineSelectedMeshes : MonoBehaviour {

	static CombineSelectedMeshes instance;
	public GameObject replaceWith;

	// Use this for initialization
	void Start () {
		instance = FindObjectOfType<CombineSelectedMeshes>();
	}

	// Update is called once per frame
	void Update () {
		
	}
#if UNITY_EDITOR
	public void PerformSwap()
	{
		GameObject tempReplacement;
		Quaternion q;
		foreach (Transform t in Selection.transforms) {
			tempReplacement = Instantiate(replaceWith, t.parent);
			q = t.rotation;
			q.eulerAngles += tempReplacement.transform.eulerAngles;
			tempReplacement.transform.SetPositionAndRotation(t.position, q);
			t.gameObject.SetActive(false);
		}
	}

	[MenuItem("Window/SwapSelection")]
	public static void DoTheSwap()
	{
		if (instance == null) {
			instance = FindObjectOfType<CombineSelectedMeshes>();
			if (instance == null) {
				instance = new GameObject("MeshCombiner", new System.Type[] { typeof(CombineSelectedMeshes) }).GetComponent<CombineSelectedMeshes>();
			}
		}

		instance.PerformSwap();
	}

	[MenuItem("Window/Combine Meshes")]
	public static void CombineMeshes()
	{
		if (instance == null) {
			instance = FindObjectOfType<CombineSelectedMeshes>();
			if (instance == null) {
				instance = new GameObject("MeshCombiner", new System.Type[] { typeof(CombineSelectedMeshes) }).GetComponent<CombineSelectedMeshes>();
			}
		}

		if (Selection.gameObjects.Length <= 1) {
			Debug.LogWarning("Please select more than 1 mesh");
			return;
		}

		//List<SingleMatMesh> matMeshes = new List<SingleMatMesh>();
		Dictionary<Material, SingleMatMesh> matMeshes = new Dictionary<Material, SingleMatMesh>();


		MeshRenderer[] tempMesh;
		SingleMatMesh singleMat;
		//List<MeshFilter> children = new List<MeshFilter>();
		foreach (GameObject obj in Selection.gameObjects) {
			tempMesh = obj.GetComponentsInChildren<MeshRenderer>();
			if (tempMesh == null) {
				obj.SetActive(false);
				continue;
			}
			foreach (MeshRenderer m in tempMesh) {
				if (m.sharedMaterial == null) {
					continue;
				}
				if (matMeshes.TryGetValue(m.sharedMaterial, out singleMat)) {
					singleMat.AddMesh(m.GetComponent<MeshFilter>());
				} else {
					matMeshes.Add(m.sharedMaterial, new SingleMatMesh(m.GetComponent<MeshFilter>()));
				}
			}
			//children.Add(obj.GetComponentInChildren<MeshFilter>());
		}

		foreach(SingleMatMesh smm in matMeshes.Values) {
			GameObject newObj = new GameObject("CombinedMeshes: " + smm.children[0].transform.name, new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer) });
			//newObj.transform.parent = children[0].transform.parent;
			smm.TransposeObjects();
			newObj.GetComponent<MeshFilter>().mesh = new Mesh();
			newObj.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(smm.combine.ToArray());
			newObj.GetComponent<MeshRenderer>().sharedMaterial = smm.children[0].GetComponent<MeshRenderer>().sharedMaterial;
			newObj.SetActive(false);
			//CreateMeshAsset(newObj.GetComponent<MeshFilter>().sharedMesh);
			smm.TransposeObjects(newObj.transform);
		}

		foreach(GameObject go in Selection.gameObjects) {
			go.SetActive(!go.activeInHierarchy);
		}
		/*
		CombineInstance[] combine = new CombineInstance[children.Count];

		int i = 0;
		while (i < children.Count) {
			combine[i].mesh = children[i].mesh;
			combine[i].transform = children[i].transform.localToWorldMatrix;
			children[i].transform.parent.gameObject.SetActive(false);
			i++;
		}

		GameObject newerObj = new GameObject("Old Selection Parent");
		foreach (Transform obj in Selection.transforms) {
			obj.parent = newerObj.transform;
		}
		newerObj.SetActive(false);

		GameObject newObj = new GameObject("CombinedMeshes", new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer) });
		//newObj.transform.parent = children[0].transform.parent;
		newObj.GetComponent<MeshFilter>().mesh = new Mesh();
		newObj.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
		newObj.GetComponent<MeshRenderer>().sharedMaterial = children[0].GetComponent<MeshRenderer>().sharedMaterial;
		newObj.SetActive(true);
		*/
	}

	private static void CreateMeshAsset(Mesh m)
	{
		//Mesh m = newObj.GetComponent<MeshFilter>().mesh;
		AssetDatabase.CreateAsset(m, "Assets/Models/Walls/NewWalls/Newer Wall Door/CombinedMesh.obj");
		AssetDatabase.SaveAssets();
	}

	public struct SingleMatMesh
	{
		public List<MeshFilter> children;
		public List<CombineInstance> combine;
		private Vector3 startLoc;
		private bool assignedStart;


		public SingleMatMesh(List<MeshFilter> meshes)
		{
			children = meshes;
			combine = new List<CombineInstance>();
			startLoc = Vector3.zero;
			assignedStart = false;
		}

		public SingleMatMesh(MeshFilter mat)
		{
			children = new List<MeshFilter>();
			combine = new List<CombineInstance>();
			startLoc = Vector3.zero;
			assignedStart = false;
			AddMesh(mat);
		}

		public void AddMesh(MeshFilter newMesh)
		{
			if (children.Contains(newMesh)) {
				return;
			}
			children.Add(newMesh);
			combine.Add(new CombineInstance() { mesh = newMesh.sharedMesh, transform = newMesh.transform.localToWorldMatrix });
			//newMesh.gameObject.SetActive(false);
		}

		/// <summary>
		/// Moves the selection of objects to and from the origin.
		/// </summary>
		/// <param name="moveWith">Optional transform to transpose with the other transforms</param>
		/// <remarks>This helps keep the anchor of combined meshes relatively close to the mesh itself</remarks>
		public void TransposeObjects(Transform moveWith = null)
		{
			if (!assignedStart) {
				//Find the average location of all objects
				startLoc = Vector3.zero;
				foreach (MeshFilter filter in children) {
					startLoc += filter.transform.position;
				}
				startLoc = startLoc / children.Count;
				CombineInstance newInstance;
				for (int i = 0; i < children.Count; i++) {
					children[i].transform.position -= startLoc;

					//Have to copy the changes to a new instance and replace it in the list
					newInstance = combine[i];
					newInstance.transform = children[i].transform.localToWorldMatrix;
					combine[i] = newInstance;
				}

				assignedStart = true;
			} else {
				CombineInstance newInstance;
				for (int i = 0; i < children.Count; i++) {
					children[i].transform.position += startLoc;

					newInstance = combine[i];
					newInstance.transform = children[i].transform.localToWorldMatrix;
					combine[i] = newInstance;
				}

				if (moveWith != null) {
					moveWith.position = startLoc;
				}

				startLoc = Vector3.zero;
				assignedStart = false;
			}
		}
	}
#endif
}
