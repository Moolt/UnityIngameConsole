using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutableFromConsole]
public class GameControllerCommands : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ConsoleMethod("do_something", "Does something")]
    public void DoSomething()
    {
        Debug.Log("yay");
    }
}
