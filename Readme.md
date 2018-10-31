# Overview

This handy Unity plug-in helps you to implement your own developer console for your game. It's very easy to set up and enables you to call already existing code from the console with little to none effort.

## Setup

Todo

# Usage

![alt text](https://github.com/Moolt/UnityIngameConsole/blob/master/Documentation/screenshot.gif?raw=true "screenshot")

```csharp
[ExecutableFromConsole]
public class GameControllerCommands : MonoBehaviour
{
    public GameObject cube;

    [ConsoleMethod("cube_rotate", "Rotates the cube by to the given angle.")]
    public void RotateCubeBy(float degrees)
    {
        cube.transform.Rotate(Vector3.up, degrees, Space.World);
    }

    [ConsoleMethod("cube_rotation", "Prints the current rotation of the cube.")]
    public void GetRotation()
    {
        ConsoleIO.NextLine();
        ConsoleIO.Write("Current rotation is: ");
        ConsoleIO.WriteBold(cube.transform.rotation.eulerAngles.ToString());
    }

    // [...]
}
````
Todo