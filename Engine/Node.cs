using Board;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Node
{
    public class Node
    {
        public Board.Board board;
        public Node(Board.Board newBoard)
        {
            board = newBoard;
        }

        public Move.Move BestMove(int Depth)
        {
            List<Move.Move> MoveList = board.Status().Item2;
            Dictionary<int, Move.Move> MoveDict = new Dictionary<int,  Move.Move>();
            int l = MoveList.Count;

            int alpha = -1_000_000;
            int beta  = 1_000_000;
            
            if (!this.board.Side)
            {
                // White (maximising player)
                int MaxEval = -1_000_000;
                for (int i = 0; i < l; i++)
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
                
                for (int i = 0; i < l; i++)
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
                List<(int,int)> PiecePositions = board.PiecePositions[false];
                int l = PiecePositions.Count;
                int multiplier = Weights.Weights.Multipliers[false];

                for (int i = 0; i < l; i++)
                {
                    (int, int) coords = PiecePositions[i];
                    Piece.Piece piece = this.board.board[coords.Item2,coords.Item1];

                    Eval += piece.Value + Weights.Weights.PieceWeights[piece.HashValue][coords.Item2,coords.Item1] * multiplier;

                }

                PiecePositions = board.PiecePositions[true];
                l = PiecePositions.Count;
                multiplier = Weights.Weights.Multipliers[true];

                for (int i = 0; i < l; i++)
                {
                    (int, int) coords = PiecePositions[i];
                    Piece.Piece piece = this.board.board[coords.Item2,coords.Item1];

                    Eval += piece.Value + Weights.Weights.PieceWeights[piece.HashValue][coords.Item2,coords.Item1] * multiplier;
                }
            }
            else
            {
                // Debug.Log("Endgame");
                List<(int,int)> PiecePositions = board.PiecePositions[false];
                int l = PiecePositions.Count;
                int multiplier = Weights.Weights.Multipliers[false];

                for (int i = 0; i < l; i++)
                {
                    (int, int) coords = PiecePositions[i];
                    Piece.Piece piece = this.board.board[coords.Item2,coords.Item1];

                    Eval += piece.Value + Weights.Weights.EndgameWeights[piece.HashValue][coords.Item2,coords.Item1] * multiplier;
                }

                PiecePositions = board.PiecePositions[true];
                l = PiecePositions.Count;
                multiplier = Weights.Weights.Multipliers[true];

                for (int i = 0; i < l; i++)
                {
                    (int, int) coords = PiecePositions[i];
                    Piece.Piece piece = this.board.board[coords.Item2,coords.Item1];

                    Eval += piece.Value + Weights.Weights.EndgameWeights[piece.HashValue][coords.Item2,coords.Item1] * multiplier;
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
            int l = MoveList.Count;
            
            if (!board.Side)
            {
                int MaxEval = -1_000_000;
                
                for (int i = 0; i < l; i++)
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
                
                for (int i = 0; i < l; i++)
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