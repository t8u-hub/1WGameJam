
using UnityEngine;

public class SeAudioManager : MonoSingleton<SeAudioManager>
{
    public enum SeType
    {
        EnemyDamage,
        GaugeMax, //
        SpecialAttack, //
        Damage, //
        Horizontal,//
        VerticalEnd,//
        VerticalStart,//
        Jump, // 
        Middle,//
        Attack,//
        Long,//
        Result,
        GameStart, //
    }

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip[] _audioClipArray;

    public void Play(SeType seType)
    {
        _audioSource.PlayOneShot(_audioClipArray[(int)seType]);
    }
}
