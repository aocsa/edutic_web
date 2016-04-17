using System; 
using System.Drawing;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;

namespace YComponents
{

	/// <summary>
	/// Single image slide view.
	/// type = 1
	/// </summary>
	public class SingleImageSlideView : UIView,ISlideView
	{
		public SingleImageSlideView  (nfloat pos) : base()
		{
			slidePos = pos;
			initView ();
		}

		nfloat slideHeight = 0 ;
		nfloat slidePos =0 ;

		#region ISlideView implementation

		public nfloat GetHeight ()
		{
			throw new NotImplementedException ();
		}

		public nfloat GetPosition ()
		{
			throw new NotImplementedException ();
		}

		#endregion

		UIView mainView ;
		UILabel titleLabel, subtitleLabel, authorLabel,contentLabel ;
		nfloat contentHeight = 0 ;
		UIImageView image ;

		void initView()
		{
			//content
			initResizableText ();

			mainView = new UIView (new CGRect(112,40,800, 136 +  contentHeight+ 46));
			//mainView.Layer.BorderColor = new CGColor (48, 48, 48);
			mainView.Layer.BorderWidth = 1;
			//mainView.Layer.MasksToBounds = false;
			Add (mainView);

			//set frame
			slideHeight = mainView.Frame.Size.Height + 80;
			var frame = new CGRect(0,slidePos,YConstants.DeviceWidht, slideHeight);
			Frame = frame;

			//BackgroundColor = UIColor.Red;

			//init controls
			mainView.Add(contentLabel);

			//titleLabel = YConstants.GetNewTextLabel(26,26,256,22,22,UIColor.Black,1);
			//titleLabel.Text = "Aves Tipicas";
			//mainView.Add (titleLabel);

			subtitleLabel = YConstants.GetNewTextLabel (134, 46, 600, 48,32, UIColor.Purple, 2);
			subtitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
			subtitleLabel.Text = "Diferentes tipos de Aves en el Peru";
			mainView.Add (subtitleLabel);

			authorLabel = YConstants.GetNewTextLabel (134, 104, 376, 14, 16, UIColor.Gray, 1);
			authorLabel.Text = "Autor del Articulo";
			mainView.Add (authorLabel);

			//image
			image = new UIImageView(new CGRect(26,26,92,92)){ContentMode = UIViewContentMode.ScaleToFill};
			image.Image = UIImage.FromFile ("MyImage.png");
			mainView.Add (image);

		}

		void initResizableText()
		{
			contentLabel = new UILabel (new CGRect(26,146, 748, 200));
			contentLabel.LineBreakMode = UILineBreakMode.WordWrap; 
			contentLabel.TextColor = UIColor.Gray;
			contentLabel.Font = UIFont.FromName (YConstants.FontName, 24); 
			//mainView.Add (contentLabel);

			contentLabel.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. quis nostrud exercitation END";
			contentHeight = YConstants.ResizeHeigthWithText(contentLabel,maxHeight:960f); 
		}

	}
}

