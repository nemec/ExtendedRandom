using System;
using System.Collections.Generic;
using System.Linq;

using SysRandom = System.Random;

namespace ExtendedRandom
{
    /// <summary>
    /// Extensions to the built in <see cref="System.Random"/> class. All
    /// methods are available both as extension methods and static members
    /// that use a global <see cref="System.Random"/>.
    /// </summary>
    public static class Random
    {
        private static SysRandom _rand = new SysRandom();

        // Not really relevant to Random...
        /*public static IEnumerable<int> Range(int end)
        {
            return Range(0, end);
        }

        public static IEnumerable<int> Range(int start, int end, int step = 1)
        {
            if (step == 0)
            {
                throw new ArgumentException("Step size must be nonzero", "step");
            }
            if (step > 0 && end < start)
            {
                throw new ArgumentException(
                    "Step size is increasing, but end < start.", "step");
            }
            if (step < 0 && start < end)
            {
                throw new ArgumentException(
                    "Step size is decreasing, but start < end.", "step");
            }

            var current = start;
            while (step > 0 && current < end ||
                    step < 0 && current > end)
            {
                yield return current;
                current += step;
            }
        }*/

        /// <summary>
        /// Re-initialize the shared randomness object.
        /// </summary>
        public static void Seed()
        {
            _rand = new SysRandom(Environment.TickCount);
        }

        /// <summary>
        /// Re-initialize the shared randomness object with the given seed.
        /// </summary>
        /// <param name="seed"></param>
        public static void Seed(int seed)
        {
            _rand = new SysRandom(seed);
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and 
        /// less than System.Int32.MaxValue.
        /// </returns>
        public static int Next()
        {
            return _rand.Next();
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="end">
        /// The exclusive upper bound of the random number to be generated.
        /// Must be greater than or equal to zero.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less 
        /// than maxValue; that is, the range of return values ordinarily 
        /// includes zero but not maxValue. However, if maxValue equals 
        /// zero, maxValue is returned.
        /// </returns>
        public static int Next(int end)
        {
            return _rand.Next(end);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="start">
        /// The inclusive lower bound of the random number returned.
        /// </param>
        /// <param name="end">
        /// The exclusive upper bound of the random number returned. end must
        /// be greater than or equal to start.
        /// </param>
        /// <returns></returns>
        public static int Next(int start, int end)
        {
            return _rand.Next(start, end);
        }

        /// <summary>
        /// Return a random number within a specified range, bound to a
        /// step size. Random number will be a multiple of
        /// <paramref name="step"/> added to <paramref name="start"/>.
        /// If <paramref name="step"/> is negative, the random range will
        /// count *down* from start until it reaches end.
        /// </summary>
        /// <param name="start">
        /// The inclusive "lower" bound of the random number returned.
        /// If <paramref name="step"/> is negative, this will represent
        /// the *inclusive* upper bound.
        /// </param>
        /// <param name="end">
        /// The exclusive "upper" bound of the random number returned.
        /// </param>
        /// <param name="step">
        /// The step size the random number is bound to. May by negative.
        /// </param>
        /// <returns></returns>
        public static int Next(int start, int end, int step)
        {
            if (step == 0)
            {
                throw new ArgumentException("Step size must be nonzero", "step");
            }
            if (step > 0 && end < start)
            {
                throw new ArgumentException(
                    "Step size is increasing, but end < start.", "step");
            }
            if (step < 0)
            {
                if (start < end)
                {
                    throw new ArgumentException(
                        "Step size is decreasing, but start < end.", "step");
                }

                // Built-in randomness requires start < end
                var tmp = start + -Math.Sign(step);  // Make inclusive
                start = end - step;
                end = tmp;
                step *= -1;
            }

            return _rand.Next(start/step, end/step)*step;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="ArgumentNullException">buffer is null.</exception>
        public static void NextBytes(byte[] buffer)
        {
            _rand.NextBytes(buffer);
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            return _rand.NextDouble();
        }

        /// <summary>
        /// Return a random element from the non-empty sequence seq.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq">Sequence to select from.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// sequence is empty.
        /// </exception>
        /// <returns></returns>
        public static T Choice<T>(IEnumerable<T> seq)
        {
            return Choice(_rand, seq);
        }

        /// <summary>
        /// Return a random element from the non-empty sequence seq.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Source of randomness.</param>
        /// <param name="seq">Sequence to select from.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// sequence is empty.
        /// </exception>
        /// <returns></returns>
        public static T Choice<T>(this SysRandom source, IEnumerable<T> seq)
        {
            var list = seq as IList<T>;
            if (list != null)
            {
                if (!list.Any())
                {
                    throw new ArgumentOutOfRangeException(
                        "seq", "Sequence must not be empty.");
                }
                return list.Skip(source.Next(list.Count)).First();
            }

            // Found on SO http://stackoverflow.com/a/11315054/564755
            var result = default(T);
            var cnt = 0;
            foreach (var item in seq)
            {
                cnt++;
                if (source.Next(cnt) == 0)
                {
                    result = item;
                }
            }

            if (cnt == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "seq", "Sequence must not be empty.");
            }

            return result;
        }

        /// <summary>
        /// Randomly choose an item from the list where each item in the list 
        /// has a probability of being chosen relative to its weight. Each
        /// item's weight is generated by running the given
        /// <see cref="Func{T,U}"/> (where U is an <see cref="int"/>) on each
        /// item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(IEnumerable<T> seq, Func<T, int> getWeight)
        {
            var enumerable = seq as IList<T> ?? seq.ToList();
            return WeightedChoice(enumerable, enumerable.Select(getWeight));
        }

        /// <summary>
        /// Randomly choose an item from the list where each item in the list 
        /// has a probability of being chosen relative to its weight. Each
        /// item's weight is generated by running the given
        /// <see cref="Func{T,U}"/> (where U is an <see cref="int"/>) on each
        /// item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="seq"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(this SysRandom random, IEnumerable<T> seq, Func<T, int> getWeight)
        {
            var enumerable = seq as IList<T> ?? seq.ToList();
            return WeightedChoice(random, enumerable, enumerable.Select(getWeight));
        }

        /// <summary>
        /// Randomly choose an item from <paramref name="seq"/> where each
        /// item in the list has a probability of being chosen relative to its 
        /// counterpart in the <paramref name="weights"/> list. Assumes the
        /// count of <paramref name="weights"/> is equal to 
        /// <paramref name="seq"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(IEnumerable<T> seq, IEnumerable<int> weights)
        {
            return WeightedChoice(_rand, seq, weights);
        }

        /// <summary>
        /// Choose an item from <paramref name="seq"/> where each item in the
        /// list has a probability of being chosen relative to its counterpart
        /// in the <paramref name="weights"/> list. Assumes the count of
        /// <paramref name="weights"/> is equal to <paramref name="seq"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="seq"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(this SysRandom source, IEnumerable<T> seq, IEnumerable<int> weights)
        {
            var runningTotal = Accumulate(weights).ToList();
            var total = runningTotal[runningTotal.Count - 1];
            var idx = (int)(source.NextDouble() * total);
            return seq.Skip(BisectRight(runningTotal, idx)).First();
        }

        /// <summary>
        /// Randomly choose an item from the list where each item in the list 
        /// has a probability of being chosen relative to its weight. Each
        /// item's weight is generated by running the given
        /// <see cref="Func{T,U}"/> (where U is an <see cref="int"/>) on each
        /// item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(IEnumerable<T> seq, Func<T, double> getWeight)
        {
            var enumerable = seq as IList<T> ?? seq.ToList();
            return WeightedChoice(enumerable, enumerable.Select(getWeight));
        }

        /// <summary>
        /// Randomly choose an item from the list where each item in the list 
        /// has a probability of being chosen relative to its weight. Each
        /// item's weight is generated by running the given
        /// <see cref="Func{T,U}"/> (where U is an <see cref="int"/>) on each
        /// item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="seq"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(this SysRandom random, IEnumerable<T> seq, Func<T, double> getWeight)
        {
            var enumerable = seq as IList<T> ?? seq.ToList();
            return WeightedChoice(random, enumerable, enumerable.Select(getWeight));
        }

        /// <summary>
        /// Choose an item from <paramref name="seq"/> where each item in the
        /// list has a probability of being chosen relative to its counterpart
        /// in the <paramref name="weights"/> list. Assumes the count of
        /// <paramref name="weights"/> is equal to <paramref name="seq"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(IEnumerable<T> seq, IEnumerable<double> weights)
        {
            return WeightedChoice(_rand, seq, weights);
        }

        /// <summary>
        /// Choose an item from <paramref name="seq"/> where each item in the
        /// list has a probability of being chosen relative to its counterpart
        /// in the <paramref name="weights"/> list. Assumes the count of
        /// <paramref name="weights"/> is equal to <paramref name="seq"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="seq"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(this SysRandom source, IEnumerable<T> seq, IEnumerable<double> weights)
        {
            var runningTotal = Accumulate(weights).ToList();
            var total = runningTotal[runningTotal.Count - 1];
            var idx = source.NextDouble() * total;
            return seq.Skip(BisectRight(runningTotal, idx)).First();
        }

        /// <summary>
        /// Return the index where to insert item <paramref name="toInsert"/> 
        /// in list <paramref name="sortedItems"/>, assuming 
        /// <paramref name="sortedItems"/> is sorted.
        /// </summary>
        /// <param name="sortedItems"></param>
        /// <param name="toInsert"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        private static int BisectRight(IList<int> sortedItems, int toInsert, int low = 0, int? high = null)
        {
            // From Python source code.
            if (!high.HasValue)
            {
                high = sortedItems.Count;
            }
            while (low < high)
            {
                var mid = (low + high.Value)/2;
                if (toInsert < sortedItems[mid])
                {
                    high = mid;
                }
                else
                {
                    low = mid + 1;
                }
            }
            return low;
        }

        /// <summary>
        /// Return the index where to insert item <paramref name="toInsert"/> 
        /// in list <paramref name="sortedItems"/>, assuming 
        /// <paramref name="sortedItems"/> is sorted.
        /// </summary>
        /// <param name="sortedItems"></param>
        /// <param name="toInsert"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        private static int BisectRight(IList<double> sortedItems, double toInsert, int low = 0, int? high = null)
        {
            // From Python source code.
            if (!high.HasValue)
            {
                high = sortedItems.Count;
            }
            while (low < high)
            {
                var mid = (low + high.Value)/2;
                if (toInsert < sortedItems[mid])
                {
                    high = mid;
                }
                else
                {
                    low = mid + 1;
                }
            }
            return low;
        }

        /// <summary>
        /// Shuffle the sequence and return it as a new 
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(IList<T> seq)
        {
            return Shuffle(_rand, seq);
        }

        /// <summary>
        /// Shuffle the sequence and return it as a new 
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this SysRandom source, IList<T> seq)
        {
            if (source == null) { throw new ArgumentException("source"); }
            if (seq == null) { throw new ArgumentException("seq"); }

            return ShuffleIterator(source, seq);
        }

        private static IEnumerable<T> ShuffleIterator<T>(SysRandom source, IEnumerable<T> seq)
        {
            var buffer = seq.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = source.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        /// <summary>
        /// Return a k length list of unique elements chosen from the
        /// population sequence. Used for random sampling without replacement.
        /// 
        /// To choose a sample from a range of integers, use 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="population"></param>
        /// <param name="k">Sample size.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sample<T>(IEnumerable<T> population, int k)
        {
            return Sample(_rand, population, k);
        } 

        /// <summary>
        /// Return a k length list of unique elements chosen from the
        /// population sequence. Used for random sampling without replacement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="population"></param>
        /// <param name="k">Sample size.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sample<T>(this SysRandom source, IEnumerable<T> population, int k)
        {
            var list = population as IList<T>;
            if (list != null)
            {
                // From Python's random source
                var selected = new HashSet<int>();
                var count = list.Count;

                for (var i = 0; i < k; i++)
                {
                    var j = source.Next(count);
                    while (selected.Contains(j))
                    {
                        j = source.Next(count);
                    }
                    selected.Add(j);
                    yield return list[j];
                }
            }
            else
            {
                // http://en.wikipedia.org/wiki/Reservoir_sampling
                var idx = 0;
                var samples = new List<T>();
                var iter = population.GetEnumerator();
                while (samples.Count < k && iter.MoveNext())
                {
                    samples.Add(iter.Current);
                    idx++;
                }
                while (iter.MoveNext())
                {
                    idx++;
                    var r = source.Next(idx);
                    if (r < k)
                    {
                        samples[r] = iter.Current;
                    }
                }

                foreach (var sample in samples)
                {
                    yield return sample;
                }
            }
        }

        #region Accumulations

        private static IEnumerable<int> Accumulate(IEnumerable<int> weights)
        {
            var total = 0;
            foreach (var weight in weights)
            {
                total += weight;
                yield return total;
            }
        }

        private static IEnumerable<double> Accumulate(IEnumerable<double> weights)
        {
            var total = 0.0;
            foreach (var weight in weights)
            {
                total += weight;
                yield return total;
            }
        } 

        #endregion
    }
}
