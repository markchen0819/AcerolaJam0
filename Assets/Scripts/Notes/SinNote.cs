using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinNote : NoteBase
{
    float freqency = 1;
    float width = 0.17f;
    float sinDir= 1.0f;
    float baseSpeed = 0.0f;

    public void CalculateSinData()
    {
        float cycleTime = secondsPerBeat / 2; // hardcode for now
        freqency = 1 / cycleTime;
        baseSpeed = noteSpeed;

        float rnd = Random.Range(-1.0f, 1.0f);
        if(rnd > 0.0)
        {
            sinDir = 1.0f;
        }
        else
        {
            sinDir = -1.0f;
        }
    }
    protected override void CustomUpdate()
    {
        float gameTime = gc.GetTime();
        float distanceToTarget = gameTime - noteTime;

        float sinValue = Mathf.Sin(gameTime * (1.0f /(secondsPerBeat / 6)));
        float scale = 0.25f;
        baseSpeed = noteSpeed + sinValue * 0.5f * scale; // speedChange
        Vector3 target = targetPos + direction * distanceToTarget * baseSpeed * distanceMultiplier;
        target.x = target.x + sinDir * Mathf.Sin(gameTime * freqency) * width;

        this.transform.localPosition = target;
    }
}