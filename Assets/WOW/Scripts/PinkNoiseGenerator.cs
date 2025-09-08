using UnityEngine;

public class PinkNoiseGenerator
{
    private System.Random rand = new System.Random();
    private int[] rows;
    private int counter = 0;

    public PinkNoiseGenerator(int numRows = 16)
    {
        rows = new int[numRows];
        for (int i = 0; i < numRows; i++)
        {
            rows[i] = rand.Next();
        }
    }

    public float NextValue()
    {
        int i = 0;
        int n = counter++;
        while ((n & 1) == 1)
        {
            n >>= 1;
            i++;
        }
        if (i < rows.Length)
        {
            rows[i] = rand.Next();
        }

        long sum = 0;
        foreach (var r in rows) sum += r;
        return (sum / (float)rows.Length) / int.MaxValue;
    }
}
