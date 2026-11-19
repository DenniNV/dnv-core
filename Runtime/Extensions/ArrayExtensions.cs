using System;
using Random = System.Random;

namespace Libraries.DNV.Extensions
{
    public static class ArrayExtensions
    {
        public static int[] Shuffle(this int[] self, int seed)
        {
            if(self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            var random = new Random(seed);

            for (var i = 0; i < self.Length - 1; i++)
            {
                var randomIndex = random.Next(i, self.Length);
                (self[i], self[randomIndex]) = (self[randomIndex], self[i]);
            }

            return self;
        }
    }
}
