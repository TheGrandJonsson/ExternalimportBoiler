using System;
using System.Runtime.InteropServices;

namespace Project.ExternalCalls
{

    //External imports class
    public class ExternalMethods : IDisposable
    {
        private delegate int MyExternalMethod(Int32 handle);
        private MyExternalMethod _myMethod;


        private IntPtr _libraryHandle;

        public ExternalImports()
        {
            _libraryHandle = LoadLibrary(@"myClibrary.dll");
            _myMethod = (MyExternalMethod)LoadExternalFunction<MyExternalMethod>(@"MyMethod");
        }
        public int MyInternalWrapper(Int32 handle)
        {
            return _myMethod(handle);
        }

        private Delegate LoadExternalFunction<T>(string functionName)
        where T : class
        {
            try
            {


                IntPtr functionPointer = GetProcaddress(_libraryHandle, functionName);
                if (functionPointer == IntPtr.Zero)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                var result = Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(T));
                return result;
            }
            catch (e)
            {
                throw e;
            }
        }
        ~ExternalImports(){
            Dispose(false);
        }
        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            try
            {
                if(disposing)
                {
                    _myMethod = null;
                }
                if(_libraryHandle != IntPtr.Zero)
                {
                    if(!FreeLibrary(_libraryHandle))
                    {
                        Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
                    }
                    _libraryHandle = IntPtr.Zero;
                }
            }
            catch(e){
                throw e;
            }
        }
        [DllImport("kernel32.dll", setLastError = true)]
        internal extern static IntPtr LoadLibrary(string libraryName);
        [DllImport("kernel32.dll", setLastError = true)]
        internal extern static IntPtr FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll", setLastError = true)]
        internal extern IntPtr GetProcaddress(IntPtr hModule, string procName);

    }

}