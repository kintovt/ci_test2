using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts
{
    [Serializable]
    public class UserProgress
    {
        private List<string> avatarUrLs;
        private int stamina;
        private DateTime lastStaminaRestore;
        private int staminaRestoreRate = 4; // units per minute;

        public IReadOnlyList<string> AvatarURLs => avatarUrLs;
        public string LastSelected { get; set; }

        public int StaminaRestoreRate => staminaRestoreRate;
        public int Stamina
        {
            get
            {
                //stamina = 
                return stamina;
            }
            set
            {
                var clamped = Mathf.Clamp(value,0, 100);
                if(stamina == clamped)
                    return;
                stamina = clamped;
                saveRequest?.Invoke();
                staminaLevelChanged?.Invoke(Stamina);
            }
        }


        public UnityEvent<int> staminaLevelChanged { get; } = new();
        public UnityEvent saveRequest { get; } = new();
        
        public UserProgress(List<string> avatarUrLs, int stamina, string lastSelected)
        {
            this.avatarUrLs = avatarUrLs;
            if (stamina < 20)
                stamina = 100;
            Stamina = stamina;
            LastSelected = lastSelected;
        }

        public void AddAvatar(string url)
        {
            LastSelected = url;
            if (avatarUrLs.Contains(url))
            {
                Debug.LogWarning($"Avatar url:{url} have been already added");
                saveRequest?.Invoke();
                return;
            }
            avatarUrLs.Add(url);
            saveRequest?.Invoke();
        }

        public void ConsumeStamina(int amount)
        {
            Stamina -= amount;
        }
    }

    public static class UserDefaults
    {
        public static string avatarURL1 = "https://d1a370nemizbjq.cloudfront.net/9bcc6840-8b8b-420d-a9d8-bc9c275fce8f.glb";
        public static string avatarURL = "https://d1a370nemizbjq.cloudfront.net/6924757f-9f0f-4f26-a659-0ff7c78bd3f9.glb";
    }
}