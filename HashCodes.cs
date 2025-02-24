using System.Data;

namespace HashCode
{
    static class HashCode
    {
        public static int GetBoardHash<T>(T[,] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.GetLength(0) != 8 || array.GetLength(1) != 8)
                throw new ArgumentException("Array must be 8x8.");

            int hash = 17;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    T element = array[i, j];
                    int elementHash = EqualityComparer<T>.Default.GetHashCode(element ?? throw new NoNullAllowedException());
                    unchecked // Allows overflow which is acceptable for hash codes
                    {
                        hash = hash * 31 + elementHash;
                    }
                }
            }
            return hash;
        }
    }
}