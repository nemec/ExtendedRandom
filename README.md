Extended Random
===============

Provides extensions to the built in .Net `System.Random` class. Features
come in two parts:

* Static methods that work on a global `System.Random` object. This lets
  developers without complex needs (e.g. multiple Random objects) quickly
  access a source of randomness.
* Extension methods on `System.Random` objects. These add advanced features
  that use randomness as an input. See below for more information.
  
We're on [NuGet](https://www.nuget.org/packages/ExtendedRandom/)!

##Extra Features

* `Next(start, end, step)`
  * Chooses a random integer between `start` (inclusive) and `end` (exclusive),
    but bound to a multiple of `step`. Equivalent to creating a range of
    integers from `start` to `end` with a given `step` size and then choosing
    randomly from that range.

* `Choice(seq)`
  * Returns a random element from the input sequence.

* `WeightedChoice(seq, weights)`
  * Returns a random element from the input sequence, but assigns each element
    a weight from the `weights` list such that the probability of choosing
    that element is the ratio of that weight to the total sum of all weights.
    For example, a sequence of weights {2, 3, 5} would give the second element
    a 0.3 (30%) chance of being chosen.

* `WeightedChoice(seq, getWeight)`
  * Similar to the above, but `getWeight` is a delegate that calculates the
    weight of each item (given the item) prior to choosing.

* `Shuffle(seq)`
  * Return a new sequence containing all elements in the old sequence in
    random order.

* `Sample(population, k)`
  * Returns a `k` random items from the input sequence, without replacement.