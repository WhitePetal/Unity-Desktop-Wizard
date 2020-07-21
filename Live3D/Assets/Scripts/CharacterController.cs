using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // 存储所有键盘按键位置的根物体
    // 所有的键盘按键都作为该物体的子物体
    [SerializeField] private Transform keyBoardRoot = null;
    // 鼠标
    [SerializeField] private Transform mouse;
    [SerializeField] private float toffset = 0.05f;

    // 存储 按键名 和 KeyBoard对象 构成的字典表
    // 每个按键物体身上都会挂载一个 KeyBoard 对象
    private Dictionary<KeyCode, KeyBoard> keyBoadDic = new Dictionary<KeyCode, KeyBoard>();
    // 角色的 animator
    private Animator animator;
    // PlayeInput
    private MPlayerInput playerInput;

    // 当前手掌要移动到的位置
    private Transform target;
    // 手掌的引用
    private Transform rightHand;
    private Rigidbody rightHandRig;
    private Transform leftHand;
    private Rigidbody leftHandRig;
    private Transform leftLArm;


    private void Awake()
    {
        // 初始化，获取所有按键物体身上的 KeyBoadr 脚本
        KeyBoard[] keyBoards = keyBoardRoot.GetComponentsInChildren<KeyBoard>();
        // 初始化，建表
        for (int i = 0; i < keyBoards.Length; i++)
        {
            keyBoadDic.Add((KeyCode)Enum.Parse(typeof(KeyCode) ,keyBoards[i].name), keyBoards[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 获取 MplayerUnput、Animator 和 右手的 Transform、Rigidbody 的引用
        playerInput = MPlayerInput.Single;
        animator = GetComponent<Animator>();
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        rightHandRig = rightHand.GetComponent<Rigidbody>();
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        leftHandRig = leftHand.GetComponent<Rigidbody>();
        leftLArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);

        // 注册按键按下 和 松开的回调
        playerInput.RegisterKeyDownCallBackSelfKey((key) => EnterKeyBoard(key));
        playerInput.RegisterKeyUpCallBackSelfKey((key) => ReleaseKeyBoard(key));
    }

    // 设置当前手掌要移动到的目标
    // 为什么这么一句话要装成一个函数？
    // 因为这个方法是当时还没构思完整时就写上了，后面也懒得删掉了
    private void SetTarget(Transform target)
    {
        this.target = target;
    }

    // 键盘按下时的回调
    private void EnterKeyBoard(KeyCode key)
    {
        // 安全判断
        if (keyBoadDic.ContainsKey(key))
        {
            SetTarget(keyBoadDic[key].transform); // 设置当前的手掌移动目标
            keyBoadDic[key].EnterKeyBoard(); // 调用对应按键的 KeyBoard 的 按下键盘方法(这么做是为了解耦，键盘就做键盘的事，角色就做角色的事)
        }
    }

    // 键盘抬起时的回调
    private void ReleaseKeyBoard(KeyCode key)
    {
        // 安全判断
        if(keyBoadDic.ContainsKey(key)) keyBoadDic[key].ReleaseKeyBoard(); // 调用对应按键的方法
    }

    // LateUpdate 中负责执行对于骨骼控制的代码
    private void LateUpdate()
    {
        //leftHand.right = leftHand.position - mouse.position;
        Vector3 mouseTarget = mouse.position;
        //mouseTarget.y += 0.08f;
        //leftHand.position = mouseTarget;
        leftHandRig.velocity = ((mouseTarget - leftHand.position) * 50f * Time.fixedDeltaTime);
        //leftLArm.right = -mouseTarget + leftLArm.position;
        //leftHand.forward = -Vector3.up;
        //leftLArm.right = Vector3.Slerp(leftLArm.right, leftLArm.position - mouse.position, 4f * Time.fixedDeltaTime);
        float xangle = Vector3.SignedAngle(leftLArm.forward, -Vector3.up, leftLArm.right);
        //leftLArm.Rotate(leftLArm.forward, xangle);
        leftHand.right =  leftHand.position - mouse.position;
        xangle = Vector3.SignedAngle(leftHand.forward, -Vector3.up, leftHand.right);
        leftHand.Rotate(leftHand.forward, xangle);
        //leftHand.forward = -Vector3.up;
        //Vector3 mtoh = leftHand.position - mouseTarget;
        //mtoh.y = 0;
        //leftHand.right = mtoh;
        // 安全判断，如果 target == null 就没必要移动骨骼
        if (target == null) return;
        // 计算 目标按键 指向 手掌的向量
        Vector3 t = rightHand.position - target.position;
        Vector3 tt = t + t.normalized * toffset;

        // 因为我们采用的是布娃娃系统来控制骨骼的运动
        // 所以这里需要通过刚体来控制手掌的移动，让手掌朝着目标进行移动
        // 这里采用设置速度(Velocity) 的方式
        // 大家也可以试着用 AddForce 的方式
        rightHandRig.velocity = (-tt) * 200f * Time.fixedDeltaTime;

        rightHand.up = Vector3.Slerp(rightHand.up, -Vector3.Cross(t.normalized, Vector3.up), 4f * Time.fixedDeltaTime);
        //Quaternion targetRot = Quaternion.FromToRotation(-rightHand.right, -t);

        //rightHand.rotation = Quaternion.RotateTowards(rightHand.rotation, targetRot, Time.fixedDeltaTime);

    }

    private void OnAnimatorMove()
    {

    }

}
