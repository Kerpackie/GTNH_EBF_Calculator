namespace GTNH_EBF_Calculator.Core;

public class OverclockCalculator
{
	private static readonly double LOG2 = Math.Log(2);
	private static readonly int HEAT_DISCOUNT_THRESHOLD = 900;
	private static readonly int HEAT_OC_THRESHOLD = 1800;

	/// <summary>
	/// Recipe Required Voltage
	/// </summary>
	public long BaseRecipeVoltage { get; set; }
	public long RecipeVoltage { get; set; }
	/// <summary>
	/// Machine Operating Voltage
	/// </summary>
	public long MachineVoltage { get; set; }
	/// <summary>
	/// Recipe Base Duration in Seconds
	/// </summary>
	public int BaseDurationSecs { get; set; }
	/// <summary>
	/// Recipe Duration in Ticks
	/// </summary>
	public int Duration { get; set; }
	/// <summary>
	/// Required Recipe Heat
	/// </summary>
	public int RecipeHeat { get; set; }
	/// <summary>
	/// Actual Machine Heat
	/// </summary>
	public int MachineHeat { get; set; }

	/// <summary>
	/// How much duration is divided by for each 1800k above requied heat. Default 4.
	/// </summary>
	public int DurationDecreasePerHeatOC { get; private set; } = 4;

	/// <summary>
	/// Enable Heat Overclocking every 1800k
	/// </summary>
	public bool HeatOC { get; private set; } = true;

	/// <summary>
	/// Enable Heat EU Discount at 900k intervals
	/// </summary>
	public bool HeatDiscounts { get; private set; } = true;

	/// <summary>
	/// Value used for heat discount per 900 heat, default is 0.95 - 5% discount
	/// </summary>
	public double HeatDiscountAmount { get; private set; } = 0.95;

	/// <summary>
	/// Power Overclock Multiplier for EU, default is 4.
	/// </summary>
	public double EUIncreasePerOC { get; private set; } = 4;

	/// <summary>
	/// How much the duration is divided per overclock by power tier. Default is 2.
	/// </summary>
	public double DurationDecreasePerPowerTierOC { get; private set; } = 2;
	
	public OverclockCalculator(long recipeVoltage, long machineVoltage, int baseDurationSecs, int recipeHeat, int machineHeat)
	{
		BaseRecipeVoltage = recipeVoltage;
		MachineVoltage = machineVoltage;
		BaseDurationSecs = baseDurationSecs;
		RecipeHeat = recipeHeat;
		MachineHeat = machineHeat;
	}

	public EBFOverclock CalculateEbfOverclock()
	{
		var ebfOverclock = new EBFOverclock
		{
			RecipeVoltage = BaseRecipeVoltage,
			MachineVoltage = MachineVoltage,
			MachineHeat = MachineHeat,
			RecipeHeat = RecipeHeat,
			BaseDurationSecs = BaseDurationSecs
		};
		
		// Calculate the heat discounts

		var heatDiscounts = HeatDiscounts ? (MachineHeat - RecipeHeat) / HEAT_DISCOUNT_THRESHOLD : 0;
		var heatDiscountMultiplier = Math.Pow(HeatDiscountAmount, heatDiscounts);
		
		ebfOverclock.HeatDiscountQuantity = heatDiscounts;
		
		// Power Overclocks
		
		var recipePowerTier = CalculateRecipePowerTier(heatDiscountMultiplier);
		var machinePowerTier = CalculateMachinePowerTier();
		
		var powerOverclockCount = CalculateAmountOfNeededOverclocks(machinePowerTier, recipePowerTier);
		powerOverclockCount = Math.Min(powerOverclockCount, CalculateRecipeToMachineVoltageDifference());
		powerOverclockCount = Math.Max(powerOverclockCount, 0);
		
		ebfOverclock.PowerTierOverclockQuantity = powerOverclockCount;
		
		
		// Calculate the amount of heat overclocks

		var heatOverclockCount = CalculateAmountOfHeatOverclocks(heatDiscountMultiplier);
		
		heatOverclockCount = Math.Min(heatOverclockCount, powerOverclockCount);
		
		ebfOverclock.HeatOverclockQuantity = heatOverclockCount;
		
		// Calculate Recipe Voltage
		RecipeVoltage = (long) Math.Floor(BaseRecipeVoltage * Math.Pow(EUIncreasePerOC, powerOverclockCount));
		
		RecipeVoltage = CalculateFinalRecipeEUt(heatDiscountMultiplier);
		
		ebfOverclock.RecipeOperatingVoltage = RecipeVoltage;
		
		// Calculate duration
		Duration = BaseDurationSecs * 20;
		ebfOverclock.BaseDurationTicks = Duration;
		
		Duration = (int) Math.Floor(Duration / Math.Pow(DurationDecreasePerPowerTierOC, powerOverclockCount - heatOverclockCount));
		Duration = (int) Math.Floor(Duration / Math.Pow(DurationDecreasePerHeatOC, heatOverclockCount));

		if (Duration < 1)
		{
			Duration = 1;
		}
		
		ebfOverclock.OperatingDurationTicks = Duration;
		ebfOverclock.OperatingDurationSecs = Duration / 20;

		return ebfOverclock;
	}
	
	private long CalculateFinalRecipeEUt(double heatDiscountMultiplier)
	{
		var recipeEUt = Math.Ceiling(RecipeVoltage * heatDiscountMultiplier);
		
		return (long) recipeEUt;
	}
	
	private int CalculateRecipeToMachineVoltageDifference()
	{
		var diff = Math.Ceiling(CalculatePowerTier(MachineVoltage)) - Math.Ceiling(CalculatePowerTier(RecipeVoltage));

		return (int) diff;
	}

	private int CalculateAmountOfNeededOverclocks(double machinePowerTier, double recipePowerTier)
	{
		var overclocks = Math.Min
		(CalculateAmountOfOverclocks(machinePowerTier, recipePowerTier),
			Math.Ceiling(
				Math.Log(BaseDurationSecs * 20) / Math.Log(DurationDecreasePerPowerTierOC)));

		return (int) overclocks;
	}
	
	private int CalculateAmountOfHeatOverclocks(double heatDiscountMultiplier)
	{
		var heatOverclocks = Math.Min((MachineHeat - RecipeHeat) / HEAT_OC_THRESHOLD, 
			CalculateAmountOfOverclocks(
				CalculateMachinePowerTier(), 
				CalculateRecipePowerTier(heatDiscountMultiplier)));

		return heatOverclocks;
	}
	
	private int CalculateAmountOfOverclocks(double machinePowerTier, double recipePowerTier)
	{
		var overclocks = machinePowerTier - recipePowerTier;
		
		return (int)overclocks;
	}

	/*private int CalculateAmountOfPowerOverclocks()
	{
		var powerOverclocks = Math.Min((MachineVoltage - RecipeVoltage) / 32, 0);
	}*/
	
	private double CalculateMachinePowerTier()
	{
		var tier = CalculatePowerTier(MachineVoltage);
		
		return tier;
	}
	
	private double CalculateRecipePowerTier(double heatDiscountMultiplier)
	{
		var tier = CalculatePowerTier(BaseRecipeVoltage * heatDiscountMultiplier);
		
		return tier;
	}

	private double CalculatePowerTier(double voltage)
	{
		var tier = 1 + Math.Max(0, (Math.Log(voltage) / LOG2) - 5) / 2;

		return tier;
	}


}