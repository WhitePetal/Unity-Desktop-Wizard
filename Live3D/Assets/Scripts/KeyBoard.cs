using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class KeyBoard : MonoBehaviour
{
    // 存储粒子系统
    private VisualEffect vfx;

    // Start is called before the first frame update
    void Start()
    {
        // 初始化粒子系统
        vfx = GetComponentInChildren<VisualEffect>();
    }

    // 该键被按下时调用
    public void EnterKeyBoard()
    {
        vfx.Play(); // 开启粒子系统
    }

    // 该键被松开时调用
    public void ReleaseKeyBoard()
    {
        vfx.Stop(); // 关闭粒子系统
    }
}
