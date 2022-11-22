using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class BattleCard : MonoBehaviour
    {

        public TMP_Text motionTitleLabel;
        public TMP_Text energyLabel;
        public TMP_Text rarityText;
        public RawImage avatarPreview;
        public RawImage background;
        public RawImage label;
        public Slider progress;
        public Color redColor;
        public RawImage topImage;
        public MotionCardData data;
        public Texture2D[] rarityBackgrounds;
        public Texture2D[] rarityLabels;
        public bool isRed => topImage.color == redColor;


        public void FillCard(MotionCardData data, Texture previewTexture)
        {
            if (data != null)
            {
                background.texture = rarityBackgrounds[data.rarity];
                label.texture = rarityLabels[data.rarity];
                label.SetNativeSize();
                this.data = data;
                avatarPreview.texture = previewTexture;
                energyLabel.text = data.energy.ToString();
                motionTitleLabel.text = data.title;
                progress.value = 0;
                SetBlack();
            }
        }

        public void UpdateSliderProgress(float progress)
        {
            this.progress.value = progress;
        }

        public void SetRed()
        {
            topImage.color = redColor;
        }

        public void SetBlack()
        {
            topImage.color = Color.black;
        }
    }
}
