//This random number generator was adapted from https://github.com/lordofduct/spacepuppy-unity-framework-3.0

using System;
using BitConverter = System.BitConverter;

namespace Melt
{
    /// <summary>
    /// Interface/contract for a random number generator.
    /// </summary>
    public interface IRandom
    {
        float Next();
        double NextDouble();
        int Next(int size);
        int Next(int low, int high);
    }

    public class ThreadSafeRNG : IRandom
    {
        private Random globalRNG;

        [ThreadStatic]
        private static Random threadLocalRNG;

        public ThreadSafeRNG()
        {
            globalRNG = new Random();
        }

        public ThreadSafeRNG(int seed)
        {
            globalRNG = new Random(seed);
        }

        public void initializeLocal()
        {
            int seed;
            lock (globalRNG) seed = globalRNG.Next();
            threadLocalRNG = new Random(seed);
        }

        public int Next(int maxValue)
        {
            if (threadLocalRNG == null)
                initializeLocal();
            return threadLocalRNG.Next(maxValue);
        }

        public float Next()
        {
            if (threadLocalRNG == null)
                initializeLocal();
            return threadLocalRNG.Next();
        }

        public double NextDouble()
        {
            if (threadLocalRNG == null)
                initializeLocal();
            return threadLocalRNG.Next();
        }

        public int Next(int minValue, int maxValue)
        {
            if (threadLocalRNG == null)
                initializeLocal();
            return threadLocalRNG.Next(minValue, maxValue);
        }
    }


    public static class RandomUtil
    {
        #region Static Fields

        private static MicrosoftRNG _defaultRNG = new MicrosoftRNG();

        public static IRandom Standard { get { return _defaultRNG; } }

        public static IRandom CreateRNG(int seed)
        {
            return new MicrosoftRNG(seed);
        }

        public static IRandom CreateRNG()
        {
            return new MicrosoftRNG();
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Return 0 or 1. Numeric version of Bool.
        /// </summary>
        /// <returns></returns>
        public static int Pop(this IRandom rng)
        {
            return rng.Next(1000) % 2;
        }

        public static int Sign(this IRandom rng)
        {
            int n = rng.Next(1000) % 2;
            return n + n - 1;
        }

        /// <summary>
        /// Return a true randomly.
        /// </summary>
        /// <returns></returns>
        public static bool Bool(this IRandom rng)
        {
            return (rng.Next(1000) % 2 != 0);
        }

        public static bool Bool(this IRandom rng, float oddsOfTrue)
        {
            int i = rng.Next(100000);
            int m = (int)(oddsOfTrue * 100000);
            return i < m;
        }

        /// <summary>
        /// Return -1, 0, 1 randomly. This can be used for bizarre things like randomizing an array.
        /// </summary>
        /// <returns></returns>
        public static int Shift(this IRandom rng)
        {
            return (rng.Next(999) % 3) - 1;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Select between min and max, exclussive of max.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static float Range(this IRandom rng, float max, float min = 0.0f)
        {
            return (float)(rng.NextDouble() * (max - min)) + min;
        }

        /// <summary>
        /// Select between min and max, exclussive of max.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int Range(this IRandom rng, int max, int min = 0)
        {
            return rng.Next(min, max);
        }

        /// <summary>
        /// Select an weighted index from 0 to length of weights.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int Range(this IRandom rng, params float[] weights)
        {
            int i;
            float w;
            float total = 0f;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsPositiveInfinity(w)) return i;
                else if (w >= 0f && !float.IsNaN(w)) total += w;
            }

            if (rng == null) rng = RandomUtil.Standard;
            float r = rng.Next();
            float s = 0f;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / total;
                if (s > r)
                {
                    return i;
                }
            }

            //should only get here if last element had a zero weight, and the r was large
            i = weights.Length - 1;
            while (i > 0 || weights[i] <= 0f) i--;
            return i;
        }

        /// <summary>
        /// Select an weighted index from 0 to length of weights.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int Range(this IRandom rng, float[] weights, int startIndex, int count = -1)
        {
            int i;
            float w;
            float total = 0f;
            int last = count < 0 ? weights.Length : System.Math.Min(startIndex + count, weights.Length);
            for (i = startIndex; i < last; i++)
            {
                w = weights[i];
                if (float.IsPositiveInfinity(w)) return i;
                else if (w >= 0f && !float.IsNaN(w)) total += w;
            }

            if (rng == null) rng = RandomUtil.Standard;
            float r = rng.Next();
            float s = 0f;

            for (i = startIndex; i < last; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / total;
                if (s > r)
                {
                    return i;
                }
            }

            //should only get here if last element had a zero weight, and the r was large
            i = last - 1;
            while (i > 0 || weights[i] <= 0f) i--;
            return i;
        }

        #endregion




        #region Special Types

        private class MicrosoftRNG : System.Random, IRandom
        {

            public MicrosoftRNG() : base()
            {

            }

            public MicrosoftRNG(int seed) : base(seed)
            {

            }

            float IRandom.Next()
            {
                lock (this) return(float)this.NextDouble();
            }

            double IRandom.NextDouble()
            {
                lock (this) return this.NextDouble();
            }

            int IRandom.Next(int size)
            {
                lock (this) return this.Next(size);
            }

            int IRandom.Next(int low, int high)
            {
                lock (this) return this.Next(low, high);
            }
        }

        public class VB_RNG : IRandom
        {

            #region Fields

            private int _seed;

            #endregion

            #region Constructor

            public VB_RNG()
            {
                this.Randomize();
            }

            public VB_RNG(double seed)
            {
                this.Randomize(seed);
            }

            #endregion

            #region Methods

            public float Next()
            {
                return this.VBNext(1f);
            }

            public int Next(int size)
            {
                return (int)(this.Next() * size);
            }

            public int Next(int low, int high)
            {
                return (int)(this.Next() * (high - low)) + low;
            }

            public double NextDouble()
            {
                return (double)this.Next();
            }

            public float VBNext(float num)
            {
                int num1 = _seed;
                if ((double)num != 0.0)
                {
                    if ((double)num < 0.0)
                    {
                        long num2 = (long)BitConverter.ToInt32(BitConverter.GetBytes(num), 0) & (long)uint.MaxValue;
                        num1 = checked((int)(num2 + (num2 >> 24) & 16777215L));
                    }
                    num1 = checked((int)((long)num1 * 1140671485L + 12820163L & 16777215L));
                }
                _seed = num1;
                return (float)num1 / 1.677722E+07f;
            }

            public void Randomize()
            {
                System.DateTime now = System.DateTime.Now;
                float timer = (float)checked((60 * now.Hour + now.Minute) * 60 + now.Second) + (float)now.Millisecond / 1000f;
                int num1 = _seed;
                int num2 = BitConverter.ToInt32(BitConverter.GetBytes(timer), 0);
                int num3 = (num2 & (int)ushort.MaxValue ^ num2 >> 16) << 8;
                int num4 = num1 & -16776961 | num3;
                _seed = num4;
            }

            public void Randomize(double num)
            {
                int num1 = _seed;
                int num2 = !BitConverter.IsLittleEndian ? BitConverter.ToInt32(BitConverter.GetBytes(num), 0) : BitConverter.ToInt32(BitConverter.GetBytes(num), 4);
                int num3 = (num2 & (int)ushort.MaxValue ^ num2 >> 16) << 8;
                int num4 = num1 & -16776961 | num3;
                _seed = num4;
            }

            #endregion

        }

        #endregion

    }
}