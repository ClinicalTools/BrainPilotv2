using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour {

	public LineCastSelector selector;
	
	//Use player prefs
	//Manage settings and their names

	private const int ENABLED = 1;
	private const int DISABLED = 0;
	private const int NOT_FOUND = -1;

	public enum SettingType
	{
		drone_size,
		drone_persistance,
		movement_style,
		lesson,
		toggle_click,
		sound_master,
		sound_ambient,
		sound_narration
	}

	[System.Serializable]
	public struct DefaultValue
	{
		public SettingType settingType;
		public string key;
		public bool value;
	}

	public List<DefaultValue> defaultValues;

	private Dictionary<SettingType, HashSet<string>> allSettings;

	// Use this for initialization
	void Start()
	{
		if (allSettings == null) {
			allSettings = new Dictionary<SettingType, HashSet<string>>();
		}
		if (selector == null) {
			selector = FindObjectOfType<LineCastSelector>();
		}
		
		//We want to initialize our dataset based on existing settings
		List<SettingsElement> a = new List<SettingsElement>(GetComponentsInChildren<SettingsElement>(true));
		foreach (SettingsElement element in a) {
			if (!allSettings.ContainsKey(element.type)) {
				allSettings.Add(element.type, new HashSet<string>() { element.settingKey });
			} else {
				allSettings[element.type].Add(element.settingKey);
			}
			

			int value = GetValue(element.type, element.settingKey);

			if (value != NOT_FOUND) {
				Debug.Log("Attempting to resolve " + GetPrefsString(element.type, element.settingKey));
				defaultValues.RemoveAll(x =>
					x.settingType == element.type &&
					x.key.Equals(element.settingKey));
			}
			if (value == ENABLED) {
				Debug.Log(GetPrefsString(element.type, element.settingKey) + " has been enabled");
				element.Invoke();
			}
		}

		foreach(DefaultValue val in defaultValues) {
			Debug.Log("Setting default value " + GetPrefsString(val.settingType, val.key) + " to " + val.value);
			//SetValue(val.settingType, val.key, val.value, val.radio);
			a.Find(x =>
				x.type == val.settingType &&
				x.settingKey == val.key).OnClick();
		}
	}

	public void EnableElement(SettingType type, string name, bool disableOthersOfType = false)
	{
		SetValue(type, name, true, disableOthersOfType);
	}

	public void EnableElement(SettingsElement element)
	{
		SetValue(element.type, element.settingKey, true, element.isRadio);
	}

	public void DisableElement(SettingType type, string name, bool disableOthersOfType = false)
	{
		SetValue(type, name, false, disableOthersOfType);
	}

	public void DisableElement(SettingsElement element)
	{
		SetValue(element.type, element.name, false, element.isRadio);
	}

	public bool CheckValue(SettingType type, string name)
	{
		if (PlayerPrefs.GetInt(GetPrefsString(type, name)) != ENABLED) {
			return false;
		}
		return true;
	}

	public int GetValue(SettingType type, string name)
	{
		return PlayerPrefs.GetInt(GetPrefsString(type, name), NOT_FOUND);
	}

	private string GetPrefsString(SettingType type, string name)
	{
		return type.ToString() + ":" + name;
	}

	private void SetSingleValue(SettingType type, string name, bool enabled)
	{
		if (!allSettings.ContainsKey(type)) {
			allSettings.Add(type, new HashSet<string>());
		}
		allSettings[type].Add(name);

		PlayerPrefs.SetInt(GetPrefsString(type, name), enabled ? 1 : 0);
		PlayerPrefs.Save();
		Debug.Log("Saved entry: " + GetPrefsString(type, name) + " = " + (enabled ? 1 : 0));
		return;
	}

	private void SetValue(SettingType type, string name, bool value, bool disableOtherOfType = false)
	{
		if (disableOtherOfType) {
			if (!allSettings.ContainsKey(type)) {
				allSettings.Add(type, new HashSet<string>() { name });
			}
			string[] keys2 = new string[allSettings[type].Count];
			var enumerator = allSettings[type].GetEnumerator();
			do {
				if (CheckValue(type, enumerator.Current)) {
					SetSingleValue(type, enumerator.Current, false);
				}
			} while (enumerator.MoveNext());
		}

		SetSingleValue(type, name, value);
	}

	public void CloseMenu()
	{
		if (selector == null) {
			selector = FindObjectOfType<LineCastSelector>();
		}
		FindObjectOfType<OVRCursor>().GetComponent<MeshRenderer>().enabled = false;
		selector.Enable();
		transform.GetChild(1).gameObject.SetActive(false);
	}

	public void OpenMenu()
	{
		if (selector == null) {
			selector = FindObjectOfType<LineCastSelector>();
		}
		selector.Disable();
		transform.GetChild(1).gameObject.SetActive(true); 
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
