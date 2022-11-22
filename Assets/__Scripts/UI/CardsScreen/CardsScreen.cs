using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using _Scripts.UI;


public class CardsScreen : MonoBehaviour
{
    [SerializeField] private TextAsset motionCardsArrayJson;
    [SerializeField] private MotionCardData[] _motionCardsData;
    [SerializeField] private Transform _motionCardsContainer;
    [SerializeField] private GameObject _motionCardPrefab;
    [SerializeField] private RenderTexture _avatarRenderTexture;
    [SerializeField] private TMP_Text _energyCounter;
    [SerializeField] private TMP_Text _cardQuantityText;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _closeButton;
    //
    [SerializeField] private int _totalEnergyAmount;
    [SerializeField] private int _maxEnergyAmount;

    [SerializeField] public SlotsBlock _slotsBlock;

    public UnityEvent<MotionCardData> onPreviewDisplayed { get; } = new();
    public UnityEvent onCardsSelected { get; } = new();

    private MotionCardData[] _selectedCards;



    public MotionCardData[] GetSelectedCards()
    {
        return _selectedCards;
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        DeselectAllCards();
        CleanCardsContainer();
        PopulateCardsContainer();
        _slotsBlock.onSlotEmptied.AsObservable().TakeUntilDisable(this).Subscribe(data => OnMotionRemoved(data));
        _readyButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => FinishCardSelection());
        _closeButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => ExitCardSelection());
        UpdateEnergyAmount();
        SetEnergyCounter();
        SetReadyButton();
        _cardQuantityText.text = _motionCardsData.Length.ToString();
    }

    public void InitializeMotionCardsData()
    {
        MotionCardTextData[] MotionCardTextData = JsonUtility.FromJson<MotionCardsDataArray>(motionCardsArrayJson.text).data;
        _motionCardsData = new MotionCardData[MotionCardTextData.Length];
        for(int i=0; i< MotionCardTextData.Length; i++)
        {
            _motionCardsData[i] = new MotionCardData(MotionCardTextData[i]);
        }
    }

    private void CleanCardsContainer()
    {
        _selectedCards = new MotionCardData[4] { null, null, null, null };
        foreach (Transform t in _motionCardsContainer)
        {
            Destroy(t.gameObject);
        }
    }
    public void Randomize<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    private async void PopulateCardsContainer()
    {
        Randomize<MotionCardData>(_motionCardsData);
        for (var i = 0; i < _motionCardsData.Length; i++)
        {
            var instance = Instantiate(_motionCardPrefab, _motionCardsContainer);
            var motionCard = instance.GetComponent<MotionCard>();
            if (motionCard != null)
            {
                motionCard.SetData(_motionCardsData[i]);
                motionCard.onPreviewDisplayed.AsObservable().TakeUntilDisable(this).Subscribe(data => OnPreviewDisplayed(data));
                motionCard.onMotionSelected.AsObservable().TakeUntilDisable(this).Subscribe(data => OnMotionSelected(data));
            }
            await Task.Delay(100);
        }
    }

    private void UpdateCardsContainer()
    {
        foreach (Transform t in _motionCardsContainer)
        {
            var motionCard = t.GetComponent<MotionCard>();
            motionCard.gameObject.SetActive(!motionCard.GetData().selected);
        }
        //_motionCardsContainer.GetComponent<SwipableObject>().UpdatePosition();
    }

    private void OnPreviewDisplayed(MotionCardData data)
    {
        Debug.Log(data.clip + " motion displaying");
        onPreviewDisplayed.Invoke(data);
    }

    private void OnMotionSelected(MotionCardData data)
    {

        if (SelectedCardsCount() < _selectedCards.Length) {
            data.selected = true;
            for (var i = 0; i < _selectedCards.Length; i++) {
                if (_selectedCards[i] == null || _selectedCards[i].clip == null)
                {
                    _selectedCards[i] = data;
                    Debug.Log(data.clip + " motion added to slot " + i);
                    break;
                }
            }
            UpdateEnergyAmount();
            _slotsBlock.FillNextSlot(data, _totalEnergyAmount > _maxEnergyAmount);
            SetEnergyCounter();
            SetReadyButton();
            UpdateCardsContainer();
        }
    }

    private void UpdateEnergyAmount()
    {
        Debug.Log($"CardScreen.UpdateEnergyAmount()");
        _totalEnergyAmount = 0;
        if (_selectedCards == null) return;
        for (var i = 0; i < _selectedCards.Length; i++)
        {
            if (_selectedCards[i] != null && _selectedCards[i].clip != null)
            {
                _totalEnergyAmount += _selectedCards[i].energy;
            }
        }
    }

    private void SetReadyButton()
    {
        var error = _totalEnergyAmount > _maxEnergyAmount;
        var buttonSprites = _readyButton.GetComponent<ButtonSpriteSet>();
        var spriteState = new SpriteState();
        spriteState.disabledSprite = error ? buttonSprites.error : buttonSprites.disabled;
        _readyButton.spriteState = spriteState;

        _readyButton.GetComponent<ButtonErrorState>().Error = error;
        _readyButton.interactable = !error && (_slotsBlock.GetFilledSlotsCount() == _slotsBlock.GetTotalSlotsCount());
    }

    private void SetEnergyCounter()
    {
        Debug.Log($"CardScreen.SetEnergyCounter()");
        _energyCounter.text = $"Energy {_totalEnergyAmount}/{_maxEnergyAmount}";
        Color colorTrue, colorFalse;
        ColorUtility.TryParseHtmlString("#FAFF07", out colorTrue);
        ColorUtility.TryParseHtmlString("#FF3232", out colorFalse);
        _energyCounter.color = _totalEnergyAmount <= _maxEnergyAmount ? colorTrue : colorFalse;
    }

    private void OnMotionRemoved(MotionCardData data)
    {
        Debug.Log(data.clip + " motion removed from slot");
        data.selected = false;
        for (var i = 0; i < _selectedCards.Length; i++)
        {
            if (_selectedCards[i]?.clip == data.clip)
            {
                _selectedCards[i] = null;
                break;
            }
        }
        UpdateEnergyAmount();
        SetEnergyCounter();
        SetReadyButton();
        UpdateCardsContainer();
        if(_totalEnergyAmount <= _maxEnergyAmount) _slotsBlock.SetAllYellow();
    }

    private void FinishCardSelection()
    {
        Debug.Log($"CardScreen.FinishCardSelection()");
        Debug.Log("card selection finished, " + _selectedCards + " selected");
        onCardsSelected.Invoke();
    }

    private void ExitCardSelection()
    {
        Debug.Log($"CardScreen.ExitCardSelection()");
        DeselectAllCards();
        _selectedCards = null;
        _slotsBlock.ClearAllSlots();
        UpdateEnergyAmount();
        SetReadyButton();
        onCardsSelected.Invoke();
    }

    private void DeselectAllCards()
    {
        Debug.Log($"CardScreen.DeselectAllCards()");
        foreach (var data in _motionCardsData)
        {
            data.selected = false;
        }
    }

    private int SelectedCardsCount()
    {
        var count = 0;
        for (var i = 0; i < _selectedCards.Length; i++)
        {
            if (_selectedCards[i] != null && _selectedCards[i].clip != null)
            {
                count++;
            }
        }
        return count;
    }
}
