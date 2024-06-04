namespace GTNH_EBF_Calculator.Core;

public static class GregtechValues
{
	// Dictionary of Coil:Heat
	public static readonly Dictionary<string, int> Coils = new()
	{
		{ "Cupronickel", 1801 },
		{ "Kanthal", 2701 },
		{ "Nichrome", 3601 },
		{ "TPV-Alloy", 4501 },
		{ "HSS-G", 5401 },
		{ "HSS-S", 6301 },
		{ "Naquadah", 7201 },
		{ "NaquadahAlloy", 8101 },
		{ "Trinium", 9001 },
		{ "Electrum Flux", 9901 },
		{ "Awakened Draconium", 10801 },
		{ "Infinity", 11701 },
		{ "Hypogen", 12601 },
		{ "Eternal", 13501 }
	};
	
	public static readonly long[] Voltage = {
		8L, 32L, 128L, 512L, 2048L, 8192L, 32_768L, 131_072L, 524_288L,
		2_097_152L, 8_388_608L, 33_554_432L, 134_217_728L, 536_870_912L, Int32.MaxValue - 7,
		8_589_934_592L
	};
	
	public static readonly long[] VoltagePractical = 
		Voltage
			.Select(i => (long)((i * 30) / 32))
			.ToArray();
}