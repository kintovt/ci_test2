using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "DanceConfig", menuName = "Dance", order = 0)]
    public class DanceConfig : ScriptableObject
    {
        [SerializeField] private List<DanceData> dances;

        public IReadOnlyList<DanceData> Dances => dances;
    }

    [Serializable]
    public class DanceData
    {
        [SerializeField] private string location;
        [FoldoutGroup("Audio"), SerializeField] private AudioClip audio;
        [FoldoutGroup("Audio"), SerializeField] private float trackStart = 73f;
        [FoldoutGroup("Audio"), SerializeField] private float danceDuration = 20f;
        [FoldoutGroup("Animation"), SerializeField] private string[] animationTriggers;
        [FoldoutGroup("Animation"), SerializeField] private bool randomDance = true;
        [FoldoutGroup("Animation"), SerializeField] private List<Move> moves;

        public float Duration => danceDuration;
        public float TrackStart => trackStart;
        public IList<Move> Moves => moves;
        //  [SerializeField] private RuntimeAnimatorController animatorController;

        //public RuntimeAnimatorController AnimatorController => animatorController;

        public AudioClip Audio => audio;
        public string[] AnimationTriggers => animationTriggers;
        public bool RandomDanceFromList => randomDance;
        public string Location => location;
    }

    [Serializable]
    public class Move
    {
         public float timeStamp;
         public string element;
    }
}