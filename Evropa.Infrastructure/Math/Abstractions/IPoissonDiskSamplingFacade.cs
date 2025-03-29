using System.Drawing;

namespace Evropa.Infrastructure.Math.Abstractions;

public interface IPoissonDiskSamplingFacade
{
	/// <summary>
	/// Generates a Poisson disk sampling of points within a specified rectangle.
	/// </summary>
	/// <param name="width">The width of the rectangle.</param>
	/// <param name="height">The height of the rectangle.</param>
	/// <param name="minDistance">The minimum distance between points.</param>
	/// <param name="k">The number of attempts to find a valid point.</param>
	/// <returns>A list of points representing the Poisson disk sampling.</returns>
	Point[] GeneratePoissonDiskSampling(int width, int height, float minDistance, int k);
}
