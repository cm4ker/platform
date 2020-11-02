﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    /// <summary>
    /// Level of optimization.
    /// </summary>
    public enum AquilaOptimizationLevel : int
    {
        /// <summary><see cref="OptimizationLevel.Debug"/></summary>
        Debug = 0,

        O1, O2, O3, O4, O5, O6, O7, O8, O9,

        Ox = Release,

        /// <summary><see cref="OptimizationLevel.Release"/></summary>
        Release = O9,
    }

    /// <summary>
    /// Helper methods for the <see cref="AquilaOptimizationLevel"/>.
    /// </summary>
    internal static class PhpOptimizationLevelExtension
    {
        public static OptimizationLevel AsOptimizationLevel(this AquilaOptimizationLevel level)
            => level != AquilaOptimizationLevel.Debug ? OptimizationLevel.Release : OptimizationLevel.Debug;

        public static AquilaOptimizationLevel AsPhpOptimizationLevel(this OptimizationLevel level)
            => level != OptimizationLevel.Debug ? AquilaOptimizationLevel.Release : AquilaOptimizationLevel.Debug;

        public static int GraphTransformationCount(this AquilaOptimizationLevel level) => level.IsDebug() ? 0 : ((int)level - 1); // O2 .. O9

        public static bool IsDebug(this AquilaOptimizationLevel level) => level == AquilaOptimizationLevel.Debug;

        public static bool IsRelease(this AquilaOptimizationLevel level) => level != AquilaOptimizationLevel.Debug;
    }
}
