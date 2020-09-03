using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using System;

[RequireComponent(typeof(ARRaycastManager))]

public class ARPlacementScript : MonoBehaviour
{


    public GameObject placementIndicator;
    public GameObject objectToPlace;

    private ARRaycastManager rayCastManager;
    private ARPlaneManager planeManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private Quaternion targetRotation;

    private GameObject aso;

    private void Start()
    {
        targetRotation = placementIndicator.transform.rotation;
    }

    void Awake()
    {
        aso = GameObject.Find("AR Session Origin");
        rayCastManager = aso.GetComponent<ARRaycastManager>();
        planeManager = aso.GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }
    private void PlaceObject()
    {
        GameObject obj = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        obj.transform.up = placementPose.up;
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            placementIndicator.transform.Rotate(new Vector3(0, 90, 0));
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
    // private void UpdatePlacementPose()
    void UpdatePlacementPose()
    {

        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        rayCastManager.Raycast(screenCenter, hits, TrackableType.Planes);


        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            //hits[0].

            placementPose.rotation = Quaternion.FromToRotation(aso.transform.up, planeManager.GetPlane(hits[0].trackableId).normal);
            //  =planeManager.GetPlane(hits[0].trackableId).transform;
        }
    }
}
