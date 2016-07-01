using UnityEngine;
using System.Collections;

public class Pair<A, B>{
    public A left;
    public B right;

    public Pair(A _a, B _b)
    {
        left = _a;
        right = _b;
    }
}
