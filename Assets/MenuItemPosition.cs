using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TranformUpdateEvent : UnityEvent<Transform> { }

public class MenuItemPosition : MonoBehaviour {

    public int thisPositionIndex;
    public bool useChildIndex;
    public MenuPositionController controller;

    public Transform targetTransform;

    public TranformUpdateEvent targetTransformUpdated;

    private void Start()
    {
        GetTargetAnchor();
    }

    [ContextMenu("Get Target Position")]
    public void GetTargetAnchor()
    {
        if (useChildIndex)
            thisPositionIndex = transform.GetSiblingIndex();

        controller = controller ?? transform.parent.GetComponent<MenuPositionController>();

        targetTransform = controller.GetAnchorTransform(thisPositionIndex);
        targetTransformUpdated.Invoke(targetTransform);
    }

}
