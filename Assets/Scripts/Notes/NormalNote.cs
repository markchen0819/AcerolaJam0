
public class NormalNote : NoteBase
{
    protected override void CustomUpdate()
    {
        float gameTime = gc.GetTime();
        float distanceToTarget = gameTime - noteTime;

        this.transform.localPosition = targetPos + direction * distanceToTarget * noteSpeed * distanceMultiplier;
    }
}
