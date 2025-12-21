using Godot;

namespace Wayblazer;

public partial class MusicManager : AudioStreamPlayer
{
	public override void _Ready()
	{
		_backgroundTrackOne = GD.Load<AudioStream>(Constants.Music.BACKGROUND_1);
		_backgroundTrackTwo = GD.Load<AudioStream>(Constants.Music.BACKGROUND_2);

		PlayNextTrack();
	}

	private void PlayNextTrack()
	{
		if (Stream == _backgroundTrackOne)
		{
			Stream = _backgroundTrackTwo;
		}
		else
		{
			Stream = _backgroundTrackOne;
		}

		// play the current track and set a timer for when it finishes to switch to the next track
		Timer timer = new Timer();
		timer.WaitTime = Stream.GetLength();
		timer.OneShot = true;
		timer.Timeout += () => PlayNextTrack();
		AddChild(timer);

		Play();
		timer.Start();
	}

	public override void _Process(double delta)
	{
		if (!IsPlaying())
		{
			Stream = _backgroundTrackOne;
			Play();


		}
	}

	private AudioStream _backgroundTrackOne;
	private AudioStream _backgroundTrackTwo;
}
