namespace Evropa.World.Domain.Grids;

public readonly struct Grid : IGrid
{
    public readonly IGrid[] Children;

    public Grid(IGrid[] children)
    {
        Children = children;
    }
}