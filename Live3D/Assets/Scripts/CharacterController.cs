using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // 存储所有键盘按键位置的根物体
    // 所有的键盘按键都作为该物体的子物体
    [SerializeField] private Transform keyBoardRoot = null;

    // 存储 按键名 和 KeyBoard对象 构成的字典表
    // 每个按键物体身上都会挂载一个 KeyBoard 对象
    private Dictionary<string, KeyBoard> keyBoadDic = new Dictionary<string, KeyBoard>();
    // 角色的 animator
    private Animator animator;
    // PlayeInput
    private MPlayerInput playerInput;

    // 当前手掌要移动到的位置
    private Transform target;
    // 手掌的引用
    private Transform rightHand;
    private Rigidbody rightHandRig;


    private void Awake()
    {
        // 初始化，获取所有按键物体身上的 KeyBoadr 脚本
        KeyBoard[] keyBoards = keyBoardRoot.GetComponentsInChildren<KeyBoard>();
        // 初始化，建表
        for (int i = 0; i < keyBoards.Length; i++)
        {
            keyBoadDic.Add(keyBoards[i].name, keyBoards[i]);
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
        // 通过 ToString 获取键盘名
        // 待优化：ToString 会产生 GC，虽然对于桌面精灵这样的小程序来说没什么大问题
        // 可以在 字典初始化时使用 Enum.Parse 来把字符串转成枚举
        // 这样在这里就不用 ToString 了
        // 话说我有时间在这里写这些注释，干嘛不直接改了。 唉，就是这么懒
        string keyStr = key.ToString();
        // 安全判断
        if (keyBoadDic.ContainsKey(keyStr))
        {
            SetTarget(keyBoadDic[keyStr].transform); // 设置当前的手掌移动目标
            keyBoadDic[keyStr].EnterKeyBoard(); // 调用对应按键的 KeyBoard 的 按下键盘方法(这么做是为了解耦，键盘就做键盘的事，角色就做角色的事)
        }
    }

    // 键盘抬起时的回调
    private void ReleaseKeyBoard(KeyCode key)
    {
        // ToString 获取键盘名
        // 与上面一样，可以进行优化
        string keyStr = key.ToString();
        // 安全判断
        if(keyBoadDic.ContainsKey(keyStr)) keyBoadDic[keyStr].ReleaseKeyBoard(); // 调用对应按键的方法
    }

    // LateUpdate 中负责执行对于骨骼控制的代码
    private void LateUpdate()
    {
        // 安全判断，如果 target == null 就没必要移动骨骼
        if (target == null) return;
        // 计算 目标按键 指向 手掌的向量
        Vector3 t = rightHand.position - target.position;

        // 因为我们采用的是布娃娃系统来控制骨骼的运动
        // 所以这里需要通过刚体来控制手掌的移动，让手掌朝着目标进行移动
        // 这里采用设置速度(Velocity) 的方式
        // 大家也可以试着用 AddForce 的方式
        rightHandRig.velocity = (target.position - rightHand.position) * 200f * Time.fixedDeltaTime;
        // 将手掌缓动旋转至手掌朝向目标
        rightHand.right = Vector3.Slerp(rightHand.right, t.normalized, Time.fixedDeltaTime);

    }

    private void OnAnimatorMove()
    {

    }

}
