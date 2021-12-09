﻿using UnityEngine;
using System.Collections;

namespace unitycoder_MobilePaint
{

	public class SetNewCanvasImageExample : MonoBehaviour 
	{
		MobilePaint mobilePaint;

		public Texture2D[] images;
		int index=0;

		void Start () 
		{
			mobilePaint = PaintManager.mobilePaint;
			if (mobilePaint==null) Debug.LogError("MobilePaint not assigned", gameObject);
			if (images==null || images.Length<1) Debug.LogError("No mask images assigned to array", gameObject);
		}

		public void NextImage()
		{
			index = (index+1) % images.Length; // wrap around
			mobilePaint.SetCanvasImage(images[index]);
		}

	}
}