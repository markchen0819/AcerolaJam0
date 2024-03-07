using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGrooveEvent : MonoBehaviour
{
    public GameObject track;
    public Animator trackAnimator;
    public float duration = 0.2f;

    Vector3 originalPos;
    Quaternion originalRot;
    Vector3 stopPos;
    Quaternion stopRot;
    public void Awake()
    {
        originalPos = track.transform.position;
        originalRot = track.transform.rotation;
    }
    public void Execute(bool start)
    {
        if(start)
        {
            Debug.Log("TrackGroove");
            trackAnimator.SetBool("stopGroove", false);
            trackAnimator.Play("TrackGroove");
        }
        else // stop
        {
            Debug.Log("TrackStop");
            trackAnimator.SetBool("stopGroove", true);
        }
    }

}
