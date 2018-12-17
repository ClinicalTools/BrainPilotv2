using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPositionController : MonoBehaviour {

    public Transform lowerButtonOrigin;
    public Transform mainCamera;
    [Range(0, 5)]
    public int numberOfSubItems = 3;

    public List<Transform> anchors;
    protected List<Vector3> anchorPositions;

    private void Awake()
    {
        GenerateAnchors();
    }

    [ContextMenu("Generate Anchors")]
    public void GenerateAnchors()
    {
        ClearAnchorObjects();
        RecalculatePositions();
        CreateAnchorObjects();
    }

    [ContextMenu("Update Anchors")]
    public void UpdateAnchors()
    {
        RecalculatePositions();
        MoveAnchorObjects();
    }
    
    public void RecalculatePositions()
    {
        mainCamera = mainCamera ?? Camera.main.transform;
        lowerButtonOrigin = lowerButtonOrigin ?? transform.parent;

        float yDistance = mainCamera.position.y - lowerButtonOrigin.position.y;
        float margin = yDistance / (float)numberOfSubItems;
        float lowerYValue = yDistance / 2f;
        anchorPositions = new List<Vector3>();
        
        for (int i = 0; i < numberOfSubItems; i++)
        {
            anchorPositions.Add(new Vector3(lowerButtonOrigin.position.x, lowerButtonOrigin.position.y + (lowerYValue + i * margin), lowerButtonOrigin.position.z));
           
        }

    }

    public void ClearAnchorObjects()
    {
        if (anchors == null)
        {
            anchors = new List<Transform>();
            return;
        }
        foreach(var anchor in anchors)
        {
            Destroy(anchor.gameObject);
        }
        anchors.Clear();
    }

    public void CreateAnchorObjects()
    {
        anchors = new List<Transform>();
        foreach(Vector3 position in anchorPositions)
        {
            var anchor = new GameObject("Anchor_");
            anchor.transform.SetParent(transform);
            anchor.transform.position = position;
            anchors.Add(anchor.transform);
        }
    }

    public void MoveAnchorObjects()
    {
        RecalculatePositions();
        if (anchors == null)
            anchors = new List<Transform>();

        for (int i = 0; i < anchorPositions.Count; i++)
        {
            if (anchors[i] == null)
            {
                var anchor = new GameObject("Anchor_");
                anchor.transform.SetParent(transform);
                anchor.transform.position = anchorPositions[i];
                anchors.Add(anchor.transform);
            }
            else
            {
                anchors[i].position = anchorPositions[i];
            }
        }
    }

    public Transform GetAnchorTransform(int index)
    {
        return anchors?[index];
    }



}
