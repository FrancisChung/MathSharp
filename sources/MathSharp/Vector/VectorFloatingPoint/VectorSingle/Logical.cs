﻿using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using MathSharp.Utils;
using static MathSharp.SoftwareFallbacks;
using static MathSharp.Utils.Helpers;

namespace MathSharp
{
    public static partial class Vector
    {
        [MethodImpl(MaxOpt)]
        public static float GetX(Vector128<float> vector) => vector.GetElement(0);

        [MethodImpl(MaxOpt)]
        public static float GetY(Vector128<float> vector) => vector.GetElement(1);

        [MethodImpl(MaxOpt)]
        public static float GetZ(Vector128<float> vector) => vector.GetElement(2);

        [MethodImpl(MaxOpt)]
        public static float GetW(Vector128<float> vector) => vector.GetElement(3);

        [MethodImpl(MaxOpt)]
        public static Vector128<float> Shuffle(Vector128<float> vector, byte control)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(vector, control);
            }

            return Shuffle(vector, vector, control);
        }

        [MethodImpl(MaxOpt)]
        public static Vector128<float> FillWithX(Vector128<float> vector)
            => Shuffle(vector, ShuffleValues.XXXX);

        [MethodImpl(MaxOpt)]
        public static Vector128<float> FillWithY(Vector128<float> vector)
            => Shuffle(vector, ShuffleValues.YYYY);

        [MethodImpl(MaxOpt)]
        public static Vector128<float> FillWithZ(Vector128<float> vector)
            => Shuffle(vector, ShuffleValues.ZZZZ);

        [MethodImpl(MaxOpt)]
        public static Vector128<float> FillWithW(Vector128<float> vector)
            => Shuffle(vector, ShuffleValues.WWWW);

        [MethodImpl(MaxOpt)]
        public static Vector128<float> Shuffle(Vector128<float> left, Vector128<float> right, byte control)
        {
            if (Sse.IsSupported)
            {
                return Sse.Shuffle(left, right, control);
            }

            return Shuffle_Software(left, right, control);
        }
    }
}
