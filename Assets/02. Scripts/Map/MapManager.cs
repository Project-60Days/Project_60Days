using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : ManagementBase
{
    Camera mapCamera;
    MapController mapGenerator;
    MapUiController mapUiController;
    ResourceManager resourceManager;

    // Player 스크립트로 옮기기
    int currentHealth;
    int maxHealth = 1;

    IEnumerator GetAdditiveSceneObjects()
    {
        yield return new WaitForEndOfFrame();

    }

    public void CreateMapScene()
    {
        StartCoroutine(GetAdditiveSceneObjects());
    }

    public override EManagerType GetManagemetType()
    {
        throw new System.NotImplementedException();
    }
}
