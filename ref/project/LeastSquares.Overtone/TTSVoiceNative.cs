using System;
using System.Threading;
using UnityEngine;

namespace LeastSquares.Overtone;

public class TTSVoiceNative
{
	public const int Timeout = 8000;

	private readonly ReaderWriterLock _lock = new ReaderWriterLock();

	public IntPtr Pointer { get; set; }

	public FixedPointerToHeapAllocatedMem ConfigPointer { get; set; }

	public FixedPointerToHeapAllocatedMem ModelPointer { get; set; }

	public bool Disposed { get; private set; }

	public static TTSVoiceNative LoadVoiceFromResources(string voiceName)
	{
		TextAsset val = Resources.Load<TextAsset>(voiceName ?? "");
		TextAsset val2 = Resources.Load<TextAsset>(voiceName + ".config");
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogError((object)("Failed to find voice model " + voiceName + ".bytes in Resources"));
			return null;
		}
		if ((Object)(object)val2 == (Object)null)
		{
			Debug.LogError((object)("Failed to find voice model " + voiceName + ".config.json in Resources"));
			return null;
		}
		byte[] bytes = val2.bytes;
		byte[] bytes2 = val.bytes;
		FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem = FixedPointerToHeapAllocatedMem.Create(bytes, (uint)bytes.Length);
		FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem2 = FixedPointerToHeapAllocatedMem.Create(bytes2, (uint)bytes2.Length);
		IntPtr intPtr = TTSNative.OvertoneLoadVoice(fixedPointerToHeapAllocatedMem.Address, fixedPointerToHeapAllocatedMem.SizeInBytes, fixedPointerToHeapAllocatedMem2.Address, fixedPointerToHeapAllocatedMem2.SizeInBytes);
		TTSNative.OvertoneSetSpeakerId(intPtr, 0L);
		return new TTSVoiceNative
		{
			Pointer = intPtr,
			ConfigPointer = fixedPointerToHeapAllocatedMem,
			ModelPointer = fixedPointerToHeapAllocatedMem2
		};
	}

	public void SetSpeakerId(int speakerId)
	{
		TTSNative.OvertoneSetSpeakerId(Pointer, speakerId);
	}

	public void AcquireReaderLock()
	{
		_lock.AcquireReaderLock(8000);
	}

	public void ReleaseReaderLock()
	{
		_lock.ReleaseReaderLock();
	}

	public void Dispose()
	{
		_lock.AcquireWriterLock(8000);
		Disposed = true;
		ConfigPointer.Free();
		ModelPointer.Free();
		TTSNative.OvertoneFreeVoice(Pointer);
		_lock.ReleaseWriterLock();
	}
}
