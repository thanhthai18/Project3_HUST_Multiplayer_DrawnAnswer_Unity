using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace unitycoder_MobilePaint
{
	public class CustomPatternsUI : MonoBehaviour {
		
		MobilePaint mobilePaint;
		public Button buttonTemplate;
		
		[SerializeField] private int padding = 8;
		
		void Start () 
		{
			mobilePaint = PaintManager.mobilePaint;

			if (mobilePaint==null) Debug.LogError("No MobilePaint assigned at "+transform.name);
			if (buttonTemplate==null) Debug.LogError("No buttonTemplate assigned at "+transform.name);
			
			// build custom brush buttons for each custom brush
			Vector2 newPos = new Vector2(padding,-padding);
			
			for(int i=0;i<mobilePaint.customPatterns.Length;i++)
			{
				// instantiate buttons
				var newButton = Instantiate(buttonTemplate,Vector3.zero,Quaternion.identity) as Button;
				newButton.transform.SetParent(transform,false);
				
				RectTransform rectTrans = newButton.GetComponent<RectTransform>();
				
				// wrap inside panel width
				if (newPos.x+rectTrans.rect.width>=GetComponent<RectTransform>().rect.width)
				{
					newPos.x=0+padding;
					newPos.y -= rectTrans.rect.height+padding;
					// NOTE: maximum Y is not checked..so dont put too many custom brushes.. would need to add paging or scrolling
				}
				rectTrans.anchoredPosition = newPos;
				newPos.x += rectTrans.rect.width+padding;
				
				// assign brush image
				// NOTE: have to use rawimage, instead of image (because cannot cast Texture2D into Image)
				// i've read that rawimage causes extra drawcall per drawimage, thats not nice if there are tens of images..
				newButton.GetComponent<RawImage>().texture = mobilePaint.customPatterns[i];
				var index = i;
				
				// event listener for button clicks, pass custom brush array index number as parameter
				newButton.onClick.AddListener(delegate {this.SetCustomPattern(index);});
				
				
			}
		}
		
		// send current brush index to mobilepaint
		public void SetCustomPattern(int index)
		{
			mobilePaint.selectedPattern = index;
			mobilePaint.ReadCurrentCustomPattern(); // tell mobile paint to read custom brush pixel data
			ClosePanel();
		}
		
		public void ClosePanel()
		{
			gameObject.SetActive(false);
		}
		
		public void OpenPanel()
		{
			gameObject.SetActive(true);
		}
		
	}
}