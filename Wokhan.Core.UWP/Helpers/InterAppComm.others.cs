#if !__UAP__
using System;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Wokhan.UWP.Helpers
{
    public class InterAppComm
    {
        public bool IsActive => false;

        public Task SendRequest(ValueSet valueSet)
        {
            throw new NotImplementedException();
        }
    }
}
#endif