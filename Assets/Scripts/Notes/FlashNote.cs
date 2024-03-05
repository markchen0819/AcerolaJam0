

public class FlashNote : NoteBase
{
    float updateTimeOffset = 0.0f;
    float nextUpdateTime = 0.0f;
    float flashFreqency = 3;
    public void CalculateFlashData()
    {
        updateTimeOffset = secondsPerBeat / flashFreqency;
        nextUpdateTime = noteTime - secondsPerBeat;
    }
    protected override void CustomUpdate()
    {
        float gameTime = gc.GetTime();
   
        if(gameTime>nextUpdateTime)
        {
            float distanceToTarget = gameTime - noteTime;
            this.transform.localPosition = targetPos + direction * distanceToTarget * noteSpeed * distanceMultiplier;
            nextUpdateTime += updateTimeOffset;
        }
    }
}
