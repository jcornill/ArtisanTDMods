using UnityEngine;

namespace Kirthos.ArtisanTDModLoader
{
    public abstract class ModBase : MonoBehaviour
    {
        public abstract string DisplayName { get; }
        public abstract string Version { get; }
        public abstract string Creator { get; }
    }
}