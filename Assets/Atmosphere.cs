using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere
{
	public readonly int width;
	public readonly int height;

	private AtmosTile[,] grid;

	private const float drag = 0.98f;
	private const float accel = 0.1f;
	private const int maxFlow = 1000000;
	private const int wiggle = 5;
	private const int diffusionRate = 5;

	public Atmosphere(int width, int height)
	{
		this.width = width;
		this.height = height;

		grid = new AtmosTile[width, height];

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				grid[x, y] = new AtmosTile();
			}
		}
	}

	public void CalculateFlux()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				AtmosTile tile = grid[x, y];

				// North flux
				if (y < height - 1)
				{
					tile.flux.x = CFlux(tile.flux.x, tile, grid[x, y + 1]);
				}
				// South flux
				if (y > 0)
				{
					tile.flux.y = CFlux(tile.flux.y, tile, grid[x, y - 1]);
				}
				// East flux
				if (x < width - 1)
				{
					tile.flux.z = CFlux(tile.flux.z, tile, grid[x + 1, y]);
				}
				// West flux
				if (x > 0)
				{
					tile.flux.w = CFlux(tile.flux.w, tile, grid[x - 1, y]);
				}
			}
		}
	}

	private int CFlux(int currentFlux, AtmosTile tile, AtmosTile neighborTile)
	{
		int diff = tile.GetMoles() - neighborTile.GetMoles();
		int flux = Mathf.FloorToInt(currentFlux * drag + diff * accel);
		return Mathf.Clamp(flux, 0, maxFlow);
	}

	public void SimulateFlux()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				AtmosTile tile = grid[x, y];

				if (y < height - 1)
				{
					SFlux(tile.flux.x, tile, grid[x, y + 1]);
				}
				// South flux
				if (y > 0)
				{
					SFlux(tile.flux.y, tile, grid[x, y - 1]);
				}
				// East flux
				if (x < width - 1)
				{
					SFlux(tile.flux.z, tile, grid[x + 1, y]);
				}
				// West flux
				if (x > 0)
				{
					SFlux(tile.flux.w, tile, grid[x - 1, y]);
				}
			}
		}
	}

	private void SFlux(int flux, AtmosTile tile, AtmosTile neighborTile)
	{
		float totalMoles = tile.GetMoles();

		for (int i = 0; i < 4; ++i)
		{
			if (tile.moles[i] == 0)
			{
				continue;
			}

			// Total moles changes after each gas so we need
			// a fixed value to ensure it transfers properly
			float ratio = tile.moles[i] / totalMoles;

			int moles = Mathf.Min(Mathf.FloorToInt(flux * ratio), tile.moles[i]);
			tile.moles[i] -= moles;
			neighborTile.moles[i] += moles;
		}
	}

	public void WiggleDiffusion()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				AtmosTile tile = grid[x, y];

				tile.flux.x += Random.Range(0, wiggle);
				tile.flux.y += Random.Range(0, wiggle);
				tile.flux.z += Random.Range(0, wiggle);
				tile.flux.w += Random.Range(0, wiggle);
			}
		}
	}

	public void SimulateDiffusion()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				AtmosTile tile = grid[x, y];

				// North diffusion
				if (y < height - 1)
				{
					SDiffusion(tile, grid[x, y + 1]);
				}
				// South diffusion
				if (y > 0)
				{
					SDiffusion(tile, grid[x, y - 1]);
				}
				// East diffusion
				if (x < width - 1)
				{
					SDiffusion(tile, grid[x + 1, y]);
				}
				// West diffusion
				if (x > 0)
				{
					SDiffusion(tile, grid[x - 1, y]);
				}
			}
		}
	}

	private void SDiffusion(AtmosTile tile, AtmosTile neighborTile)
	{
		int gas1 = tile.GetRandomGas();
		int gas2 = neighborTile.GetRandomGas();
		if (gas1 == gas2) { return; }

		int rand = Random.Range(1, diffusionRate);

		if (tile.moles[gas1] >= rand && neighborTile.moles[gas2] >= rand)
		{
			tile.moles[gas1] -= rand;
			neighborTile.moles[gas1] += rand;

			tile.moles[gas2] += rand;
			neighborTile.moles[gas2] -= rand;
		}
	}

	public void SimulateDiffusionBad()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				AtmosTile tile = grid[x, y];

				for (int i = 0; i < 4; ++i)
				{
					if (tile.moles[i] == 0)
					{
						continue;
					}

					// North diffusion
					if (y < height - 1)
					{
						int diff = Mathf.Max(0, grid[x, y].moles[i] - grid[x, y + 1].moles[i]);
						diff = Mathf.FloorToInt(diff * 0.5f);

						grid[x, y].moles[i] -= diff;
						grid[x, y + 1].moles[i] += diff;
					}
					// South diffusion
					if (y > 0)
					{
						int diff = Mathf.Max(0, grid[x, y].moles[i] - grid[x, y- 1].moles[i]);
						diff = Mathf.FloorToInt(diff * 0.5f);

						grid[x, y].moles[i] -= diff;
						grid[x, y - 1].moles[i] += diff;
					}
					// East diffusion
					if (x < width - 1)
					{
						int diff = Mathf.Max(0, grid[x, y].moles[i] - grid[x + 1, y].moles[i]);
						diff = Mathf.FloorToInt(diff * 0.5f);

						grid[x, y].moles[i] -= diff;
						grid[x + 1, y].moles[i] += diff;
					}
					// West diffusion
					if (x > 0)
					{
						int diff = Mathf.Max(0, grid[x, y].moles[i] - grid[x - 1, y].moles[i]);
						diff = Mathf.FloorToInt(diff * 0.5f);

						grid[x, y].moles[i] -= diff;
						grid[x - 1, y].moles[i] += diff;
					}
				}
			}
		}
	}

	public void ResetFlux()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				grid[x, y].flux = Vector4Int.zero;
			}
		}
	}

	public void SetFlux(int x, int y, Vector4Int flux)
	{
		grid[x, y].flux = flux;
	}

	public void AddFlux(int x, int y, Vector4Int flux)
	{
		grid[x, y].flux += flux;
	}

	public int GetMoles(int x, int y, int index)
	{
		return grid[x, y].moles[index];
	}

	public int GetTotalMoles(int x, int y)
	{
		return grid[x, y].GetMoles();
	}

	public void SetMoles(int x, int y, int index, int amount)
	{
		grid[x, y].moles[index] = amount;
	}

	public void AddMoles(int x, int y, int index, int amount)
	{
		grid[x, y].moles[index] += amount;
	}
}