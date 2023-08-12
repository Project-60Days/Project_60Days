using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NameCard : MonoBehaviour
{
    [SerializeField] Transform resources;

    Image[] food = new Image[5]; 
    Image[] water = new Image[5]; 
    Image[] battery = new Image[5];

    Color32 grey = new Color32(204, 204, 204, 255);
    Color32 yellow = new Color32(228, 255, 0, 255);

    void Start()
    {
        InitInfo();
    }

    public void InitInfo()
    {
        food = resources.GetChild(0).GetComponentsInChildren<Image>();
        water = resources.GetChild(1).GetComponentsInChildren<Image>();
        battery = resources.GetChild(2).GetComponentsInChildren<Image>();
    }

    public void ClickedFood(int number)
    {
        TurnOff(food);

        for (int i = 1; i <= number; i++)
        {
            food[i].color = yellow;
        }
    }

    public void ClickedWater(int number)
    {
        TurnOff(water);

        for (int i = 1; i <= number; i++)
        {
            water[i].color = yellow;
        }
    }

    public void ClickedBattery(int number)
    {
        TurnOff(battery);

        for (int i = 1; i <= number; i++)
        {
            battery[i].color = yellow;
        }
    }

    public void TurnOff(Image[] resource)
    {
        foreach (var item in resource.Select((value, index) => (value, index)))
        {
            if (item.index != 0)
                item.value.GetComponent<Image>().color = grey;
        }
    }

    public void Reset()
    {
        TurnOff(food);
        TurnOff(water);
        TurnOff(battery);
    }

    public void TurnOffButton(int number)
    {

        switch (number)
        {
            case 1:
                TurnOff(food);
                break;
            case 2:
                TurnOff(water);
                break;
            case 3:
                TurnOff(battery);
                break;
            default:
                break;
        }
    }
}
