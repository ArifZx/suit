using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsUI : MonoBehaviour
{
    [SerializeField]
    GameObject[] stars;

    public void SetStar(int num)
    {
        for(var i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < num);
        }
    }
}
