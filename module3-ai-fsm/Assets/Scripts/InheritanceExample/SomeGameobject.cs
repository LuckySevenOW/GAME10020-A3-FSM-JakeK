using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeGameobject : MonoBehaviour
{
    TestClass test;
    void Start()
    {
        // this USES a constructor
        // test = new TestClass(10);

        test = new TestClass();
        test.foo = 10;

        //Vector2 a = new Vector2();
    }

}
