using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
public class NewSelection : MonoBehaviour
{
	public QuickSelectAnimation qsa;
	LineRenderer line;
	Vector3 startPos;
	Vector3 endPos;
	Vector3[] lineArray;

	public LineCastSelector selector;

    // Start is called before the first frame update
    void Start()
    {
		line = GetComponent<LineRenderer>();
		qsa = GetComponentInChildren<QuickSelectAnimation>(true);
		line.startWidth = .1f;
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
			transform.position = startPos;
			active = true;
			LoadNearbySelections();
			ActivateAnimation();
			DebugSelections();
		} else if (OVRInput.GetUp(OVRInput.Button.One)) {
			//New Selection
			//Figure out which
			active = false;
			line.positionCount = 0;
			line.SetPositions(new Vector3[0]);

			DeactivateAnimation();
		}
		if (active) {
			/*
			lineArray[0] = startPos;
			//endPos = selector.direction * selector.distance + selector.transform.position;
			endPos = selector.line.GetPosition(1);
			lineArray[1] = endPos;
			line.positionCount = 2;
			line.SetPositions(lineArray);
			*/
			DrawElements();
		}
    }

	public float sphereRadius;
	List<SelectableElement> elementList = new List<SelectableElement>();
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
				if (!elementList.Contains(el)) {
					elementList.Add(el);
				}
			}
		}
		Debug.Log(elementList.Count);
	}

	private void ActivateAnimation()
	{
		//Update buttons
		UnityEngine.UI.Button b;
		EventTrigger trigger = null; ;
		int Z_TOP = 9;
		int DEFAULT = 0;

		//Adjust the button functionality
		for(int i = 0; i < elementList.Count && i < qsa.transform.childCount; i++) {
			b = qsa.transform.GetChild(i).GetComponent<UnityEngine.UI.Button>();

			//Add button hover events
			if ((trigger = b.GetComponent<EventTrigger>()) == null) {
				trigger = b.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(
				delegate 
				{
					Debug.Log("I WAS ENTERED");
					elementList[i].GetComponent<MaterialSwitchState>().Brighten();
					elementList[i].gameObject.layer = Z_TOP;
				});
			trigger.triggers.Add(entry);
			EventTrigger.Entry exit = new EventTrigger.Entry();
			exit.eventID = EventTriggerType.PointerExit;
			exit.callback.AddListener(
				delegate
				{
					Debug.Log("I WAS left");
					elementList[i].GetComponent<MaterialSwitchState>().Darken();
					elementList[i].gameObject.layer = DEFAULT;
				});
			trigger.triggers.Add(exit);
			//Add click event
			b.onClick.AddListener(
				delegate
				{
					//Adjust to make the cursor go to the collision point?
					//Need custom data class to maintain this collision point maybe?
					//Or calculate it from the center --> closest point on edge?

					//selector.cursor.position = elementList[i].transform.position;
					selector.SelectNew(elementList[i], elementList[i].transform.position);
					//selector.SetSavedCursorPos(elementList[i].transform.position);
				});
		}

		//Activate animation
		qsa.Activate(elementList.Count);
	}

	private void DeactivateAnimation()
	{
		qsa.Deactivate();
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
