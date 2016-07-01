using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryText : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //find the score counter in the scene
        VoxelCounter vc = GameObject.Find("ScoreCounter").GetComponent<VoxelCounter>();
        //find the result text game object's text component
        Text victoryLabel = GameObject.Find("ResultText").GetComponent<Text>();
        //display the scores of the player team and the team opposing the player
        victoryLabel.text = "Final score!\nPlayer Team: " + vc.getPlayerTeamScore() + "\tEnemy Team: " + vc.getEnemyTeamScore();
        //stop the mouse pointer from moving
        Cursor.lockState = CursorLockMode.None;
        //make the mouse cursor invisible
        Cursor.visible = true;

        //if the player score is less than the enemies'
        if (vc.getPlayerTeamScore() < vc.getEnemyTeamScore())
        {
            //set the label for defeat
            victoryLabel.text += "\n\nYOU LOOOOOOOOSE!!!!";
        }
        //if the player score is greater than the enemies'
        else if (vc.getPlayerTeamScore() > vc.getEnemyTeamScore())
        {
            //set the label for victory
            victoryLabel.text += "\n\nYOU ARE VICTORY!!!!";
        }
        //if both teams had the same score by the end
        else
        {
            //set the label for draw
            victoryLabel.text += "\n\nYOU BOTH SUCK!!!!";
        }
	}
	
	// Update is called once per frame
	void Update () {
        //allow the cursor to move
        Cursor.lockState = CursorLockMode.None;
	}

    public void Restart()
    {
        //restart the game by loading the main game level
        SceneManager.LoadScene("game");
    }
}
