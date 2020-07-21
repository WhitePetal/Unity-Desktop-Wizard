using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Mouse : MonoBehaviour
{
    [SerializeField] private float xMoveRange = 4f;
    [SerializeField] private float yMoveRange = 4f;
    [SerializeField] private VisualEffect mouseVFX = null;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        MPlayerInput.Single.RegisterMouseMoveCallBack((movement) => MouseMove(movement));
        MPlayerInput.Single.RegisterMouseLeftClick(() => ClickLeft());
        MPlayerInput.Single.RegisterMouseRightClick(() => ClickRight());
    }

    private void MouseMove(Vector3 movement)
    {
        Vector3 move = movement;
        move.z = move.y;
        move.y = 0f;
        Vector3 target = transform.position + move * Time.deltaTime;
        if (Mathf.Abs(target.x - startPos.x) > xMoveRange) move.x = 0f;
        if (Mathf.Abs(target.z - startPos.z) > yMoveRange) move.z = 0f;
        transform.Translate(move * 0.2f * Time.deltaTime, Space.World);
    }

    private void ClickLeft()
    {
        mouseVFX.SendEvent("LeftPlay");
    }
    private void ReleaseLeft()
    {
        mouseVFX.SendEvent("LeftStop");
    }
    private void ClickRight()
    {
        mouseVFX.SendEvent("RightPlay");
    }
    private void ReleaseRight()
    {
        mouseVFX.SendEvent("RightStop");
    }
}
