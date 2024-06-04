namespace GTNH_EBF_Calculator.Core;

public class EBFOverclock
{
	private static readonly double LOG2 = Math.Log(2);
	private static readonly int HEAT_DISCOUNT_THRESHOLD = 900;
	private static readonly int HEAT_OC_THRESHOLD = 1800;

	/// <summary>
	/// Recipe Required Voltage
	/// </summary>
	public long RecipeVoltage { get; set; }
	
	/// <summary>
	/// Machine Operating Voltage
	/// </summary>
	public long MachineVoltage { get; set; }
	
	public long RecipeOperatingVoltage { get; set; }
	/// <summary>
	/// Recipe Base Duration in Seconds
	/// </summary>
	public int BaseDurationSecs { get; set; }
	/// <summary>
	/// Recipe Duration in Seconds
	/// </summary>
	public int OperatingDurationSecs { get; set; }
	
	public int BaseDurationTicks { get; set; }
	public int OperatingDurationTicks { get; set; }
	/// <summary>
	/// Required Recipe Heat
	/// </summary>
	public int RecipeHeat { get; set; }
	/// <summary>
	/// Actual Machine Heat
	/// </summary>
	public int MachineHeat { get; set; }

	public int HeatDiscountQuantity { get; set; }
	public int HeatOverclockQuantity { get; set; }
	public int PowerTierOverclockQuantity { get; set; }

	
}