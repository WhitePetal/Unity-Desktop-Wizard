using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Transform keyBoardRoot;

    private Dictionary<string, KeyBoard> keyBoadDic = new Dictionary<string, KeyBoard>();
    private Animator animator;
    private MPlayerInput playerInput;

    private Transform target;
    private Transform rightHand;


    private void Awake()
    {
        KeyBoard[] keyBoards = keyBoardRoot.GetComponentsInChildren<KeyBoard>();
        for(int i = 0; i < keyBoards.Length; i++)
        {
            keyBoadDic.Add(keyBoards[i].name, keyBoards[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<MPlayerInput>();
        animator = GetComponent<Animator>();
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        playerInput.RegisterKeyDownCallBackSelfKey((key) => EnterKeyBoard(key));
        playerInput.RegisterKeyUpCallBackSelfKey((key) => ReleaseKeyBoard(key));
    }

    private void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void EnterKeyBoard(KeyCode key)
    {
        string keyStr = key.ToString();
        if (keyBoadDic.ContainsKey(keyStr))
        {
            SetTarget(keyBoadDic[keyStr].transform);
            keyBoadDic[keyStr].EnterKeyBoard();
        }
    }

    private void ReleaseKeyBoard(KeyCode key)
    {
        string keyStr = key.ToString();
        if(keyBoadDic.ContainsKey(keyStr)) keyBoadDic[keyStr].ReleaseKeyBoard();
    }


    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 t = rightHand.position - target.position;

        //rightHand.up = Vector3.Cross(Vector3.up, rightHand.right);
        rightHand.transform.GetComponent<Rigidbody>().velocity = (target.position - rightHand.position) * 200f * Time.fixedDeltaTime;
        //Quaternion rot = Quaternion.FromToRotation(rightHand.right, t);
        //rightHand.rotation = Quaternion.RotateTowards(rightHand.rotation, rot, Time.fixedDeltaTime);
        rightHand.right = Vector3.Slerp(rightHand.right, t.normalized, Time.fixedDeltaTime);
        //rightHand.right = t;

    }

    private void OnAnimatorMove()
    {
        //Transform rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        //rightLowerArm.right = -target.position + rightLowerArm.position;
    }

}
