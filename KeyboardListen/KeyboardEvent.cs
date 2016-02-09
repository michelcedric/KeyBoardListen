using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardListen
{
    public class KeyboardEvent
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static string _key = string.Empty;
        private static int? _processID = null;

        private static IntPtr hookId = IntPtr.Zero;
        private delegate IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProcedure procedure = HookCallback;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProcedure lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        /// <summary>
        /// 
        /// </summary>
        public void StartListen()
        {
            hookId = SetHook(procedure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public KeyboardEvent(string key)
        {
            _key = key;
            StartListen();
        }

        public KeyboardEvent(string key, int processID)
        {
            _key = key;
            _processID = processID;
            StartListen();
        }

        /// <summary>
        /// 
        /// </summary>
        public KeyboardEvent()
        {
            StartListen();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopListen()
        {
            UnhookWindowsHookEx(hookId);
        }

        /// <summary>
        /// 
        /// </summary>
        ~KeyboardEvent()
        {
            StopListen();
        }

        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler KeyEvent;

        private static bool IsActifProcess()
       {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process process = Process.GetProcessById((int)pid);
            return process.Id == _processID;
        }

        #region PRIVATES

        private IntPtr SetHook(HookProcedure procedure)
        {
            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule)
                return SetWindowsHookEx(WH_KEYBOARD_LL, procedure, GetModuleHandle(module.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int pointerCode = Marshal.ReadInt32(lParam);
                string pressedKey = ((Keys)pointerCode).ToString();


                if (_processID != null)
                {
                    if (IsActifProcess())
                    {
                        RaiseEvent(pressedKey);
                    }
                }
                else
                {
                    RaiseEvent(pressedKey);
                }

            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private static void RaiseEvent(string pressedKey)
        {
            if (string.IsNullOrEmpty(_key))
            {
                KeyEvent(pressedKey, null);
            }
            if (string.Compare(_key, pressedKey, true) == 0)
            {
                KeyEvent(_key, null);
            }
        }

        #endregion
    }
}

