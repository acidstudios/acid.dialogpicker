using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace AcidStudios.iOS.Dialog
{
	/// <summary>
    /// PickerDialog.
	/// Show a Standard UIPickerView inside a Modal Window.
    /// </summary>
	public class PickerDialog<T> : BaseDialogPicker<UIPickerView, nint>
	{
		#region Properties
        /// <summary>
        /// Component Count for UIPickerView.
		/// See UIPickerViewDataSource(numberOfComponentsInPickerView) in Apple Documentation.
		/// 
        /// </summary>
        /// <value>Component Count.</value>
		public Func<UIPickerView, nint> ComponentCount { get; set; }
        /// <summary>
		/// Rows count in a Component Count in UIPickerView
		/// See UIPickerViewDataSource(pickerView:numberOfRowsInComponent:) in Apple Documentation.
        /// </summary>
        /// <value>The rows in component.</value>
		public Func<UIPickerView, nint, nint> RowsInComponent { get; set; }
        /// <summary>
		/// Return the Title of the Picker Option in UIPickerView.
		/// See UIPickerViewDataSource(pickerView:titleForRow:forComponent:) in Apple Documentation.
        /// </summary>
        /// <value>The title for row.</value>
		public Func<UIPickerView, nint, nint, NSString> TitleForRow { get; set; }
        /// <summary>
		/// Return an Attributed String to fill Title in UIPickerView.
		/// See UIPickerViewDataSource(pickerView:attributedTitleForRow:forComponent:) in Apple Documentation.
        /// </summary>
        /// <value>The attributed title for row.</value>
		public Func<UIPickerView, nint, nint, NSAttributedString> AttributedTitleForRow { get; set; }
        /// <summary>
		/// Return a UIView to fill a Picker Option in UIPickerView
		/// See UIPickerViewDataSource(pickerView:viewForRow:forComponent:) in Apple Documentation.
        /// </summary>
        /// <value>The view for row.</value>
		public Func<UIPickerView, nint, nint, UIView> ViewForRow { get; set; }
		#endregion

		#region Constructor
		public PickerDialog(NSCoder coder) : base(coder) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.PickerDialog`1"/> class.
        /// </summary>
		/// <param name="showCancelButton">Display or Hide the Cancel Button.</param>
		public PickerDialog(bool showCancelButton) : this(null, null, null, null, showCancelButton) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.PickerDialog`1"/> class.
        /// </summary>
        /// <param name="textColor">Text color.</param>
        /// <param name="buttonColor">Button color.</param>
        /// <param name="cancelButtonColor">Cancel button color.</param>
        /// <param name="font">Font.</param>
		/// <param name="showCancelButton">Display or Hide the Cancel Button.</param>
		public PickerDialog(UIColor textColor, UIColor buttonColor, UIColor cancelButtonColor, UIFont font, bool showCancelButton) : base(textColor, buttonColor, cancelButtonColor, font, showCancelButton) { }
		#endregion

		#region Public Methods
        /// <summary>
        /// Show the UIPickerView in a Modal Window
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="doneButtonTitle">Done button title.</param>
        /// <param name="cancelButtonTitle">Cancel button title.</param>
        /// <param name="callback">Callback, will return a Index of the Row in the Array DataSource.</param>
		public new void Show(string title, string doneButtonTitle, string cancelButtonTitle, Action<nint> callback)
		{
			this.picker.DataSource = this;
			this.picker.WeakDelegate = this;

			base.Show(title, doneButtonTitle, cancelButtonTitle, callback);
		}
		#endregion
       
		#region Abstract Methods Implementation
		public override UIPickerView CreatePickerView() => new UIPickerView(new CGRect(0, 30, 300, 216));

		public override void OnButtonTapped(object sender, EventArgs eventArgs)
		{
			UIButton button = sender as UIButton;

			if (button.Tag == kPickerDialogDoneButtonTag)
			{

				this.OnValueSelected?.Invoke(this.picker.SelectedRowInComponent(0));
			}
			else
			{
				this.OnValueSelected?.Invoke(-1);
			}

			this.Close();
		}
		#endregion

		#region DataSource and WeakDelegate implementation
		[Export("numberOfComponentsInPickerView:")]
		nint GetComponentCount(UIPickerView pickerView)
		{
			if(this.ComponentCount != null) {
				return this.ComponentCount.Invoke(pickerView);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		[Export("pickerView:numberOfRowsInComponent:")]
	    nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			if(this.RowsInComponent != null)
			{
				return this.RowsInComponent.Invoke(pickerView, component);
			}
            else
            {
                throw new NotImplementedException();
            }
		}

		[Export("pickerView:titleForRow:forComponent:")]
		NSString GetTitleForRow(UIPickerView pickerView, nint row, nint component)
		{
			if(this.TitleForRow != null)
			{
				return this.TitleForRow.Invoke(pickerView, row, component);
			}
			return NSString.Empty;
		}
        
        [Export("pickerView:attributedTitleForRow:forComponent:")]
		NSAttributedString GetAttributedTitleForRow(UIPickerView pickerView, nint row, nint component)
        {
			if (this.AttributedTitleForRow != null)
            {
				return this.AttributedTitleForRow.Invoke(pickerView, row, component);
            }
			return null;
        }

        [Export("pickerView:viewForRow:forComponent:")]
		UIView GetViewForRow(UIPickerView pickerView, nint row, nint component)
        {
			if (this.ViewForRow != null)
            {
				return this.ViewForRow.Invoke(pickerView, row, component);
            }
			return null;
        }
        #endregion
	}
    
}