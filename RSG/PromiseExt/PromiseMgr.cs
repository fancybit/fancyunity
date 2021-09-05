using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;
using UnityEngine.Networking;

namespace FancyUnity
{
    public class PromiseMgr : Singleton<PromiseMgr>
    {
        public UnityPromise<TResult> Do<TWaitter, TResult>(
            TWaitter waitter,
            Func<TWaitter, TResult> callback)
            where TWaitter : YieldInstruction
        {
            var promise = new UnityPromise<TResult>();
            StartCoroutine(RunWaitter(promise, waitter, callback));
            return promise;
        }

        public IEnumerator RunWaitter<TWait, TResult>(
            UnityPromise<TResult> promise,
            TWait waitter,
            Func<TWait, TResult> callback)
        {
            yield return waitter;
            promise.Resolve(callback.Invoke(waitter));
        }

        public void tt(int p)
        {
            var result = Do(new WaitForSeconds(2f), (waitter) =>
            {
                return 111 + "123";
            }).Then(new WWW("www.baidu.com"), (waitter) =>
            {
                Debug.Log(waitter.text);
                return "456";
            });
        }
    }


}

