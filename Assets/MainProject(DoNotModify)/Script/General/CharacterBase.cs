using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase<T> : MonoBehaviour
{
    public T Fsm; // 캐릭터의 상태머신을 참조하는 변수
    public Rigidbody _rb;
    //public Animator _animator;
    
    protected virtual void Awake()
    {
        Fsm = GetComponent<T>();
        _rb = GetComponent<Rigidbody>();
    }
}

public interface IMonsterType
{
    MonsterType MonsterType { get; }
}

public enum MonsterType
{
    Normal,
    Elite
}

public interface IDamageable
{
    void GetDamaged(float damage);
    void GetSlowed(float time);
}