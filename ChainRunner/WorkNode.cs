using System;
using UnityEngine;

namespace FancyUnity
{
    public abstract class AWorkNode:CustomYieldInstruction
    {
        public AWorkNode Before;
        public AWorkNode After;
        protected bool waiting;

        public override bool keepWaiting => waiting;

        public T LastInstrument<T>() where T : CustomYieldInstruction
        {
            return (Before as OriginNode).Instrument as T;
        }

        public virtual AWorkNode Then<T>(T instru) where T : CustomYieldInstruction
        {
            var result = new OriginNode(instru);
            result.Before = this;
            this.After = result;
            return result;
        }

        public virtual AWorkNode Then(Action<CodeNode> workCode)
        {
            var result = new CodeNode(workCode);
            result.Before = this;
            this.After = result;
            return result;
        }

        public virtual AWorkNode Start()
        {
            var startPoint = this;
            while (startPoint.Before != null) startPoint = startPoint.Before;
            ChainRunner.Inst.StartCoroutine(startPoint);
            return this;
        }

        public virtual void Done()
        {
            waiting = false;
        }
    }
}