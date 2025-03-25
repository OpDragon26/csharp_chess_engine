using Board;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using Piece;
using UnityEngine;

namespace Node
{
    public struct Node
    {
        public Board.Board board;
        public Node(Board.Board newBoard)
        {
            board = newBoard;
        }

        public Move.Move BestMove(int Depth)
        {
            List<Move.Move> MoveList = MoveFinder.FilteredSearch(board, board.Side, true);
            Dictionary<int, Move.Move> MoveDict = new Dictionary<int,  Move.Move>();
            int l = MoveList.Count;

            int alpha = Int32.MinValue;
            int beta  = Int32.MaxValue;
            
            if (!this.board.Side)
            {
                // White (maximising player)
                int MaxEval = Int32.MinValue;
                for (int i = 0; i < l; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(ref MoveList[i]);
                    if (MoveBoard.KingInCheck(true))
                        continue;
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
                int MinEval = Int32.MaxValue;
                
                for (int i = 0; i < l; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(ref MoveList[i]);
                    if (MoveBoard.KingInCheck(true))
                        continue;
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
                ulong whitePieces = board.SideBitboards[false];
                ulong blackPieces = board.SideBitboards[true];
                int wMultiplier = Weights.Weights.Multipliers[false];
                int bMultiplier = Weights.Weights.Multipliers[true];
                
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((whitePieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                            Eval += board.board[i,j].Value + Weights.Weights.PieceWeights[board.board[i,j].HashValue][i,j] * wMultiplier;
                        else if ((blackPieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                            Eval += board.board[i,j].Value + Weights.Weights.PieceWeights[board.board[i,j].HashValue][i,j] * bMultiplier;
                    }
                }
            }
            else
            {
                ulong whitePieces = board.SideBitboards[false];
                ulong blackPieces = board.SideBitboards[true];
                int wMultiplier = Weights.Weights.Multipliers[false];
                int bMultiplier = Weights.Weights.Multipliers[true];
                
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((whitePieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                            Eval += board.board[i,j].Value + Weights.Weights.EndgameWeights[board.board[i,j].HashValue][i,j] * wMultiplier;
                        else if ((blackPieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                            Eval += board.board[i,j].Value + Weights.Weights.EndgameWeights[board.board[i,j].HashValue][i,j] * bMultiplier;
                    }
                }
            }

            return Eval;
        }

        public int Minimax(int Depth, int alpha, int beta)
        {
            Outcome Status = board.Status(false);
            if (Status == Outcome.Draw)
                return 0;

            if (Depth == 0) 
                return this.StaticEvaluate();

            Move.Move[] MoveList = MoveFinder.Search(board, board.Side, true);
            int l = MoveList.Length;
            
            if (!board.Side)
            {
                // white - maximising side
                bool found = false;
                int MaxEval = Int32.MinValue;
                
                for (int i = 0; i < l; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(ref MoveList[i]);
                    if (MoveBoard.KingInCheck(false))
                        continue;
                    Node Child = new Node(MoveBoard);
                    found = true; // if at least one move passed the check test, set found to true

                    // Finding eval
                    int Eval = Child.Minimax(Depth - 1, alpha, beta);
                    MaxEval = Math.Max(MaxEval, Eval);
                    alpha = Math.Max(alpha, Eval);

                    if (beta <= alpha) break;
                }

                if (found) // if there was at least one legal move in the position return the eval
                    return MaxEval;
                
                // no legal moves for white
                // return the eval based on the outcome
                if (board.KingInCheck(false)) // checkmate
                    return Int32.MinValue; // white was checkmated, return the best eval for black
                return 0; // no moves but not in check -> stalemate

            }
            else
            {
                // black - minimising side
                bool found = false;
                int MinEval = Int32.MaxValue;
                
                for (int i = 0; i < l; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(ref MoveList[i]);
                    if (MoveBoard.KingInCheck(true))
                        continue;
                    Node Child = new Node(MoveBoard);
                    found = true;

                    // Finding eval
                    int Eval = Child.Minimax(Depth - 1, alpha, beta);
                    MinEval= Math.Min(MinEval, Eval);
                    beta = Math.Min(beta, Eval);

                    if (beta <= alpha) break;
                }

                if (found) // if there was at least one legal move in the position return the eval
                    return MinEval;
                
                // no legal moves for black
                // return the eval based on the outcome
                if (board.KingInCheck(true)) // checkmate
                    return Int32.MaxValue; // black was checkmated, return the best eval for white
                return 0; // no moves but not in check -> stalemate
            }
        }
    }
}