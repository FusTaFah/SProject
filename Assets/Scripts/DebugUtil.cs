using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DebugUtil : MonoBehaviour {
    //text of the debugging system
    Text debugText;
    //list of debug items
    List<string> debugList;
    public GameObject placeHolder;

	// Use this for initialization
	void Start () {
        debugText = gameObject.GetComponent<Text>();
        debugList = new List<string>();
	}

    //add a string to the debug list
    public void AppendDebugger(string debugItem)
    {
        debugList.Add(debugItem);
    }

    //add a vector to the debug list
    public void AppendDebugger(Vector3 debugItem)
    {
        debugList.Add(debugItem.x + " " + debugItem.y + " " + debugItem.z);
    }

    public void MarkLocation(Vector3 position)
    {
        Instantiate(placeHolder, position, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
        //display all debug items
        debugText.text = "";
        foreach(string x in debugList){
            debugText.text += x + "\n";
        }
        debugList.Clear();
	}
}
