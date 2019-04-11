using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubSceneListener : MonoBehaviour {

	[SerializeField]
	protected SubSceneManager subSceneManager;

	public abstract void Invoke();

	protected void RetrieveSubSceneManager()
	{
#if UNITY_EDITOR
		CustomUtilities.LoadAsset.Load(out subSceneManager, "Assets/Data");
#else
		throw new System.Exception("Cannot load sub scene manager!");
#endif
	}
}
