using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class KeyCodeController : MonoBehaviour
{
    //截获按钮
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static Dictionary<KeyCode, bool> keyDownDic = new Dictionary<KeyCode, bool>();

    // Use this for initialization
    void Start()
    {
        _hookID = SetHook(_proc);
    }
    void OnApplicationQuit()
    {
        UnhookWindowsHookEx(_hookID);
    }
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {

        int vkCode = Marshal.ReadInt32(lParam);
        KeyCode key = KeyCode.None;
        try
        {
            key = (KeyCode)(vkCode + 32);
        }
        catch { }

        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            
            key = (KeyCode)(vkCode + 32);
            if(!keyDownDic.ContainsKey(key) || keyDownDic[key] == false)
            {
                MPlayerInput.Single.KeyDownCallBack(key);
                keyDownDic[key] = true;
            }
        }

        if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
        {
            key = (KeyCode)(vkCode + 32);
            keyDownDic[key] = false;
            MPlayerInput.Single.KeyUpCallBack(key);
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
