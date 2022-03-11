using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	[SerializeField]
	private Text fpsTextBox;

	private float time = 0;
	private float fpsTotal = 0;
	private int frameSteps = 0;

	void Update()
	{
		fpsTotal += 1f / Time.unscaledDeltaTime;
		++frameSteps;

		// Average every second
		if (Time.time > time)
		{
			time = Time.time + 0.5f;

			fpsTextBox.text = Mathf.Round(fpsTotal / frameSteps) + " FPS";
			fpsTotal = 0;
			frameSteps = 0;
		}
	}
}