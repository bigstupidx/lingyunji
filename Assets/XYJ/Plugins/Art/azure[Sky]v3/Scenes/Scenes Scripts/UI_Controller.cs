﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour {
	public AzureSky_Controller azureController;
	public Material skyMaterial;
	public Slider timelineSlider;
	public Slider cloudCovarage;
	public Slider timeToRefreshGI;

	// Update is called once per frame
	void Update ()
	{
		azureController.TIME_of_DAY = timelineSlider.value;
		azureController.ReflectionProbeTimeToUpdate = timeToRefreshGI.value;
		skyMaterial.SetFloat ("_WispyCovarage", cloudCovarage.value);
	}
}
