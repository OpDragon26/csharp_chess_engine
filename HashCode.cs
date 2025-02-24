namespace HashCodeHelper
{
    public static class HashCodeHelper
    {
        public static int GetBoardHash<T>(T[,] array) // Written by DeepSeek, don't ask me how or why it works but it does
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
                    T element = array[i, j] ?? throw new ArgumentNullException();
                    int elementHash = element.GetHashCode();
                    unchecked // Allows overflow which is acceptable for hash codes
                    {
                        hash = hash * 31 + elementHash;
                    }
                }
            }
            return hash;
        }
        public static int GetArrayHash<T>(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int hash = 17;
            for (int i = 0; i < array.Length; i++)
            {
                T Element = array[i] ?? throw new ArgumentNullException();
                int ElementHash = Element.GetHashCode();
                unchecked
                {
                    hash = hash * 31 + ElementHash;
                }
            }

            return hash;
        }
    }
}