using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutableFromConsole]
public class GameControllerCommands : MonoBehaviour {

    public GameObject cube;

    [ConsoleMethod("cube_rotate", "Rotates the cube by to the given angle.")]
    public void RotateCubeBy(float degrees)
    {
        cube.transform.Rotate(Vector3.up, degrees, Space.World);
    }

    [ConsoleMethod("cube_scale", "Scales the cube by the given factor.")]
    public void ScaleCubeBy([ConsoleParameter("Factor. 1 is default")] float factor)
    {
        cube.transform.localScale = Vector3.one * factor;
    }

    [ConsoleMethod("cube_random_color", "Randomizes the color of the cube.")]
    public void RandomizeCubeColor()
    {
        var cubeRenderer = cube.GetComponent<MeshRenderer>();
        var mat = cubeRenderer.material;
        mat.SetColor("_Color", new Color(Random.value, Random.value, Random.value));
    }
}
