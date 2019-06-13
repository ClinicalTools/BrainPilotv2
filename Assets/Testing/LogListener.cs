using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LogListener : MonoBehaviour {

	private const string COLLAPSE_LOG_REGEX = "([(][0-9]+[)])?\n$";
	private const int LOG_MESSAGE_MAX_LEN = 300;
	private const int LIMIT_LOG_COUNT = 25;

	private Tuple<Vector3, Vector2, Vector3, Vector3> consoleWorldPos;

	public TMPro.TMP_InputField log;
	public UnityEngine.UI.Scrollbar scrollbar;

	private Canvas canvas;
	private int i;
	private List<Log> logList;
	private int duplicateCount;
	private MatchEvaluator regEval;
	private Regex reg;

	public bool limitLogs;
	public bool enableStack = false;
	public bool collapse = true;
	public bool drawToScreen = false;

	private void Awake()
	{
		i = 0;
		if (log == null) {
			log = GetComponentInChildren<TMPro.TMP_InputField>();
		}
		if (Application.platform == RuntimePlatform.Android) {
			GetComponent<OVRRaycaster>().enabled = true;
			GetComponent<GraphicRaycaster>().enabled = false;
		} else {
			GetComponent<GraphicRaycaster>().enabled = true;
			GetComponent<OVRRaycaster>().enabled = false;
		}
		canvas = GetComponent<Canvas>();
		if (canvas.worldCamera == null) {
			canvas.worldCamera = Camera.main;
		}
		
		//Save the canvas world position
		consoleWorldPos = Tuple.Create(canvas.transform.position, //Position
										new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height), //Width/height
										canvas.transform.localEulerAngles, //Rotation
										canvas.transform.localScale); //Scale

		//Invert the default toggle screen setting so that we can toggle it back and draw the screen as necessary
		drawToScreen = !drawToScreen;
		ToggleScreenDraw();

		logList = new List<Log>();

		reg = new Regex(COLLAPSE_LOG_REGEX);
		regEval = new MatchEvaluator(MatchEvaluator);
		duplicateCount = 1;
	}

	private void Start()
	{
		print(Application.platform);
		Application.logMessageReceived += WriteToCanvas;
		GetComponent<OVRRaycaster>().enabled = true;
	}

	/// <summary>
	/// Represents a log in the in-game console
	/// </summary>
	private class Log
	{
		string logString;
		string stackTrace;
		LogType type;
		int textIndex;

		public Log(string log, string stack, LogType type)
		{
			logString = log; stackTrace = stack; this.type = type;
		}

		public bool Equals(Log obj)
		{
			return (type.Equals(obj.type) &&
					logString.GetHashCode().Equals(obj.logString.GetHashCode()));/* &&
					stackTrace.GetHashCode().Equals(obj.stackTrace.GetHashCode()));*/
					
		}

		public int GetIndex()
		{
			return textIndex;
		}

		public void SetIndex(int i)
		{
			textIndex = i;
		}

		public void DecrementIndex(int i)
		{
			textIndex -= i;
		}
	}

	/// <summary>
	/// Writes a log to the in-game canvas when the console logs something
	/// </summary>
	/// <param name="logString">Log message</param>
	/// <param name="stackTrace">Log stack trace</param>
	/// <param name="type">Type of log</param>
	void WriteToCanvas(string logString, string stackTrace, LogType type)
	{
		if (type.Equals(null)) {
			return;
		}

		//If you're hitting the log limit, remove the top 10 logs
		if (limitLogs && logList.Count > LIMIT_LOG_COUNT) {
			//log.text = log.text.Substring(log.text.Length / 2);
			RemoveNFromLog(10);
		}

		//If log length gets too long, remove some logs to keep performance decent
		if (log.text.Length > 10000) {
			for(int j = 0; j < logList.Count; i++) {
				if (logList[j].GetIndex() > 4000) {
					RemoveNFromLog(j);
					break;
				}
			}
		}

		
		Log newLog = new Log(logString, stackTrace, type);
		logList.Add(newLog);

		//If collapse content is available and a duplicate log is entered, display the duplicate count of the newest log
		if (collapse && logList.Count > 2 && logList[logList.Count - 2].Equals(newLog)) {
			log.text = reg.Replace(log.text, regEval);
		} else {
			//Reset the duplicate log count if we have something new
			duplicateCount = 1;
			newLog.SetIndex(log.text.Length);

			//Add the log type to the start of the log
			log.text += type.ToString() + ":: ";

			//Add the log message. Truncate if too long
			if (logString.Length > LOG_MESSAGE_MAX_LEN) {
				log.text += logString.Substring(0, LOG_MESSAGE_MAX_LEN) + "...";
			} else {
				log.text += logString;
			}

			//Show the stack trace if enabled
			if (enableStack) {
				log.text += "\n\nStack: " + stackTrace;
			}

			//Add a divider to make it easier to read
			log.text += "\n-----" + i + "-----\n";
		}
		i++;

		//Scroll to the bottom. Done on the next frame to avoid errors with the scrollbar
		//StartCoroutine(ResetScroll());
		//NextFrame.Function(delegate { scrollbar.value = 0f; });
		scrollbar.value = 0;
	}
	private IEnumerator ResetScroll()
	{
		yield return null;
		scrollbar.value = 0;
	}

	///<summary>
	///Returns a string of the number of duplicate logs
	///</summary>
	private string MatchEvaluator(Match m)
	{
		duplicateCount++;
		return "(" + duplicateCount + ")\n";
	}

	///<summary>
	///Clears the loglist and the log text
	///</summary>
	public void ClearLog()
	{
		log.text = "";
		i = 0;
		logList.Clear();
		duplicateCount = 1;
	}

	///<summary>
	///Removes a specified number of logs from the start of the log
	///</summary>
	public void RemoveNFromLog(int n)
	{
		int removedLen = logList[n].GetIndex();
		if (removedLen < 0) {
			return;
		}
		log.text = log.text.Substring(removedLen);
		logList.RemoveRange(0, n);
		foreach (Log log in logList) {
			log.DecrementIndex(removedLen);
		}
	}

	#region ----------BUTTON METHODS----------

	///<summary>
	///Toggles the log count limit on or off
	///</summary>
	public void ToggleLimitLogCount(Text display)
	{
		limitLogs = !limitLogs;
		if (display != null) {
			if (limitLogs) {
				display.text = "On";
				display.color = new Color(0f, 1f, 0f);
			} else {
				display.color = new Color(1f, 0f, 0f);
				display.text = "Off";
			}
		}
	}
	
	//Helper method for code (when not called from a button)
	private void ToggleLimitLogCount()
	{
		ToggleLimitLogCount(null);
	}

	///<summary>
	///Toggles whether or not to show the stack
	///</summary>
	public void ToggleStack(Text display)
	{
		enableStack = !enableStack;
		if (display != null) {
			if (enableStack) {
				display.text = "On";
				display.color = new Color(0f, 1f, 0f);
			} else {
				display.color = new Color(1f, 0f, 0f);
				display.text = "Off";
			}
		}
	}

	//Helper method for code (when not called from a button)
	private void ToggleStack()
	{
		ToggleStack(null);
	}

	///<summary>
	///Removes a specified number of logs from the start of the log
	///</summary>
	public void ToggleCollapse(Text display)
	{
		collapse = !collapse;
		if (display != null) {
			if (collapse) {
				display.text = "On";
				display.color = new Color(0f, 1f, 0f);
			} else {
				display.color = new Color(1f, 0f, 0f);
				display.text = "Off";
			}
		}
	}

	//Helper method for code (when not called from a button)
	private void ToggleCollapse()
	{
		ToggleCollapse(null);
	}

	public void ToggleScreenDraw(Text display)
	{
		drawToScreen = !drawToScreen;
		if (!drawToScreen) {
			canvas.renderMode = RenderMode.WorldSpace;
			canvas.GetComponent<CanvasScaler>().enabled = false;
			canvas.GetComponent<OVRRaycaster>().enabled = true;
			FindObjectOfType<UnityEngine.EventSystems.OVRInputModule>().enabled = true;

			canvas.transform.position = consoleWorldPos.Item1;
			canvas.GetComponent<RectTransform>().sizeDelta = consoleWorldPos.Item2;
			//canvas.transform.Rotate(consoleWorldPos.Item3);
			canvas.transform.localEulerAngles = consoleWorldPos.Item3;
			canvas.transform.localScale = consoleWorldPos.Item4;
		} else if (!OVRManager.isHmdPresent) {
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.GetComponent<CanvasScaler>().enabled = true;
			canvas.GetComponent<OVRRaycaster>().enabled = false;
			FindObjectOfType<UnityEngine.EventSystems.OVRInputModule>().enabled = false;
		} else {
			return;
		}

		if (display != null) {
			if (drawToScreen) {
				display.text = "On";
				display.color = new Color(0f, 1f, 0f);
			} else {
				display.color = new Color(1f, 0f, 0f);
				display.text = "Off";
			}
		}
	}

	private void ToggleScreenDraw()
	{
		ToggleScreenDraw(null);
	}

	public void CreateTestLog()
	{
		CreateTestLog("This is a test log message!");
	}

	public void CreateTestLog(string message)
	{
		CreateTestLog(message, LogType.Log);
	}

	public void CreateTestLog(string message, LogType logType)
	{
		WriteToCanvas(message, "No stack trace available", logType);
	}
	#endregion
}
