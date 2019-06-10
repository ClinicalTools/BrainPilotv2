using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode,
RequireComponent(typeof(Button))]
public class SettingsElement : MonoBehaviour {

	[Header("Settings Identifiers")]
	public SettingsManager.SettingType type;
	public string settingKey;

	[Header("Element Options")]
	[Tooltip("Whether or not only one menu setting can be enabled at a time")]
	public bool isRadio = true;
	[Tooltip("Whether or not the setting should remain selected")]
	public bool isPersistant = true;
	[Tooltip("Whether or not the menu page should return upon selection")]
	public bool returnOnSelect = false;

	[Header("Scene Variables")]
	public SettingsManager manager;
	public MenuPageElement menuPage;
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
		if (menuPage == null) {
			menuPage = GetComponentInParent<MenuPageElement>();
		}
	}

	public void OnClick()
	{
		Invoke();
		CheckOptions();
		UpdateManager();
	}

	public void Invoke()
	{
		onActivate.Invoke();
		isChosen = true;

		if (b == null) {
			b = GetComponent<Button>();
		}

		if (isPersistant) {
			ColorBlock c = b.colors;
			c.normalColor = SELECTED_COLOR;
			b.colors = c;
		}
	}

	public void CheckOptions()
	{
		if (b == null) {
			b = GetComponent<Button>();
		}

		if (isRadio) {
			SettingsElement[] ses = transform.parent.GetComponentsInChildren<SettingsElement>();
			foreach (SettingsElement se in ses) {
				if (se.GetIsChosen() && !se.settingKey.Equals(settingKey)) {
					se.Deselect();
				}
			}
		}

		if (returnOnSelect) {
			if (menuPage == null) {
				menuPage = GetComponentInParent<MenuPageElement>();
			}
			menuPage.GoBack();
		}
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
