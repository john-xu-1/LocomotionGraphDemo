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
        lgd.locomotionFunction = (LocomotionGraph.LocomotionGraphDebugger.HandleLocomotionGraphFunction) dropdown.value;

        switch (lgd.locomotionFunction)
        {
            case LocomotionGraphDebugger.HandleLocomotionGraphFunction.displayPlatformGraph:
                // reset depth to inputted value (should display inputField)
                updateDepth();

                Debug.Log("Select a floor tile to display the graph which shows the platforms 'connect' to platform you have selected.");
                break;
            case LocomotionGraphDebugger.HandleLocomotionGraphFunction.addConnectionToPlatformGraph:
                // set depth to zero to ensure clarity of which platforms you are adding connections to (should hide inputField)
                lgd.generateChunkGraphDepth = 0;

                Debug.Log("Start by selecting the platform you wish to add connection to.");
                break;
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

    public void InputReceived(string message)
    {
        Debug.Log(message);
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