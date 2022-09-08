using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooImage : UiBase
{
    Animator _animator;

    public override void Initialize()
    {
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void StopWaving()
    {
        _animator.SetTrigger("StopWaving");
    }
}
