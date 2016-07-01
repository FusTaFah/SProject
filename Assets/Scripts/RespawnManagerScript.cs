using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnManagerScript : MonoBehaviour {

    List<Pair<float, GameObject>> respawningCombatants;

	// Use this for initialization
	void Start () {
        respawningCombatants = new List<Pair<float, GameObject>>();
        GameObject.Find("Camera").GetComponent<Camera>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        foreach(Pair<float, GameObject> combatant in respawningCombatants)
        {
            if (combatant.left >= 5.0f)
            {
                combatant.right.SetActive(true);

                if (combatant.right.tag == "Player")
                {
                    //reset the player's position
                    GameObject.Find("Camera").GetComponent<Camera>().enabled = false;
                    combatant.right.GetComponent<Camera>().enabled = true;
                    combatant.right.transform.position = new Vector3(0.0f, 82.8f, 0.0f);
                }
                else if (combatant.right.tag == "PlayerTeam")
                {
                    //reset the player team member's position
                    combatant.right.transform.position = new Vector3(0.0f, 82.8f, 0.0f);
                }
                else if (combatant.right.tag == "EnemyTeam")
                {
                    //reset the enemy team member's position
                    combatant.right.transform.position = new Vector3(0.0f, -82.8f, 0.0f);
                }

                respawningCombatants.Remove(combatant);
            }

            combatant.left += Time.deltaTime;
        }
        
    }

    public void HandleDeath(GameObject combatant)
    {
        combatant.GetComponent<Rigidbody>().velocity.Set(0.0f, 0.0f, 0.0f);
        if (combatant.tag == "Player")
        {
            combatant.transform.position.Set(0.12f, 8.23f, -46.78f);
            combatant.transform.rotation = Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            combatant.GetComponent<Camera>().enabled = false;
            GameObject.Find("Camera").GetComponent<Camera>().enabled = true;
        }
        combatant.SetActive(false);
        respawningCombatants.Add(new Pair<float, GameObject>(0.0f, combatant));
    }
}
