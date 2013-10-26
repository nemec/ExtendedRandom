using ExtendedRandom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtendedRandomUnitTest
{
    [TestClass]
    public class ExtendedRandomUnitTest
    {
        [TestMethod]
        public void WeightedChoice_WithDoubleWeights_ReturnsCorrectWeighting()
        {
            var weights = new[] {2.0, 3, 5};
            var items = new[] {"first", "second", "third"};
            var totals = new[] {0, 0, 0};

            for (var i = 0; i < 1000000; i++)
            {
                var item = Random.WeightedChoice(items, weights);
                for (var j = 0; j < items.Length; j++)
                {
                    if (items[j] == item)
                    {
                        totals[j]++;
                    }
                }
            }

            // Statistically, this should never fail...
            Assert.AreEqual(totals[0], 200000, 2000);
            Assert.AreEqual(totals[1], 300000, 2000);
            Assert.AreEqual(totals[2], 500000, 2000);
        }

        [TestMethod]
        public void WeightedChoice_WithIntegerWeights_ReturnsCorrectWeighting()
        {
            var weights = new[] {2, 3, 5};
            var items = new[] {"first", "second", "third"};
            var totals = new[] {0, 0, 0};

            for (var i = 0; i < 1000000; i++)
            {
                var item = Random.WeightedChoice(items, weights);
                for (var j = 0; j < items.Length; j++)
                {
                    if (items[j] == item)
                    {
                        totals[j]++;
                    }
                }
            }

            // Statistically, this should never fail...
            Assert.AreEqual(totals[0], 200000, 2000);
            Assert.AreEqual(totals[1], 300000, 2000);
            Assert.AreEqual(totals[2], 500000, 2000);
        }

        [TestMethod]
        public void WeightedChoice_WithIntegerWeightsUnsorted_ReturnsCorrectWeighting()
        {
            var weights = new[] {3, 2, 5};
            var items = new[] {"first", "second", "third"};
            var totals = new[] {0, 0, 0};

            for (var i = 0; i < 1000000; i++)
            {
                var item = Random.WeightedChoice(items, weights);
                for (var j = 0; j < items.Length; j++)
                {
                    if (items[j] == item)
                    {
                        totals[j]++;
                    }
                }
            }

            // Statistically, this should never fail...
            Assert.AreEqual(totals[0], 300000, 2000);
            Assert.AreEqual(totals[1], 200000, 2000);
            Assert.AreEqual(totals[2], 500000, 2000);
        }

        [TestMethod]
        public void WeightedChoice_WithWeightsBasedOnLength_ReturnsCorrectWeighting()
        {
            var items = new[] {"to", "one", "three"};
            var totals = new[] {0, 0, 0};

            for (var i = 0; i < 1000000; i++)
            {
                var item = Random.WeightedChoice(items, s => s.Length);
                for (var j = 0; j < items.Length; j++)
                {
                    if (items[j] == item)
                    {
                        totals[j]++;
                    }
                }
            }

            // Statistically, this should never fail...
            Assert.AreEqual(totals[0], 200000, 2000);
            Assert.AreEqual(totals[1], 300000, 2000);
            Assert.AreEqual(totals[2], 500000, 2000);
        }
    }
}
