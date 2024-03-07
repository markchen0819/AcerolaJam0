using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackgroundEvent : MonoBehaviour
{
    public List<GameObject> bgs;

    public void Execute(int index)
    {
        for(int i=0; i < bgs.Count; ++i)
        {
            bgs[i].SetActive(false);
        }
        bgs[index].SetActive(true);
    }
}
