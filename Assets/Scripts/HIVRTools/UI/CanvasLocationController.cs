using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(WorldCanvasSmoothFollow))]
public class CanvasLocationController : MonoBehaviour {

    public List<Transform> canvasAnchors;

    public Camera playerMainCamera;

    public float lookDelay = 2.0f;

    float currentLookTimer;

    Transform currentAnchor;

    public WorldCanvasSmoothFollow follower;



    public bool IsActive
    {
        get;
        set;
    }

    private void Start()
    {
        follower = GetComponent<WorldCanvasSmoothFollow>();
        RebuildCanvasAnchorsFromTags();
        IsActive = true;
        MoveToNewAnchor();

    }

    private void OnDestroy()
    {

    }

    private void RefreshOnLevelLoad()
    {

    }

    void RebuildCanvasAnchorsFromTags()
    {
        canvasAnchors = new List<Transform>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("CanvasAnchor"))
        {
            if (go != null && go.GetComponent<CanvasAnchor>() != null)
            {
                RegisterAnchor(go.transform);
            }
        }
    }

    public void RegisterAnchor(Transform anchor)
    {
        if (canvasAnchors == null)
            canvasAnchors = new List<Transform>();
        if (!canvasAnchors.Contains(anchor))
        {
            canvasAnchors.Add(anchor);
            MoveToNewAnchor();
        }
            
    }

    public void UnregisterAnchor(Transform anchor)
    {
        if (canvasAnchors.Contains(anchor))
        {
            canvasAnchors.Remove(anchor);
        }

    }

    private void LateUpdate()
    {
        if (IsActive)
            UpdateAnchor();

    }

    private void UpdateAnchor()
    {
        if (currentAnchor == null)
            currentAnchor = ClosestAnchorToLook();

        Transform closestAnchor = ClosestAnchorToLook();

        if (currentAnchor == closestAnchor)
        {
            currentLookTimer = 0;
            return;
        }

        if (currentAnchor != closestAnchor)
        {
            if (currentLookTimer > lookDelay)
                MoveToNewAnchor();
            else
                RunCountdown();
        }
    }

    void RunCountdown()
    {
        currentLookTimer += Time.deltaTime;
    }

    void MoveToNewAnchor()
    {
        currentAnchor = ClosestAnchorToLook();
        follower.target = currentAnchor;
    }

    Transform ClosestAnchorToLook()
    {
        float closestAngle = Mathf.Infinity;
        Transform closestTransform = null;

        foreach (Transform testTransform in canvasAnchors)
        {
            if (testTransform == null)
            {
                //canvasAnchors.Remove(testTransform);
                continue;
            }
                
            Vector3 cameraToTransform = testTransform.position - playerMainCamera.transform.position;
            cameraToTransform.Normalize();
            float angle = Vector3.Angle(playerMainCamera.transform.forward, cameraToTransform);
            if (angle < closestAngle)
            {
                closestAngle = angle;
                closestTransform = testTransform;
            }
        }

        return closestTransform;
    }


}
