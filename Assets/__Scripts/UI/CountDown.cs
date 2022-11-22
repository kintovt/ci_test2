using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class CountDown : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;


        [Button]
        public async Task StartCountDown(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                text.text = i.ToString();
                text.gameObject.SetActive(true);
                await Task.Delay(1000);
                text.gameObject.SetActive(false);
            }
        }

    }
}