namespace Evropa.Infrastructure.Math.Implementations;

using System.Drawing;
using Evropa.Infrastructure.Math.Abstractions;
using MathNet.Numerics.Distributions;

public class PoissonDiskSamplingFacade : IPoissonDiskSamplingFacade
{
	public Point[] GeneratePoissonDiskSampling(int width, int height, float minDistance, int k)
	{
		var x = new int[k];
		var y = new int[k];

		Poisson.Samples(x, 1.0);
		Poisson.Samples(y, 1.0);
		var points = new Point[k];
		for (int i = 0; i < k; i++)
			points[i] = new Point(x[i] * width, y[i] * height);

		return points;
	}
}
