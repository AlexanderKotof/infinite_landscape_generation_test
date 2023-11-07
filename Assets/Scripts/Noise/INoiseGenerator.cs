namespace Test.Noise
{
    public interface INoiseGenerator
    {
        float[,] GenerateNoiseMap(int x, int y);
    }
}
