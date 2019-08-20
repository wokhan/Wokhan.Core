#if __UAP__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Wokhan.Core.Extensions;

namespace Wokhan.UWP.Extensions
{
    public static partial class ResourcesExtensions
    {
        private static ResourceContext _resourceContext;
        private static ResourceContext ResourceContext => _resourceContext = (_resourceContext ?? ResourceContext.GetForViewIndependentUse());

        private static ResourceMap _resourceMap;
        private static ResourceMap ResourceMap => _resourceMap = (_resourceMap ?? ResourceManager.Current.MainResourceMap.GetSubtree("Resources"));
            
        public static string Translate(this string src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            ResourceCandidate res = ResourceMap.GetValue(src, ResourceContext);
            if (res == null)
            {
                return $"!!{src}!!";
            }

            return res.IsMatchAsDefault ? res.ValueAsString.ToPseudo() : res.ValueAsString;
        }

    }
}
#endif