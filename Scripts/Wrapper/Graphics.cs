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

using System;
using static SK.Libretro.Header;

namespace SK.Libretro
{
    internal sealed class Graphics
    {
        public bool Enabled { get; set; }
        public bool UseCoreRotation { get; set; }
        public retro_pixel_format PixelFormat { get; set; }

        private readonly Wrapper _wrapper;
        private IGraphicsProcessor _processor;

        public Graphics(Wrapper wrapper, bool useCoreRotation)
        {
            _wrapper        = wrapper;
            UseCoreRotation = useCoreRotation;
        }

        public void Init(IGraphicsProcessor graphicsProcessor) => _processor = graphicsProcessor;

        public void DeInit()
        {
            _processor?.DeInit();
            _processor = null;
        }

        public unsafe void Callback(void* data, uint width, uint height, ulong pitch)
        {
            if (data == null || _processor == null || !Enabled)
                return;

            if (_wrapper.Core.HwAccelerated)
                ProcessFrameHardware(width, height);
            else
                ProcessFrameSoftware(data, width, height, pitch);
        }

        private unsafe void ProcessFrameHardware(uint width, uint height)
        {
            byte[] bufferSrc = new byte[width * height * 4];
            fixed (byte* bufferSrcPtr = bufferSrc)
            {
                _wrapper.OpenGL.glReadPixels(0, 0, (int)width, (int)height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bufferSrcPtr);
                _processor.ProcessFrameXRGB8888VFlip((uint*)bufferSrcPtr, (int)width, (int)height, (int)width * 4);
            }
            _wrapper.OpenGL.SwapBuffers();
        }

        private unsafe void ProcessFrameSoftware(void* data, uint width, uint height, ulong pitch)
        {
            switch (PixelFormat)
            {
                case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                    _processor.ProcessFrame0RGB1555((ushort*)data, (int)width, (int)height, Convert.ToInt32(pitch));
                    break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                    _processor.ProcessFrameXRGB8888((uint*)data, (int)width, (int)height, Convert.ToInt32(pitch));
                    break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                    _processor.ProcessFrameRGB565((ushort*)data, (int)width, (int)height, Convert.ToInt32(pitch));
                    break;
            }
        }
    }
}
