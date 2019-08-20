using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Wokhan.Shared.Store
{
    public partial class LicenceManager //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        public static bool IsValid => _isValid();

        public static bool IsExpired => !_isValid();

        public static bool IsTrial => _isTrial();

        public static int RemainingDays => _remainingDays();

        public static async Task InitInstance() => await _initInstance();
        
        public static async Task GoToPurchase(Action<bool> callback, string id = null, bool isFree = false)
        {
            await _goToPurchase(callback, id, isFree);
        }

        private static async Task GoToAddonPurchase(string addOnId, Action<bool> callback)
        {
            await _goToAddonPurchase(addOnId, callback);
        }
    }
}
