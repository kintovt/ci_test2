using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MotionSlots : MonoBehaviour
{
    [SerializeField] private GameObject _motionSlotPrefab;

    public UnityEvent slotsFilled = new();

    private Transform _content;

    private int _totalSlotsAmount = 5;
    private int _filledSlotsAmount = 0;
    private int[] _motionIds;

    private async void OnEnable()
    {
        _motionIds = new int[_totalSlotsAmount];
        _content = GetComponent<ScrollRect>().content;
        foreach (Transform t in _content)
        {
            Destroy(t.gameObject);
        }
        for (var i = 0; i < _totalSlotsAmount; i++)
        {
            var instance = Instantiate(_motionSlotPrefab, _content);
            var motionSlot = instance.GetComponent<MotionSlot>();
            motionSlot.slotNum = i;
            motionSlot.slotFilled.AsObservable().TakeUntilDisable(this).Subscribe(_ => OnSlotFilled(motionSlot.slotNum, motionSlot.GetMotionPreview().ClipId));
            await Task.Delay(100);
        }
    }

    private void OnSlotFilled(int slotId, int clipId)
    {
        _filledSlotsAmount += 1;
        Debug.Log("clip " + clipId + " added to slot " + slotId);
        _motionIds[slotId] = clipId;
        Debug.Log("filled " + _filledSlotsAmount + " slots from " + _totalSlotsAmount);
        if (_filledSlotsAmount == _totalSlotsAmount)
        {
            slotsFilled?.Invoke();
        }
    }

    public int[] GetMotionIds()
    {
        return _motionIds;
    }
}
