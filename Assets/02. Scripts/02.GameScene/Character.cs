using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    string name;
    string job;
    string[] equipments;

    bool isDead;
}

/// <summary>
/// Enum들을 갖고있는 구조체
/// </summary>
public struct Enums
{
    EHungerType eHunger;
    EThirstType eThirst;
    EConditionType eCondition;
    EBodyHealthType eBody;
    EInfectionType eInfection;
    EPartsType eParts;
} 
#endregion

public abstract class Character : MonoBehaviour
{
    Data data;
    Enums enums;

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