namespace Evropa.Mathematics.Sampling;

public interface ISampler<TParam, TPoint>
{
    TPoint[] Sample(TParam param);
}
