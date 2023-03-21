using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
enum Food
{
    Normal,
    Hunger,
    Starving
}

enum Water
{
    Normal,
    Thirst,
    Dehydration
}

enum Condition
{
    Normal,
    Anx,
    Crazy,
    Machine
}

enum BodyHealth
{
    Normal,
    Hurt,
    Injury,
    Disease
}

enum Infection
{
    Normal,
    Bite,
    Zombie
}

enum Parts
{
    None,
    Eyeball,
    LeftArm,
    RightArm,
    Body,
    Legs
}
#endregion

#region 구조체
/// <summary>
/// 변수들을 갖고있는 구조체
/// 공격, 방어, 체력, 배고픔 정도, 목마름 정도, 직업, 장비, 생존 여부
/// </summary>
public struct Data
{
    float attack;
    float defense;
    float health;
    float hungerFigure;
    float thirstFigure;
    float reliability;
    float battery;

    string job;
    string[] equipments;

    bool isDead;
}

/// <summary>
/// 필요한 모든 Enum들을 갖고있는 구조체
/// </summary>
public struct Enums
{
    Food food;
    Water water;
    BodyHealth bodyhealth;
    Infection infection;
    Condition condition;
} 
#endregion

public abstract class Character : MonoBehaviour
{
    public Data data;
    public Enums enums;
    
    /// <summary>
    /// 초기화 함수. Start에서 호출
    /// </summary>
    public abstract void InitSetting();

    /// <summary>
    /// 캐릭터의 상태변화를 업데이트 시켜주는 함수. Update에서 호출
    /// </summary>
    public abstract void StateUpdate();

    public virtual void Start()
    {
        InitSetting();
    }

    public virtual void Update()
    {
        StateUpdate();
    }
}