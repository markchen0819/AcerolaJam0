using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CycleColorBG6 : MonoBehaviour
{
    public float speed = 0.1f;
    public float bloomIntensity = 2.0f;
    private float hue = 0.0f;

    //https://docs.unity3d.com/Packages/com.unity.postprocessing@3.4/manual/Manipulating-the-Stack.html
    public PostProcessVolume volume;
    public Bloom bloom;
    private void Awake()
    {
        volume.profile.TryGetSettings<Bloom>(out bloom);
    }
    void Update()
    {
        hue = (hue + Time.deltaTime * speed) % 1.0f;
        Color newColor = Color.HSVToRGB(hue, 1.0f, 1.0f) * bloomIntensity;
        bloom.color.value = newColor;
    }
}
