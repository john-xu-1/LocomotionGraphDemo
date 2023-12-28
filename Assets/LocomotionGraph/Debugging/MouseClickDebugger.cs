using NoiseTerrain;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ChunkHandler;

namespace LocomotionGraph
{
    public class MouseClickDebugger : LocomotionGraphDebugger
    {
        public ChunkHandler.ChunkHandler chunks;
        public ProceduralMapGenerator generator;

        Vector2Int lastClickChunkID;
        Vector2Int lastClickID;
        public enum HandleMouseClickFunction { placePlayer, resetChunk, placeLava, placeWater, toggleTile, displayPlatformGraph, printPlatformPath, isValidWall }
        public HandleMouseClickFunction clickFunction;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            HandleMouseClickDebugging();

            //debug platformGraph
            //DisplayPlatformGraph();
        }




        private void HandleMouseClickDebugging()
        {
            if (clickFunction == HandleMouseClickFunction.resetChunk)
            {
                Vector2Int clickChunkID = chunks.GetChunkID(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (Input.GetMouseButton(0) && (lastClickChunkID == null || lastClickChunkID != clickChunkID))
                {
                    Chunk clickedChunk = chunks.GetChunk(clickChunkID);
                    if (clickedChunk != null)
                    {
                        Debug.Log($"resetting {clickChunkID} chunk");
                        bool[,] resetBoolMap = generator.GenerateBoolMap(clickChunkID);
                        clickedChunk.SetTiles(resetBoolMap);
                        generator.visibleChunkIDs.Remove(clickChunkID);
                        generator.toFixChunkIDs.Add(clickChunkID);
                        generator.toDisplayChunks.Add(clickChunkID);
                    }
                    lastClickChunkID = clickChunkID;
                }
            }
            else if (clickFunction == HandleMouseClickFunction.toggleTile)
            {
                Vector2Int clickTile = TilePosFromClick(Input.mousePosition);// new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1) && (lastClickID == null || lastClickID != clickTile))
                {
                    Debug.Log($"TileClicked {clickTile}");
                    Chunk clickedChunk = chunks.GetChunk(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (clickedChunk != null)
                    {
                        bool value = chunks.GetTile(clickTile);
                        chunks.SetTile(clickTile, !value);

                    }
                    lastClickID = clickTile;
                }

            }
            else if (clickFunction == HandleMouseClickFunction.placeWater)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    Vector2Int clickTile = TilePosFromClick(Input.mousePosition); //new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                    if (!chunks.GetTile(clickTile))
                    {
                        generator.PlaceWater(clickTile);
                    }
                }

            }
            else if (clickFunction == HandleMouseClickFunction.placeLava)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    Vector2Int clickTile = TilePosFromClick(Input.mousePosition);// new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                    if (!chunks.GetTile(clickTile))
                    {
                        generator.PlaceLava(clickTile);
                    }
                }

            }
            else if (clickFunction == HandleMouseClickFunction.displayPlatformGraph)
            {
                HandleDisplayPlatformGraph();
            }
            else if (clickFunction == HandleMouseClickFunction.printPlatformPath)
            {
                HandlePrintPlatformPath();
            }
            else if (clickFunction == HandleMouseClickFunction.isValidWall)
            {
                HandleIsValidWall();

            }


        }

        //private bool CheckValidWall(Vector2Int tilePos, bool isRightWall, )
        //{

        //}

        private Vector2Int TilePosFromClick(Vector2 mousePosition)
        {
            return new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        }


    }
}