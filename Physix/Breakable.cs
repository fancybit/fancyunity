using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FancyUnity
{
    [AddComponentMenu("FancyBit/Unity")]
    [RequireComponent(typeof(UnityEngine.U2D.IK.FabrikSolver2D))]
    public class Breakable : MonoBehaviour
    {

        private UnityEngine.U2D.IK.FabrikSolver2D _solver;
        private Floatable _ctrlPt;
        private FixedJoint2D _joint;

        // Start is called before the first frame update
        void Start()
        {
            _solver = GetComponent<UnityEngine.U2D.IK.FabrikSolver2D>();
            var chain = _solver.GetChain(0);
            var boneCount = chain.lengths.Length;
            var bone = chain.effector;
            for (var i = 0; i < boneCount; ++i)
            {
                bone = bone.parent;
            }
            //_joint.
            _ctrlPt = _solver.GetChain(0).target.GetComponent<Floatable>();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}