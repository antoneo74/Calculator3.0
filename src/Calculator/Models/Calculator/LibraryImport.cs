using System;
using System.Runtime.InteropServices;

namespace Calculator3.Models.Calculator
{
    public class LibraryImport
    {
        public struct Result
        {
            public bool error;
            public double res;
        }

        [DllImport("CalculatorDll")]
        public static extern Result Calculate(IntPtr calc, string expression, double x);

        [DllImport("CalculatorDll")]
        public static extern IntPtr Constructor();
    }
}
