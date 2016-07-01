using UnityEngine;
using System.Collections;

public class BulletSpan : MonoBehaviour {

    //variable to keep track of how long this bullet has been fired for.
    float timer;
    //boolean to say whether or not this bullet is currently in flight
    bool beingUsed;
    //the type of bullet, either "RifleShot" or "Bullet" 
    string bulletType;

    //public set and get methods for timer
    public float Timer { get { return timer; } set { timer = value; } }
    //public set and get methods for beingUsed
    public bool BeingUsed { get { return beingUsed; } set { beingUsed = value; } }
    //public set and get methods for bulletType
    public string BulletType { get { return bulletType; } set { bulletType = value; } }

    // Use this for initialization
    void Start()
    {
        //initialise bulletType to empty string
        bulletType = "";
        //initialised beingUsed to true
        beingUsed = true;
        //initialise timer to 0
        timer = 0.0f;
    }

    void Update()
    {
        //if the bullet is in flight
        if (beingUsed)
        {
            //increase the time the bullet has been flying for
            timer += Time.deltaTime;
            //if the time the bullet has been flying for is greater than 3 seconds
            if (timer >= 3.0f)
            {
                //flag this bullet for removal by the manager
                beingUsed = false;
                //reset the timer
                timer = 0.0f;
            }
        }
    }

    //when the bullet collides with any collision object
    void OnCollisionEnter(Collision coll)
    {
        //flag this bullet for removal by the manager
        beingUsed = false;
        //reset the timer
        timer = 0.0f;
        
    }
}
