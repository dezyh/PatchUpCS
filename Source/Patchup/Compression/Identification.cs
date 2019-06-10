namespace Patchup.Compression
{
    public static class Identification
    {
        public static bool Png(byte[] data)
        {
            byte[] identifier = new byte[] {137, 80, 78, 71, 13, 10, 26, 10};

            for (int i = 0; i < identifier.Length; i++)
            {
                if (data[i] != identifier[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Zip(byte[] data)
        {
            byte[] identifier = new byte[] {80, 75, 3, 4};

            for (int i = 0; i < identifier.Length; i++)
            {
                if (data[i] != identifier[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
