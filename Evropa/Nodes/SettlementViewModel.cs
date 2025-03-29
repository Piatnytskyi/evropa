namespace Evropa.Nodes;

using Godot;

public partial class SettlementViewModel : Node
{
	[Export]
	public string Name { get; set; } = "Settlement";

	[Export]
	public int Population { get; set; } = 0;

	[Export]
	public int Happiness { get; set; } = 0;

	[Export]
	public int Resources { get; set; } = 0;

	[Export]
	public int Food { get; set; } = 0;

	[Export]
	public int Gold { get; set; } = 0;

	[Export]
	public int LandSize { get; set; } = 0;

	[Export]
	public int LandValue { get; set; } = 0;
}
