using Xunit;
using Wokhan.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wokhan.Collections.Generic.Extensions;

namespace Wokhan.Core.Extensions.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact()]
        public void AsArrayTest()
        {
            var result = ObjectExtensions.AsArray("whatever");
            
            Assert.Single(result);
            Assert.Equal("whatever", result[0]);
        }

        [Fact()]
        public void GetValueFromPathTest()
        {
            var result = ObjectExtensions.GetValueFromPath(new { P1 = new { P2 = new { P3 = "Here" } } }, "P1.P2.P3");

            Assert.Equal("Here", result);
        }

        [Fact()]
        public void SafeConvertTest()
        {
            var result = ObjectExtensions.SafeConvert("2", typeof(double));
            Assert.Equal(2.0, result);

            result = ObjectExtensions.SafeConvert("", typeof(double));
            Assert.Null(result);

            result = ObjectExtensions.SafeConvert(null, typeof(double));
            Assert.Null(result);

            result = ObjectExtensions.SafeConvert(DBNull.Value, typeof(double));
            Assert.Null(result);

            result = ObjectExtensions.SafeConvert(DBNull.Value, typeof(double));
            Assert.Null(result);
        }

        [Fact]
        public void CustomPropertyTest()
        {
            long memory;

            // Defining a zone for x not to exist outside of it
            {
                var x = new object();

                x.SetCustomProperty("test", 1);
                Assert.Equal(1, x.GetCustomProperty<int>("test"));
                memory = GetAndLogMemoryUse();

                x.SetCustomProperty("test", 2);
                Assert.Equal(2, x.GetCustomProperty<int>("test"));
                memory = GetAndLogMemoryUse();

                var tt = new byte[80000];
                //x.SetCustomProperty("test", new byte[10000]);
                memory = GetAndLogMemoryUse();

                x.SetCustomProperty("test", new byte[0]);
                memory = GetAndLogMemoryUse();

                var toto = new byte[80000];
                //x.SetCustomProperty("test", new byte[1000000]);
                memory = GetAndLogMemoryUse();
            }

            // Here x should be GCed, freeing memory.
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            //GC.WaitForPendingFinalizers();

            memory = GetAndLogMemoryUse(true);

        }

        private static long GetAndLogMemoryUse(bool waitForGC = true)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            var memory = GC.GetTotalMemory(waitForGC);
            Console.WriteLine($"Memory used: {memory}");
            return memory;
        }
    }
}