using Xunit;
using Wokhan.Collections.Generic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Wokhan.Collections.Generic.Extensions.Tests
{
    public class EnumerableExtensionsTests
    {

        TestData first = new() { P1 = "b", P2 = "d" };
        TestData second = new() { P1 = "a", P2 = "c" };
        TestData third = new() { P1 = "a", P2 = "a" };

        [Fact()]
        public void ApplyToAllTest()
        {
            var results = EnumerableExtensions.ApplyToAll(new[] { first, second }, x => x.P1 += "_updated").ToList();

            Assert.Equal("b_updated", results[0].P1);
            Assert.Equal("a_updated", results[1].P1);
        }

        [Fact()]
        public void AverageCheckedTest()
        {
            var result = EnumerableExtensions.AverageChecked(new IConvertible[] { "2", "", "4" }, true);
            Assert.Equal(3, result);

            result = EnumerableExtensions.AverageChecked(new IConvertible[] { "2", "3", "4" }, true);
            Assert.Equal(3, result);

        }

        [Fact()]
        public void GreatestCommonDivTest()
        {
            var result = EnumerableExtensions.GreatestCommonDiv(new[] { 2, 4, 6, 12 });
            
            Assert.Equal(2, result);
        }

        [Fact()]
        public void AsObjectCollectionTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void AsObjectCollectionTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void GetInnerTypeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void GetInnerTypeTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByManyTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByManyTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByManyTest2()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByManyTypedTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByManyTest3()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByManyTest4()
        {
            Assert.True(false, "This test needs an implementation");
        }

        private class TestData
        {
            public string P1 { get; set; }
            public string P2 { get; set; }

        }

        [Fact()]
        public void OrderByAllTest()
        {
            var resultStringArray = EnumerableExtensions.OrderByAll(new[] { "b", "d", "a", "c" });

            Assert.Equal(new[] { "a", "b", "c", "d" }, resultStringArray);

            var resultEnumerable = EnumerableExtensions.OrderByAll(new[] { first, second, third });

            Assert.Equal(new[] { third, second, first }, resultEnumerable);
        }

        [Fact()]
        public void OrderByAllTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByAllTest2()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OrderByAllTypedTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ReplaceAllTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void AsParallelTest()
        {
            // Ignored   
        }

        [Fact()]
        public void AddAllTest()
        {
            var collection = new List<string> { "a" };
            EnumerableExtensions.AddAll(collection, new[] { "b", "c" });

            Assert.Equal(new[] { "a", "b", "c" }, collection);
        }

        [Fact()]
        public void ToObjectTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToObjectTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToObjectTest2()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void WithProgressTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void WithProgressTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void MergeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void MergeTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToDataTableTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToDataTableTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToPivotTableTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void PivotTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}