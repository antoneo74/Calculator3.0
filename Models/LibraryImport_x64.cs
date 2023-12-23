using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
        public static extern Result Test(IntPtr calc, string expression, double x);

        [DllImport("CalculatorDll.dll")]
        public static extern IntPtr Constructor();        
    }
}
