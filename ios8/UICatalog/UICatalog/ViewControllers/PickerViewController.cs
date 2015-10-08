using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("PickerViewController")]
	public class PickerViewController : UIViewController
	{
		private enum ColorComponent {
			Red,
			Green,
			Blue
		}

		private struct RGB {
			public const float max = 255f;
			public const float min = 0f;
			public const float offset = 5f;
		}

		private readonly int _numberOfColorValuesPerComponent = (int)RGB.max / (int)RGB.offset + 1;

		[Outlet]
		UIPickerView PickerView { get; set; }

		[Outlet]
		UIView ColorSwatchView { get; set; }

		float _redColor;
		private float RedColor {
			get { return _redColor; }
			set {
				_redColor = value;
				UpdateColorSwatchViewBackgroundColor ();
			}
		}

		private float _greenColor;
		private float GreenColor {
			get {
				return _greenColor;
			}
			set {
				_greenColor = value;
				UpdateColorSwatchViewBackgroundColor ();
			}
		}

		private float _blueColor;
		private float BlueColor {
			get {
				return _blueColor;
			}
			set {
				_blueColor = value;
				UpdateColorSwatchViewBackgroundColor ();
			}
		}

		public PickerViewController (IntPtr handle)
			: base (handle)
		{
			_redColor = RGB.min;
			_greenColor = RGB.min;
			_blueColor = RGB.min;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Show that a given row is selected. This is off by default.
			PickerView.ShowSelectionIndicator = true;

			ConfigurePickerView();
		}

		private void UpdateColorSwatchViewBackgroundColor()
		{
			ColorSwatchView.BackgroundColor = new UIColor(RedColor, GreenColor, BlueColor, 1f);
		}

		private void ConfigurePickerView()
		{
			// Set the default selected rows (the desired rows to initially select will vary from app to app).
			Dictionary<ColorComponent, int> selectedRows = new Dictionary<ColorComponent, int> {
				{ ColorComponent.Red, 13 },
				{ ColorComponent.Green, 41 },
				{ ColorComponent.Blue, 24 }
			};

			// kvp - means KeyValuePair
			foreach (var kvp in selectedRows) {
				var selectedRow = kvp.Value;
				var colorComponent = kvp.Key;

				// Note that the delegate method on UIPickerViewDelegate is not triggered when manually
				// calling UIPickerView.selectRow(:inComponent:animated:). To do this, we fire off delegate
				// method manually.
				PickerView.Select (selectedRow, (int)colorComponent, animated: true);
				Selected (PickerView, selectedRow, (int)colorComponent);
			}
		}

		#region UIPickerViewDelegate

		[Export ("pickerView:attributedTitleForRow:forComponent:")]
		private NSAttributedString GetAttributedTitle (UIPickerView pickerView, int row, int component)
		{
			float colorValue = row * RGB.offset;

			float value = colorValue / RGB.max;
			float redColorComponent = RGB.min;
			float greenColorComponent = RGB.min;
			float blueColorComponent = RGB.min;

			switch( (ColorComponent)component)
			{
				case ColorComponent.Red:
					redColorComponent = value;
					break;

				case ColorComponent.Green:
					greenColorComponent = value;
					break;

				case ColorComponent.Blue:
					blueColorComponent = value;
					break;

				default:
					throw new InvalidOperationException ("Invalid row/component combination for picker view.");
			}

			UIColor foregroundColor = new UIColor (redColorComponent, greenColorComponent, blueColorComponent, alpha: 1f);

			// Set the foreground color for the entire attributed string.
			UIStringAttributes attributes = new UIStringAttributes {
				ForegroundColor = foregroundColor
			};

			var title = new NSAttributedString (string.Format ("{0}", (int)colorValue), attributes);
			return title;
		}

		[Export ("pickerView:didSelectRow:inComponent:")]
		private void Selected (UIPickerView pickerView, int row, int component)
		{
			float colorComponentValue = RGB.offset * (float)row / RGB.max;

			switch ((ColorComponent)component)
			{
				case ColorComponent.Red:
					RedColor = colorComponentValue;
					break;

				case ColorComponent.Green:
					GreenColor = colorComponentValue;
					break;

				case ColorComponent.Blue:
					BlueColor = colorComponentValue;
					break;
			}
		}

		#endregion

		#region UIPickerViewAccessibilityDelegate

		[Export("pickerView:accessibilityLabelForComponent:")]
		private string GetAccessibilityLabel(UIPickerView pickerView, int component)
		{
			switch ((ColorComponent)component)
			{
				case ColorComponent.Red:
					return "Red color component value".Localize ();

				case ColorComponent.Green:
					return "Green color component value".Localize ();

				case ColorComponent.Blue:
					return "Blue color component value".Localize ();

				default:
					throw new InvalidOperationException ();
			}
		}

		#endregion

		#region UIPickerViewDataSource

		[Export("numberOfComponentsInPickerView:")]
		public int GetComponentCount (UIPickerView pickerView)
		{
			return Enum.GetValues (typeof(ColorComponent)).Length;
		}

		[Export("pickerView:numberOfRowsInComponent:")]
		public int GetRowsInComponent (UIPickerView pickerView, int component)
		{
			return _numberOfColorValuesPerComponent;
		}

		#endregion
	}
}
