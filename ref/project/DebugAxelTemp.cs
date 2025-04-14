using UnityEngine;

public class DebugAxelTemp : MonoBehaviour
{
	private int loopClipLength = 4096;

	private int sampleRate = 11025;

	private float[] clipData;

	private void Update()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		if (Input.GetKeyDown((KeyCode)32))
		{
			clipData = new float[loopClipLength];
			for (int i = 0; i < clipData.Length; i++)
			{
				clipData[i] = Random.Range(-1f, 1f);
			}
			AudioClip.Create("Speech Loop", loopClipLength, 1, sampleRate, true, new PCMReaderCallback(callback_audioRead));
		}
	}

	private void callback_audioRead(float[] output)
	{
		for (int i = 0; i < output.Length; i++)
		{
			output[i] = clipData[i];
		}
	}
}
