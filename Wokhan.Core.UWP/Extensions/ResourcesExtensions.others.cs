#if !__UAP__
using System;
using Android.Content;
using Android.Content.Res;
using Java.Util;
using Wokhan.Core.Extensions;

namespace Wokhan.UWP.Extensions
{
    public static partial class ResourcesExtensions
    {
        private static string _packageName;
        private static Resources _resources;
        private static Resources _defResources;

        public static string GetStringDef(this Resources src, int id)
        {
            try
            {
                return src.GetString(id);
            }
            catch
            {
                return null;
            }
        }

        public static string Translate(this string src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var id = _resources.GetIdentifier(src, "string", _packageName);
            return _resources.GetStringDef(id)
                ?? _defResources.GetStringDef(id).ToPseudo()
                ?? $"!!{src}!!";
        }

        public static void Init(Context context)
        {
            _resources = context.Resources;
            _packageName = context.PackageName;

            var conf = new Configuration(context.Resources.Configuration);
            conf.SetLocale(Locale.English);
            Context localizedContext = context.CreateConfigurationContext(conf);
            _defResources = localizedContext.Resources;
        }
    }
}
#endif