using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class MotionsList : MonoBehaviour
{
    [Inject] private AvatarSpawner avatarSpawner;

    [SerializeField] private AnimationClip[] _animationClips;
    [SerializeField] private GameObject _motionPreviewPrefab;
    [SerializeField] private RenderTexture _renderTexture;

    public UnityEvent motionSelected = new();

    private Transform _content;
    private int _clipId;

    // Start is called before the first frame update
    private async void OnEnable()
    {
        _content = GetComponent<ScrollRect>().content;
        foreach (Transform t in _content)
        {
            Destroy(t.gameObject);
        }
        for (var i = 0; i < _animationClips.Length; i++)
        {
            avatarSpawner.Animator.SetInteger("clipId", i + 1);
            avatarSpawner.Animator.Play(_animationClips[i].name, 0, .5f);
            var texture = GetTexture2D(_renderTexture);
            var instance = Instantiate(_motionPreviewPrefab, _content);
            var motionPreview = instance.GetComponent<MotionPreview>();
            motionPreview.ClipId = i + 1;
            motionPreview.SetTexture(texture);
            var button = instance.GetComponent<Button>();
            button.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => SwitchDance(motionPreview.ClipId));
            instance.transform.SetSiblingIndex(i);
            await Task.Delay(100);
        }
        avatarSpawner.Animator.SetInteger("clipId", 0);
    }

    private Texture2D GetTexture2D(RenderTexture renderTexture)
    {
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA64, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private void SwitchDance(int clipId)
    {
        if (avatarSpawner.Animator.isActiveAndEnabled)
        {
            avatarSpawner.Animator.SetInteger("clipId", clipId);
            _clipId = clipId;
            motionSelected?.Invoke();
        }
    }

    public AnimationClip GetCurrentClip()
    {
        return _animationClips[_clipId - 1];
    }

    public AnimationClip GetAnimationClip(int i)
    {
        return _animationClips[i];
    }

    private void ChangeAnimation()
    {

    }
}
