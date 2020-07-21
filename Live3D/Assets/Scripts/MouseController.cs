using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

[StructLayout(LayoutKind.Sequential)]
public class POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
public class MouseHookStruct
{
    public POINT pt;
    public int hwnd;
    public int wHitTestCode;
    public int dwExtraInfo;
}

public class MouseController : MonoBehaviour
{
    //截获按钮
    private const int WH_KEYBOARD_LL = 14;
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;

    private static LowLevelMouseProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static Dictionary<KeyCode, bool> keyDownDic = new Dictionary<KeyCode, bool>();

    private static Vector3 lastMousePos = Vector3.zero;
    public static Vector3 mouseDeltMove = Vector3.zero;
    private static bool first = true;
    private static bool haveMouseEvent;

    // Use this for initialization
    void Start()
    {
        _hookID = SetHook(_proc);
    }

    void OnApplicationQuit()
    {
        UnhookWindowsHookEx(_hookID);
    }
    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        MPlayerInput.Single.MouseEventCallBack();

        if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            MPlayerInput.Single.MouseClickCallBack();
            MPlayerInput.Single.MouseLeftClickCallBack();
        }
        if (nCode >= 0 && wParam == (IntPtr)WM_RBUTTONDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            MPlayerInput.Single.MouseClickCallBack();
            MPlayerInput.Single.MouseRightClickCallBack();
        }

        if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONUP)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            MPlayerInput.Single.MouseReleaseCallBack();
            MPlayerInput.Single.MouseLeftRelaseCallBack();
        }
        if (nCode >= 0 && wParam == (IntPtr)WM_RBUTTONUP)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            MPlayerInput.Single.MouseReleaseCallBack();
            MPlayerInput.Single.MouseRightRelease();
        }

        if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEMOVE)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            MouseHookStruct M_MouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            Vector3 curMousePos = new Vector3(-M_MouseHookStruct.pt.x, M_MouseHookStruct.pt.y, 0f);
            if (first)
            {
                lastMousePos = curMousePos;
                first = false;
            }
            mouseDeltMove = lastMousePos - curMousePos;
            MPlayerInput.Single.MouseMoveCallBack(mouseDeltMove);
            lastMousePos = curMousePos;
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
