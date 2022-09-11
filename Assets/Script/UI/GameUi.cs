using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameUi : UiBase
{ 

    [SerializeField]
    private Button _button;

    [SerializeField]
    private WeaponIconUi[] _weaponIconDict;

    [SerializeField]
    private CutinPlayer _cutinPlayer;

    [SerializeField]
    private DamageText _scoreText;

    [SerializeField]
    ObjectAnimator _tipsAnimator;

    [SerializeField]
    Animation _gaugeAnimation;

    /// <summary>
    /// 必殺技ゲージ
    /// </summary>
    [SerializeField]
    private Slider _attackGauge;

    private class Weapon
    {
        public int Id { get; }
        public CsvDefine.ActionData.AttackType Type { get; }
        public int Level { get; }

        public Weapon(int id, int type, int level)
        {
            Id = id;
            Type = (CsvDefine.ActionData.AttackType)type;
            Level = level;
        }
    }

    private List<Weapon> _weaponList;
    private List<Weapon> _earnedWeapon = new List<Weapon>();

    public override void Initialize()
    {
        _weaponList = new CsvReader().Create(CsvDefine.WeaponData.PATH).CsvData
            .Select(csvData =>
                new Weapon(csvData[CsvDefine.WeaponData.WEAPON_ID],
                csvData[CsvDefine.WeaponData.ATTACK_TYPE],
                csvData[CsvDefine.WeaponData.WEAPON_LEVEL]))
            .ToList();

        _earnedWeapon.Add(_weaponList.Find(weapon => weapon.Id == 101));

        _attackGauge.minValue = 0;
        _attackGauge.maxValue = BattleManager.MAX_GAUGE_VALUE;

        if (ResultTempData.Instance.GetData() == null)
        {
            _tipsAnimator.gameObject.SetActive(true);
        }
        else
        {
            Destroy(_tipsAnimator.gameObject);
            OnClickStartButton();
        }
    }

    private void OnClickStartButton()
    {
        var battleManager = BattleManager.Instance;
        if (battleManager == null)
        {
            Debug.LogError("BattleManagerがない");
            return;
        }

        battleManager.GameStart(this);
        _button.onClick.AddListener(() => { });
        _button.gameObject.SetActive(false);
    }

    public void UpdateItemUiIfNeed(int itemId)
    {
        if (_earnedWeapon.Any(weapon => weapon.Id == itemId))
        {
            // 獲得済なので何もしない
            return;
        }

        var data = _weaponList.Find(weapon => weapon.Id == itemId);
        var isHighest = _earnedWeapon.Where(weapon => weapon.Type == data.Type)
            .All(weapon => weapon.Level < data.Level);

        _earnedWeapon.Add(_weaponList.Find(weapon => weapon.Id == itemId));
        if (isHighest)
        {
            _weaponIconDict[(int)data.Type - 1].SetNewWeapon(data.Level);
        }
    }

    public void PlayGaugeAnim()
    {
        _gaugeAnimation.Play();
    }

    public void Update()
    {
        if (_tipsAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _tipsAnimator.PlayFade(1, 0, () =>
                {
                    OnClickStartButton();
                    Destroy(_tipsAnimator.gameObject);
                });
            }
            return;
        }

        if (BattleManager.Instance == null || BattleManager.Instance.StopUpdate)
        {
            return;
        }

        _scoreText.SetValue(BattleManager.Instance.CurrentScore);
        _attackGauge.value = BattleManager.Instance.CurrentGauge;
    }

    public void PlayCutin(int charaLevel)
    {
        var cutin = Instantiate<CutinPlayer>(_cutinPlayer, transform);
        cutin.PlayCutin(charaLevel);
    }
}
