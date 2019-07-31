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
		if (!selector.isActive) {
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
				//Figure out which button to select
				active = false;
				line.positionCount = 0;
				line.SetPositions(new Vector3[0]);

				//Find out if over a button
				/*UnityEngine.UI.Button b = qsa.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>();
				PointerEventData data = new PointerEventData(EventSystem.current);
				b.OnPointerClick(data);*/
				DeactivateAnimation();
				qsa.CheckForButtonClick(activeButton);
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

	public float sphereRadius;
	List<SelectableElement> elementList = new List<SelectableElement>();
	private void LoadNearbySelections()
	{
		//Sphere cast from startPos;
		Collider[] c = Physics.OverlapSphere(startPos, sphereRadius);
		//Debug.Log(c.Length);
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

	private UnityEngine.UI.Button activeButton;
	private bool animationActive;
	private void ActivateAnimation()
	{
		//Update buttons
		EventTrigger trigger = null;
		int Z_TOP = 9;
		int DEFAULT = 0;

		//Adjust the button functionality
		for(int i = 0; i < elementList.Count && i < qsa.transform.childCount; i++) {
			UnityEngine.UI.Button b = qsa.transform.GetChild(i).GetComponent<UnityEngine.UI.Button>();
			SelectableElement element = elementList[i];
			//Add button hover events
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
					SetCanvasCursorName(((BrainElement)element.selectable).elementName);
				});
			trigger.triggers.Add(entry);
			EventTrigger.Entry exit = new EventTrigger.Entry();
			exit.eventID = EventTriggerType.PointerExit;
			exit.callback.AddListener(
				delegate
				{
					element.GetComponent<MaterialSwitchState>()?.Darken();
					element.gameObject.layer = DEFAULT;
					if (activeButton == b) {
						activeButton = null;
					}
					SetCanvasCursorName("");
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
					print(newPos);
					selector.cursor.gameObject.SetActive(true);
					FindObjectOfType<OVRCursor>().GetComponent<MeshRenderer>().enabled = false;
					selector.SelectNew(element, newPos);
					activeButton = null;
					//selector.SetSavedCursorPos(elementList[i].transform.position);
				});
		}

		animationActive = true;
		//Activate animation
		qsa.Activate(elementList.Count);
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

	private void DebugSelections()
	{
		string s = "";
		foreach (SelectableElement e in elementList) {
			s += e.name + ": " + (e.transform.position - startPos) + "\n";
		}
		print(s);
	}
}
