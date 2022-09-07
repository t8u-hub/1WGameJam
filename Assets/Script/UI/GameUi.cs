using UnityEngine;
using UnityEngine.UI;

public class GameUi : UiBase
{
    [SerializeField]
    private Button _button;

    public override void Initialize()
    {
        _button.onClick.AddListener(OnClickStartButton);
    }

    private void OnClickStartButton()
    {
        var battleManager = BattleManager.Instance;
        if (battleManager == null)
        {
            Debug.LogError("BattleManagerがない");
            return;
        }

        battleManager.GameStart();
        _button.onClick.AddListener(() => { });
        _button.gameObject.SetActive(false);
    }

}
