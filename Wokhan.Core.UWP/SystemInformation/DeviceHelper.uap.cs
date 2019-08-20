#if __UAP__

using Windows.ApplicationModel.Resources.Core;

namespace Wokhan.UWP.SystemInformation
{
    public partial class DeviceHelper
    {
        static DeviceHelper()
        {
            ResourceContext.GetForCurrentView().QualifierValues.TryGetValue("DeviceFamily", out var family);
            IsMobile = (family == "Mobile");
#if !ARM
            IsX86 = true;
#else
            IsX86 = false;
#endif
        }

    }
}

#endif