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
		//line.startWidth = .05f;
		lineArray = new Vector3[2];
		line.useWorldSpace = true;
    }

	public bool active;

    // Update is called once per frame
    void Update()
    {
		if (!selector.isActive) {
			if (OVRInput.GetDown(OVRInput.Button.Two)) {
				//Activate
				Activate();
			} else if (OVRInput.GetUp(OVRInput.Button.Two)) {
				//New Selection
				//Figure out which button to select
				Deactivate();
			}

			if (active) {
				transform.LookAt(selector.transform);
			}
		} else if (animationActive) {
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
		}
    }

	private void Activate()
	{
		startPos = selector.cursor.position;
		transform.position = startPos;
		active = true;
		LoadNearbySelections();
		ActivateAnimation();
		DebugSelections();
	}

	private void Deactivate()
	{
		active = false;
		//line.positionCount = 0;
		line.SetPositions(new Vector3[0]);

		//Find out if over a button
		/*UnityEngine.UI.Button b = qsa.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>();
		PointerEventData data = new PointerEventData(EventSystem.current);
		b.OnPointerClick(data);*/
		DeactivateAnimation();
		qsa.CheckForButtonClick(activeButton);
	}

	public void CanceledClick(bool click)
	{
		if (active && !click) {
			Deactivate();
		}
	}

	private int CompareColliderDistance(Collider x, Collider y)
	{
		//Try to disable warnings about convex mesh colliders, fail completely
		#pragma warning disable 1234
		float xDist = (x.ClosestPoint(startPos) - startPos).sqrMagnitude;
		if (xDist == 0) {
			Debug.Log(x.name, x.gameObject);
		}
		float yDist = (y.ClosestPoint(startPos) - startPos).sqrMagnitude;
		if (yDist == 0) {
			Debug.Log(y.name, y.gameObject);
		}
		#pragma warning restore 1234

		if (xDist == yDist) {
			//print(x.name + ", " + y.name + ": 0");
			return 0;
		}

		if (xDist == 0) {
			//Collider is same as currently selected OR
			//Collider.ClosestPoint() didn't work (Not convex mesh)
			//print(x.name + ", " + y.name + ": 1");
			return 1;
		}
		if (yDist == 0) {
			//print(x.name + ", " + y.name + ": -1");
			return -1;
		}
		//print("----" + x.name + ", " + y.name + ": " + xDist.CompareTo(yDist));
		return xDist.CompareTo(yDist);
	}

	public float sphereRadius;
	List<SelectableElement> elementList = new List<SelectableElement>();
	private void LoadNearbySelections()
	{
		//Sphere cast from startPos;
		Collider[] c = Physics.OverlapSphere(startPos, sphereRadius);

		//Sort by distance
		List<Collider> collisionList = new List<Collider>(c);
		collisionList.RemoveAll(
			(Collider x) =>
				x.GetComponent<SelectableElement>() != null && 
				x.GetComponent<SelectableElement>().selectable.Equals(selector.furthestSelectable));
		collisionList.Sort(CompareColliderDistance);
		
		/*
		// Debug
		foreach (Collider col in collisionList) {
			print(col.name + ": " + (col.ClosestPoint(startPos) - startPos).sqrMagnitude);
		}
		collisionList.Sort(CompareColliderDistance);
		foreach (Collider col in collisionList) {
			print(col.name + ": " + (col.ClosestPoint(startPos) - startPos).sqrMagnitude);
		}*/

		//Assign to elementList, removing unnecessary elements
		SelectableElement el;
		elementList.Clear();
		for(int i = 0; i < collisionList.Count; i++) {
			el = collisionList[i].GetComponent<SelectableElement>();
			if (el == null || el.selectable.Equals(selector.furthestSelectable)) {
				//Debug.Log("removed " + collisionList[i].name);
				collisionList.RemoveAt(i);
				i--;
			} else {
				if (elementList == null) {
					elementList = new List<SelectableElement>();
				}
				if (!elementList.Contains(el)) {
					if (el.selectable != null && el.selectable is BrainElement) {
						//print("Adding " + ((BrainElement)el.selectable).elementName);
						elementList.Add(el);
					} else {
						//Debug.Log("Removed " + collisionList[i].name + ", because selectable is " + el.selectable?.ToString());
						collisionList.RemoveAt(i);
						i--;
					}
				}
			}
		}
		Debug.Log("Total elements: " + elementList.Count);
	}

	private UnityEngine.UI.Button activeButton;
	private bool animationActive;
	private void ActivateAnimation()
	{
		//Update buttons
		int Z_TOP = 9;
		int DEFAULT = 0;

		//Adjust the button functionality
		for(int i = 0; i < elementList.Count && i < qsa.transform.childCount; i++) {
			UnityEngine.UI.Button b = qsa.transform.GetChild(i).GetComponent<UnityEngine.UI.Button>();
			SelectableElement element = elementList[i];
			//Add button hover events
			EventTrigger trigger;
			if ((trigger = b.GetComponent<EventTrigger>()) == null) {
				trigger = b.gameObject.AddComponent<EventTrigger>();
			}
			trigger.triggers.Clear();

			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(
				delegate 
				{
					element.GetComponent<MaterialSwitchState>()?.Brighten();
					element.gameObject.layer = Z_TOP;
					activeButton = b;
					if (element.selectable is BrainElement) {
						SetCanvasCursorName(((BrainElement)element.selectable).elementName);
					} else {
						SetCanvasCursorName("");
					}

					Vector3 targetPos = element.GetComponent<Collider>().ClosestPoint(selector.cursor.position);
					if (targetPos == selector.cursor.position) {
						targetPos = element.transform.position;
					}
					SetLine(transform.position, targetPos);
				});
			trigger.triggers.Add(entry);
			EventTrigger.Entry exit = new EventTrigger.Entry();
			exit.eventID = EventTriggerType.PointerExit;
			exit.callback.AddListener(
				delegate
				{
					if (activeButton != null) {
						//Active button is only null here when OnClick has been called
						//We don't want to now darken the selected piece
						element.GetComponent<MaterialSwitchState>()?.Darken();
					}

					element.gameObject.layer = DEFAULT;
					if (activeButton == b) {
						activeButton = null;
					}
					SetCanvasCursorName("");
					DisableLine();
				});
			trigger.triggers.Add(exit);

			b.onClick.RemoveAllListeners();
			//Add click event
			b.onClick.AddListener(() =>
				
				{
					//Adjust to make the cursor go to the collision point?
					//Need custom data class to maintain this collision point maybe?
					//Or calculate it from the center --> closest point on edge?
					Debug.Log("Clicked " + b.name + ", Element: " + element.name);
					//selector.cursor.position = elementList[i].transform.position;
					Vector3 newPos = element.GetComponent<Collider>().ClosestPoint(selector.cursor.position);
					if (newPos.Equals(selector.cursor.position)) {
						newPos = element.transform.position;
					}
					selector.cursor.gameObject.SetActive(true);
					FindObjectOfType<OVRCursor>().GetComponent<MeshRenderer>().enabled = false;
					selector.SelectNew(element, newPos);
					element.GetComponent<MaterialSwitchState>().Activate();
					activeButton = null;
					DisableLine();
					//selector.SetSavedCursorPos(elementList[i].transform.position);
				});
		}

		animationActive = true;
		//Activate animation
		qsa.Activate(elementList.Count);
	}

	private void DisableLine()
	{
		line.enabled = false;
	}

	private void SetLine(Vector3 startPos, Vector3 endPos)
	{
		line.enabled = true;
		Vector3[] points = new Vector3[] { startPos, endPos };
		line.SetPositions(points);
	}

	List<string> partsList = new List<string>();
	[ContextMenu("Print parts list")]
	private void PrintPartsList()
	{
		PrintList(partsList);
	}

	private void PrintList<T>(List<T> list)
	{
		string stuff = "";
		foreach (T obj in list) {
			stuff += obj.ToString() + "\n";
		}
		print(stuff);
	}

	private void SetCanvasCursorName(string name)
	{
		FindObjectOfType<OVRInputModule>().m_Cursor.GetComponentInChildren<TMPro.TextMeshPro>().text = name;
	}

	private void DeactivateAnimation()
	{
		animationActive = false;
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

	[ContextMenu("List Non convex mesh colliders")]
	private void ListNonConvexMeshes()
	{
		MeshCollider[] colliders = FindObjectsOfType<MeshCollider>();
		List<MeshCollider> badColliders = new List<MeshCollider>();
		foreach(MeshCollider mc in colliders) {
			if (!mc.convex) {
				badColliders.Add(mc);
			}
		}

		string s = "";
		foreach(MeshCollider mc in badColliders) {
			Debug.Log(mc.name, mc.gameObject);
			s += mc.name + "\n";
		}

		print(s);
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
