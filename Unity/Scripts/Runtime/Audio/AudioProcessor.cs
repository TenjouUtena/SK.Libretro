﻿/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System.Collections.Generic;
using UnityEngine;

namespace SK.Libretro.Unity
{
    [RequireComponent(typeof(AudioSource))]
    internal sealed class AudioProcessor : MonoBehaviour, IAudioProcessor
    {
        private const int AUDIO_BUFFER_SIZE = 65536;

        private readonly List<float> _audioBuffer = new List<float>(AUDIO_BUFFER_SIZE);
        private AudioSource _audioSource;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (_audioBuffer.Count >= data.Length)
            {
                _audioBuffer.CopyTo(0, data, 0, data.Length);
                _audioBuffer.RemoveRange(0, data.Length);
            }
        }

        public void Init(int sampleRate) => MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (_audioSource != null)
                _audioSource.Stop();

            AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
            audioConfig.sampleRate = sampleRate;
            _ = AudioSettings.Reset(audioConfig);

            _audioSource = GetComponent<AudioSource>();
            _audioSource.Play();
        });

        public void DeInit()
        {
            MainThreadDispatcher mainThreadDispatcher = MainThreadDispatcher.Instance;
            if (mainThreadDispatcher != null)
                mainThreadDispatcher.Enqueue(() =>
                {
                    if (_audioSource != null)
                        _audioSource.Stop();
                });
        }

        public unsafe void ProcessSamples(float[] samples) => _audioBuffer.AddRange(samples);
    }
}
