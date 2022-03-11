using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosTile
{
	// Measured in 1/1000th moles
	public int[] moles = new int[4] { 0, 0, 0, 0 };

	/* 
	 * Flux is how much material flows in each cardinal direction
	 * 
	 * Velocity is the sum of flux to determine wind speed and direction
	 */
	public Vector4Int flux = Vector4Int.zero;
	public Vector2 velocity = Vector2.zero;

	public int GetMoles()
	{
		int m = 0;
		foreach (int i in moles)
		{
			m += i;
		}
		return m;
	}

	public int GetRandomGas()
	{
		int gas = Random.Range(0, GetMoles());

		int mole = 0;
		for (int i = 0; i < 4; ++i)
		{
			mole += moles[i];

			if (gas < mole)
			{
				return i;
			}
		}

		return 0;
	}
}