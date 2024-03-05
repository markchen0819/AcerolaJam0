using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBase : MonoBehaviour
{
    [SerializeField]
    protected float noteTime = 0;
    [SerializeField]
    protected float noteSpeed = 1;
    [SerializeField]
    protected Vector3 startPos;
    [SerializeField]
    protected Vector3 targetPos;
    [SerializeField]
    protected Vector3 direction;

    protected float secondsPerBeat = 2.0f;
    protected float distanceMultiplier = 50.0f;
    protected bool hasCollided = false;

    protected GameController gc = null;

    public void Init(GameController gc)
    {
        this.gc = gc;
    }

    public virtual void SetNoteData(float noteTime, float noteSpeed,
        Vector3 start, Vector3 target, float secondsPerBeat)
    {
        this.noteTime = noteTime;
        this.noteSpeed = noteSpeed;
        this.startPos = start;
        this.targetPos = target;
        direction = (target - start).normalized;
        this.secondsPerBeat = secondsPerBeat;
    }

    void Update()
    {
        if (gc == null) return;

        CustomUpdate();

        if(gc.GetTime() - noteTime > secondsPerBeat)
        {
            this.gameObject.SetActive(false);
        }
    }
    protected virtual void CustomUpdate()
    {
        Debug.Log("Implement in derived class!");
    }

    public void SetCollided()
    {
        hasCollided = true;
    }

}
