using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borders : MonoBehaviour
{
    [SerializeField] GameObject[] borders;
    [SerializeField] Material[] materials;

    public GameObject GetNormalBorder()
    {
        borders[0].GetComponent<MeshRenderer>().material = materials[0];
        return borders[0];
    }

    public GameObject GetAvailableBorder()
    {
        borders[0].GetComponent<MeshRenderer>().material = materials[1];
        return borders[0];
    }

    public GameObject GetUnAvailableBorder()
    {
        borders[0].GetComponent<MeshRenderer>().material = materials[2];
        return borders[0];

    }
    
    public GameObject GetDisturbanceBorder()
    {
        return borders[1];
    }

    public void OffAllBorders()
    {
        foreach (var item in borders)
        {
            item.SetActive(false);
        }
    }
}
