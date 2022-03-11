using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Gases
{
	public enum Gas { Oxygen, CarbonDioxide, Nitrogen, Hydrogen }

	/*
	 * Specific heat capacity is the heat capacity of a substance
	 * divided by its mass.
	 * 
	 * joule per kelvin per kilogram
	 * J/K/kg
	 * 
	 * It takes	4184 joules to heat up 1kg of water by 1 kelvin.
	 */
	public static int GetSpecificHeat(Gas gas)
	{
		switch (gas)
		{
			case Gas.Oxygen: return 0;
			case Gas.CarbonDioxide: return 0;
			case Gas.Nitrogen: return 0;
			case Gas.Hydrogen: return 14300;
			default: return 0;
		}
	}
}