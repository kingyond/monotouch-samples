// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sound
{
	[Register ("SoundViewController")]
	partial class SoundViewController
	{
		[Outlet]
		UIKit.UILabel LengthOfRecordingLabel { get; set; }

		[Outlet]
		UIKit.UIButton PlayRecordedSoundButton { get; set; }

		[Outlet]
		UIKit.UILabel RecordingStatusLabel { get; set; }

		[Outlet]
		UIKit.UIButton StartRecordingButton { get; set; }

		[Outlet]
		UIKit.UIButton StopRecordingButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StartRecordingButton != null) {
				StartRecordingButton.Dispose ();
				StartRecordingButton = null;
			}

			if (StopRecordingButton != null) {
				StopRecordingButton.Dispose ();
				StopRecordingButton = null;
			}

			if (PlayRecordedSoundButton != null) {
				PlayRecordedSoundButton.Dispose ();
				PlayRecordedSoundButton = null;
			}

			if (RecordingStatusLabel != null) {
				RecordingStatusLabel.Dispose ();
				RecordingStatusLabel = null;
			}

			if (LengthOfRecordingLabel != null) {
				LengthOfRecordingLabel.Dispose ();
				LengthOfRecordingLabel = null;
			}
		}
	}
}
