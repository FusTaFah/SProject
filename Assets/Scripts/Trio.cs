using UnityEngine;
using System.Collections;

//generic class to hold 3 values of any kind
public class Trio<A, B, C>{
    //left value
    public A left;
    //middle value
    public B middle;
    //right value
    public C right;

    //constructor for the Trio class
    public Trio(A _left, B _middle, C _right)
    {
        //initialise the left value
        left = _left;
        //initialise the middle value
        middle = _middle;
        //initialise the right value
        right = _right;
    }
}
