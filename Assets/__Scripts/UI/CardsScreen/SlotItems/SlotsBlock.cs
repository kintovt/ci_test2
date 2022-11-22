using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotsBlock : MonoBehaviour
{
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private GameObject _slotItemPrefab;
    [SerializeField] private int _totalSlotsCount;

    public UnityEvent<MotionCardData> onSlotEmptied { get; } = new();

    public SlotItem[] _slots;
    private int _filledSlotsCount;

    private void OnEnable()
    {
        _filledSlotsCount = 0;
        CleanSlotsContainer();
        PopulateSlotsContainer();
    }

    private void CleanSlotsContainer()
    {
        foreach (Transform t in _slotsContainer)
        {
            Destroy(t.gameObject);
        }
    }

    private async void PopulateSlotsContainer()
    {
        _slots = new SlotItem[_totalSlotsCount];
        for (var i = 0; i < _totalSlotsCount; i++)
        {
            var instance = Instantiate(_slotItemPrefab, _slotsContainer);
            var slotItem = instance.GetComponent<SlotItem>();
            slotItem.onSlotEmptied.AsObservable().TakeUntilDisable(this).Subscribe(data => ClearSlotItem(data));
            if (slotItem != null)
            {
                _slots[i] = slotItem;
            }
            await Task.Delay(100);
        }
    }

    public void FillNextSlot(MotionCardData data, bool red = false)
    {
        for (var i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsEmpty())
            {
                _slots[i].FillSlot(data, red);
                _filledSlotsCount++;
                break;
            }
        }
    }

    public int GetTotalSlotsCount()
    {
        return _totalSlotsCount;
    }

    public int GetFilledSlotsCount()
    {
        return _filledSlotsCount;
    }

    public void ClearAllSlots()
    {
        _filledSlotsCount = 0;
    }

    private void ClearSlotItem(MotionCardData data)
    {
        _filledSlotsCount--;
        onSlotEmptied.Invoke(data);
    }

    public void SetAllYellow()
    {
        for (var i = 0; i < _slots.Length; i++)
        {
            _slots[i].SetErrorColor(false);
        }
    }
}
