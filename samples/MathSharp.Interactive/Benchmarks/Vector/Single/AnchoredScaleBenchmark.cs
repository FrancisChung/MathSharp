﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using BenchmarkDotNet.Attributes;
using OpenTK;

namespace MathSharp.Interactive.Benchmarks.Vector.Single
{
    using Vector = MathSharp.Vector;
    using SysVector2 = System.Numerics.Vector2;

    [CoreJob]
    [RPlotExporter]
    [RankColumn]
    [Orderer]
    public class AnchoredScaleBenchmark
    {
        private Vector2 _openTkTranslation;
        private Vector2 _openTkAnchor;
        private Vector2 _openTkScale;
        private Vector2 _openTkAmount;

        private SysVector2 _sysTranslation;
        private SysVector2 _sysAnchor;
        private SysVector2 _sysScale;
        private SysVector2 _sysAmount;

        private Vector128<float> _mathSharpTranslation;
        private Vector128<float> _mathSharpAnchor;
        private Vector128<float> _mathSharpScale;
        private Vector128<float> _mathSharpAmount;

        private Vector2 _result;
        private SysVector2 _sysResult;

        [GlobalSetup]
        public void Setup()
        {
            _openTkTranslation = new Vector2(1.7f, 2.3f);
            _openTkAnchor = new Vector2(1.0f, 0.0f);
            _openTkScale = new Vector2(7.0f, 3.6f);
            _openTkAmount = new Vector2(0.5f, 0.25f);

            _sysTranslation = new SysVector2(1.7f, 2.3f);
            _sysAnchor = new SysVector2(1.0f, 0.0f);
            _sysScale = new SysVector2(7.0f, 3.6f);
            _sysAmount = new SysVector2(0.5f, 0.25f);

            _mathSharpTranslation = Vector128.Create(1.7f, 2.3f, 0f, 0f);
            _mathSharpAnchor = Vector128.Create(1.0f, 0.0f, 0f, 0f);
            _mathSharpScale = Vector128.Create(7.0f, 3.6f, 0f, 0f);
            _mathSharpAmount = Vector128.Create(0.5f, 0.25f, 0f, 0f);
        }

        [Benchmark]
        public void MathSharp()
        {
            Vector128<float> newScale = Vector.Multiply(_mathSharpScale, _mathSharpAmount);
            Vector128<float> deltaT = Vector.Multiply(_mathSharpScale, Vector.Subtract(Vector.SingleConstants.One, _mathSharpAmount));
            deltaT = Vector.Multiply(deltaT, _mathSharpAnchor);
            Vector128<float> result = Vector.Multiply((Vector.Add(_mathSharpTranslation, deltaT)), newScale);

            result.Store(out _result);
        }

        [Benchmark]
        public void SystemNumerics()
        {
            SysVector2 newScale = _sysScale * _sysAmount;
            SysVector2 deltaT = _sysScale * (SysVector2.One - _sysAmount);
            deltaT *= _sysAnchor;
            _sysResult = (_sysTranslation + deltaT) * newScale;
        }

        [Benchmark]
        public void OpenTk()
        {
            Vector2 newScale = _openTkScale * _openTkAmount;
            Vector2 deltaT = _openTkScale * (Vector2.One - _openTkAmount);
            deltaT *= _openTkAnchor;
            _result = (_openTkTranslation + deltaT) * newScale;
        }
    }

    public static unsafe class OpenGlVectorExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Store(this Vector128<float> hwVector, out Vector2 vector)
        {
            if (Sse.IsSupported)
            {
                fixed (void* pDest = &vector)
                {
                    Sse.StoreLow((float*)pDest, hwVector);
                }

                return;
            }

            Store_Software(hwVector, out vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Store_Software(this Vector128<float> hwVector, out Vector2 vector)
        {
            // JIT naturally uses SSE to store here so we are good :yay:
            vector = Unsafe.As<Vector128<float>, Vector2>(ref hwVector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Load(this Vector2 vector)
        {
            if (Sse.IsSupported)
            {
                // Construct 2 separate vectors, each having the first element being the value
                // and the rest being 0
                Vector128<float> lo = Sse.LoadScalarVector128(&vector.X);
                Vector128<float> hi = Sse.LoadScalarVector128(&vector.Y);

                // Unpack these to (lo, mid, 0, 0), the desired vector
                return Sse.UnpackLow(lo, hi);
            }

            return SoftwareFallback(vector);

            static Vector128<float> SoftwareFallback(Vector2 vector)
            {
                return Vector128.Create(vector.X, vector.Y, 0, 0);
            }
        }
    }
}