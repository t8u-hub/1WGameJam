using UnityEngine;

[CreateAssetMenu(fileName = "CharaSpriteSetting", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]

public class PlayerSpriteDataSetting : ScriptableObject
{
    public enum Type
    {
        Normal,
        Damaged,
        Attack,
    }

    [SerializeField]
    private Sprite[] _normalSprite;

    [SerializeField]
    private Sprite[] _damageSprite;

    [SerializeField]
    private Sprite[] _attackSprite;

    public Sprite GetSprite(Type type, int charaPhase = 0)
    {
        switch (type)
        {
            case Type.Damaged:
                return _damageSprite[charaPhase];
            case Type.Attack:
                return _attackSprite[charaPhase];
        }

        return _normalSprite[charaPhase];
    }
}
