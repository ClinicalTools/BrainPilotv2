using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NewSelection : MonoBehaviour
{
	LineRenderer line;
	Vector3 startPos;
	Vector3 endPos;
	Vector3[] lineArray;

	public LineCastSelector selector;

    // Start is called before the first frame update
    void Start()
    {
		line = GetComponent<LineRenderer>();
		lineArray = new Vector3[2];
		line.useWorldSpace = true;
    }

	bool active;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) {
			//Activate
			startPos = selector.cursor.position;
			active = true;
			LoadNearbySelections();
			DebugSelections();
		} else if (OVRInput.GetUp(OVRInput.Button.One)) {
			//New Selection
			//Figure out which 
			active = false;
			line.positionCount = 0;
			line.SetPositions(new Vector3[0]);
		}
		if (active) {
			lineArray[0] = startPos;
			//endPos = selector.direction * selector.distance + selector.transform.position;
			endPos = selector.line.GetPosition(1);
			lineArray[1] = endPos;
			line.positionCount = 2;
			line.SetPositions(lineArray);

			DrawElements();
		}
    }

	public float sphereRadius;
	List<SelectableElement> elementList;
	private void LoadNearbySelections()
	{
		//Sphere cast from startPos;
		Collider[] c = Physics.OverlapSphere(startPos, sphereRadius);
		Debug.Log(c.Length);
		List<Collider> collisionList = new List<Collider>(c);
		SelectableElement el;
		for(int i = 0; i < collisionList.Count; i++) {
			el = collisionList[i].GetComponent<SelectableElement>();
			if (el == null) {
				Debug.Log("removed " + collisionList[i].name);
				collisionList.RemoveAt(i);
				i--;
			} else {
				if (elementList == null) {
					elementList = new List<SelectableElement>();
				}
				elementList.Add(el);
			}
		}
	}

	private class MeshData : IComparer
	{
		public int triCount;
		public string name;
		public bool isEnabled;

		public MeshData(string name, int triCount, bool enabled)
		{
			this.name = name;
			this.triCount = triCount;
			isEnabled = enabled;
		}

		public override string ToString()
		{
			return (isEnabled ? "Enabled: " : "Disabled: ") + name + ": " + triCount;
		}

		public int Compare(object x, object y)
		{
			return 1;
		}

		public static int CompareByTri(MeshData x, MeshData y)
		{
			if (x.isEnabled && y.isEnabled) {
				return x.triCount.CompareTo(y.triCount);
			} else {
				if (x.isEnabled && !y.isEnabled) {
					return -1;
				} else {
					return 1;
				}
			}
		}
	}

	private void DrawElements()
	{
		//Each element will have text and will highlight the piece when hovered over
		
	}

	[ExecuteInEditMode,ContextMenu("Get mesh levels")]
	private void GetMeshLevels()
	{
		MeshFilter[] allMeshes = FindObjectsOfType<MeshFilter>();
		List<MeshData> meshData = new List<MeshData>();
		MeshData md;
		foreach(MeshFilter mf in allMeshes) {
			bool enabled = mf.GetComponent<MeshRenderer>().enabled && mf.gameObject.activeInHierarchy;
			md = new MeshData(mf.name, mf.sharedMesh.triangles.Length, enabled);
			meshData.Add(md);
		}
		meshData.Sort(MeshData.CompareByTri);
		StringBuilder data = new StringBuilder();
		int interval = 6;
		int iCount = 0;
		int totalTri = 0;
		foreach(MeshData mesh in meshData) {
			data.Append(mesh.ToString() + "\n");
			totalTri += mesh.triCount;
			iCount++;
			if (iCount == interval) {
				iCount = 0;
				data.Append("-----Total: " + totalTri + "\n");
			}
		}
		File.WriteAllText("C:\\Users\\Will\\Desktop\\MeshSizes.txt", data.ToString());
	}

	private void DebugSelections()
	{
		string s = "";
		foreach (SelectableElement e in elementList) {
			s += e.name + ": " + (e.transform.position - startPos) + "\n";
		}
		print(s);
	}
}
