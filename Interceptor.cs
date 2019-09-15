// <copyright file="Interceptor.cs" company="N/A">
// Copyright (c) Simon Wendel. All rights reserved.
// </copyright>

namespace ComboInterceptor
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using static ComboInterceptor.NativeMethods;

#pragma warning disable SA1305 // win32 calls ahead, disable SA1305 - Field names should not use Hungarian notation

    public class Interceptor
    {
#pragma warning disable SA1310 // win32-like naming, disable SA1310 - Field names should not contain underscore

        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

#pragma warning restore SA1310 // Field names should not contain underscore

        private readonly WinHookCallback processCallback;

        private IntPtr hookId = IntPtr.Zero;
        private bool modifierHeld;
        private int modifier;

        private int key;

        public Interceptor()
        {
            processCallback = HookCallback;
        }

        public event EventHandler KeyComboIntercepted;

        public void Start(int modifier, int key)
        {
            this.modifier = modifier;
            this.key = key;
            hookId = SetHook(processCallback);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(hookId);
        }

        private static IntPtr SetHook(WinHookCallback callback)
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                var name = module.ModuleName;
                var handle = GetModuleHandle(name);
                return SetWindowsHookEx(WH_KEYBOARD_LL, callback, handle, 0);
            }
        }

        private static int GetKeyCode(IntPtr lParam)
        {
            return Marshal.ReadInt32(lParam);
        }

        private static bool IsValid(int nCode)
        {
            return nCode >= 0;
        }

        private static bool IsKeyDown(IntPtr wParam)
        {
            return wParam == (IntPtr)WM_KEYDOWN;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var keyCode = GetKeyCode(lParam);
            if (IsValid(nCode) && modifierHeld && keyCode == key && IsKeyDown(wParam))
            {
                Task.Run(() => KeyComboIntercepted?.Invoke(this, EventArgs.Empty));
                return Break();
            }

            if (IsValid(nCode) && keyCode == modifier)
            {
                modifierHeld = IsKeyDown(wParam);
            }

            return Continue(nCode, wParam, lParam);
        }

        private IntPtr Continue(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private IntPtr Break()
        {
            return (IntPtr)1;
        }

#pragma warning restore SA1305
    }
}
