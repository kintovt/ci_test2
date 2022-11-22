using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    [SerializeField] private RawImage _background;
    [SerializeField] public RawImage _preview;
    [SerializeField] private EnergyLabel _energyLabel;
    [SerializeField] private RawImage _stroke;

    [SerializeField] private Texture2D _emptyBackground;
    [SerializeField] private Texture2D[] _filledBackground;
    [SerializeField] private Texture2D[] _strokes;
    [SerializeField] private Texture2D[] _energyLabelImages;

    public UnityEvent<MotionCardData> onSlotEmptied { get; } = new();

    private Button _button;

    private MotionCardData _data;

    public MotionCardData data { get => _data; }
    private RenderTexture _renderTexture;

    // Start is called before the first frame update
    private void Start()
    {
        _renderTexture = (RenderTexture)_preview.texture;
        _button = GetComponent<Button>();
        _button.enabled = false;
        _preview.enabled = false;
        _background.texture = _emptyBackground;
        _energyLabel.gameObject.SetActive(false);
        _stroke.gameObject.SetActive(false);
    }

    public void FillSlot(MotionCardData data, bool red = false)
    {
        Debug.Log("FillSlot");
        _data = data;
        _preview.texture = TextureUtils.GetTexture2D((RenderTexture)_preview.texture);
        _preview.enabled = true;
        _button.enabled = true;
        _background.texture = _filledBackground[data.rarity];
        _energyLabel.gameObject.SetActive(true);
        _stroke.gameObject.SetActive(true);
        SetErrorColor(red);
        _energyLabel.SetEnergyValue(data.energy);
    }

    public void ClearSlot()
    {
       
        _preview.texture = _renderTexture;
        _button.enabled = false;
        _preview.enabled = false;
        _background.texture = _emptyBackground;
        _energyLabel.gameObject.SetActive(false);
        _stroke.gameObject.SetActive(false);
        onSlotEmptied.Invoke(_data);
        _data = null;
    }

    public bool IsEmpty()
    {
        return _data == null;
    }

    public void SetErrorColor(bool red)
    {
        if (red)
        {
            _stroke.texture = _strokes[1];
            _energyLabel.label.texture = _energyLabelImages[1];
        }
        else
        {
            _stroke.texture = _strokes[0];
            _energyLabel.label.texture = _energyLabelImages[0];
        }
    }
}
