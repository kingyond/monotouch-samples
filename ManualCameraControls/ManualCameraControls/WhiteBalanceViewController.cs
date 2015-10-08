using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;
using CoreFoundation;
using System.Timers;

namespace ManualCameraControls
{
	public partial class WhiteBalanceViewController : UIViewController
	{
		#region Private Variables
		private NSError Error;
		private bool Automatic = true;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}

		/// <summary>
		/// Gets or sets the sample timer.
		/// </summary>
		/// <value>The sample timer.</value>
		public Timer SampleTimer { get; set; }
		#endregion

		#region Constructors
		public WhiteBalanceViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Sets the temperature and tint.
		/// </summary>
		private void SetTemperatureAndTint() {
			// Grab current temp and tint
			// NOTE: The following line explodes in Xamarin with no error being thrown...
			AVCaptureWhiteBalanceTemperatureAndTintValues TempAndTint = new AVCaptureWhiteBalanceTemperatureAndTintValues(Temperature.Value, Tint.Value);

			// Convert Color space
			var gains = ThisApp.CaptureDevice.GetDeviceWhiteBalanceGains (TempAndTint);

			// Set the new values
			ThisApp.CaptureDevice.LockForConfiguration(out Error);
			ThisApp.CaptureDevice.SetWhiteBalanceModeLockedWithDeviceWhiteBalanceGains (gains, (time) => {
				// Ignore callback for now
			});
			ThisApp.CaptureDevice.UnlockForConfiguration();
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Hide no camera label
			NoCamera.Hidden = ThisApp.CameraAvailable;

			// Attach to camera view
			ThisApp.Recorder.DisplayView = CameraView;

			// Set min and max values
			Temperature.MinValue = 1.0f;
			Temperature.MaxValue = ThisApp.CaptureDevice.MaxWhiteBalanceGain;

			Tint.MinValue = 1.0f;
			Tint.MaxValue = ThisApp.CaptureDevice.MaxWhiteBalanceGain;

			// Create a timer to monitor and update the UI
			SampleTimer = new Timer (5000);
			SampleTimer.Elapsed += (sender, e) => {
				// Convert color space
				var TempAndTint = ThisApp.CaptureDevice.GetTemperatureAndTintValues(ThisApp.CaptureDevice.DeviceWhiteBalanceGains);

				// Update slider positions
				Temperature.BeginInvokeOnMainThread(() =>{
					Temperature.Value = TempAndTint.Temperature;
				});

				Tint.BeginInvokeOnMainThread(() =>{
					Tint.Value = TempAndTint.Tint;
				});
			};

			// Watch for value changes
			Segments.ValueChanged += (object sender, EventArgs e) => {

				// Lock device for change
				ThisApp.CaptureDevice.LockForConfiguration(out Error);

				// Take action based on the segment selected
				switch(Segments.SelectedSegment) {
				case 0:
					// Activate auto focus and start monitoring position
					Temperature.Enabled = false;
					Tint.Enabled = false;
					ThisApp.CaptureDevice.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
					SampleTimer.Start();
					Automatic = true;
					break;
				case 1:
					// Stop auto focus and allow the user to control the camera
					SampleTimer.Stop();
					ThisApp.CaptureDevice.WhiteBalanceMode = AVCaptureWhiteBalanceMode.Locked;
					Automatic = false;
					Temperature.Enabled = true;
					Tint.Enabled = true;
					break;
				}

				// Unlock device
				ThisApp.CaptureDevice.UnlockForConfiguration();
			};

			// Monitor position changes
			Temperature.TouchUpInside += (object sender, EventArgs e) => {

				// If we are in the automatic mode, ignore changes
				if (Automatic) return;

				// Update white balance
				SetTemperatureAndTint();
			};

			Tint.TouchUpInside += (object sender, EventArgs e) => {

				// If we are in the automatic mode, ignore changes
				if (Automatic) return;

				// Update white balance
				SetTemperatureAndTint();
			};

			GrayCardButton.TouchUpInside += (sender, e) => {

				// If we are in the automatic mode, ignore changes
				if (Automatic) return;

				// Get gray card values
				var gains = ThisApp.CaptureDevice.GrayWorldDeviceWhiteBalanceGains;

				// Set the new values
				ThisApp.CaptureDevice.LockForConfiguration(out Error);
				ThisApp.CaptureDevice.SetWhiteBalanceModeLockedWithDeviceWhiteBalanceGains (gains, (time) => {
					// Ignore callback for now
				});
				ThisApp.CaptureDevice.UnlockForConfiguration();
			};
		}

		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Start udating the display
			if (ThisApp.CameraAvailable) {
				// Remap to this camera view
				ThisApp.Recorder.DisplayView = CameraView;

				ThisApp.Session.StartRunning ();
				SampleTimer.Start ();
			}
		}

		/// <summary>
		/// Views the will disappear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillDisappear (bool animated)
		{
			// Stop display
			if (ThisApp.CameraAvailable) {
				SampleTimer.Stop ();
				ThisApp.Session.StopRunning ();
			}

			base.ViewWillDisappear (animated);

		}
		#endregion
	}
}
