namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;
using Evropa.World.Core.Structs.Samplings;

public interface IPlaneSampler<TRegion> where TRegion : ISamplingRegion
{
    Vector2[] Sample(TRegion region);
}