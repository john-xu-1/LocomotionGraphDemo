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

    public void SaveGraphButton()
    {
        NodesSave save = new NodesSave();
        save.nodes = new List<NodeSave>();
        save.nodeIDs = new List<int>();

        // loop through every nodeChunk in filledChunk and create a NodeSave from the nodeID and connectedPlatforms
        foreach(FilledChunk filledChunk in lgd.locomotionGraph.RoomChunk.filledChunks)
        {
            foreach(NodeChunk node in filledChunk.platforms)
            {
                NodeSave nodeSave = new NodeSave();
                nodeSave.nodeID = node.nodeID;
                nodeSave.connectionIDs = node.connectedPlatforms;
                if(node.connectedPlatforms != null)
                {
                    foreach (int connection in node.connectedPlatforms)
                    {
                        if (save.nodeIDs.Contains(connection) == false)
                        {
                            save.nodeIDs.Add(connection);
                        }
                    }
                    save.nodes.Add(nodeSave);
                }

                if (save.nodeIDs.Contains(node.nodeID) == false) save.nodeIDs.Add(node.nodeID);
                
            }
        }

        string json = JsonUtility.ToJson(save, true);
        Clingo_02.ClingoUtil.CreateFile(json, "RoomChunk.txt");
    }

    public void UpdateGraphButton()
    {
        NodesSave save = Utility.GetJsonObject<NodesSave>("RoomChunk");

        // need to check to verify that the current roomchunk has the correct nodes

        // loop through NodesSave and update the RoomChunk's platforms
        foreach(NodeSave nodeSave in save.nodes)
        {
            lgd.locomotionGraph.RoomChunk.GetPlatform(nodeSave.nodeID).connectedPlatforms = nodeSave.connectionIDs;
        }

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
    public List<int> connectionIDs;
}