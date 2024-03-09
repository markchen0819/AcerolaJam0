using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void EventCallback();
public class SceneEvent
{
    private float triggerTime;
    private EventCallback callback;

    public SceneEvent(float triggerTime, EventCallback callback)
    {
        this.triggerTime = triggerTime;
        this.callback = callback;
    }

    public void Trigger()
    {
        callback?.Invoke(); 
    }

    public float GetTriggerTime() { return triggerTime; }

}

public class SceneEventSequencer : MonoBehaviour
{
    GameController gc;
    public CameraShakeEvent cameraShakeEvent;
    public TrackGrooveEvent trackGrooveEvent;
    public ChangeBackgroundEvent changeBackgroundEvent;
    public GlitchScreenEvent glitchScreenEvent;
    public SpeedChangeEvent speedChangeEvent;

    private Queue<SceneEvent> events = new Queue<SceneEvent>();

    public void Init(GameController gc)
    {
        this.gc = gc;
    }
    private void Update()
    {
        if (gc == null) return;

        if(events.Count>0)
        {
            SceneEvent e = events.Peek();
            if (e != null && gc.GetTime() >= e.GetTriggerTime() )
            {
                e.Trigger();
                events.Dequeue();
            }
        }

    }
    public void CreateSceneEvents()
    {
        float offset = -0.13f;
        float secondsPerBeat = gc.GetSecondsPerBeat();
        float beatToSkipBegin = gc.GetBeatToSkipBegin();

        // Normal
        // Drag
        events.Enqueue(new SceneEvent((48 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(1); }));
        // make sure not show on screen
        events.Enqueue(new SceneEvent((72 + beatToSkipBegin) * secondsPerBeat + offset, () =>
        {
            speedChangeEvent.Execute(0.3f, 1.0f, 1.0f, gc.GetTimingGroupOne());
        }));
        // fast to slow in 
        float duration = secondsPerBeat;
        events.Enqueue(new SceneEvent((79 + beatToSkipBegin) * secondsPerBeat + offset, () =>
        {
            speedChangeEvent.Execute(duration, 1.0f, 0.2f, gc.GetTimingGroupOne());
        }));

        // Bounce
        events.Enqueue(new SceneEvent((80 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(2); }));

        // Flash
        events.Enqueue(new SceneEvent((112 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(3); }));
        for (int i= 112; i<144; ++i)
        {
            events.Enqueue(new SceneEvent((i + beatToSkipBegin) * secondsPerBeat + offset, cameraShakeEvent.Execute));
            events.Enqueue(new SceneEvent((i + beatToSkipBegin) * secondsPerBeat + secondsPerBeat/2 + offset, cameraShakeEvent.Execute));
        }
        events.Enqueue(new SceneEvent((143 + beatToSkipBegin) * secondsPerBeat + offset, glitchScreenEvent.Execute));

        // Sin
        events.Enqueue(new SceneEvent((144 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(4); }));

        // Mix
        events.Enqueue(new SceneEvent((176 + beatToSkipBegin) * secondsPerBeat + offset, () => { trackGrooveEvent.Execute(true); }));
        events.Enqueue(new SceneEvent((176 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(5); }));
        events.Enqueue(new SceneEvent((224 + beatToSkipBegin) * secondsPerBeat + offset, () => { trackGrooveEvent.Execute(false); }));
        // Normal

        events.Enqueue(new SceneEvent((224 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(0); }));

        // fast to slow in 
        // dont share dutation due to closure
        float duration2 = secondsPerBeat * (239.0f - 226.0f);
        events.Enqueue(new SceneEvent((224 + beatToSkipBegin) * secondsPerBeat + offset, () =>
        {
            speedChangeEvent.Execute(duration2, 0.5f, 0.1f, gc.GetTimingGroupTwo());
        }));
    }


}
