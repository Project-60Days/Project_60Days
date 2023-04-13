using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollView : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;

    public void GeneratePrefab()
    {
        GameObject nameCard = Instantiate(prefab, parent);
    }
}
