namespace Evropa.Mathematics.Sampling;

using System;

public class Sampler<TParam, TPoint> : ISampler<TParam, TPoint>
{
    private readonly ISamplingStrategy<TParam, TPoint> _strategy;

    public Sampler(ISamplingStrategy<TParam, TPoint> strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public TPoint[] Sample(TParam param) => _strategy.Sample(param);
}
