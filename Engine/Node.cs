using Board;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Node
{
    public class Node
    {
        public Board.Board board;
        public List<Node> ChildNodes = new List<Node>();
        public Node(Board.Board newBoard)
        {
            board = newBoard;
        }

        public List<Move.Move> GenerateChildNodes()
        {
            List<Node> NodeList = new List<Node>();

            (Outcome, List<Move.Move>) outcome = this.board.Status();
            
            if (outcome.Item1 == Outcome.Ongoing)
            {
                List<Move.Move> MoveList = outcome.Item2;

                for (int i = 0; i < MoveList.Count; i++)
                {
                    Board.Board MoveBoard = board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false, false);

                    NodeList.Add(new Node(MoveBoard));
                }
                this.ChildNodes = NodeList;

                return MoveList;
            }
            this.ChildNodes = new List<Node>(); 

            return new List<Move.Move>();
        }

        public Move.Move BestMove(int Depth)
        {
            List<Move.Move> MoveList = board.Status().Item2;
            Dictionary<int, Move.Move> MoveDict = new Dictionary<int,  Move.Move>();
            
            int alpha = -1_000_000;
            int beta  = 1_000_000;
            
            if (!this.board.Side)
            {
                // White (maximising player)
                int MaxEval = -1_000_000;

                for (int i = 0; i < MoveList.Count; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false, false);
                    Node Child = new Node(MoveBoard);

                    // Finding eval
                    int Eval = Child.Minimax(Depth, alpha, beta);
                    MaxEval = Math.Max(MaxEval, Eval);
                    alpha = Math.Max(alpha, Eval);

                    if (!MoveDict.ContainsKey(Eval))
                        MoveDict.Add(Eval, MoveList[i]);
                    
                    if (beta <= alpha) break;
                }

                return MoveDict[MaxEval];
            }
            else 
            {
                // Black
                int MinEval = 1_000_000;
                
                for (int i = 0; i < MoveList.Count; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false, false);
                    Node Child = new Node(MoveBoard);

                    // Finding eval
                    int Eval = Child.Minimax(Depth, alpha, beta);
                    MinEval= Math.Min(MinEval, Eval);
                    beta = Math.Min(beta, Eval);
                    
                    if (!MoveDict.ContainsKey(Eval))
                        MoveDict.Add(Eval, MoveList[i]);

                    if (beta <= alpha) break;
                }

                return MoveDict[MinEval];
            }
        }

        int StaticEvaluate()
        {
            int Eval = 0;
            
            //Debug.Log(board.PieceCounter);

            if (!board.Endgame())
            {
                for (int i = 0; i < board.PiecePositions[false].Count; i++)
                {
                    (int, int) coords = board.PiecePositions[false][i];
                    Eval += this.board.board[coords.Item2,coords.Item1].Value + Weights.Weights.PieceWeights[this.board.board[coords.Item2,coords.Item1].GetHashCode()][coords.Item2,coords.Item1] * Weights.Weights.Multipliers[false];

                }
                for (int i = 0; i < board.PiecePositions[true].Count; i++)
                {
                    (int, int) coords = board.PiecePositions[true][i];
                    Eval += this.board.board[coords.Item2,coords.Item1].Value + Weights.Weights.PieceWeights[this.board.board[coords.Item2,coords.Item1].GetHashCode()][coords.Item2,coords.Item1] * Weights.Weights.Multipliers[true];
                }
            }
            else
            {
                Debug.Log("Endgame");

                for (int i = 0; i < board.PiecePositions[false].Count; i++)
                {
                    (int, int) coords = board.PiecePositions[false][i];
                    Eval += this.board.board[coords.Item2,coords.Item1].Value + Weights.Weights.EndgameWeights[this.board.board[coords.Item2,coords.Item1].GetHashCode()][coords.Item2,coords.Item1] * Weights.Weights.Multipliers[false];

                }
                for (int i = 0; i < board.PiecePositions[true].Count; i++)
                {
                    (int, int) coords = board.PiecePositions[true][i];
                    Eval += this.board.board[coords.Item2,coords.Item1].Value + Weights.Weights.EndgameWeights[this.board.board[coords.Item2,coords.Item1].GetHashCode()][coords.Item2,coords.Item1] * Weights.Weights.Multipliers[true];
                }
            }

            return Eval;
        }

        public int Minimax(int Depth, int alpha, int beta)
        {
            (Outcome, List<Move.Move>) Status = board.Status();
            if (Status.Item1 == Outcome.White)
                return 1_000_000;
            if (Status.Item1 == Outcome.Black)
                return -1_000_000;
            if (Status.Item1 == Outcome.Draw)
                return 0;

            if (Depth == 0) 
                return this.StaticEvaluate();

            List<Move.Move> MoveList = Status.Item2;
            if (!board.Side)
            {
                int MaxEval = -1_000_000;
                
                for (int i = 0; i < MoveList.Count; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false, false);
                    Node Child = new Node(MoveBoard);

                    // Finding eval
                    int Eval = Child.Minimax(Depth - 1, alpha, beta);
                    MaxEval = Math.Max(MaxEval, Eval);
                    alpha = Math.Max(alpha, Eval);

                    if (beta <= alpha) break;
                }

                return MaxEval;
            }
            else
            {
                int MinEval = 1_000_000;
                
                for (int i = 0; i < MoveList.Count; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false, false);
                    Node Child = new Node(MoveBoard);

                    // Finding eval
                    int Eval = Child.Minimax(Depth - 1, alpha, beta);
                    MinEval= Math.Min(MinEval, Eval);
                    beta = Math.Min(beta, Eval);

                    if (beta <= alpha) break;
                }

                return MinEval;
            }
        }
    }
}