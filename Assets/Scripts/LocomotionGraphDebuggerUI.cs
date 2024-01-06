using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocomotionGraphDebuggerUI : MonoBehaviour
{
    public LocomotionGraph.LocomotionGraphDebugger lgd;
    public CameraController cameraController;
    public GameObject debugUI;
    public Dropdown dropdown;
    public InputField input;

    public void updateDepth()
    {
        lgd.generateChunkGraphDepth = int.Parse(input.text);
    }

    public void updateChoice()
    {
        if (dropdown.value == 0)
        {
            lgd.locomotionFunction = LocomotionGraph.LocomotionGraphDebugger.HandleLocomotionGraphFunction.displayPlatformGraph;
        }
        else if (dropdown.value == 1)
        {
            lgd.locomotionFunction = LocomotionGraph.LocomotionGraphDebugger.HandleLocomotionGraphFunction.printPlatformPath;
        }
        else if (dropdown.value == 2)
        {
            lgd.locomotionFunction = LocomotionGraph.LocomotionGraphDebugger.HandleLocomotionGraphFunction.isValidWall;
        }
    }

    public void toggleDebuUI()
    {
        debugUI.SetActive(!debugUI.activeSelf);
        lgd.isUIActive = debugUI.activeSelf;
        cameraController.isUIActive = debugUI.activeSelf;
    }

}
