﻿using System.Collections.Generic;
using System.Runtime.Intrinsics;
using Xunit;
using static MathSharp.Utils.Helpers;

namespace MathSharp.UnitTests.VectorTests.VectorSingle.ComparisonTests
{
    public class LessThan
    {
        public static IEnumerable<object[]> Data =>
            new[]
            {
                new object[] { Vector128.Create(0f), Vector128.Create(0f), new []{ true, true, true, true } },
                new object[] { Vector128.Create(0f), Vector128.Create(1f), new []{ false, false, false, false } },
                new object[] { Vector128.Create(1f), Vector128.Create(0f), new []{ true, true, true, true } },
                new object[] { Vector128.Create(1f, -1f, 1f, -1f), Vector128.Create(0f), new []{ true, false, true, false } },
                new object[] { Vector128.Create(float.NaN), Vector128.Create(0f), new []{ true, true, true, true } },
                new object[]
                {
                    Vector128.Create(0f, float.MaxValue, float.MinValue, float.PositiveInfinity),
                    Vector128.Create(float.Epsilon, float.MaxValue - 1000000000000000000000000000000000000f, float.MinValue + 1000000000000000000000000000000000000f, float.NegativeInfinity),
                    new []{ false, true, false, true }
                },
                new object[]
                {
                    Vector128.Create(1f, 10000000f, 0.000001f, -1),
                    Vector128.Create(1f, 10000000f, 0.000001f, -1),
                    new []{ true, true, true, true }
                }
            };

        [Theory]
        [MemberData(nameof(Data))]
        public static void LessThan_Theory(Vector128<float> left, Vector128<float> right, bool[] expected)
        {
            Vector128<int> result1 = Vector.LessThan(left, right).AsInt32();

            Assert.True(AreAllNotEqual(expected, result1));
        }
    }
}