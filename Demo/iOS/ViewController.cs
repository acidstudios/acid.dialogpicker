using System;

using UIKit;

namespace DialogPicker.iOS
{
    public partial class ViewController : UIViewController
    {      
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
			Button.SetTitle("Show DatePicker", UIControlState.Normal);
            Button.TouchUpInside += delegate
            {
				var datePicker = new AcidStudios.iOS.Dialog.DatePickerDialog(new Foundation.NSLocale("es_US"), true);
				datePicker.Show("Hello there", "OK", "Cancel", Foundation.NSDate.Now, Foundation.NSDate.Now.AddSeconds(-40000), Foundation.NSDate.Now, UIDatePickerMode.Date,(Foundation.NSDate obj) => {
					if (obj != null)
					{
						Console.WriteLine(obj.ToString());
					}
				});
            };

			var items = new System.Collections.Generic.List<Country>
			{
				new Country { Name = "Argentina" },
				new Country { Name = "Belice" },
				new Country { Name = "Brazil" },
				new Country { Name = "Chile" },
				new Country { Name = "Colombia" },
				new Country { Name = "Guatemala" },
				new Country { Name = "México" },
				new Country { Name = "Perú" },
				new Country { Name = "Honduras" }
			};

			PickerButton.TouchUpInside += delegate {
				var picker = new AcidStudios.iOS.Dialog.PickerDialog<object>(true)
				{
					ComponentCount = delegate (UIPickerView pickerView)
					{
						return 1;
					},
					RowsInComponent = delegate (UIPickerView pickerView, nint component)
					{
						return items.Count;
					},
					TitleForRow = delegate (UIPickerView pickerView, nint row, nint component)
					{
						return new Foundation.NSString(items[(int)row].Name);
					}
				};


				picker.Show("Regular Picker", "OK", "Cancel", (nint obj) =>
				{
					if (obj != -1 && items[(int)obj] is Country country)
					{
						Console.WriteLine($"Selected Country: {country.Name}");
					}
				});
			};
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
