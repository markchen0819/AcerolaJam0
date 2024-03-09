using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // Camera Shake
    public float duration = 0.2f;
    public float scale = 0.01f;
    public GameObject cam;
    private Vector3 originalPos;

    // Hurt Image
    public float hurtImageOpacity = 0.7f;
    public Image hurtImage;
    public TextMeshProUGUI hitCountText;
    public AudioSource hitSFX;

    private void Awake()
    {
        originalPos = cam.transform.position;
    }
    public void PlayHitUI(int hitCount)
    {
        hitCountText.text = hitCount.ToString();
        StartCoroutine(Hit());
        hitSFX.Play();
    }

    private IEnumerator Hit()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // camera shake
            float x = Random.Range(-1f, 1f) * scale;
            float y = Random.Range(-1f, 1f) * scale;
            float z = Random.Range(-1f, 1f) * scale;
            cam.transform.position = originalPos + new Vector3(x, y, z);
            elapsed += Time.deltaTime;

            // hurt image opacity
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(hurtImageOpacity, 0, t);
            hurtImage.color = new Color(hurtImage.color.r, hurtImage.color.g, hurtImage.color.b, alpha);

            yield return null;
        }
        cam.transform.position = originalPos;
        hurtImage.color = new Color(hurtImage.color.r, hurtImage.color.g, hurtImage.color.b, 0);
    }
}
