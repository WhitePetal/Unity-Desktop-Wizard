using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class KeyBoard : MonoBehaviour
{
    private VisualEffect vfx;

    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    public void EnterKeyBoard()
    {
        vfx.Play();
    }

    public void ReleaseKeyBoard()
    {
        vfx.Stop();
    }
}
