using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FancyUnity;
using UnityEngine.Networking;

public class PromiseTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void DoThen()
    {
        // Use the Assert class to test conditions
        var proMgr = PromiseMgr.Inst;
        var result = proMgr.Do(new WaitForSeconds(3f), (waitter) =>
        {
            Debug.Log(123);
            return 0;
        }).Then(new WaitForSeconds(1f), (waitter) =>
        {
            Debug.Log(456);
            return 0;
        });
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PromiseTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
