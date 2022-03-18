using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aquila.Core
{
    /// <summary>
    /// Aquila comparison semantic.
    /// </summary>
    public static class AqComparison
    {
        static Exception InvalidTypeCodeException(string op, string left, string right)
            => new InvalidOperationException($"{op}({left}, {right})");

        public static bool Clt(long lx, double dy) => (double)lx < dy;
        public static bool Cgt(long lx, double dy) => (double)lx > dy;
        public static bool Ceq(long lx, double dy) => (double)lx == dy;
        public static bool Ceq(long lx, bool by) => (lx != 0) == by;
        public static bool Ceq(long lx, string sy) => Equals(sy, lx);

        public static bool Clt(int lx, double dy) => (double)lx < dy;
        public static bool Cgt(int lx, double dy) => (double)lx > dy;
        public static bool Ceq(int lx, double dy) => (double)lx == dy;
        public static bool Ceq(int lx, bool by) => (lx != 0) == by;
        public static bool Ceq(int lx, string sy) => Equals(sy, lx);

        public static bool Ceq(double dx, string sy) => Equals(sy, dx);
        public static bool Ceq(string sx, long ly) => Equals(sx, ly);
        public static bool Ceq(string sx, double dy) => Equals(sx, dy);
        public static bool Ceq(string sx, bool by) => Convert.ToBoolean(sx) == by;


        public static int Compare(bool bx, bool by) => (bx ? 2 : 1) - (by ? 2 : 1);


        /// <summary>
        /// Compares two long integer values.
        /// </summary>
        /// <returns>(+1,0,-1)</returns>
        public static int Compare(long x, long y) => (x > y) ? +1 : (x < y ? -1 : 0);

        /// <summary>
        /// Compares two double values.
        /// </summary>
        /// <returns>(+1,0,-1)</returns>
        /// <remarks>We cannot used <see cref="Math.Sign(double)"/> on <c>x - y</c> since the result can be NaN.</remarks>
        public static int Compare(double x, double y) => (x > y) ? +1 : (x < y ? -1 : 0);
    }
}