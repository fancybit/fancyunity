using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FancyCSharp;

namespace FancyUnity
{
    public class OriginNode : AWorkNode
    {
        public bool Waitting = true;

        public CustomYieldInstruction Instrument;


        public OriginNode(CustomYieldInstruction instrument)
        {
            Instrument = instrument;
        }

        public override bool keepWaiting => Instrument.keepWaiting;

    }
}
