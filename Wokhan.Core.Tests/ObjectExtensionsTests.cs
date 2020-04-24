using NUnit.Framework;
using System;
using Wokhan.Core.Extensions;

namespace Wokhan.Core.Tests
{
    public class ObjectExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void CustomPropertyTest()
        {
            long memory;

            // Defining a zone for x not to exist outside of it
            {
                var x = new object();
                
                x.SetCustomProperty("test", 1);
                Assert.AreEqual(x.GetCustomProperty<int>("test"), 1);
                memory = GetAndLogMemoryUse();

                x.SetCustomProperty("test", 2);
                Assert.AreEqual(x.GetCustomProperty<int>("test"), 2);
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