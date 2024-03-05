
using UnityEngine;

public class BouncyNote : NoteBase
{
    float freqency = 1;
    float height = 0.5f;
    float heightOffset = -1.0f;

    public void CalculateBounceData()
    {
        float cycleTime = secondsPerBeat / 6; // hardcode for now
        freqency = 1 / cycleTime;
    }
    protected override void CustomUpdate()
    {
        float gameTime = gc.GetTime();
        float distanceToTarget = gameTime - noteTime;

        Vector3 target = targetPos + direction * distanceToTarget * noteSpeed * distanceMultiplier;
        target.y = Mathf.Abs(Mathf.Sin(gameTime * freqency)) * height + heightOffset;

        this.transform.localPosition = target;
    }
}
