using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/*
 * 1. Generate Room Chunk -> SetRoomChunkThread()
 * 2. Generate Locomotion graph -> GenerateLocomotionGraphThread()
 */

namespace LocomotionGraph
{
    public class LocomotionGraph : MonoBehaviour
    {
        RoomChunk roomChunk;
        public RoomChunk RoomChunk { get { return roomChunk; } }

        public int jumpHeight = 6;
        protected int seed;

        public delegate void OnLocomotionGraphSetupComplete();
        public event OnLocomotionGraphSetupComplete onLocomotionGraphSetupComplete;

        protected bool[,] boolMapThread;
        protected Vector2Int minTileThread, maxTileThread;


        public List<PlatformChunk> GetPlatforms()
        {
            List<PlatformChunk> platforms = new List<PlatformChunk>();
            foreach (FilledChunk filledChunk in roomChunk.filledChunks)
            {
                foreach (PlatformChunk platform in filledChunk.platforms)
                {
                    platforms.Add(platform);
                }
            }
            return platforms;
        }
        public PlatformChunk GetPlatform(int platformID)
        {
            return roomChunk.GetPlatform(platformID);
        }

        public LocomotionSolver ls;
        public bool generatingLocomotionGraph = false;
        // creates and solves locomotion graph
        public IEnumerator SolveLocomotionGraph()
        {
            generatingLocomotionGraph = true;
            generateLocomotionGraphThreadCompleted = false;
            Thread thread = new Thread(GenerateLocomotionGraphThread);
            thread.Start();
            while (!generateLocomotionGraphThreadCompleted)
            {
                yield return null;
            }

            ls.Solve(nodeChunks, seed);

            while (!ls.ready)
            {
                yield return null;
            }

            generatingLocomotionGraph = false;
        }

        List<NodeChunk> nodeChunks;
        bool generateLocomotionGraphThreadCompleted = false;
        private void GenerateLocomotionGraphThread()
        {
            List<int> platformIDs = roomChunk.GetPlatformIDs();
            nodeChunks = new List<NodeChunk>();
            foreach (int platformID in platformIDs)
            {
                NodeChunk nodeChunk = roomChunk.GetPlatform(platformID);
                nodeChunks.Add(nodeChunk);
            }
            roomChunk.SetPlatformEdges(platformIDs, jumpHeight, checkConnection);
            generateLocomotionGraphThreadCompleted = true;
        }

        public void SetRoomChunkThread()
        {
            roomChunk = new RoomChunk(boolMapThread, jumpHeight, minTileThread, maxTileThread);
            //platformSetupComplete = true;
            onLocomotionGraphSetupComplete?.Invoke();
        }

        public bool checkConnection = false;
    }
}

