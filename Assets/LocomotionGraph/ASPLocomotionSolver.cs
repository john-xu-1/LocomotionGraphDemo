using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    public class ASPLocomotionSolver : LocomotionSolver
    {
        public ASPLocomotionGenerator generator;

        protected override bool GetReady()
        {
            return generator.done;
        }
        public override void Solve(List<NodeChunk> nodeChunks, int seed)
        {
            generator.SetNodeChunkMemory(nodeChunks);
            generator.StartGenerator("0", seed);
        }

        public Clingo_02.AnswerSet GetAnswerset()
        {
            return generator.GetAnswerSet();
        }
    }
}