using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FancyUnity;
using UnityEngine.Networking;

public class ChainRunnerTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void test1()
    {

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator WorkTest()
    {
        var chian = new CodeNode((work) =>
        {
            Debug.Log("²âÊÔÊä³ö");
            work.Done();
        })
        .Then(new WWW("https://www.baidu.com/"))
        .Then(work => {
            Debug.Log(work.LastInstrument<WWW>().text);
            work.Done();
        })
        .Start();
        yield return chian;
    }
}
