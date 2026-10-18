namespace Evropa.Mathematics.Sampling.Regions;

public interface ISamplingRegion<TPoint>
{
    TPoint Min { get; }
    TPoint Max { get; }
    bool Contains(TPoint point);
}
