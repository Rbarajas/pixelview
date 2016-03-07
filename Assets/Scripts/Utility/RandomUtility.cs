using UnityEngine;

namespace PixelView.Time_.Utility
{
    public static class RandomUtility
    {
        public static int RandomArrayIndex<T>(T[] array, System.Func<T, int> elementToLikelihood = null)
        {
            if (elementToLikelihood == null)
                return Random.Range(0, array.Length);

            var aggregatedLikelihood = 0;
            foreach (var element in array)
                aggregatedLikelihood += elementToLikelihood(element);

            var randomValue = Random.Range(0, aggregatedLikelihood);

            var index = 0;
            while (true)
            {
                var likelihood = elementToLikelihood(array[index]);

                if (randomValue < likelihood)
                    return index;

                randomValue -= likelihood;

                ++index;
            }
        }

        public static T RandomArrayElement<T>(T[] array, System.Func<T, int> elementToLikelihood = null)
        {
            return array[RandomArrayIndex(array, elementToLikelihood)];
        }
    }
}