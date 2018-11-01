# Overview

This handy Unity plug-in helps you to implement your own developer console for your game. It's very easy to set up and enables you to call already existing code from the console with little to none effort.

The plug-in also comes with a ui prefab that works out of the box but can be easily extended to your hearts content.

## Setup

If you want to get a quick overview you should check out the [demo project](https://github.com/Moolt/UnityIngameConsole/archive/master.zip). 
You can also download the [package](https://github.com/Moolt/UnityIngameConsole/raw/master/ingame-console.unitypackage) containing only the essential scripts and assets.

If you chose to download the package, you first need to create a canvas. Then drop the `Prefabs/Console/Console.prefab` into the canvas and you're done. Start the game and open the console with `Tab`, which is the default key.

Try using the default commands like `help` or `chelp`.

# Extension
Inside of the `ConsoleLogic` script there are already some predefined commands like `help`, `chelp`, `quit`, `close` and `clear`.
Implementing your own commands is very easy and will be shown in the examples below.

## Implementing commands

The console searches for __objects in the scene__ with executable scripts. You can mark your script as executable by adding the `[ExecutableFromConsole]` attribute just above the class definition.

Aditionally, you have to mark the methods you want to call from the console by adding the `[ConsoleMethod]` attribute to them. This attribute takes two arguments: The `command name` and a `description` (optional). The description will be shown if you call `chelp` for the command.

The code below is taken from the demo project. The method `RotateCubeBy` is registered as the `cube_rotate` command and takes one parameter.

`cube_rotation` takes no parameter and just outputs text to the console using the static class `ConsoleIO`.

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
```

The image below shows how the above commands can be executed in the console.

![alt text](https://raw.githubusercontent.com/Moolt/UnityIngameConsole/master/Documentation/screenshot.gif "screenshot")

## Documenting parameters

Similar to methods, parameters can also be documented. This is strictly optional though.
When `chelp` is called for a command, the default parameter names are displayed for the usage example.
You can be more specific by adding a `[ConsoleParameter]` attribute with defining a custom description for the parameter.

```csharp
//[...]
public void ScaleCubeBy([ConsoleParameter("Factor. 1 is default")] float factor)
{
    //[...]
}
```

The image below shows a default parameter-name and a custom description using `[ConsoleParameter]`.

![alt text](https://raw.githubusercontent.com/Moolt/UnityIngameConsole/master/Documentation/chelp.png "screenshot")

# Limitations and trouble shooting

If you run into any problems, you may find some answers here.

## Target objects

If you want the commands of any script to appear in the console, there should always be an instance of it in the scene.
Scripts without instances will __not__ show up in the console.

## Ambiguity

GameObjects containing executable scripts should be limited to one instance per scene to avoid ambiguity.
Commands should also have distinct names across all executable scripts.

# Good to know

These things might be helpful for you while working with the console.

## GameObjects as Parameters

`GameObjects` in the scene can also be passed to methods as parameters.
If you pass a `GameObjects` name to the command, the reference to the `GameObject` will automatically be resolved and passed to the function.

```csharp
[ConsoleMethod("destroy", "Destroys the object with the given name.")]
public void DestroyObject([ConsoleParameter("Name of object.")] GameObject gameObject)
{
    Destroy(gameObject);
}
```

The command above could be used like ``> destroy someObject``.

## Input history

All commands entered by the user are stored. Use the arrow keys to navigate through earlier inputs.
The amout of commands stored is limited to 10 by default.

# BaseConsoleIO

The `ConsoleLogic` script reads and writes to the UI via the `ConsoleIO` script. `ConsoleIO` inherits from the `BaseConsoleIO` abstract class and can therefore be replaced with any other class inheriting from this base class.

Instead of reading from an ui textbox you could, for example, read commands from a network stream. You could also output to a textfile instead.