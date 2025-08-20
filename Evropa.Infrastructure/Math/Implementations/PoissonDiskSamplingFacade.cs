namespace Evropa.Infrastructure.Math.Implementations;

using System;
using System.Drawing;
using Evropa.Infrastructure.Math.Abstractions;
using MathNet.Numerics.Random;

public class PoissonDiskSamplingFacade : IPoissonDiskSamplingFacade
{
    private readonly RandomSource _randomSource;

    public PoissonDiskSamplingFacade(RandomSource randomSource)
    {
        _randomSource = randomSource;
    }

    public PointF[] GeneratePoissonDiskSampling(float width, float height, float minDistance, int k)
    {
        const float tau = 2 * (float)Math.PI;
        float cellSize = minDistance / (float)Math.Sqrt(2);

        int gridWidth = (int)Math.Ceiling(width / cellSize);
        int gridHeight = (int)Math.Ceiling(height / cellSize);
        PointF?[] grid = new PointF?[gridWidth * gridHeight];

        var queue = new List<PointF>();

        // Helper method to get grid coordinates
        (int x, int y) GetGridCoords(PointF point)
        {
            return ((int)Math.Floor(point.X / cellSize), (int)Math.Floor(point.Y / cellSize));
        }

        // Helper method to check if point fits
        bool Fits(PointF point, int gridX, int gridY)
        {
            int minX = Math.Max(gridX - 2, 0);
            int maxX = Math.Min(gridX + 3, gridWidth);
            int minY = Math.Max(gridY - 2, 0);
            int maxY = Math.Min(gridY + 3, gridHeight);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    var gridPoint = grid[x + y * gridWidth];
                    if (gridPoint.HasValue)
                    {
                        float dx = point.X - gridPoint.Value.X;
                        float dy = point.Y - gridPoint.Value.Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                        if (distance <= minDistance)
                            return false;
                    }
                }
            }
            return true;
        }

        // Generate initial point
        var initialPoint = new PointF(
            (float)_randomSource.NextDouble() * width,
            (float)_randomSource.NextDouble() * height
        );
        
        queue.Add(initialPoint);
        var (gridX, gridY) = GetGridCoords(initialPoint);
        grid[gridX + gridY * gridWidth] = initialPoint;

        // Main algorithm loop
        while (queue.Count > 0)
        {
            int queueIndex = _randomSource.Next(queue.Count);
            var currentPoint = queue[queueIndex];
            
            // Remove point from queue (swap with last and remove)
            queue[queueIndex] = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);

            // Try to generate k candidates around current point
            for (int i = 0; i < k; i++)
            {
                float alpha = tau * (float)_randomSource.NextDouble();
                float d = minDistance * (float)Math.Sqrt(3 * _randomSource.NextDouble() + 1);
                
                float px = currentPoint.X + d * (float)Math.Cos(alpha);
                float py = currentPoint.Y + d * (float)Math.Sin(alpha);

                // Check if point is within bounds
                if (px < 0 || px >= width || py < 0 || py >= height)
                    continue;

                var candidatePoint = new PointF(px, py);
                var (candidateGridX, candidateGridY) = GetGridCoords(candidatePoint);

                // Check if point fits (maintains minimum distance)
                if (!Fits(candidatePoint, candidateGridX, candidateGridY))
                    continue;

                // Add valid point to queue and grid
                queue.Add(candidatePoint);
                grid[candidateGridX + candidateGridY * gridWidth] = candidatePoint;
            }
        }

        // Return all valid points from grid
        return grid.Where(p => p.HasValue).Select(p => p.Value).ToArray();
    }
}
