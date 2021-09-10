using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FancyCSharp;

namespace FancyUnity
{
    /*
    public class CodeNode<T> : AWorkNode where T:CustomYieldInstruction
    {
        protected bool codeStarted;
        protected Action<CodeNode<T>> workCode;

        public CodeNode(Action<CodeNode<T>> workCode)
        {
            this.workCode = workCode;
        }

        public override bool keepWaiting
        {
            get
            {
                if (!codeStarted)
                {
                    workCode?.Invoke(Before as CodeNode<T>);
                    codeStarted = true;
                }
                if (!waiting)
                {
                    if (After != null)
                    {
                        ChainRunner.Inst.StartCoroutine(After);
                    }
                    waiting = false;
                }
                return waiting;
            }
        }

        public override void Reset()
        {
            base.Reset();
            codeStarted = false;
            waiting = true;
        }

    }*/

    public class CodeNode : AWorkNode
    {
        protected bool codeStarted;
        protected Action<CodeNode> workCode;

        public CodeNode(Action<CodeNode> workCode)
        {
            this.workCode = workCode;
            new WaitForSeconds(3f);
        }



        public override bool keepWaiting
        {
            get
            {
                if (!codeStarted)
                {
                    workCode?.Invoke(this);
                    codeStarted = true;
                }
                if (!waiting)
                {
                    if (After != null)
                    {
                        ChainRunner.Inst.StartCoroutine(After);
                    }
                    waiting = false;
                }
                return waiting;
            }
        }

        public override void Reset()
        {
            base.Reset();
            codeStarted = false;
            waiting = true;
        }

    }
}
