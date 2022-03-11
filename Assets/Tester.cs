using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tester : MonoBehaviour
{
	private Atmosphere atmosphere;

	[SerializeField]
	private int width = 16;
	[SerializeField]
	private int height = 16;

	private float time = 0;
	private const float timeStep = 1f/60f;

	private int gasType = 0;
	[SerializeField]
	private Text gasTextBox;
	[SerializeField]
	private Text amountTextBox;

	private Plane floor = new Plane(Vector3.up, Vector3.zero);
	private Vector2Int mousePos = Vector2Int.zero;
	private bool mousePosIsValid = false;

	void Start()
	{
		atmosphere = new Atmosphere(width, height);
	}

	void Update()
	{
		// Raycast to plane
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		mousePos = Vector2Int.zero;
		mousePosIsValid = false;

		if (floor.Raycast(ray, out float enter))
		{
			Vector3 hitPos = ray.GetPoint(enter);
			mousePos = new Vector2Int(Mathf.FloorToInt(hitPos.x + 0.5f),
				Mathf.FloorToInt(hitPos.z + 0.5f));
			
			// Check if mouse pos is valid
			if (mousePos.x >= 0 && mousePos.y >= 0 &&
				mousePos.x < atmosphere.width &&
				mousePos.y < atmosphere.height)
			{
				mousePosIsValid = true;
			}
		}

		// Gas selection
		Vector2 scroll = Input.mouseScrollDelta;
		if (scroll.y == -1f)
		{
			++gasType;
			if (gasType == 4) { gasType = 0; }
		}
		else if (scroll.y == 1f)
		{
			--gasType;
			if (gasType == -1) { gasType = 3; }
		}

		switch (gasType)
		{
			case 0: gasTextBox.text = "Oxygen"; break;
			case 1: gasTextBox.text = "Carbon Dioxide"; break;
			case 2: gasTextBox.text = "Nitrogen"; break;
			case 3: gasTextBox.text = "Hydrogen"; break;
		}

		int amountToAdd = Input.GetKey(KeyCode.LeftShift) ? 50 : 10;
		amountTextBox.text = amountToAdd + " Moles";

		// Simulate atmosphere
		if (Time.time > time)
		{
			time = Time.time + timeStep;

			atmosphere.CalculateFlux();
			atmosphere.SimulateFlux();
			atmosphere.SimulateDiffusion();

			if (mousePosIsValid)
			{
				if (Input.GetMouseButton(0)) // Add gas
				{
					atmosphere.AddMoles(mousePos.x, mousePos.y, gasType, amountToAdd * 1000);
				}
				if (Input.GetMouseButton(1)) // Remove gas
				{
					atmosphere.SetMoles(mousePos.x, mousePos.y, 0, 0);
					atmosphere.SetMoles(mousePos.x, mousePos.y, 1, 0);
					atmosphere.SetMoles(mousePos.x, mousePos.y, 2, 0);
					atmosphere.SetMoles(mousePos.x, mousePos.y, 3, 0);
				}
				if (Input.GetMouseButton(3)) // Stir gas
				{
					atmosphere.SetFlux(mousePos.x, mousePos.y, new Vector4Int(3000, 3000, 3000, 3000));
				}
			}

			if (Input.GetKey("g")) // Blow gas
			{
				atmosphere.AddFlux(6, 0, new Vector4Int(0, 0, 1000, 0));
				atmosphere.AddFlux(7, 0, new Vector4Int(0, 0, 750, 0));
				atmosphere.AddFlux(8, 0, new Vector4Int(0, 0, 500, 0));
				atmosphere.AddFlux(9, 0, new Vector4Int(0, 0, 250, 0));
			}
		}

		// Hotkeys
		if (Input.GetKeyDown("q")) // Measure moles
		{
			MeasureMoles();
		}

		if (Input.GetKeyDown("z")) // Clear
		{
			atmosphere.ResetFlux();
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					atmosphere.SetMoles(x, y, 0, 0);
					atmosphere.SetMoles(x, y, 1, 0);
					atmosphere.SetMoles(x, y, 2, 0);
					atmosphere.SetMoles(x, y, 3, 0);
				}
			}
		}
		if (Input.GetKeyDown("x")) // Fill air
		{
			atmosphere.ResetFlux();
			int totalMoles = 7500;
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					atmosphere.SetMoles(x, y, 0, Mathf.FloorToInt(totalMoles * 0.21f));
					atmosphere.SetMoles(x, y, 1, 0);
					atmosphere.SetMoles(x, y, 2, Mathf.FloorToInt(totalMoles * 0.78f));
					atmosphere.SetMoles(x, y, 3, Mathf.FloorToInt(totalMoles * 0.01f));
				}
			}
		}
		if (Input.GetKeyDown("c")) // Fill random
		{
			atmosphere.ResetFlux();
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					atmosphere.SetMoles(x, y, 0, Random.Range(0, 3500));
					atmosphere.SetMoles(x, y, 1, Random.Range(0, 3500));
					atmosphere.SetMoles(x, y, 2, Random.Range(0, 3500));
					atmosphere.SetMoles(x, y, 3, Random.Range(0, 3500));
				}
			}
		}

		if (Input.GetKeyDown("v")) // Set wave
		{
			atmosphere.ResetFlux();
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					int moles = 0;
					if (x > width * 0.75f)
					{
						moles = (int)Mathf.Pow(x - width * 0.75f, 3) * 500 / width;
					}
					atmosphere.SetMoles(x, y, 0, moles);
					atmosphere.SetMoles(x, y, 1, 0);
					atmosphere.SetMoles(x, y, 2, 0);
					atmosphere.SetMoles(x, y, 3, 0);
				}
			}
		}
		if (Input.GetKeyDown("b")) // Set slant wave
		{
			atmosphere.ResetFlux();
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					int moles = x * y * 500 / width;

					atmosphere.SetMoles(x, y, 0, moles);
					atmosphere.SetMoles(x, y, 1, 0);
					atmosphere.SetMoles(x, y, 2, 0);
					atmosphere.SetMoles(x, y, 3, 0);
				}
			}
		}
		if (Input.GetKeyDown("n")) // Set mix wall
		{
			atmosphere.ResetFlux();
			for (int x = 0; x < width / 2; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					atmosphere.SetMoles(x, y, 0, 8000);
					atmosphere.SetMoles(x, y, 1, 0);
					atmosphere.SetMoles(x, y, 2, 0);
					atmosphere.SetMoles(x, y, 3, 0);
				}
			}
			for (int x = width / 2; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					atmosphere.SetMoles(x, y, 0, 0);
					atmosphere.SetMoles(x, y, 1, 8000);
					atmosphere.SetMoles(x, y, 2, 0);
					atmosphere.SetMoles(x, y, 3, 0);
				}
			}
		}

		if (Input.GetKeyDown("f")) // Random flux
		{
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					atmosphere.SetFlux(x, y, new Vector4Int(Random.Range(0, 5000),
						Random.Range(0, 5000), Random.Range(0, 5000),
						Random.Range(0, 5000)));
				}
			}
		}
	}
	
	private void MeasureMoles()
	{
		int moles = 0;

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				moles += atmosphere.GetMoles(x, y, 0);
			}
		}

		Debug.Log(moles);
	}

	private void OnDrawGizmos()
	{
		float moles = 0;

		// Grid outline
		Gizmos.color = Color.white;
		Gizmos.DrawLine(new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, height - 0.5f));
		Gizmos.DrawLine(new Vector3(-0.5f, 0, -0.5f), new Vector3(width - 0.5f, 0, -0.5f));
		Gizmos.DrawLine(new Vector3(width - 0.5f, 0, height - 0.5f), new Vector3(-0.5f, 0, height - 0.5f));
		Gizmos.DrawLine(new Vector3(width - 0.5f, 0, height - 0.5f), new Vector3(width - 0.5f, 0, -0.5f));

		for (int x = width - 1; x >= 0; --x)
		{
			for (int y = height - 1; y >= 0; --y)
			{
				float offset = 0f;
				for (int i = 0; i < 4; ++i)
				{
					if (i == 0) { Gizmos.color = Color.cyan; }
					else if (i == 1) { Gizmos.color = Color.gray; }
					else if (i == 2) { Gizmos.color = Color.red; }
					else if (i == 3) { Gizmos.color = new Color(1f, 0f, 1f); }

					moles = atmosphere.GetMoles(x, y, i) / 1000f;
					if (moles > 0)
					{
						Gizmos.DrawCube(new Vector3(x, moles / 2f + offset, y), new Vector3(1, moles, 1));
						offset += moles;
					}
				}
			}
		}
		
		moles = mousePosIsValid ? atmosphere.GetTotalMoles(mousePos.x, mousePos.y) / 1000f : 0;

		Gizmos.color = mousePosIsValid ? Color.white : Color.red;
		Gizmos.DrawWireCube(new Vector3(mousePos.x, moles / 2f, mousePos.y), new Vector3(1, moles, 1));
	}
}