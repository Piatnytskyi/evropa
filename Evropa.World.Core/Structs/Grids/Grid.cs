namespace Evropa.World.Core.Structs.Grids;

public readonly struct Grid : IGrid
{
    public readonly IGrid[] Children;

    public Grid(IGrid[] children)
    {
        Children = children;
    }
}