using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MotionCardData
{
    public AnimationClip clip;
    public string title;
    public int energy;
    public int rarity;
    public bool selected;

    public MotionCardData(MotionCardTextData data)
    {
        clip = Resources.Load<AnimationClip>("Motions/" + data.clip);
        title = data.title;
        energy = data.energy;
        rarity = data.rarity;
        selected = data.selected;
    }
}

[System.Serializable]
public class MotionCardTextData
{
    public string clip;
    public string title;
    public int energy;
    public int rarity;
    public bool selected;
}

[System.Serializable]
public class MotionCardsDataArray
{
    public MotionCardTextData[] data;
}

