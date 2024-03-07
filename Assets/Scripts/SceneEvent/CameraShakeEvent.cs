using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeEvent : MonoBehaviour
{
    public float duration = 0.2f;
    public float scale = 0.01f;
    public GameObject cam;
    private Vector3 originalPos;
    public void Execute()
    {
        //Debug.Log("Camera Shake");
        originalPos = cam.transform.position;
        StartCoroutine(Shake());
    }
    private IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * scale;
            float y = Random.Range(-1f, 1f) * scale;
            float z = Random.Range(-1f, 1f) * scale;
            cam.transform.position = originalPos + new Vector3(x, y, z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = originalPos;
    }
}
