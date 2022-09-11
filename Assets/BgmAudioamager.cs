using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BgmAudioamager : MonoSingleton<BgmAudioamager>
{
    public enum BgmType
    {
        Title,
        Game,
        Result,

        None,
    }

    [Header("BGM")]

    [SerializeField]
    private AudioMixer _audioMixer;

    [SerializeField]
    private AudioClip[] _bgmAudioClip;

    [SerializeField]
    private AudioSource[] _bgmAudioSource;

    [SerializeField]
    private AudioMixerSnapshot[] _snapShot;

    private BgmType _currentBgmType = BgmType.None;
    private int _activeIndex = 1;

    public void PlayBgm(BgmType type)
    {
        if (_currentBgmType == type)
        {
            return;
        }

        var prevIndex = _activeIndex;
        _activeIndex = (_activeIndex == 0) ? 1 : 0;
        _bgmAudioSource[_activeIndex].clip = _bgmAudioClip[(int)type];
        _bgmAudioSource[_activeIndex].Play();

        switch (type)
        {
            case BgmType.Title:
                _bgmAudioSource[_activeIndex].volume = .8f;
                break;
            case BgmType.Game:
                _bgmAudioSource[_activeIndex].volume = 1f;
                break;
            case BgmType.Result:
                _bgmAudioSource[_activeIndex].volume = .5f;
                break;
        }

        if (_activeIndex == 0)
        {
            _audioMixer.TransitionToSnapshots(_snapShot, new float[] { 1, 0 }, .3f);
        }
        else
        {
            _audioMixer.TransitionToSnapshots(_snapShot, new float[] { 0, 1 }, .3f);
        }
    }
}
