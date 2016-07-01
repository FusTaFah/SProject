using UnityEngine;
using System.Collections;

public class VoxelCounter : MonoBehaviour {

    //integer which counts the amount of frames shown in a second
    int fpsCounter;
    //integer which is responsibe for showing the frames per second to the player
    int fpsCounterDisplay;
    //timer to delay showing the frames per second
    float fpsTimer;
    //integer that holds the value of player team voxels
    int playerTeamVoxelCount;
    //integer that holds the value of enemy team voxels
    int enemyTeamVoxelCount;

	// Use this for initialization
	void Start () {
        //initialise all counters to 0
        playerTeamVoxelCount = 0;
        enemyTeamVoxelCount = 0;
        fpsCounter = 0;
        fpsCounterDisplay = fpsCounter;
        fpsTimer = 0.0f;
        //do not allow this object to be destroyed upon loading the victory screen
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        //if one second has passed
        if (fpsTimer >= 1.0f)
        {
            //the current value of the FPS counter is assigned to the displayed FPS counter
            fpsCounterDisplay = fpsCounter;
            //reset the fps counter
            fpsCounter = 0;
            //reset the fps timer
            fpsTimer = 0.0f;
        }
        else
        {
            //increase the fps counter by 1 as update is called once per frame
            fpsCounter++;
            //increment the fps timer delay
            fpsTimer += Time.deltaTime;
        }
	}

    //returns the player team score
    public int getPlayerTeamScore()
    {
        return playerTeamVoxelCount;
    }

    //returns the enemy team score
    public int getEnemyTeamScore()
    {
        return enemyTeamVoxelCount;
    }

    //sets the player team score
    public void setPlayerTeamScore(int score)
    {
        playerTeamVoxelCount = score;
    }

    //sets the enemy team score
    public void setEnemyTeamScore(int score)
    {
        enemyTeamVoxelCount = score;
    }

    //returns the FPS counter value
    public int getFPS()
    {
        return fpsCounterDisplay;
    }
}
