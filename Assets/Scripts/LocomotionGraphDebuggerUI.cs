using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocomotionGraph;

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

    public void saveGraphButton()
    {
        NodesSave save = new NodesSave();
        save.nodes = new List<NodeSave>();
        save.nodeIDs = new List<int>();

        //loop through every nodeCuhnk in filledChunk and create a NodeSave from nodeSave and connectedPlatform
        foreach (FilledChunk filledChunk in lgd.locomotionGraph.RoomChunk.filledChunks)
        {
            foreach (NodeChunk node in filledChunk.platforms)
            {
                NodeSave nodeSave = new NodeSave();
                nodeSave.nodeID = node.nodeID;
                nodeSave.connectionIDS = node.connectedPlatforms;
                if (node.connectedPlatforms != null)
                {
                    foreach (int connection in node.connectedPlatforms)
                    {
                        if (!save.nodeIDs.Contains(connection))
                        {
                            save.nodeIDs.Add(connection);
                        }
                    }
                    save.nodes.Add(nodeSave);
                }
            }
        }

        string json = JsonUtility.ToJson( save, true);
        Clingo_02.ClingoUtil.CreateFile(json, "RoomChunk.txt");
    }

    public void UpdateGraphButton()
    {
        NodesSave save = Utility.GetJsonObject<NodesSave>("RoomChunk");

        // need to check to verify that the current roomchunk has the correct nodes

        //loop through NodesSave and update the roomCunk's platform
        foreach (NodeSave nodeSave in save.nodes)
        {
            lgd.locomotionGraph.RoomChunk.GetPlatform(nodeSave.nodeID).connectedPlatforms = nodeSave.connectionIDS;
        }
    }



    [System.Serializable]
    public struct NodesSave
    {
        public List<NodeSave> nodes;
        public List<int> nodeIDs;
    }

    [System.Serializable]
    public struct NodeSave
    {
        public int nodeID;
        public List<int> connectionIDS;
    }

}
