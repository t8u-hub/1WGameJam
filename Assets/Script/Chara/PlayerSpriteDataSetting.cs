using UnityEngine;

[CreateAssetMenu(fileName = "CharaSpriteSetting", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]

public class PlayerSpriteDataSetting : ScriptableObject
{
    public enum Type
    {
        Normal,
        Run,
        Damaged,
        Jump,
        Attack,
    }

    [SerializeField]
    private Sprite[] _normalSprite;

    [SerializeField]
    private Sprite[] _runSprite;

    [SerializeField]
    private Sprite[] _jumpSprite;

    [SerializeField]
    private Sprite[] _damageSprite;

    [SerializeField]
    private Sprite[] _attackSprite;

    public Sprite GetSprite(Type type, int charaPhase, int attackType = 1)
    {
        switch (type)
        {
            case Type.Damaged:
                return _damageSprite[charaPhase - 1];
            case Type.Normal:
                return _normalSprite[charaPhase - 1];
            case Type.Run:
                return _runSprite[charaPhase - 1];
            case Type.Jump:
                return _jumpSprite[charaPhase - 1];
        }

        return _attackSprite[5 * (charaPhase - 1) + (attackType - 1)];
    }
}
