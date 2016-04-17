using System; 
using System.Drawing;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;

namespace YComponents
{
	/// <summary>
	/// Ipad index view.
	/// Vista para el indice del ModuloLector
	/// </summary>
	public class IpadIndexView : UIView
	{
		public IpadIndexView ()
		{
			Frame = new CGRect (0, 0, 1024, 768);

			initMainView ();
			initBackView ();
			GTParallaxScroll pscroll = new GTParallaxScroll (0, 0, 1024, 768, GTScrollOrientation.Horizontal);
			pscroll.SetContent (mainView, mainWidth);
			pscroll.SetBackContent (backView, backWidth);
			Add (pscroll);
		}



		UIView mainView, backView  ; 
		nfloat mainWidth, backWidth ;
		void initMainView()
		{
			//static 
			nfloat lowidth =  660 , iwidth = 390 , pos = 0;
			mainView = new UIView (new CGRect (0, 0, lowidth * 3 +iwidth * 4*3, 768));

			for (int i = 0; i < 3; i++) { 
				pos += lowidth;
				for (int j = 0; j < 4; j++) { 
					var page = new PageIndexView (pos, 0);
					page.Index = j;
					page.Section = i;
					page.PageIndexTapped += delegate(object sender, int idx) {
						int b = (sender as PageIndexView).Section ;
					}; 
					//page.Alpha = (nfloat)0.5;
					for (int k = 0; k < 7; k++)
						page.addSlideButton (new UISlideButton (40 * k, 0, 36, 36){ ImageUrl = "assets/icons/contenido" + (k + 1) + ".png" }); 
					mainView.Add (page);
					pos += iwidth;
				} 
			}
  
			mainWidth = lowidth * 3 +iwidth * 4*3;

		  
		}

 

		void initBackView()
		{
			nfloat lowidth =  660/2 , iwidth = 390/2 , pos = 0/2;
			backView = new UIView (new CGRect(0,0,(lowidth + 4* iwidth) *3,768));


			for (int i = 0; i < 3; i++) { 
				UIImageView image = new UIImageView (new CGRect (pos,0,1366,768));
				image.Image = UIImage.FromFile ("assets/00"+(i+1)+"fondo.png");
				image.ContentMode = UIViewContentMode.ScaleAspectFit;
				backView.Add (image);
				pos += (lowidth + 4* iwidth);
			}

			//backView.Frame = new CGRect (0,0,pos,768);
			backWidth = pos;
		}
	}
}

