using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void KeyDownCallBack(KeyCode key);
public delegate void KeyUpCallBack(KeyCode key);

public class MPlayerInput : MonoBehaviour
{
    public static MPlayerInput Single;

    private Dictionary<KeyCode, Action> keyDownDic = new Dictionary<KeyCode, Action>();
    private Dictionary<KeyCode, Action> keyUpDic = new Dictionary<KeyCode, Action>();
    private List<KeyDownCallBack> keyDownSelfKeyList = new List<KeyDownCallBack>();
    private List<KeyUpCallBack> keyUpSelfKeyList = new List<KeyUpCallBack>();

    private Action<Vector3> mouseMoveList = (movement) => { };
    private Action mouseEventCall = () => { };
    private Action mouseClickCall = () => { };
    private Action mouseReleaseCall = () => { };
    private Action mouseLeftClickCall = () => { };
    private Action mouseRightClickCall = () => { };
    private Action mouseLeftReleaseCall = () => { };
    private Action mouseRightReleaseCall = () => { };

    private void Awake()
    {
        if(Single != null)
        {
            Destroy(Single.gameObject);
        }

        Single = this;
    }

    public void RegisterMouseMoveCallBack(Action<Vector3> callBack)
    {
        mouseMoveList += callBack;
    }
    public void RegisterMouseEventCallBack(Action callBack)
    {
        mouseEventCall += callBack;
    }
    public void RegisterMouseClickCallBack(Action callBack)
    {
        mouseClickCall += callBack;
    }
    public void RegisterMouseRelaeaseCallBack(Action callBack)
    {
        mouseReleaseCall += callBack;
    }

    public void MouseMoveCallBack(Vector3 movement)
    {
        mouseMoveList.Invoke(movement);
    }
    public void MouseEventCallBack()
    {
        mouseEventCall.Invoke();
    }
    public void MouseClickCallBack()
    {
        mouseClickCall.Invoke();
    }
    public void MouseReleaseCallBack()
    {
        mouseReleaseCall.Invoke();
    }
    public void RegisterMouseLeftClick(Action c)
    {
        mouseLeftClickCall += c;
    }
    public void MouseLeftClickCallBack()
    {
        mouseLeftClickCall.Invoke();
    }
    public void RegisterMouseRightClick(Action c)
    {
        mouseRightClickCall += c;
    }
    public void MouseRightClickCallBack()
    {
        mouseRightClickCall.Invoke();
    }
    public void RegisterMouseLeftRelease(Action c)
    {
        mouseLeftReleaseCall += c;
    }
    public void MouseLeftRelaseCallBack()
    {
        mouseLeftReleaseCall.Invoke();
    }
    public void RegisterMouseRightRelase(Action c)
    {
        mouseRightReleaseCall += c;
    }
    public void MouseRightRelease()
    {
        mouseRightReleaseCall.Invoke();
    }

    public void RegisterKeyDownCallBack(KeyCode key, Action callBack)
    {
        if (!keyDownDic.ContainsKey(key)) keyDownDic[key] = callBack; 
        else keyDownDic[key] += callBack;
    }
    public void RegisterKeyDownCallBackSelfKey(KeyDownCallBack callBack)
    {
        keyDownSelfKeyList.Add(callBack);
    }
    public void RegisterKeyUpCallBack(KeyCode key, Action callBack)
    {
        if (!keyUpDic.ContainsKey(key)) keyUpDic[key] = callBack;
        else keyUpDic[key] += callBack;
    }
    public void RegisterKeyUpCallBackSelfKey(KeyUpCallBack callBack)
    {
        keyUpSelfKeyList.Add(callBack);
    }

    public void KeyDownCallBack(KeyCode key)
    {
        if(keyDownDic.ContainsKey(key)) keyDownDic[key].Invoke();
        KeyDownCallBackSelfKey(key);
    }
    public void KeyUpCallBack(KeyCode key)
    {
        if (keyUpDic.ContainsKey(key)) keyUpDic[key].Invoke();
        KeyUpCallBackSelfKey(key);
    }
    private void KeyDownCallBackSelfKey(KeyCode key)
    {
        for(int i = 0; i < keyDownSelfKeyList.Count; ++i)
        {
            keyDownSelfKeyList[i].Invoke(key);
        }
    }
    private void KeyUpCallBackSelfKey(KeyCode key)
    {
        for (int i = 0; i < keyUpSelfKeyList.Count; ++i)
        {
            keyUpSelfKeyList[i].Invoke(key);
        }
    }
}
