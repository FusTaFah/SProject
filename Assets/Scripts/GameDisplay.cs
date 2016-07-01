using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDisplay : MonoBehaviour {
    //the test on the score UI component
    Text counter;
    //reference to the game object that counts all remaining voxels
    VoxelCounter vc;
    //time until the game ends
    float countdownTimer;
    //player data
    GameObject player;
    //relevant textures
    public Texture healthTexture;
    public Texture jetpackTexture;
    public Texture crosshairTexture;
    bool paused;

	// Use this for initialization
	void Start () {
        //time until the game ends is initialised to 60; 1 minute game
        countdownTimer = 60.0f;
        //find the voxel counter object in the game scene
        vc = GameObject.Find("ScoreCounter").GetComponent<VoxelCounter>();
        //make a reference to this object's text component
        counter = gameObject.GetComponent<Text>();
        //initialise the player gameobject reference
        player = GameObject.FindGameObjectWithTag("Player");
        paused = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (player != null)
        {
            //set the frames per second counter
            counter.text = "FPS: " + vc.getFPS()
                //set the player team voxel counter
                + "\n\nPlayer Voxels left: " + vc.getPlayerTeamScore()
                //set the enemy team voxel counter
                + "\nEnemy Voxels Left: " + vc.getEnemyTeamScore()
                //set the time remaining
                + "\nTime Remaining: " + Mathf.Floor(countdownTimer);
            //if the time has run out, load the results screen
            if (countdownTimer <= 0) SceneManager.LoadScene("ResultScreen");
            //else decrement the time remaining
            else countdownTimer -= Time.deltaTime;
        }else
        {
            counter.text = "";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
        }

        if (paused)
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }else
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
	}

    void OnGUI()
    {
        if(player != null)
        {
            {
                Vector2 rect1 = new Vector2(Screen.width / 10.0f, 9.0f * Screen.height / 10.0f);
                Vector2 rect2 = rect1 + new Vector2(player.GetComponent<CharacterControls>().GetHealth() * 10.0f, -20.0f);
                Rect r = new Rect(rect1.x, Screen.height - rect1.y, rect2.x - rect1.x, (rect2.y - rect1.y));
                GUI.DrawTexture(r, healthTexture);
            }

            {
                Vector2 rect1 = new Vector2(Screen.width / 10.0f, 8.0f * Screen.height / 10.0f);
                Vector2 rect2 = rect1 + new Vector2(player.GetComponent<CharacterControls>().GetJetpackFuel() * 100.0f, -20.0f);
                Rect r = new Rect(rect1.x, Screen.height - rect1.y, rect2.x - rect1.x, (rect2.y - rect1.y));
                GUI.DrawTexture(r, jetpackTexture);
            }

            {
                Vector2 rect0 = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
                Vector2 rect1 = new Vector2(rect0.x - 20, rect0.y - 20);
                Vector2 rect2 = new Vector2(rect0.x + 20, rect0.y - 60);
                //Vector2 rect2 = rect0 - new Vector2(crosshairTexture.width, crosshairTexture.height);
                Rect r = new Rect(rect1.x, Screen.height - rect1.y, rect2.x - rect1.x, (rect2.y - rect1.y));
                GUI.DrawTexture(r, crosshairTexture);
            }
        }
    }
}
