using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNote : NoteBase
{
    public List<GameObject> children;

    public float childDistOffset = 0.5f;

    protected override void CustomUpdate()
    {
        float gameTime = gc.GetTime();
        float distanceToTarget = gameTime - noteTime;

        this.transform.localPosition = targetPos + direction * distanceToTarget * noteSpeed * distanceMultiplier;

        Vector3 parentPos = Vector3.zero;
        children[0].transform.localPosition = parentPos - ( direction * childDistOffset * 1.0f);
        children[1].transform.localPosition = parentPos - ( direction * childDistOffset * 2.0f);
        children[2].transform.localPosition = parentPos -  (direction * childDistOffset * 3.0f);
        children[3].transform.localPosition = parentPos - ( direction * childDistOffset * 4.0f);
    }
}
