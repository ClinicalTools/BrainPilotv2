using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour {

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
		sound_master,
		sound_ambient,
		sound_narration
	}

	private Dictionary<SettingType, Dictionary<string, bool>> allSettings;

	// Use this for initialization
	void Start()
	{
		if (allSettings == null) {
			allSettings = new Dictionary<SettingType, Dictionary<string, bool>>();
		}
		
		//We want to initialize our dataset based on existing settings
		List<SettingsElement> a = new List<SettingsElement>(GetComponentsInChildren<SettingsElement>(true));
		bool isEnabled = false;
		foreach (SettingsElement element in a) {
			//Add the setting element if it doesn't already exist
			if (!allSettings.ContainsKey(element.type)) {
				allSettings.Add(element.type, new Dictionary<string, bool>());
			}

			//Check the value saved in player prefs
			if (!allSettings[element.type].ContainsKey(element.settingKey)) {
				isEnabled = CheckValue(element.type, element.settingKey);
				allSettings[element.type].Add(element.settingKey, isEnabled);
				if (isEnabled) {
					Debug.Log(GetPrefsString(element.type, element.settingKey) + " has been enabled");
					element.Invoke();
				}
			}
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

		try {
			return allSettings[type][name];
		} catch (KeyNotFoundException) {
			Debug.LogWarning("Key " + type.ToString() + ":" + name + " not found!\n"
				+ (allSettings.ContainsKey(type) ? "Type not found." : "Element not found."));
			return false;
		}
	}

	private string GetPrefsString(SettingType type, string name)
	{
		return type.ToString() + ":" + name;
	}

	private void SetSingleValue(SettingType type, string name, bool enabled)
	{
		if (!allSettings.ContainsKey(type)) {
			allSettings.Add(type, new Dictionary<string, bool>());
		}

		if (!allSettings[type].ContainsKey(name)) {
			allSettings[type].Add(name, enabled);
		} else {
			allSettings[type][name] = enabled;
		}
		PlayerPrefs.SetInt(GetPrefsString(type, name), enabled ? 1 : 0);
		PlayerPrefs.Save();
		Debug.Log("Saved entry: " + GetPrefsString(type, name) + " = " + (enabled ? 1 : 0));
		return;
	}

	private void SetValue(SettingType type, string name, bool value, bool disableOtherOfType = false)
	{
		if (disableOtherOfType) {
			//Find others with the same type and disable them
			if (!allSettings.ContainsKey(type)) {
				allSettings.Add(type, new Dictionary<string, bool>());
			}
			string[] keys = new string[allSettings[type].Count];
			allSettings[type].Keys.CopyTo(keys,0);
			foreach (string key in keys) {
				if (CheckValue(type, key)) {
					SetSingleValue(type, key, false);
				}
			}
		}

		SetSingleValue(type, name, value);
	}
}
