using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TrainCamBehavior : MonoBehaviour {

    public Transform target;

    // Use this for initialization
    void Start()
    {

        //UnityEngine.XR.InputTracking.disablePositionalTracking = true;

    }


    // Update is called once per frame
    void Update () {

        // Disable Position Tracking for the Camera
        //transform.position = -InputTracking.GetLocalPosition(XRNode.CenterEye);
        //transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye));

        if (target == null)
        {
            transform.localRotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye));
        }
        else
        {
            transform.rotation = target.rotation * Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye));
        }

    }
}
