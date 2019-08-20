using System.Runtime.CompilerServices;

namespace Wokhan.UWP.Extensions
{
    public static partial class ResourcesExtensions
    {
        public static string TranslateProperty([CallerMemberName] string propName = null) => propName.Translate();
    }
}
