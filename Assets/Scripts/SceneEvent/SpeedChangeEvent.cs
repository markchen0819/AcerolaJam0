using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChangeEvent : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.0f;

    [SerializeField]
    List<NoteBase> timingGroup = new List<NoteBase>();

    float fromSpeed = 0.0f;
    float toSpeed = 0.0f;

    public void Execute(float duration, float fromSpeed, float toSpeed, List<NoteBase> timingGroup)
    {
        this.duration = duration;
        this.timingGroup = timingGroup;
        this.fromSpeed = fromSpeed;
        this.toSpeed = toSpeed;

        StartCoroutine(SpeedChange());
    }

    private IEnumerator SpeedChange()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            foreach (NoteBase n in timingGroup)
            {
                n.OverrideSpeed(Mathf.Lerp(fromSpeed, toSpeed, t));
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        foreach (NoteBase n in timingGroup)
        {
            n.OverrideSpeed(toSpeed);
        }
    }
}
