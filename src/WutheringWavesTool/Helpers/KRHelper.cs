namespace WutheringWavesTool.Helpers;

public static class KRHelper
{
    public static byte[]? Xor(byte[] data, byte xorKey)
    {
        if (data == null)
        {
            return null;
        }
        if (data.Length == 0)
        {
            return data;
        }
        byte[] array = new byte[data.Length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (byte)(data[i] ^ xorKey);
        }
        return array;
    }
}
