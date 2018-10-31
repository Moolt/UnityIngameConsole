using UnityEngine;
using IngameConsole;

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

    [ConsoleMethod("cube_scaling", "Prints the current scale of the cube.")]
    public void GetScale()
    {
        ConsoleIO.NextLine();
        ConsoleIO.Write("Current scale is: ");
        ConsoleIO.WriteBold(cube.transform.localScale.ToString());
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
