using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class MotionPreview : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int _clipId;
    [SerializeField] private RawImage _image;

    public int ClipId { get => _clipId; set => _clipId = value; }

    // Start is called before the first frame update
    private void Start()
    {
        //_button = GetComponent<Button>();
        //_button.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => SwitchDance(ClipId));
    }

    public void SetTexture(Texture2D texture2D)
    {
        _image.texture = texture2D;
    }

    public Texture2D GetTexture()
    {
        return (Texture2D)_image.texture;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
