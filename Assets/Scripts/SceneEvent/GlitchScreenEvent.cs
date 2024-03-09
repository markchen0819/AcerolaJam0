using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlitchScreenEvent : MonoBehaviour
{
    public float duration = 2.0f;

    public PostProcessVolume volume;
    public GlitchEffect glitch;

    public float blockSize = 45;
    public float displacementAmount = 0.1f;
    public void Awake()
    {
        volume.profile.TryGetSettings<GlitchEffect>(out glitch);
    }
    public void Execute()
    {
        glitch.blockSize.value = blockSize;
        glitch.displacementAmount.value = displacementAmount;
        glitch.enabled.Override(true);
        StartCoroutine(Pixel());
    }
    private IEnumerator Pixel()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            glitch.displacementAmount.value = Mathf.Lerp(displacementAmount, 0.0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        glitch.blockSize.value = 1024;
        glitch.displacementAmount.value = 0;
        glitch.enabled.Override(false);

    }
}
