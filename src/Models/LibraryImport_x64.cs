using System;
using System.Runtime.InteropServices;

namespace Calculator3.Models
{
    public class LibraryImport_x64
    {
        public struct Result
        {
            public bool error;
            public double res;
        }
        
        [DllImport("CalculatorDll.dll")]
        public static extern Result Calculate(IntPtr calc, string expression, double x);

        [DllImport("CalculatorDll.dll")]
        public static extern IntPtr Constructor();        
    }
}
