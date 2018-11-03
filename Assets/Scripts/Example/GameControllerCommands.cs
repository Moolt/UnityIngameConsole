using UnityEngine;
using IngameConsole;

[ExecutableFromConsole]
public class GameControllerCommands : MonoBehaviour
{
    public GameObject cube;
    private BaseWriter _writer = new FormattedWriter();

    [ConsoleMethod("cube_rotate", "Rotates the cube by to the given angle.")]
    public void RotateCubeBy(float degrees)
    {
        if (CheckForCube())
        {
            cube.transform.Rotate(Vector3.up, degrees, Space.World);
        }
    }

    [ConsoleMethod("cube_rotation", "Prints the current rotation of the cube.")]
    public void GetRotation()
    {
        if (CheckForCube())
        {
            _writer.NextLine();
            _writer.Write("Current rotation is: ");
            _writer.WriteBold(cube.transform.rotation.eulerAngles.ToString());
        }
    }

    [ConsoleMethod("cube_scaling", "Prints the current scale of the cube.")]
    public void GetScale()
    {
        if (CheckForCube())
        {
            _writer.NextLine();
            _writer.Write("Current scale is: ");
            _writer.WriteBold(cube.transform.localScale.ToString());
        }
    }

    [ConsoleMethod("cube_scale", "Scales the cube by the given factor.")]
    public void ScaleCubeBy([ConsoleParameter("Factor. 1 is default")] float factor)
    {
        if (CheckForCube())
        {
            cube.transform.localScale = Vector3.one * factor;
        }
    }

    [ConsoleMethod("cube_random_color", "Randomizes the color of the cube.")]
    public void RandomizeCubeColor()
    {
        if (CheckForCube())
        {
            var cubeRenderer = cube.GetComponent<MeshRenderer>();
            var mat = cubeRenderer.material;
            mat.SetColor("_Color", new Color(Random.value, Random.value, Random.value));
        }
    }

    [ConsoleMethod("destroy", "Destroys the object with the given name.")]
    public void DestroyObject([ConsoleParameter("Name of object.")] GameObject gameObject)
    {
        Destroy(gameObject);
    }

    [ConsoleMethod("print")]
    public void Print(string text)
    {
        _writer.WriteInfo(text);
    }

    private bool CheckForCube()
    {
        if (cube == null)
        {
            _writer.WriteWarning("Cube not found.");
            return false;
        }
        return true;
    }
}
