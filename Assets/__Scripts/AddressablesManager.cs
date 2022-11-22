

using UnityEngine.AddressableAssets;
using Zenject;

namespace _Scripts
{
    public class AddressablesManager : IInitializable
    {
        public async void Initialize()
        {
            Addressables.InitializeAsync();
        }
    }
}