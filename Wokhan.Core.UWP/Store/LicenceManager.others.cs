#if !__UAP__

using System;
using System.Threading.Tasks;

namespace Wokhan.Shared.Store
{
    public partial class LicenceManager
    {
        private static async Task _initInstance()
        {
        }

        private static bool _isValid() => true;


        private static bool _isTrial() => true;

        private static int _remainingDays() => 7;

        private static async Task _goToPurchase(Action<bool> callback, string id = null, bool isFree = false)
        {
        }

        private static async Task _goToAddonPurchase(string addOnId, Action<bool> callback)
        {
        }
    }
}

#endif