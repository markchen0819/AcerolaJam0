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



        events.Enqueue(new SceneEvent((48 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(1); }));


        events.Enqueue(new SceneEvent((80 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(2); }));


        events.Enqueue(new SceneEvent((112 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(3); }));
        for (int i= 112; i<144; ++i)
        {
            events.Enqueue(new SceneEvent((i + beatToSkipBegin) * secondsPerBeat + offset, cameraShakeEvent.Execute));
            events.Enqueue(new SceneEvent((i + beatToSkipBegin) * secondsPerBeat + secondsPerBeat/2 + offset, cameraShakeEvent.Execute));
        }

        events.Enqueue(new SceneEvent((144 + beatToSkipBegin) * secondsPerBeat + offset, () => { changeBackgroundEvent.Execute(4); }));
        events.Enqueue(new SceneEvent((176 + beatToSkipBegin) * secondsPerBeat + offset, () => { trackGrooveEvent.Execute(true); }));
        events.Enqueue(new SceneEvent((224 + beatToSkipBegin) * secondsPerBeat + offset, () => { trackGrooveEvent.Execute(false); }));
    }


}
