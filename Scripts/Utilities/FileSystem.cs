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

using Newtonsoft.Json;
using System;
using System.IO;

namespace SK.Libretro
{
    public static class FileSystem
    {
        public static bool FileExists(string path) => File.Exists(path);

        public static bool CreateFile(string path)
        {
            try
            {
                using FileStream fs = File.Create(path);
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogException(e);
            }

            return false;
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                if (FileExists(path))
                    File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogException(e);
            }

            return false;
        }

        public static string[] GetFilesInDirectory(string path, string searchPattern, bool includeSubFolders = false)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Logger.Instance.LogException(e);
            }

            return null;
        }

        public static bool SerializeToJson<T>(T sourceObject, string targetPath)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(sourceObject, Formatting.Indented);
                File.WriteAllText(targetPath, jsonString);
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogException(e);
            }

            return false;
        }

        public static T DeserializeFromJson<T>(string sourcePath) where T : class
        {
            try
            {
                if (!FileExists(sourcePath))
                    return null;

                string jsonString = File.ReadAllText(sourcePath);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception e)
            {
                Logger.Instance.LogException(e);
            }

            return null;
        }
    }
}
