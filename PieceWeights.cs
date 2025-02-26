namespace Weights
{
    static class Weights
    {
        public static float WeightMultiplier = 0.01f;
        public static Dictionary<Piece.Piece, float[,]> PieceWeights = new Dictionary<Piece.Piece, float[,]>{
            {Piece.Presets.B_Pawn,
                new float[,] {
                    {0,0,0,0,0,0,0,0},
                    {2,2,2,2,2,2,2,2},
                    {1.5f,1.5f,1.6f,1.95f,1.95f,1.7f,1.5f,1.5f},
                    {1,1,1.5f,1.8f,1.9f,1.5f,1,1},
                    {1.2f,1,1.3f,1.85f,1.85f,1.3f,1,1.2f},
                    {1.5f,1.2f,1.1f,1.2f,1.2f,1.1f,1.2f,1.5f},
                    {1.3f,1.6f,1.5f,0.8f,0.8f,1.5f,1.6f,1.3f},
                    {0,0,0,0,0,0,0,0},
                }
            },
            {Piece.Presets.W_Pawn,
                new float[,] {
                    {0,0,0,0,0,0,0,0},
                    {1.3f,1.6f,1.5f,0.8f,0.8f,1.5f,1.6f,1.3f},
                    {1.5f,1.2f,1.1f,1.2f,1.2f,1.1f,1.2f,1.5f},
                    {1.2f,1,1.3f,1.85f,1.85f,1.3f,1,1.2f},
                    {1,1,1.5f,1.8f,1.9f,1.5f,1,1},
                    {1.5f,1.5f,1.6f,1.95f,1.95f,1.7f,1.5f,1.5f},
                    {2,2,2,2,2,2,2,2},
                    {0,0,0,0,0,0,0,0},
                }
            },

            {Piece.Presets.B_Knight,
                new float[,] {
                    {1.1f,0.8f,0.8f,1,1,0.8f,0.8f,1.1f},
                    {0.8f,0.8f,1.2f,0.9f,0.9f,1.2f,0.8f,0.8f},
                    {0.8f,0.9f,1,1,1,1,0.9f,0.8f},
                    {1,1,1.2f,1,1,1.2f,1,1},
                    {0.9f,1,1.1f,1.2f,1.2f,1.1f,1,0.9f},
                    {1,1.1f,1.25f,1.2f,1.2f,1.25f,1.1f,1},
                    {0.8f,1,1,1.2f,1.2f,1,1,0.8f},
                    {0.8f,0.8f,0.9f,0.9f,0.9f,0.9f,0.8f,0.8f},
                }
            },
            {Piece.Presets.W_Knight,
                new float[,] {
                    {0.8f,0.8f,0.9f,0.9f,0.9f,0.9f,0.8f,0.8f},
                    {0.8f,1,1,1.2f,1.2f,1,1,0.8f},
                    {1,1.1f,1.25f,1.2f,1.2f,1.25f,1.1f,1},
                    {0.9f,1,1.1f,1.2f,1.2f,1.1f,1,0.9f},
                    {1,1,1.2f,1,1,1.2f,1,1},
                    {0.8f,0.9f,1,1,1,1,0.9f,0.8f},
                    {0.8f,0.8f,1.2f,0.9f,0.9f,1.2f,0.8f,0.8f},
                    {1.1f,0.8f,0.8f,1,1,0.8f,0.8f,1.1f},
                }
            },

            {Piece.Presets.B_Bishop,
                new float[,] {
                    {1.1f,0.8f,0.9f,0.9f,0.9f,0.9f,0.8f,1.1f},
                    {0.8f,0.9f,1,0.8f,0.8f,1,0.9f,0.8f},
                    {1,0.9f,1,0.9f,0.9f,1,0.9f,1},
                    {1,1.3f,0.9f,0.9f,0.9f,0.9f,1.3f,1},
                    {1.1f,0.9f,1.4f,1.1f,1.1f,1.4f,0.9f,1.1f},
                    {1,1.2f,0.9f,1,1,0.9f,1.2f,1},
                    {0.9f,1.2f,0.9f,1.1f,1.1f,0.9f,1.2f,0.9f},
                    {0.9f,0.8f,0.8f,0.7f,0.7f,0.8f,0.8f,0.9f},
                }
            },
            {Piece.Presets.W_Bishop,
                new float[,] {
                    {0.9f,0.8f,0.8f,0.7f,0.7f,0.8f,0.8f,0.9f},
                    {0.9f,1.2f,0.9f,1.1f,1.1f,0.9f,1.2f,0.9f},
                    {1,1.2f,0.9f,1,1,0.9f,1.2f,1},
                    {1.1f,0.9f,1.4f,1.1f,1.1f,1.4f,0.9f,1.1f},
                    {1,1.3f,0.9f,0.9f,0.9f,0.9f,1.3f,1},
                    {1,0.9f,1,0.9f,0.9f,1,0.9f,1},
                    {0.8f,0.9f,1,0.8f,0.8f,1,0.9f,0.8f},
                    {1.1f,0.8f,0.9f,0.9f,0.9f,0.9f,0.8f,1.1f},
                }
            },

            {Piece.Presets.B_Rook,
                new float[,] {
                    {0.9f,0.9f,0.9f,0.9f,0.9f,0.9f,0.9f,0.9f},
                    {1.3f,1.3f,1.2f,1.2f,1.2f,1.2f,1.3f,1.3f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.8f,0.8f,0.8f,0.8f,0.8f,0.8f,0.8f,0.8f},
                    {1,0.9f,1.2f,1.2f,1.2f,1.2f,0.9f,1},
                }
            },
            {Piece.Presets.W_Rook,
                new float[,] {
                    {1,0.9f,1.2f,1.2f,1.2f,1.2f,0.9f,1},
                    {0.8f,0.8f,0.8f,0.8f,0.8f,0.8f,0.8f,0.8f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f,0.7f},
                    {1.3f,1.3f,1.2f,1.2f,1.2f,1.2f,1.3f,1.3f},
                    {0.9f,0.9f,0.9f,0.9f,0.9f,0.9f,0.9f,0.9f},
                }
            },

            {Piece.Presets.B_Queen,
                new float[,] {
                    {0.93f,0.93f,0.97f,1,1,0.97f,0.93f,0.93f},
                    {0.93f,0.97f,1,1,1,1,0.97f,0.93f},
                    {0.97f,1,1,1.05f,1.05f,1,1,0.97f},
                    {1,1,1.05f,1.1f,1.1f,1.05f,1,1},
                    {1,1,1.05f,1.1f,1.1f,1.05f,1,1},
                    {0.97f,1,1.05f,1.05f,1,1,1,0.97f},
                    {0.93f,0.97f,1,1,1,1,0.97f,0.93f},
                    {0.93f,0.93f,0.97f,1,1,0.97f,0.93f,0.93f},
                }
            },
            {Piece.Presets.W_Queen,
                new float[,] {
                    {0.93f,0.93f,0.97f,1,1,0.97f,0.93f,0.93f},
                    {0.93f,0.97f,1,1,1,1,0.97f,0.93f},
                    {0.97f,1,1,1.05f,1.05f,1,1,0.97f},
                    {1,1,1.05f,1.1f,1.1f,1.05f,1,1},
                    {1,1,1.05f,1.1f,1.1f,1.05f,1,1},
                    {0.97f,1,1.05f,1.05f,1,1,1,0.97f},
                    {0.93f,0.97f,1,1,1,1,0.97f,0.93f},
                    {0.93f,0.93f,0.97f,1,1,0.97f,0.93f,0.93f},
                }
            },

            {Piece.Presets.B_King,
                new float[,] {
                    {0.7f,0.75f,0.75f,0.72f,0.72f,0.75f,0.75f,0.7f},
                    {0.77f,0.77f,0.77f,0.77f,0.77f,0.77f,0.77f,0.77f},
                    {0.85f,0.85f,0.8f,08f,08f,08f,0.85f,0.85f},
                    {0.87f,0.87f,0.82f,0.82f,0.82f,0.82f,0.87f,0.87f},
                    {0.9f,0.9f,0.85f,0.85f,0.85f,0.85f,0.9f,0.9f},
                    {0.92f,0.92f,0.87f,0.87f,0.87f,0.87f,0.92f,0.92f},
                    {0.95f,0.95f,0.9f,0.9f,0.9f,0.9f,0.95f,0.95f},
                    {1.1f,1.2f,1.1f,0.9f,1.1f,0.8f,1.2f,1.2f},
                }
            },
            {Piece.Presets.W_King,
                new float[,] {
                    {1.1f,1.2f,1.1f,0.9f,1.1f,0.8f,1.2f,1.2f},
                    {0.95f,0.95f,0.9f,0.9f,0.9f,0.9f,0.95f,0.95f},
                    {0.92f,0.92f,0.87f,0.87f,0.87f,0.87f,0.92f,0.92f},
                    {0.9f,0.9f,0.85f,0.85f,0.85f,0.85f,0.9f,0.9f},
                    {0.87f,0.87f,0.82f,0.82f,0.82f,0.82f,0.87f,0.87f},
                    {0.85f,0.85f,0.8f,08f,08f,08f,0.85f,0.85f},
                    {0.77f,0.77f,0.77f,0.77f,0.77f,0.77f,0.77f,0.77f},
                    {0.7f,0.75f,0.75f,0.72f,0.72f,0.75f,0.75f,0.7f},
                }
            },

            {Piece.Presets.Empty,
                new float[,] {
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0},
                }
            },
        };
    }
}