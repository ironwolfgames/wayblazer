using Xunit;

namespace Wayblazer.Tests;

public class PlanetaryConstantsTests
{
	[Fact]
	public void Constructor_SetsAllProperties()
	{
		var constants = new PlanetaryConstants(
			gravity: 9.8f,
			atmosphericPressure: 7.5f,
			atmosphericCorrosion: 0.6f,
			tectonicVolatility: 0.8f,
			lowTemperature: -100f,
			highTemperature: 150f);

		Assert.Equal(9.8f, constants.Gravity);
		Assert.Equal(7.5f, constants.AtmosphericPressure);
		Assert.Equal(0.6f, constants.AtmosphericCorrosion);
		Assert.Equal(0.8f, constants.TectonicVolatility);
		Assert.Equal(-100f, constants.LowTemperature);
		Assert.Equal(150f, constants.HighTemperature);
	}

	[Fact]
	public void Default_ReturnsExpectedValues()
	{
		var defaultConstants = PlanetaryConstants.Default;

		Assert.Equal(5f, defaultConstants.Gravity);
		Assert.Equal(5f, defaultConstants.AtmosphericPressure);
		Assert.Equal(1f, defaultConstants.AtmosphericCorrosion);
		Assert.Equal(3f, defaultConstants.TectonicVolatility);
		Assert.Equal(-50f, defaultConstants.LowTemperature);
		Assert.Equal(50f, defaultConstants.HighTemperature);
	}

	[Fact]
	public void Default_IsNotNull()
	{
		var defaultConstants = PlanetaryConstants.Default;

		Assert.NotNull(defaultConstants);
	}

	[Theory]
	[InlineData(0f, 0f, 0f, 0f, -273f, 0f)]
	[InlineData(10f, 10f, 1f, 1f, -100f, 100f)]
	[InlineData(5f, 5f, 0.5f, 0.5f, 0f, 50f)]
	public void Constructor_HandlesVariousValidInputs(
		float gravity, float pressure, float corrosion,
		float volatility, float lowTemp, float highTemp)
	{
		var constants = new PlanetaryConstants(
			gravity, pressure, corrosion, volatility, lowTemp, highTemp);

		Assert.Equal(gravity, constants.Gravity);
		Assert.Equal(pressure, constants.AtmosphericPressure);
		Assert.Equal(corrosion, constants.AtmosphericCorrosion);
		Assert.Equal(volatility, constants.TectonicVolatility);
		Assert.Equal(lowTemp, constants.LowTemperature);
		Assert.Equal(highTemp, constants.HighTemperature);
	}
}
