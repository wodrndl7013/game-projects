using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum GachaState { Idle, Rolling, Reveal, Closing }

public class GachaStateMachine : MonoBehaviour
{
    public GachaState Current { get; private set; } = GachaState.Idle;

    public event Action<GachaState> OnStateChanged;

    public void Set(GachaState next)
    {
        if (Current == next) return;
        Current = next;
        OnStateChanged?.Invoke(Current);
    }

    public bool IsIdle => Current == GachaState.Idle;
    public bool IsBusy => Current == GachaState.Rolling || Current == GachaState.Reveal || Current == GachaState.Closing;
}

