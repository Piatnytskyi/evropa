namespace Evropa.Infrastructure.Math.Implementations;

using System.Drawing;
using Evropa.Infrastructure.Math.Abstractions;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

public class PoissonDiskSamplingFacade : IPoissonDiskSamplingFacade
{
	public Point[] GeneratePoissonDiskSampling(int width, int height, float minDistance, int k)
	{
		Poisson poisson = new Poisson(1.0 / minDistance, new MersenneTwister());
		Point[] points = new Point[k];
		for (int i = 0; i < k; i++)
		{
			int x = poisson.Sample() * width;
			int y = poisson.Sample() * height;

			points[k] = new Point(x, y);
		}

		return points;
	}
}
