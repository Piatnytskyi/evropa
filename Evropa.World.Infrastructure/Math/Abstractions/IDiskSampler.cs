namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;
using Evropa.World.Core.Structs;

public interface IDiskSampler
{
    Vector2[] Sample(SamplingRegion region);
}