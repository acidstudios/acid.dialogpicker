using System;

using UIKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace AcidStudios.iOS.Dialog
{
	/// <summary>
    /// Abstract Class to Show a T Picker inside a Modal Window.
	/// T can be a Standar UIView, so it can hold any View.
	/// K is the return value that OnValueSelected will hold.
    /// </summary>
	public abstract class BaseDialogPicker<T, K>: UIView where T : UIView
	{
		#region Constants
		public nfloat kDefaultButtonHeight = (nfloat)50.0;
		public nfloat kDefaultButtonSpacerHeight = (nfloat)1.0;
		public nfloat kCornerRadius = (nfloat)7.0;
		public const int kDoneButton = 1;
		public const int kPickerDialogDoneButtonTag = 1;
		#endregion

		#region Properties
		protected UIView dialogView;
		protected UILabel titleLabel;
		protected T picker;
		protected UIButton cancelButton;
		protected UIButton doneButton;
  
		protected Action<K> OnValueSelected;
		protected bool showCancelButton;

		protected UIColor textColor;
		protected UIColor buttonColor;
		protected UIColor cancelButtonColor;
		protected UIFont font;
		#endregion

		#region Base Constructors
		public BaseDialogPicker(NSCoder coder) : base(coder) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.BaseDialogPicker`2"/> class.
        /// </summary>
		/// <param name="showCancelButton">Display or Hide the Cancel Button.</param>
		public BaseDialogPicker(bool showCancelButton): this(UIColor.Black, UIColor.Blue, UIColor.Red, UIFont.SystemFontOfSize((nfloat)17.0), showCancelButton) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.BaseDialogPicker`2"/> class.
        /// </summary>
        /// <param name="textColor">Text color.</param>
        /// <param name="buttonColor">Button color.</param>
        /// <param name="cancelButtonColor">Cancel button color.</param>
		/// <param name="font">Font used for Text in Modal Window.</param>
		/// <param name="showCancelButton">Display or Hide the Cancel Button.</param>
		public BaseDialogPicker(UIColor textColor, UIColor buttonColor, UIColor cancelButtonColor, UIFont font, bool showCancelButton) : base(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, UIScreen.MainScreen.Bounds.Height))
		{
			this.textColor = textColor ?? UIColor.Black;
			this.buttonColor = buttonColor ?? UIColor.Blue;
			this.cancelButtonColor = cancelButtonColor ?? UIColor.Red;
			this.font = font ?? UIFont.SystemFontOfSize((nfloat)17.0);
			this.showCancelButton = showCancelButton;
			this.SetupViews();
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Show the Modal Picker.
        /// </summary>
        /// <param name="title">Window Title.</param>
        /// <param name="doneButtonTitle">Done button title.</param>
        /// <param name="cancelButtonTitle">Cancel button title.</param>
        /// <param name="callback">Callback that return selected value in Picker.</param>
		public void Show(string title, string doneButtonTitle, string cancelButtonTitle, Action<K> callback)
		{
			this.OnValueSelected = callback;
			this.titleLabel.Text = title;
			this.doneButton?.SetTitle(doneButtonTitle, UIControlState.Normal);

			if(this.showCancelButton)
			{
				this.cancelButton.SetTitle(cancelButtonTitle, UIControlState.Normal);
			}
                    
			var del = UIApplication.SharedApplication.Delegate;
            var window = del.GetWindow();
            window.AddSubview(this);
            window.BringSubviewToFront(this);
            window.EndEditing(true);

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceOrientationDidChangeNotification"), (NSNotification obj) => {
                CGSize screenSize = this.CountScreenSize();
                CGSize dialogSize = new CGSize(300, 230 + kDefaultButtonHeight + kDefaultButtonSpacerHeight);

                this.Frame = new CGRect(0, 0, screenSize.Width, screenSize.Height);
                dialogView.Frame = new CGRect((screenSize.Width - dialogSize.Width) / 2.0, (screenSize.Height - dialogSize.Height) / 2.0, dialogSize.Width, dialogSize.Height);
            });


            Animate(0.2, 0.0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                this.BackgroundColor = UIColor.FromRGBA(0, 0, 0, (nfloat)0.4);
                this.dialogView.Layer.Opacity = 1;
                this.dialogView.Layer.Transform = CATransform3D.MakeScale(1, 1, 1);
            }, () => { });

		}

        /// <summary>
        /// Close the Modal Window and removes it from Super View
        /// </summary>
		public void Close()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this, "UIDeviceOrientationDidChangeNotification", null);
            CATransform3D currentTransform = this.dialogView.Layer.Transform;

            nfloat startRotation = nfloat.Parse(this.ValueForKeyPath(new NSString("layer.transform.rotation.z")).ToString());
            CATransform3D rotation = CATransform3D.MakeRotation((nfloat)(-startRotation + Math.PI * 270 / 180), 0, 0, 0);

            CATransform3D scale = CATransform3D.MakeScale((nfloat)1, (nfloat)1, 1);
            this.dialogView.Layer.Transform = currentTransform.Concat(scale);
            this.dialogView.Layer.Opacity = 1;

            UIView.Animate(0.2, 0, UIViewAnimationOptions.TransitionNone, () =>
            {
                this.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
                this.dialogView.Layer.Transform = currentTransform.Concat(CATransform3D.MakeScale((nfloat)0.6, (nfloat)0.6, 1));
                this.dialogView.Layer.Opacity = 0;
            }, () =>
            {
                foreach (UIView v in this.Subviews)
                {
                    v.RemoveFromSuperview();
                }

                this.RemoveFromSuperview();
                this.SetupViews();
            });
        }
		#endregion

		#region Abstract Methods
        /// <summary>
        /// Creates a Picker View.
		/// Please implement it.
        /// </summary>
        /// <returns>The picker view.</returns>
		abstract public T CreatePickerView();
		abstract public void OnButtonTapped(object sender, EventArgs eventArgs); //void ButtonTapped(Object sender, EventArgs eventArgs)
        #endregion

		#region Private Methods
		void SetupViews()
		{
			this.dialogView = this.CreateContainerView();
            this.dialogView.Layer.ShouldRasterize = true;
            this.dialogView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;

            this.Layer.ShouldRasterize = true;
            this.Layer.RasterizationScale = UIScreen.MainScreen.Scale;

            this.dialogView.Layer.Opacity = 0.5f;
            this.dialogView.Layer.Transform = CoreAnimation.CATransform3D.MakeScale((nfloat)1.3, (nfloat)1.3, 1);

            this.BackgroundColor = new UIColor(0, 0, 0, 0);

            this.AddSubview(this.dialogView);
		}

		UIView CreateContainerView()
		{
			CGSize screenSize = this.CountScreenSize();
            CGSize dialogSize = new CGSize(300, 230 + kDefaultButtonHeight + kDefaultButtonSpacerHeight);

            this.Frame = new CGRect(0, 0, screenSize.Width, screenSize.Height);

            UIView dialogContainer = new UIView(new CGRect((screenSize.Width - dialogSize.Width) / 2.0, (screenSize.Height - dialogSize.Height) / 2.0, dialogSize.Width, dialogSize.Height));
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Frame = dialogContainer.Bounds,
                Colors = new CGColor[]{
                    UIColor.FromRGBA((nfloat)(218.0 / 255.0), (nfloat)(218.0 / 255.0), (nfloat)(218.0 / 255), 1).CGColor,
                    UIColor.FromRGB((nfloat)(233.0 / 255.0), (nfloat)(233.0 / 255.0), (nfloat)(233.0 / 255)).CGColor,
                    UIColor.FromRGB((nfloat)(218.0 / 255.0), (nfloat)(218.0 / 255.0), (nfloat)(218.0 / 255)).CGColor
                }
            };

            gradientLayer.CornerRadius = kCornerRadius;
            dialogContainer.Layer.InsertSublayer(gradientLayer, 0);

            dialogContainer.Layer.CornerRadius = kCornerRadius;
            dialogContainer.Layer.BorderColor = UIColor.FromRGB((nfloat)(198.0 / 255.0), (nfloat)(198.0 / 255.0), (nfloat)(198.0 / 255.0)).CGColor;
            dialogContainer.Layer.BorderWidth = 1;
            dialogContainer.Layer.ShadowRadius = (nfloat)(kCornerRadius + 5.0);
            dialogContainer.Layer.ShadowOffset = new CGSize(0 - (kCornerRadius + 5.0) / 2.0, 0 - (kCornerRadius + 5.0) / 2.0);
            dialogContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            dialogContainer.Layer.ShadowPath = UIBezierPath.FromRoundedRect(dialogContainer.Bounds, dialogContainer.Layer.CornerRadius).CGPath;

            UIView lineView = new UIView(new CGRect(0, dialogContainer.Bounds.Size.Height - kDefaultButtonHeight - kDefaultButtonSpacerHeight, dialogContainer.Bounds.Size.Width, kDefaultButtonSpacerHeight))
            {
                BackgroundColor = UIColor.FromRGB((nfloat)(198.0 / 255.0), ((nfloat)(198.0 / 255.0)), ((nfloat)(198.0 / 255.0)))
            };
            dialogContainer.AddSubview(lineView);

            UILabel titleLabel = new UILabel(new CGRect(10, 10, 280, 30))
            {
                TextAlignment = UIKit.UITextAlignment.Center,
                TextColor = this.textColor ?? UIColor.Black,
                Font = this.font?.WithSize(17.0f) ?? UIFont.SystemFontOfSize(17.0f)
            };

            this.titleLabel = titleLabel;
            dialogContainer.AddSubview(titleLabel);

			T picker = this.CreatePickerView();
			picker.SetValueForKeyPath((NSObject)(this.textColor ?? UIColor.Black), new NSString("textColor"));
			picker.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;

			dialogContainer.AddSubview(picker);
			this.picker = picker;

            this.AddButtonsToView(dialogContainer);

            return dialogContainer;
		}

		void AddButtonsToView(UIView view)
		{
			nfloat buttonWidth = view.Bounds.Size.Width / 2;

            CGRect leftButtonFrame = new CGRect(0, view.Bounds.Size.Height - kDefaultButtonHeight, buttonWidth, kDefaultButtonHeight);
            CGRect rightButtonFrame = new CGRect(buttonWidth, view.Bounds.Size.Height - kDefaultButtonHeight, buttonWidth, kDefaultButtonHeight);

            if (!this.showCancelButton)
            {
                buttonWidth = view.Bounds.Size.Width;
                leftButtonFrame = CGRect.Empty;
                rightButtonFrame = new CGRect(0, view.Bounds.Size.Height - kDefaultButtonHeight, buttonWidth, kDefaultButtonHeight);
            }

            UIUserInterfaceLayoutDirection interfaceLayoutDirection = UIApplication.SharedApplication.UserInterfaceLayoutDirection;
            bool isLeftToRightDirection = interfaceLayoutDirection == UIUserInterfaceLayoutDirection.LeftToRight;

            if (this.showCancelButton)
            {
                UIButton cancelButton = new UIButton(UIButtonType.Custom)
                {
                    Frame = isLeftToRightDirection ? leftButtonFrame : rightButtonFrame
                };
                cancelButton.SetTitleColor(this.cancelButtonColor, UIControlState.Normal);
                cancelButton.SetTitleColor(this.cancelButtonColor, UIControlState.Highlighted);
                cancelButton.TitleLabel.Font = this.font?.WithSize((nfloat)14.0) ?? UIFont.SystemFontOfSize((nfloat)14.0);
                cancelButton.Layer.CornerRadius = kCornerRadius;
				cancelButton.AddTarget(OnButtonTapped, UIControlEvent.TouchUpInside);
                view.AddSubview(cancelButton);
                this.cancelButton = cancelButton;
            }

            UIButton doneButton = new UIButton(UIButtonType.Custom)
            {
				Tag = kPickerDialogDoneButtonTag,
                Frame = isLeftToRightDirection ? rightButtonFrame : leftButtonFrame
            };
            doneButton.SetTitleColor(this.buttonColor, UIControlState.Normal);
            doneButton.SetTitleColor(this.buttonColor, UIControlState.Highlighted);
            doneButton.TitleLabel.Font = this.font?.WithSize((nfloat)14.0) ?? UIFont.SystemFontOfSize((nfloat)14.0);
            doneButton.Layer.CornerRadius = kCornerRadius;
			doneButton.AddTarget(OnButtonTapped, UIControlEvent.TouchUpInside);
            view.AddSubview(doneButton);
            this.doneButton = doneButton;
		}

		CGSize CountScreenSize()
        {
            nfloat screenWidth = UIScreen.MainScreen.Bounds.Size.Width;
            nfloat screenHeight = UIScreen.MainScreen.Bounds.Size.Height;

            return new CGSize(screenWidth, screenHeight);
        }
		#endregion
	}
}
