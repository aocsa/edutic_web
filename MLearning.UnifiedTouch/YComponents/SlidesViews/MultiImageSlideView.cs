using System; 
using System.Drawing;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;

namespace YComponents
{
	/// <summary>
	/// Multi image slide view.
	/// type = 4 
	/// </summary>
	public class MultiImageSlideView : UIView,ISlideView
	{
		public MultiImageSlideView (nfloat pos) : base()
		{
			slidePos = pos;
			addTitle ();
			initImages ();
		}

		nfloat slideHeight = 0 ;
		nfloat slidePos =0 ;

		#region ISlideView implementation

		public nfloat GetHeight ()
		{
			return slideHeight;
		}

		public nfloat GetPosition ()
		{
			return slidePos;
		}

		#endregion

		// title 

		UILabel titleLabel ;
		void addTitle()
		{
			titleLabel = new UILabel (new CGRect(112,30 , 800 , 40 ));
			titleLabel.Text = "Galeria de Aves";
			titleLabel.TextColor = UIColor.Black;
			titleLabel.Font = UIFont.FromName ("HelveticaNeue", 32);
			Add (titleLabel);
		}


		//static size of images
		nfloat margin = 112 , separation = 4;
		nfloat width1 = 398, width2 = 264 ;
		nfloat height1 = 260, height2 = 260 ;

		int imageCount = 5 ; 

		void initImages()
		{
			var view1 = get2images (100);
			var view2 = get3images (100 + height1 + separation);
			Add (view1);
			Add (view2);

			//BackgroundColor = UIColor.Red;

			//set frame
			slideHeight= 140 + separation + 2 * height1;
			var frame = new CGRect(0,slidePos,YConstants.DeviceWidht,slideHeight);
			Frame = frame;
		}


		UIView get2images(nfloat pos)
		{
			var images = new UIView (new CGRect(margin, pos , 800, height1 ));
			for (int i = 0; i < 2; i++) {
				var img = new UIImageView (new CGRect(i * (width1+separation),0,width1, height1));
				img.ContentMode = UIViewContentMode.ScaleToFill;
				img.Image = UIImage.FromFile ("MyImage.png");
				SetNeedsDisplay ();
				images.Add (img);
			}
			return images;
		}


		UIView get3images (nfloat pos)
		{
			var images = new UIView (new CGRect(margin, pos , 800, height2 ));
			for (int i = 0; i < 3; i++) {
				var img = new UIImageView (new CGRect(i * (width2+separation),0,width2, height2));
				img.ContentMode = UIViewContentMode.ScaleToFill;
				img.Image = UIImage.FromFile ("MyImage.png");
				SetNeedsDisplay ();
				images.Add (img);
			}
			return images;
		}

	}
}

