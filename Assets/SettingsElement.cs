using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode,
RequireComponent(typeof(Button))]
public class SettingsElement : MonoBehaviour {

	public SettingsManager.SettingType type;
	public string settingKey;
	[Tooltip("Whether or not only one menu setting can be enabled at a time")]
	public bool isRadio = true;
	public bool isPersistant = true;
	public SettingsManager manager;
	public UnityEngine.Events.UnityEvent onActivate;

	private Button b;
	private bool isChosen;

	private Color SELECTED_COLOR = Color.cyan;
	private Color DESELECTED_COLOR = Color.clear;

	private void OnEnable()
	{
		if (manager == null) {
			manager = FindObjectOfType<SettingsManager>();
		}
	}

	public void OnClick()
	{
		Invoke();
		UpdateManager();
	}

	public void Invoke()
	{
		onActivate.Invoke();
		if (b == null) {
			b = GetComponent<Button>();
		}

		if (isPersistant) {
			ColorBlock c = b.colors;
			c.normalColor = SELECTED_COLOR;
			b.colors = c;
		}

		if (isRadio) {
			SettingsElement[] ses = transform.parent.GetComponentsInChildren<SettingsElement>();
			foreach(SettingsElement se in ses) {
				if (se.GetIsChosen() && !se.settingKey.Equals(settingKey)) {
					se.Deselect();
				}
			}
		}

		isChosen = true;
	}

	private bool GetIsChosen()
	{
		return isChosen;
	}

	private void Deselect()
	{
		isChosen = false;
		if (isPersistant) {
			ColorBlock c = b.colors;
			c.normalColor = DESELECTED_COLOR;
			b.colors = c;
		}
	}

	private void UpdateManager()
	{
		if (manager == null) {
			manager = FindObjectOfType<SettingsManager>();
		}
		manager.EnableElement(this);
	}
}
