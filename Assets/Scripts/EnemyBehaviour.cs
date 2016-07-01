using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour {
    //publically mutable movement speed of the AI
	public float walkSpeed;
    //spherical gravity reference
	GravChange gravity;
    //a reference to the AI's rigidbody
	Rigidbody thisbody;
    //the currently targetted enemy
	GameObject targetEnemy;
    //list of all enemies of this AI in the scene
    List<GameObject> enemies;
    //the currently targetted voxel
    GameObject target;
    //boolean to keep track of whether or not an enemy combatant is detected
    bool isDetected;
    //firing switch, if true, the AI can fire
    bool canFire;
    //timer to delay main searches as part of behaviour
    float timer;
    //timer to delay firing
    float firingTimer;
    //boolean to keep track of whether or not a target voxel is found
    bool foundTarget;
    //health of this AI
    int health;
    //the current projectile the AI is firing
    int firingState;
    //bullet manager for firing projectiles
    GameObject bulletManager;
    //boolean to track whether or not it is safe to fire
    bool safeToFire;
    //Respawn manager script which handles respawns
    RespawnManagerScript rManager;

    //walking direction
    Vector3 direction;
    //direction the AI is set to fire in
    Vector3 firingDirection;
    // A vector going right of the enemy
    Vector3 m_X;
    // A vector going Above the enemy
    Vector3 m_Y;
    // A Forward facing vector from the enemy
    Vector3 m_Z; 

	// Use this for initialization
	void Start () {
        //find the bullet manager in the scene and make a reference to it
        bulletManager = GameObject.Find("BulletManager");
        //initialise the list of enemies to be empty
        enemies = new List<GameObject>();
        //set the AI health to 10
        health = 10;
        //by default the AI cannot fire
        canFire = false;
        //make a reference to the AI's rigidbody
		thisbody = gameObject.GetComponent<Rigidbody> ();
        //obtain the gravity body from the scene
		gravity = GameObject.FindGameObjectWithTag ("Sphere").GetComponent<GravChange> ();
		//depending on what the AI's team is, identify the enemy combatant
        string enemyNameTag = gameObject.tag == "Enemy" ? "PlayerTeam" : "Enemy";
        //fill the enemy list with enemy combatants
        foreach (GameObject e in GameObject.FindGameObjectsWithTag(enemyNameTag))
        {
            enemies.Add(e);
        }
        //if this AI opposes the player, add the player to the list of enemy combatants
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && gameObject.tag == "Enemy")
        {
            enemies.Add(player);
        }
        //disable Unity's default gravity
        thisbody.useGravity = false;
        //diable default rotations due to physics
        thisbody.constraints = RigidbodyConstraints.FreezeRotation;
        //detection of enemy opponents is by default set to false
		isDetected = false;
        //set behavioural delay timer to 0;
        timer = 0.0f;
        //set firing timer to 0;
        firingTimer = 0.0f;
        //initialise the firing direction to an empty vector
        firingDirection = new Vector3();
        //initialise the walking direction to an empty vector
        direction = new Vector3();
        //by default, the AI cannot find any enemies
        foundTarget = false;
        //do not begin firing until it is safe to do so
        safeToFire = false;
        //initialise the respawn manager
        rManager = GameObject.Find("RespawnManager").GetComponent<RespawnManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
        //get local x, y and z directions
        m_Y = gameObject.transform.up.normalized;
        m_Z = gameObject.transform.forward.normalized;
        m_X = Vector3.Cross(m_Z, m_Y).normalized;
        //verify if an enemy opponent can be seen
        IsDetected();
        //update enemy state every 300 millis
        if (timer >= 0.3f)
        {
            //reset the opponent's collider if it passes above the maximum surface area of 32
            //world units from the centre
            if ((gravity.transform.position - gameObject.transform.position).magnitude >= 32.0f)
            {
                gameObject.GetComponent<CapsuleCollider>().enabled = true;
            }
            
            if (isDetected)
            {
                //set the direction of movement to be towards the currently targeted enemy
                direction = (targetEnemy.transform.position - gameObject.transform.position).normalized;
                //set the firing direction to be towards the currently targeted enemy
                firingDirection = direction;
                //use the cross product to find the direction towards the player such that it is
                //tangential to the core sphere surface
                Vector3 b = Vector3.Cross(gameObject.transform.up, direction).normalized;
                Vector3 a = Vector3.Cross(b, gameObject.transform.up).normalized;
                //set the movement direction to this tangential direction
                direction = a;
                //face the current combatant towards this direction
                gameObject.transform.forward = direction;


            }
            //if the combatant can't see an enemy, find a voxel to destroy
            else
            {
                //if the combatant hasn't yet found a target voxel and the one it is firing at isn't active
                if (!foundTarget /*&& target.GetComponent<Voxel>().State != 1*/)
                {
                    //find a voxel to fire at
                    FindNextTarget();
                }
                //else if the target voxel isn't active, a voxel hasn't been found
                else
                {
                    if (target.GetComponent<Voxel>().State != 1)
                    {
                        foundTarget = false;
                    }
                }

                //temp is the position of the target voxel
                Vector3 temp = target.transform.position + new Vector3(0.5f, 0.5f, 0.5f);
                //set the movement direction to be towards the target voxel
                direction = (temp - gameObject.transform.position).normalized;
                //toObjective is a ray from the combatant's origin to the direction of the voxel
                Ray toObjective = new Ray(gameObject.transform.position, direction);
                //use the cross product to find the direction towards the player such that it is
                //tangential to the core sphere surface
                Vector3 b = Vector3.Cross(gameObject.transform.up, direction).normalized;
                Vector3 a = Vector3.Cross(b, gameObject.transform.up).normalized;
                //set the movement direction to this tangential direction
                direction = a;
                //face the current combatant towards this direction
                gameObject.transform.forward = direction;
                //prepare a raycast going towards the target voxel
                RaycastHit rchee;
                Physics.Raycast(toObjective, out rchee);
                
                //if the combatant is on the player team, declare it as the new target
                if (gameObject.tag == "PlayerTeam" ^ rchee.collider.gameObject.GetComponent<Voxel>().OnPlayerTeam)
                {
                    firingDirection = toObjective.direction;
                    safeToFire = true;
                }
                else
                {
                    safeToFire = false;
                }
            }
            //prepare a raycast
            RaycastHit rch;
            //if the AI is behind an obstacle that it cannot walk over
            Ray objectInfront = new Ray(gameObject.transform.position - m_Y, m_Z);
            if (Physics.Raycast(objectInfront, out rch, 1.5f))
            {
                //jump over it
                thisbody.AddForce(gameObject.transform.up * 10.0f, ForceMode.Impulse);
            }

            //if the AI is on top of the main gravity body
            Ray objectAbove = new Ray(gameObject.transform.position, -m_Y);
            if (Physics.Raycast(objectAbove, out rch, 1.5f))
            {
                if (rch.collider.gameObject.tag == "Sphere")
                {
                    //perform a super jump that returns the AI to play by phasing through
                    //the game world
                    gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    thisbody.AddForce(gameObject.transform.up * 10.0f, ForceMode.Impulse);
                }
            }
            timer = 0.0f;
        }
        //else increment the behavioural delay timer
        else
        {
            timer += Time.deltaTime;
        }

        //firing control mechanism
        if (canFire)
        {
            //if an enemy combatant is not yet detected
            if (!isDetected && safeToFire)
            {
                //fire a round that can harm the terrain
                bulletManager.GetComponent<BulletManager>().SpawnBullet(gameObject.transform.position + gameObject.transform.forward.normalized, Quaternion.LookRotation(firingDirection), firingDirection.normalized * 0.01f, "Bullet");
                //change the firing state for cooldowns
                firingState = FIRING_EARTH_BUSTER;
                //disable firing
                canFire = false;
            }
            //else if an enemy combatant has been detected
            else if (isDetected)
            {
                //fire a round that can harm the combatant
                bulletManager.GetComponent<BulletManager>().SpawnBullet(gameObject.transform.position + gameObject.transform.forward.normalized, Quaternion.LookRotation(firingDirection), firingDirection.normalized * 0.01f * 5.0f, "RifleShot");
                //change the firing state for cooldowns
                firingState = FIRING_DUDE_KILLER;
                //disable firing
                canFire = false;
            }

        }
        //if the AI cannot fire
        else
        {
            //set the cooldown time
            float cooldown = 0.0f;
            //if the last round fired was a terrain destroying projectie, the time is 0.3 seconds
            if (firingState == FIRING_EARTH_BUSTER) cooldown = 0.3f;
            //else if the last round fired was a combatant destroying projectie, the time is 0.1 seconds
            else if (firingState == FIRING_DUDE_KILLER) cooldown = 0.1f;

            //if the firing timer has exceeded the cooldown
            if (firingTimer >= cooldown)
            {
                //enable firing
                canFire = true;
                //reset the firing cooldown timer
                firingTimer = 0.0f;
            }
            //if the firing timer hasn't exceeded cooldown
            else
            {
                //increment the timer
                firingTimer += Time.deltaTime;
            }
        }
        //move the AI in the direction of movement multiplied by the movement speed
        thisbody.MovePosition(thisbody.position + gameObject.transform.TransformDirection(direction * walkSpeed) * Time.deltaTime); //transform.TransformDirection(gameObject.transform.forward * walkSpeed)
        //apply the artificial gravity to this game object
        gravity.Attract(gameObject);
	}

    //function for varifying if an eney combatant can or cannot be seen
    void IsDetected(){
        //by default, the AI cannot see any opponents
        isDetected = false;
        //for each enemy combatant in enemies
        foreach (GameObject ga in enemies)
        {
            //if the collider for this enemy body is enabled
            if (ga.GetComponent<CapsuleCollider>().enabled)
            {
                //obtain the direction towards the enemy
                Vector3 directionToEnemy = (ga.transform.position - gameObject.transform.position).normalized;
                //create a ray using this direction and the current AI's position
                Ray toEnemy = new Ray(gameObject.transform.position, directionToEnemy);
                //prepare a raycast
                RaycastHit rch;
                //if the raycast collides with the enemy, it implies this AI can see the enemy
                Physics.Raycast(toEnemy, out rch);
                if (rch.collider.gameObject != null && rch.collider.gameObject.tag == ga.tag)
                {
                    //set the enemy firing target to the currently observed enemy
                    targetEnemy = ga;
                    //this AI can see an enemy combatant
                    isDetected = true;
                    //no more searches needed; we have found an opponent to fire at
                    break;
                }
            }
        }
    }

    //collision behaviour for this AI
    void OnCollisionEnter(Collision coll)
    {
        //if the other body colliding with this AI is a combatant harming bullet
        if (coll.collider.gameObject.tag == "Bullet")
        {
            //decrement health by 1
            health = health - 1;
            //if health falls to or below 0
            if (health <= 0)
            {
                //reset the health of this AI
                health = 10;
                rManager.HandleDeath(gameObject);
            }
        }

    }

    //function to find the next enemy voxel
    public void FindNextTarget()
    {
        //for all the voxels in the scene
        foreach (GameObject ga in GameObject.FindGameObjectsWithTag("Voxel"))
        {
            if (ga.activeInHierarchy)
            {
                //boolean; if true, this AI belongs to the player team
                bool allegianceToPlayer = gameObject.tag == "PlayerTeam" ? true : false;

                //if the AI belongs to the player's team, this AI needs to find
                //a voxel that belongs to the team opposing the player that is currently active,

                //else if it is an AI that is opposed to the player, it needs to find 
                //a voxel that belongsto the team of the player that is currently active
                if ((allegianceToPlayer ^ ga.GetComponent<Voxel>().OnPlayerTeam) && ga.GetComponent<Voxel>().State == 1)
                {
                    //set the target to the currently observed voxel
                    target = ga;
                    //set that we have found the voxel and can move towards it
                    foundTarget = true;
                    //break out of the loop, no more searches required
                    break;
                }
            }
        }
    }

    //constant int value to represent the state of not firing
    const int NOT_FIRING = 0;
    //constant int value to represent the state of firing a round that can destroy the terrain
    const int FIRING_EARTH_BUSTER = 1;
    //constant int value to represent the state of firing a round that can destroy combatants
    const int FIRING_DUDE_KILLER = 2;
}