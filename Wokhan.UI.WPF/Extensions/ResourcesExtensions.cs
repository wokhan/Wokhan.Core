using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using Wokhan.Core.Extensions;

namespace Wokhan.UI.WPF.Extensions
{
    public static class ResourcesExtensions
    {
        public static string TranslateProperty([CallerMemberName] string propName = null) => propName.Translate();

        private static ResourceManager resourceManager = new ResourceManager("Resources", Assembly.GetCallingAssembly());

        public static string Translate(this string src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var res = resourceManager.GetString(src);
            if (res == null)
            {
                res = resourceManager.GetString(src, CultureInfo.InvariantCulture)?.ToPseudo();
            }

            return res ?? $"!!{src}!!";
        }
    }
}
