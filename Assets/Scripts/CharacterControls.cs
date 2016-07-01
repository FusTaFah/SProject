using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterControls : MonoBehaviour {

    /*
     * This character controller contains code from a tutorial by Sebastian Lague (2014)
     * "[Unity Tutorial] First Person Controller: Spherical Worlds".
     * https://github.com/SebLague/Spherical-Gravity/blob/master/FirstPersonController.cs
     * 
     * Also contains code from a tutorial by Quill18Creates (2013)
     * "Simple First-Person Shooter"
     * "http://quill18.com/unity_tutorials/FPS Tutorial.zip"
     */

    //reference to the player's rigidbody
    Rigidbody player;
    //reference to the central gravity body in the world
	GravChange gravity;
    //publically changeable movement speed of the player
	public float walkSpeed;
    //publically changeable bullet firing speed
	public float bulletSpeed;
    //firing delay timer
    float timer;
    //firing switch; false if unable to fire
    bool canFire;
    //the current projectile type being fired
    int firingState;
    //player's health
    int health;
    //reference to the camera attached to the player
    Camera cam;
    //reference to the bullet manager for firing
    GameObject bulletManager;
    //reference to debugger
    DebugUtil m_debugger;
    //rocket fuel gague
    float m_fuelGague;
    //boolean to track whether or not the player is flying
    bool m_flying;
    //Respawn manager
    RespawnManagerScript rManager;

	// Use this for initialization
	void Start () {
        //find the bullet manager in the scene
        bulletManager = GameObject.Find("BulletManager");
        //initialise health to 10
        health = 10;
        //initialise timer to 0;
        timer = 0.0f;
        //by default the player can fire
        canFire = true;
		//initialise the player
		player = gameObject.GetComponent<Rigidbody> ();
		// initialise the sphere world
		gravity = GameObject.FindGameObjectWithTag ("Sphere").GetComponent<GravChange> ();
        //disable use of unity's gravity for the player
        player.useGravity = false;
        //freexe rotation for this game object
		player.constraints = RigidbodyConstraints.FreezeRotation;
        //stop the mouse pointer from moving
        Cursor.lockState = CursorLockMode.Locked;
        //make the mouse cursor invisible
        Cursor.visible = false;
        //initialise the camera to the main scene camera
        cam = Camera.main;
        //initialise debugger by finding it in the scene
        m_debugger = GameObject.Find("Canvas").GetComponentInChildren<DebugUtil>();
        //initialise fuel gague
        m_fuelGague = 5.0f;
        //player is not flying by default
        m_flying = false;
        //initialise the respawn manager
        rManager = GameObject.Find("RespawnManager").GetComponent<RespawnManagerScript>();
	}
    
    //method to return the player's position
	Vector3 getPosition(){
        //return this gameobject's transform's position
		return gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        //refill the fuel gague if it isn't full
        if (m_fuelGague < 5.0f && !m_flying)
        {
            m_fuelGague += Time.deltaTime;
        }
        //if the player can fire
		if (canFire) {
            //if the left mouse button is pressed
            if (Input.GetButton("Fire1"))
            {
                //spawn a terrain destroying projectile
                bulletManager.GetComponent<BulletManager>().SpawnBullet(gameObject.transform.position + cam.transform.forward * 2.0f, cam.transform.rotation, cam.transform.forward * bulletSpeed, "Bullet");
                //change the internal firing state to match the fired projectile
                firingState = FIRING_EARTH_BUSTER;
                //disable firing
                canFire = false;
            }
            //if the right mouse button is pressed
            else if (Input.GetButton("Fire2"))
            {
                //spawn a combatant destroying projectile
                bulletManager.GetComponent<BulletManager>().SpawnBullet(gameObject.transform.position + cam.transform.forward * 2.0f, cam.transform.rotation, cam.transform.forward * bulletSpeed * 5.0f, "RifleShot");
                //change the internal firing state to match the fired projectile
                firingState = FIRING_DUDE_KILLER;
                //disable firing
                canFire = false;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                //spawn a world healing projectile
                bulletManager.GetComponent<BulletManager>().SpawnBullet(gameObject.transform.position + cam.transform.forward * 2.0f, cam.transform.rotation, cam.transform.forward * bulletSpeed, "Replenisher");
                //change the internal firing state to match the fired projectile
                firingState = FIRING_DUDE_KILLER;
                //disable firing
                canFire = false;
            }

        }
        //if the player cannot fire
        else if (!canFire)
        {
            //set the firing cooldown according to the last projectile fired
            float cooldown = 0.0f;
            switch (firingState)
            {
                case FIRING_EARTH_BUSTER:
                    cooldown = 0.5f;
                    break;
                case FIRING_DUDE_KILLER:
                    cooldown = 0.1f;
                    break;
                case FIRING_WORLD_HEALER:
                    cooldown = 1.0f;
                    break;
            } 
            //if the timer has exceeded the cooldown
            if (timer >= cooldown)
            {
                //set the ability to fire to true
                canFire = true;
                //reset the firing timer
                timer = 0.0f;
            }
            //else increment the timer
            else
            {
                timer += Time.deltaTime;
            }
        }
        //if the space bar is pressed
        if (Input.GetButtonDown("Jump"))
        {
            //fire a ray downwards for 1.5 world units
            Ray r = new Ray(gameObject.transform.position, -gameObject.transform.up);
            RaycastHit rch;
            //if the ray hits an object, it implies the player is grounded
            if (Physics.Raycast(r, out rch, 1.5f))
            {
                //apply the jumping force to the rigidbody
                player.AddForce(gameObject.transform.up * 10.0f, ForceMode.Impulse);
            }
            //if the player is in mid air
            else
            {
                m_flying = true;

            }
        }
        if (m_flying)
        {
            if (Input.GetButton("Jump"))
            {
                if (m_fuelGague >= 0.0f)
                {
                    Vector3 upThrust = player.velocity.sqrMagnitude <= 1.0f ? new Vector3(0.0f, 0.0f, 0.0f) : gameObject.transform.up * 10.0f / player.velocity.sqrMagnitude;
                    player.AddForce(upThrust, ForceMode.Impulse);
                    m_fuelGague -= Time.deltaTime * 2.0f;
                }
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            m_flying = false;
        }
        
        //obtain the left/right movement key presses
		float xSpeed = Input.GetAxis ("Horizontal");
        //obtain the forwards/backwards movement key presses
		float zSpeed = Input.GetAxis ("Vertical");

        //obtain the X movement of the mouse
		float rotX = Input.GetAxis ("Mouse X");
        //obtain the Y movement of the mouse
		float rotY = Input.GetAxis ("Mouse Y");

        //store the horizontal mouse movement as the player rotation vector
		Vector3 r3 = new Vector3 (0, rotX, 0);
        //set the direction of movement of the player
		Vector3 v3 = new Vector3 (xSpeed, 0, zSpeed).normalized;
        //rotate the player according to the mouse X rotation vector
		gameObject.transform.Rotate (r3);
        //rotate the camera up or down according to the Y mouse movement
		Camera.main.transform.Rotate (new Vector3 (-rotY, 0, 0));
        //move the player in the direction of movement multiplied by the movement speed
		player.MovePosition (player.position + transform.TransformDirection(v3 * walkSpeed) * Time.deltaTime);
        //apply the artificial gravity to this game object
        gravity.Attract(gameObject);
	}

    //when the player collides with a bullet
	void OnCollisionEnter(Collision coll){
        if (coll.collider.gameObject.tag == "Bullet")
        {
            //decrement health by 1
            health = health - 1;
            //if health is less than or equal to 0
            if (health <= 0)
            {
                //reset the player's health
                health = 10;
                rManager.HandleDeath(gameObject);
            }
        }

	}

    public int GetHealth()
    {
        return health;
    }

    public float GetJetpackFuel()
    {
        return m_fuelGague;
    }

    const int NOT_FIRING = 0;
    const int FIRING_EARTH_BUSTER = 1;
    const int FIRING_DUDE_KILLER = 2;
    const int FIRING_WORLD_HEALER = 3;
}