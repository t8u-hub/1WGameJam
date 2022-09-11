using UnityEngine;
using UnityEngine.UI;

public class ResultUi : UiBase
{
    [SerializeField]
    Animation _photoAnimation;

    [SerializeField]
    Animation _resultInfoAnimation;

    [SerializeField]
    Text _scoreText;

    [SerializeField]
    Text _specialNumCountText;

    [SerializeField]
    Image _iconImage;

    [SerializeField]
    float _resultAnimeTime;
    [SerializeField]
    float _towAnimeTime;
    [SerializeField]
    float _threeAnimeTime;
    [SerializeField]
    float _allAnimeTime;

    [SerializeField]
    Transform _spaceButtonIcon;

    private float _photoTime = 0f;
    private float _resultTime = 0f;


    public override void Initialize()
    {
        var resultParam = ResultTempData.Instance.GetData();
        _scoreText.text = resultParam.Score.ToString();
        _specialNumCountText.text = $"{resultParam.SpecialAttackNum} 回";
        _spaceButtonIcon.gameObject.SetActive(false);

        foreach (var item in resultParam.ItemIdList)
        {
            var iconImage = GameObject.Instantiate<Image>(_iconImage, _iconImage.transform.parent);
            iconImage.sprite = Resources.Load<Sprite>($"UiResources/UI/02_game/item/item_{item}");
            iconImage.gameObject.SetActive(true);
        }

        if (resultParam.FinalCharaLevel == 1)
        {
            _resultTime = _resultAnimeTime;
            _photoAnimation.gameObject.SetActive(false);
            _resultInfoAnimation.Play();
        }
        else if(resultParam.FinalCharaLevel == 2)
        {
            _photoAnimation.Play();
            _photoTime = _towAnimeTime;
        }
        else if (!resultParam.IsClear)
        {
            _photoAnimation.Play();
            _photoTime = _threeAnimeTime;
        }
        else
        {
            _photoAnimation.Play();
            _photoTime = _allAnimeTime;
        }
    }

    private void Update()
    {
        if (_photoTime > 0f)
        {
            _photoTime -= Time.deltaTime;
            if (_photoTime < 0f)
            {
                _photoAnimation.Stop();
                _resultInfoAnimation.Play();
                _resultTime = _resultAnimeTime;
            }
        }
        else if(_resultTime > 0f)
        {
            _resultTime  -= Time.deltaTime;
        }
        else
        {
            if (!_spaceButtonIcon.gameObject.activeSelf)
                _spaceButtonIcon.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UIManager.Instance.SetFadeColor(new Color32(255, 255, 255, 255));
                SceneManager.Instance.ChangeScene(SceneDefine.Scene.Tutorial);
            }
        }
    }
}

