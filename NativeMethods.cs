using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NISTscan
{
    class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(
            IntPtr Destination,
            IntPtr Source,
            [MarshalAs(UnmanagedType.U4)] int Length);

        [DllImport("kernel32.dll", EntryPoint = "GlobalFree")]
        public static extern IntPtr GlobalFree(
            IntPtr pointer);

        [DllImport("kernel32.dll", EntryPoint = "GlobalAlloc")]
        public static extern IntPtr GlobalAlloc(
            int wFlags,
            int dwBytes);
    }

}
