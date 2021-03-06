 #region License
// /*
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright © 2009 The MonoGame Team
// 
// All rights reserved.
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
// U.S. copyright law.
// 
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// */
using Android.Widget;


#endregion License 

using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;


namespace Microsoft.Xna.Framework.Media
{
    public sealed class Video : Java.Lang.Object, IDisposable, Android.Media.MediaPlayer.IOnCompletionListener,
	  Android.Media.MediaPlayer.IOnVideoSizeChangedListener
    {
        internal VideoPlayer Player;
		internal Android.Media.MediaPlayer media;
		private string _fileName;
		private Color _backColor = Color.Black;
       
		
		internal Video(string FileName)
		{
			_fileName = FileName;			
		}
				
		public Color BackgroundColor
		{
			set
			{
				_backColor = value;
			}
			get
			{
				return _backColor;
			}
		}
		
		public string FileName
		{
			get 
			{
				return _fileName;
			}
		}
		
		internal static string Normalize(string FileName)
		{
            int index = FileName.LastIndexOf(Path.DirectorySeparatorChar);
            string path = string.Empty;
            string file = FileName;
            if (index >= 0)
            {
                file = FileName.Substring(index + 1, FileName.Length - index - 1);
                path = FileName.Substring(0, index);
            }
            string[] files = Game.Activity.Assets.List(path);

            if (Contains(file, files))
                return FileName;
			
			// Check the file extension
			if (!string.IsNullOrEmpty(Path.GetExtension(FileName)))
			{
				return null;
			}
			
			// Concat the file name with valid extensions
			return Path.Combine(path, TryFindAnyCased(file, files, ".3gp", ".mkv", ".mp4", ".ts", ".webm"));
		}
		
		private static string TryFindAnyCased(string search, string[] arr, params string[] extensions)
        {
            return arr.FirstOrDefault(s => extensions.Any(ext => s.ToLower() == (search.ToLower() + ext)));
        }

        private static bool Contains(string search, string[] arr)
        {
            return arr.Any(s => s == search);
        }

		internal void Prepare()
		{
			
			media = new Android.Media.MediaPlayer();//.Create(Game.Activity,Android.Net.Uri.Parse("file://android_asset/Content/sintel_trailer.mp4"), Game.Instance.Window.Holder);						
			if (media != null )
			{
				var afd = Game.Activity.Assets.OpenFd(_fileName);				
				if (afd != null)
				{					
					media.SetOnCompletionListener(this);			
					media.SetOnVideoSizeChangedListener(this);
					VideoView vv = (VideoView)Game.Activity.FindViewById(MonoGame.Android.Media.VideoViewId);			
					if (vv == null) throw new InvalidOperationException("you must attach a VideoView in a framelayout with its Id set to MonoGame.Android.Media.VideoViewId");
					media.SetDisplay(vv.Holder);							
		            media.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);						
		            media.Prepare();								
				}
			}
		}
		
		
		public void Dispose()
		{
            if (media != null)
			{
				media.Dispose();
				media = null;
			}
		}

		#region IOnCompletionListener implementation
		public void OnCompletion (Android.Media.MediaPlayer mp)
		{
			if  (Player != null) Player.Stop();
		}
		#endregion

		#region IOnVideoSizeChangedListener implementation
		public void OnVideoSizeChanged (Android.Media.MediaPlayer mp, int width, int height)
		{
#if DEBUG			
			Android.Util.Log.Info("MonoGameInfo", string.Format("Video Size : {0}x{1}", width, height));
#endif			
		}
		#endregion
    }
}
namespace MonoGame.Android
{
	public class Media
	{	
	    public const int VideoViewId = 243252;
	}
}