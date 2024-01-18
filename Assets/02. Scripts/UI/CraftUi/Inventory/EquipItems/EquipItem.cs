using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipItem : ItemBase
{
    public abstract void Equip();

    public abstract void UnEquip();
}
