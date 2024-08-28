using UnityEngine;

[CreateAssetMenu]
public class ItemBase : ScriptableObject
{
    public string Code;

    public ItemData Data { get; private set; }
    public Sprite IconSprite;
    public Sprite IllustSprite;

    public string sfxName;

    public void SetData(ItemData _data)
    {
        Data = _data;

        IllustSprite = Resources.Load<Sprite>("Item/Illust/" + Code);
        IconSprite = Resources.Load<Sprite>("Item/Icon/" + Code);
    }

    public virtual void Equip() { }

    public virtual void UnEquip() { }

    public virtual bool CheckMeetCondition() => false;

    public virtual void DayEvent() { }
}