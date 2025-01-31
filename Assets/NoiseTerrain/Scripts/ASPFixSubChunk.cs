using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    public class ASPFixSubChunk : FixSubChunk
    {
        //public Clingo2.ClingoSolver solver;
        public ASPFixSubChunkGenerator aspGenerator;
        protected override bool CheckReady()
        {
            return CheckSolverStatus();
        }
        public override void Fix(ChunkHandler.SubChunk subChunk, int fixTileRuleBorder, TileRules tileRules)
        {
            int width = subChunk.tiles.GetLength(0);
            int height = subChunk.tiles.GetLength(1);

            aspGenerator.width = width;
            aspGenerator.height = height;
            aspGenerator.fixTileRuleBorder = fixTileRuleBorder;
            aspGenerator.tileRules = tileRules;
            aspGenerator.subChunk = subChunk;

            aspGenerator.starting = true;
            Debug.LogWarning($"{width} {height}");
            while (!CheckSolverStatus())
            {

            }

        }

        bool CheckSolverStatus()
        {
            return !(aspGenerator.running || aspGenerator.starting);
            //return solver.SolverStatus == Clingo2.ClingoSolver.Status.SATISFIABLE || solver.SolverStatus == Clingo2.ClingoSolver.Status.UNSATISFIABLE || solver.SolverStatus == Clingo2.ClingoSolver.Status.ERROR || solver.SolverStatus == Clingo2.ClingoSolver.Status.UNKNOWN || solver.SolverStatus == Clingo2.ClingoSolver.Status.TIMEDOUT;
        }

    }
}




