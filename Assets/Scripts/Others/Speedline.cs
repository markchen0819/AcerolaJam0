using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedline : MonoBehaviour
{
    public List<Sprite> spriteList;
    public float timeToSwapSprite = 0.2f;
    public SpriteRenderer spriteRenderer;

    int index = 0;
    float currentTime = 0;
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime > timeToSwapSprite)
        {
            index = Random.Range(0, spriteList.Count);
            spriteRenderer.sprite = spriteList[index];
            currentTime = 0;
        }
    }
}
