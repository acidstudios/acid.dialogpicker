using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace AcidStudios.iOS.Dialog
{
	/// <summary>
    /// DatePicker Dialog.
	/// Show a UIDatePickerView inside a Modal Window.
    /// </summary>
	public class DatePickerDialog : BaseDialogPicker<UIDatePicker, NSDate>
	{
		#region Properties
		NSDate defaultDate;
		UIDatePickerMode datePickerMode;
		NSLocale locale;
		#endregion

		#region Constructor
		public DatePickerDialog(NSCoder coder) : base(coder) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.DatePickerDialog"/> class.
		/// Display only a Done Button.
        /// </summary>
        /// <param name="locale">Locale for Date Picker.</param>
		public DatePickerDialog(NSLocale locale) : this(locale, false) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.DatePickerDialog"/> class.
		/// Gives the chance to Show or Hide a Cancel Button.
        /// </summary>
        /// <param name="showCancelButton">If set to <c>true</c> show cancel button.</param>
		public DatePickerDialog(bool showCancelButton) : this(null, showCancelButton) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.DatePickerDialog"/> class.
		/// Gives the chance to Show or Hide a Cancel Button and specify the Locale of DatePicker.
        /// </summary>
		/// <param name="locale">Locale for Date Picker.</param>
        /// <param name="showCancelButton">Display or Hide the Cancel Button.</param>
		public DatePickerDialog(NSLocale locale, bool showCancelButton) : this(null, null, null, null, locale, showCancelButton) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcidStudios.iOS.Dialog.DatePickerDialog"/> class.
		/// Specify the Text color of the Picker and Modal Window Title, Done, Cancel Button, the font that will Use, Locale and Display/Hide Cancel Button
        /// </summary>
        /// <param name="textColor">Text color.</param>
        /// <param name="buttonColor">Button color.</param>
        /// <param name="cancelButtonColor">Cancel button color.</param>
        /// <param name="font">Font used for Text in Modal Window.</param>
		/// <param name="locale">Locale for Date Picker.</param>
		/// <param name="showCancelButton">Display or Hide the Cancel Button.</param>
		public DatePickerDialog(UIColor textColor, UIColor buttonColor, UIColor cancelButtonColor, UIFont font, NSLocale locale, bool showCancelButton) : base(textColor, buttonColor, cancelButtonColor, font, showCancelButton) 
		{
			this.locale = locale;
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Display the DatePicker with a Default Tittle, Done and Cancel Button titles.
        /// </summary>
		/// <param name="callback">Callback, will return a NSDate Object.</param>
		public void Show(Action<NSDate> callback) => this.Show("Title", "Done", "Cancel", NSDate.Now, null, null, UIDatePickerMode.Date, callback);
        /// <summary>
		/// Display the DatePicker with a Title and a default Done and Cancel Button titles.
        /// </summary>
        /// <param name="title">Title of the Modal Window.</param>
		/// <param name="callback">Callback, will return a NSDate Object.</param>
		public void Show(string title, Action<NSDate> callback) => this.Show(title, "Done", "Cancel", NSDate.Now, null, null, UIDatePickerMode.Date, callback);
        /// <summary>
		/// Display the DatePicker with a Title, Done, Cancel Button titles and Default Date.
        /// </summary>
		/// <param name="title">Title of the Modal Window.</param>
        /// <param name="doneButtonTitle">Title of the Done Button.</param>
        /// <param name="cancelButtonTitle">Title of Cancel Button.</param>
        /// <param name="defaultDate">Default date of the DatePicker.</param>
		/// <param name="callback">Callback, will return a NSDate Object.</param>
		public void Show(string title, string doneButtonTitle, string cancelButtonTitle, NSDate defaultDate, Action<NSDate> callback) => this.Show(title, doneButtonTitle, cancelButtonTitle, defaultDate, null, null, UIDatePickerMode.Date, callback);
        /// <summary>
        /// Display the DatePicker with Title, Done, Cancel Button titles, Default Date, Minimum Date, Maximum Date and Date Picker Mode.
        /// </summary>
		/// <param name="title">Title of the Modal Window.</param>
		/// <param name="doneButtonTitle">Title of the Done Button.</param>
		/// <param name="cancelButtonTitle">Title of Cancel Button.</param>
		/// <param name="defaultDateValue">Default date of the DatePicker.</param>
        /// <param name="minDate">Minimum Date of the DatePicker.</param>
        /// <param name="maxDate">Maxium Date of the DatePicker.</param>
        /// <param name="datePickerMode">Mode of DatePicker.</param>
		/// <param name="callback">Callback, will return a NSDate Object.</param>
		public void Show(string title, string doneButtonTitle, string cancelButtonTitle, NSDate defaultDateValue, NSDate minDate, NSDate maxDate, UIDatePickerMode datePickerMode, Action<NSDate> callback)
		{

			this.datePickerMode = datePickerMode;
			this.defaultDate = defaultDateValue;
			this.picker.MaximumDate = maxDate;
			this.picker.MinimumDate = minDate;
			this.picker.Mode = datePickerMode;
			this.picker.Date = this.defaultDate ?? NSDate.Now;
         
			if (this.locale != null)
			{
				this.picker.Locale = this.locale;
			}

			base.Show(title, doneButtonTitle, cancelButtonTitle, callback);

		}
		#endregion

		#region Abstract Methods Implementation
		public override UIDatePicker CreatePickerView() => new UIDatePicker(new CGRect(0, 30, 300, 216));
		public override void OnButtonTapped(object sender, EventArgs eventArgs)
		{
			UIButton button = sender as UIButton;

            if (button.Tag == kPickerDialogDoneButtonTag)
            {
				this.OnValueSelected?.Invoke(this.picker.Date);
            }
            else
            {
				this.OnValueSelected?.Invoke(null);
            }

            this.Close();
		}
		#endregion
	}
}
