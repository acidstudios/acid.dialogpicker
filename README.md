# Acid.DialogPicker
Acid.DialogPicker is a C# iOS component that display a UIPickerView and UIDatePicker inside a Modal Window for Xamarin.
[![](https://github.com/acidstudios/acid.dialogpicker/raw/master/images/datepicker.png =179x280)](https://github.com/acidstudios/acid.dialogpicker/tree/master/images)
[![](https://github.com/acidstudios/acid.dialogpicker/raw/master/images/picker.png =179x280)](https://github.com/acidstudios/acid.dialogpickertree/master/images)

## Requirements
* Xamarin.iOS
* Visual Studio for Mac
* Visual Studio 2017(windows)
* Windows: Mac paired to Windows machine to test.

## Installation
#### NuGet
You can use NuGet to install Acid.DialogPicker, only run this command:
```csharp
Install-Package Acid.DialogPicker
```
#### Visual Studio:
* Right-click on your iOS Project
* Select Add NuGet Packages
* Search "Acid.DialogPicker"

#### Downloading the Source:
Include in your project: 
* Acid.BaseDialogPicker.cs
* Acid.DatePicker.Dialog.cs
* Acid.Picker.Dialog.cs

## Use the Library
With this package you get this 3 files:
* Acid.BaseDialogPicker.cs: Abstract class, it can hold another type of View inside a Modal Window.
* Acid.DatePicker.Dialog.cs: Inherit from BaseDialogPicker, it display a UIDatePickerView
* Acid.Picker.Dialog: Inherit from BaseDialogPicker and display a UIPickerView also it exposes some API for UIPickerDataSource and UIPickerDelegate.

After you install the NuGet Package or drop the files inside your project, go to your View that you want to display our dialog and use the Namespace
In your view add this:

```csharp
using AcidStudios.iOS.Dialog
```

To show the DatePicker.Dialog use this code:
In your view add this:

```csharp
var datePicker = new AcidStudios.iOS.Dialog.DatePickerDialog(new Foundation.NSLocale("es_US"), true);
datePicker.Show("Hello there", "OK", "Cancel", Foundation.NSDate.Now, Foundation.NSDate.Now.AddSeconds(-40000), Foundation.NSDate.Now, UIDatePickerMode.Date,(Foundation.NSDate obj) => {
	if (obj != null)
	{
		Console.WriteLine(obj.ToString());
	}
});
```
To show the Picker.Dialog use this code:
In your view add this:

```csharp
// Create yout collection of Options for your Picker
var items = new System.Collections.Generic.List<Country>
{
	new Country { Name = "México" },
	new Country { Name = "United States" },
	new Country { Name = "Canada" }
};

// Create Picker.Dialog
// Implement some UIPickerDelegate and UIPickerDataSource delegate functions
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

// Use Show to Display the Picker Dialog and define the callback 
// function to retrieve the index of selected row.
picker.Show("Regular Picker", "OK", "Cancel", (nint obj) =>
{
	if (obj != -1 && items[(int)obj] is Country country)
	{
		Console.WriteLine($"Selected Country: {country.Name}");
	}
});
```

## Special thanks to
* [@squimer](https://github.com/wimagguc) for the original work of [DatePickerDialog-iOS-Swift](https://github.com/squimer/DatePickerDialog-iOS-Swift) library in Swift

## License

This code is distributed under the terms and conditions of the [MIT license](LICENSE).