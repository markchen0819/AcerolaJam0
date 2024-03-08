using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] targets;

    [SerializeField]
    private int positionIndex = 1;
    private float zOffset = -0.95f;
    [SerializeField]
    int hitCount = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (positionIndex < 2)
            {
                ++positionIndex;
                Vector3 pos = targets[positionIndex].transform.localPosition;
                pos.z = zOffset;
                transform.localPosition = pos;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (positionIndex > 0)
            {
                --positionIndex;
                Vector3 pos = targets[positionIndex].transform.localPosition;
                pos.z = zOffset;
                transform.localPosition = pos;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collide with " + collider.gameObject.name);
        NoteBase n = collider.gameObject.GetComponent<NoteBase>();
        if(!n.HasCollided())
        {
            n.SetCollided();
            ++hitCount;
        }
    }
}
