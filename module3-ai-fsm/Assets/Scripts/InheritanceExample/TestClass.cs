using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClass : MonoBehaviour
{
    public int foo = 5;
    public float bar = 1.34f;

    public TestClass()
    {
        foo = 0;
    }

    public TestClass(int _foo)
    {
        foo = _foo;
    }
}
