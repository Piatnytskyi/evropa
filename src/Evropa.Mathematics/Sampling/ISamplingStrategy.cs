namespace Evropa.Mathematics.Sampling;

public interface ISamplingStrategy<TParam, TPoint>
{
    TPoint[] Sample(TParam param);
}
