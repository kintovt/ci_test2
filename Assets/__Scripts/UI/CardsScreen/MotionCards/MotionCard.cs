using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MotionCard : MonoBehaviour
{

    [SerializeField] private TMP_Text _motionTitleLabel;
    [SerializeField] private TMP_Text _energyLabel;
    [SerializeField] private Transform _headerBlock;
    [SerializeField] private Transform _footerBlock;
    [SerializeField] private Button _addToSlotButton;
    [SerializeField] private RawImage _avatarPreview;
    [SerializeField] private Image _cardBackground;
    [SerializeField] private Image _rarityLabel;
    [SerializeField] private TMP_Text _rarityLabelText;

    [SerializeField] private Sprite[] _cardBackgroundSprites;
    [SerializeField] private Sprite[] _rarityLabelSprites;

    public UnityEvent<MotionCardData> onPreviewDisplayed { get; } = new();
    public UnityEvent<MotionCardData> onMotionSelected { get; } = new();

    private MotionCardData _data;

    private float _screenWidth;
    private float _currentX => transform.position.x;
    private float _lastFrameX;
    private RenderTexture _avatarTexture;
    private Texture2D _previewTexture;

    private bool _isDisplayed;

    public void SetData(MotionCardData data)
    {
        _data = data;
    }

    public MotionCardData GetData()
    {
        return _data;
    }

    private void OnEnable()
    {
        _addToSlotButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OnAddToSlotButton());
    }

    // Start is called before the first frame update
    void Start()
    {
        _screenWidth = Screen.width;
        _avatarTexture = (RenderTexture)_avatarPreview.texture;
        _motionTitleLabel.text = _data.title;
        _energyLabel.text = _data.energy.ToString();
        _previewTexture = TextureUtils.GetTexture2D(_avatarTexture);
        _avatarPreview.texture = _previewTexture;
        //
        _cardBackground.sprite = _cardBackgroundSprites[_data.rarity];
        _rarityLabel.sprite = _rarityLabelSprites[_data.rarity];
        _rarityLabel.SetNativeSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastFrameX != _currentX)
        {
            if (_currentX < 0 || _currentX > _screenWidth)
            {
                transform.localScale = Vector3.one * 0.9f;
            }
            else
            {
                transform.localScale = Vector3.one * (1.1f - (0.1f * Mathf.Abs(_currentX - (_screenWidth / 2f)) / (_screenWidth / 2f)));
            }
            
            var cardInCenter = Mathf.Abs(Mathf.RoundToInt(_currentX) - Mathf.RoundToInt(_screenWidth / 2f)) < 50;
            _headerBlock.GetComponent<CanvasGroup>().DOFade(cardInCenter ? 1f : 0f, .5f);
            _footerBlock.GetComponent<CanvasGroup>().DOFade(cardInCenter ? 1f : 0f, .5f);
            if (cardInCenter)
            {
                if (!_isDisplayed)
                {
                    _isDisplayed = true;
                    onPreviewDisplayed.Invoke(_data);
                    _avatarPreview.texture = _avatarTexture;
                }
            } else
            {
                if (_isDisplayed)
                {
                    _previewTexture = TextureUtils.GetTexture2D(_avatarTexture);
                    _avatarPreview.texture = _previewTexture;
                }
                _isDisplayed = false;
            }
            _lastFrameX = _currentX;
        }
    }

    private void OnAddToSlotButton()
    {
        onMotionSelected.Invoke(_data);
    }

    private void ShowRealtimeTexture()
    {

    }

    private void SnapPreviewTexture()
    {

    }

    private void ShowPreviewTexture()
    {

    }

}
