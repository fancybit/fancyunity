using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Animations;

public class Wind2D : SerializedMonoBehaviour
{
    public float Gravity = 1f;
    public float DirAngle = 0f;
    public float MinAcc = 0.1f;
    public float MaxAcc = 1f;
    public Vector3 Velocity = Vector3.zero;
    public float MaxVelocity = 5f;
    public float Damping = 1f;

    private float _maxCtrlBoneDist = 3f;
    private IK.FabrikSolver2D _solver;
    private Transform _tailBone;
    private Transform _ctrlPoint;

    // Start is called before the first frame update
    void Start()
    {
        _solver = GetComponent<UnityEngine.U2D.IK.FabrikSolver2D>();
        var chain = _solver.GetChain(0);
        _tailBone = chain.effector;
        _ctrlPoint = chain.target;
        _maxCtrlBoneDist = (_tailBone.position - _ctrlPoint.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        //取位置
        var pos = _ctrlPoint.position;
        var bonePos = _tailBone.position;
        //限制控制点运动范围
        var dist = pos - bonePos;
        if (dist.magnitude > _maxCtrlBoneDist)
        {
            Velocity = -dist.normalized;
        }

        //运动
        var acc = Vector3.down;
        acc = acc + Matrix4x4.Rotate(Quaternion.Euler(0,0,DirAngle)).MultiplyVector(Vector3.right);
        acc.Normalize();
        acc *= Random.Range(MinAcc, MaxAcc);
        Velocity += acc;
        Velocity -= Velocity.normalized * Damping;
        if(Velocity.magnitude> MaxVelocity)
        {
            Velocity.Normalize();
            Velocity *= MaxVelocity;
        }
        pos += Velocity;
        //更新位置
        _ctrlPoint.position = pos;
    }
}
