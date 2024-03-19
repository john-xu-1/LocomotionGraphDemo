using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    public class ASPLocomotionGenerator : ASPGenerator
    {
        public Clingo_02.AnswerSet GetAnswerSet() { return solver.answerSet; }
        public bool done { get { return solver.SolverStatus == Clingo_02.ClingoSolver.Status.SATISFIABLE; } }
        protected List<NodeChunk> nodeChunks;
        public void SetNodeChunkMemory(List<NodeChunk> nodeChunks)
        {
            this.nodeChunks = nodeChunks;
        }
        protected string GetNodeChunksMemory()
        {
            Debug.Log($"Node Count: {nodeChunks.Count}");
            string aspCode = "\n";
            int edgeCount = 0;
            foreach (NodeChunk nodeChunk in nodeChunks)
            {
                aspCode += $"node({nodeChunk.nodeID}).\n";
                foreach (int connectionID in nodeChunk.connectedPlatforms)
                {
                    aspCode += $"edge({nodeChunk.nodeID},{connectionID}).\n";
                    edgeCount++;
                }
            }
            Debug.Log($"Edge Count: {edgeCount}");
            return aspCode;
        }
        protected override string getASPCode()
        {
            //    string aspCode = $@"

            //    #const enemy_max = 100.
            //    #const enemy_min = 5.

            //    piece_types(player;enemy;item).
            //    piece_enemy_types(bounce_boi;missile_launcher;rolla_boi;shotgun_boi).
            //    piece_weapon_types(drone_controller;old_shotgun;rainy_day;magnetized_shifter;vol;gaeas_touch).

            //    %% place pieces on nodes %%
            //    1{{piece(player,NodeID): node(NodeID)}}1.
            //    enemy_min{{piece(Enemy,NodeID): node(NodeID),piece_enemy_types(Enemy)}}enemy_max.
            //    :- piece_enemy_types(Type), not piece(Type,_).

            //    %% only one typ of each weapon %%
            //    6{{piece(Weapon,NodeID): node(NodeID), piece_weapon_types(Weapon)}}6.
            //    :- piece_weapon_types(Type), not piece(Type,_). 

            //    %% only one piece per room %%
            //    :- Count = {{piece(_,NodeID)}}, node(NodeID), Count > 1.

            //    %% start and end nodes %%
            //    start(NodeID) :- piece(player, NodeID).
            //    1{{end(NodeID):node(NodeID)}}1.

            //    %% flood sinks not on end path %%
            //    path(NodeID, 0, PathID) :- path(NodeID,_), node(NodeID), PathID = NodeID.
            //    path(NodeID, Step + 1, PathID) :- edge(Source, NodeID), path(Source, Step, PathID), Step < 100.

            //    %sink(PathID) :- node(PathID), end(NodeID), not path(NodeID,_,PathID), path(PathID,_).
            //    %sink_source(NodeID, SinkID) :- edge(NodeID, SinkID), sink(SinkID), not sink(NodeID).

            //    %% player reach every piece %%
            //    path(NodeID, 0) :- piece(player,NodeID).
            //    path(NodeID, Step + 1) :- edge(Source, NodeID), path(Source, Step), Step < 100.
            //    :- piece(_,NodeID), not path(NodeID,_), node(NodeID).
            //    :- end(NodeID), not path(NodeID,_).

            //    %% every piece must be on a node that has a path to the end %%
            //    :- piece(_,PathID), end(NodeID), not path(NodeID,_,PathID).


            //    #const max_teleporters = 2.
            //    teleporter(1..max_teleporters).
            //    2{{teleporter(NodeID, TeleporterID): node(NodeID), path(NodeID,_)}}2 :- teleporter(TeleporterID).
            //    :- teleporter(NodeID,_), Count = {{teleporter(NodeID,_)}}, Count != 1.

            //    %edge(Source,NodeID) :- teleporter(NodeID, T1), teleporter(Source, T2), T1 == T2, NodeID != Source.
            //    :- teleporter(NodeID,_), not path(NodeID,_).
            //    %path(NodeID, Step + 1) :- teleporter(NodeID, T1), teleporter(Source,T2), T1 == T2, path(Source, Step), Step < 100.

            //    exit(NodeID, 2) :- end(NodeID).

            //    reachable_node(NodeID) :- node(NodeID), path(NodeID,_).
            //    %:- reachable_node(PathID), start(NodeID), not path(NodeID,_,PathID).

            //    #show piece/2.
            //    %#show sink/1.
            //    #show start/1.
            //    #show exit/2.
            //    %#show sink_source/2.
            //    #show teleporter/2.

            //";

            string aspCode = $@"

                %% placing start and end nodes in unique locations %%
                1{{start(NodeID) : node(NodeID)}}1.
                1{{end(NodeID) : node(NodeID)}}1.
                :- start(SNode), end(ENode), SNode == ENode.

                %% creating gates and keys %%
                gates(1..2).
                1{{gate(GID, NodeID) : node(NodeID)}}2 :- gates(GID).
                1{{key(GID, NodeID) : node(NodeID)}}1 :- gates(GID).

                %% no keys or gates overlap with a key or a gate %%
                :- key(G1, N1), key(G2, N2), G1 != G2, N1 == N2.
                :- gate(G1, N1), gate(G2, N2), G1 != G2, N1 == N2.
                :- gate(_, GNode), key(_, KNode), GNode == KNode.

                %% the start node cannot have a key or a gate on it %%
                :- start(SNode), gate(_,GNode), SNode == GNode.
                :- key(_, KNode), start(SNode), KNode == SNode.
                
                %% the end node cannot have a key or a gate on it %%
                :- end(SNode), gate(_,GNode), SNode == GNode.
                :- key(_, KNode), end(SNode), KNode == SNode.



            ";

            string aspCodePath = $@"

                %% path stuff %%
                path_count(0..20).
                path(NodeID, 0) :- start(NodeID).

                %% end(NodeID) must be on the path
                :- end(NodeID), not path(NodeID, _).
                :- key(_, NodeID), not path(NodeID, _).
                :- gate(_, NodeID), not path(NodeID, _).

                %% if no gate there is a path between two node if an edge connects them
                %% if gate there is a path between two node as long as the key can be reached before passing through the gate
                path(NodeID2, T + 1) :- node(NodeID2), node(NodeID), path(NodeID, T), edge(NodeID,NodeID2), path_count(T+1), not gate(_,NodeID2).
                path(NodeID2, T + 1) :- node(NodeID2), node(NodeID), path(NodeID, T), edge(NodeID,NodeID2), path_count(T+1), gate(GID,NodeID2), have_key(GID, KT), T >= KT.
                

                %% find key before needing gate
                %:- key(GID, KNode), gate(GID, GNode), path(KNode, KStep), path(GNode, GStep), KStep > GStep.


                key_present(0). %% means GID key has already been aquired
                %% 1{{key(KeyID, RoomID): roomID(RoomID)}}1 :- keys_types(KeyID), not key_present(KeyID).

                have_key(GID, T) :- path(NodeID, T), key(GID, NodeID), not key_present(GID).
                have_key(GID, 0) :- key_present(GID).


                %% ensure path is longer than some integer T (10)
                %:- end(NodeID), path(NodeID, T), T < 10.


                %% poi starting points, keys and gates
                poi_path(GID, NodeID, GPID) :- key(GID, NodeID), GPID = NodeID.
                poi_path(GID, NodeID, GPID) :- gate(GID, NodeID), GPID = NodeID.

                %% poi path extension
                poi_path(GID, SinkID, GPID) :- edge(SourceID, SinkID), poi_path(GID, SourceID, GPID). 

                %% poi path termination for gate at end and key at gate
                :- end(NodeID), gate(GID, GPID), not poi_path(GID, NodeID, GPID).
                :- gate(GID, GNodeID), key(GID, KNodeID), not poi_path(GID, GNodeID, KNodeID).
            ";



            return aspCode + aspCodePath + GetNodeChunksMemory();
            //return GetNodeChunksMemory();
        }

        override protected void SATISFIABLE(Clingo_02.AnswerSet answerSet, string jobID)
        {
            FindObjectOfType<LocomotionGraphDebugger>().DisplayAnswerset(answerSet);
            
        }
    }
}