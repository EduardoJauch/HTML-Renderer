﻿// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using HtmlRenderer.Core.Entities;
using HtmlRenderer.Demo.Common;

namespace HtmlRenderer.Demo.WPF
{
    internal static class HtmlRenderingHelper
    {
        #region Fields/Consts

        /// <summary>
        /// Cache for resource images
        /// </summary>
        private static readonly Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>(StringComparer.OrdinalIgnoreCase);

        #endregion


        /// <summary>
        /// Create image to be used to fill background so it will be clear that what's on top is transparent.
        /// </summary>
        public static BitmapImage CreateImageForTransparentBackground()
        {
//            var image = new BitmapImage(10, 10);
//            using (var g = Graphics.FromImage(image))
//            {
//                g.Clear(Color.White);
//                g.FillRectangle(SystemBrushes.Control, new Rectangle(0, 0, 5, 5));
//                g.FillRectangle(SystemBrushes.Control, new Rectangle(5, 5, 5, 5));
//            }
//            return image;
            return null;
        }

        /// <summary>
        /// Get image by resource key.
        /// </summary>
        public static BitmapImage TryLoadResourceImage(string src)
        {
            BitmapImage image;
            if (!_imageCache.TryGetValue(src, out image))
            {
                var imageStream = DemoUtils.GetImageStream(src);
                if (imageStream != null)
                {
                    image = ImageFromStream(imageStream);
                    _imageCache[src] = image;
                }
            }
            return image;
        }

        /// <summary>
        /// Get image by resource key.
        /// </summary>
        public static BitmapImage ImageFromStream(Stream stream)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        /// <summary>
        /// On image load in renderer set the image by event async.
        /// </summary>
        public static void OnImageLoad(object sender, HtmlImageLoadEventArgs e)
        {
            ImageLoad(e);
        }

        /// <summary>
        /// On image load in renderer set the image by event async.
        /// </summary>
        public static void ImageLoad(HtmlImageLoadEventArgs e)
        {
            var img = TryLoadResourceImage(e.Src);
            
            if (!e.Handled && e.Attributes != null)
            {
                if (e.Attributes.ContainsKey("byevent"))
                {
                    int delay;
                    if (Int32.TryParse(e.Attributes["byevent"], out delay))
                    {
                        e.Handled = true;
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            Thread.Sleep(delay);
                            e.Callback("https://fbcdn-sphotos-a-a.akamaihd.net/hphotos-ak-snc7/c0.44.403.403/p403x403/318890_10151195988833836_1081776452_n.jpg");
                        });
                        return;
                    }
                    else
                    {
                        e.Callback("http://sphotos-a.xx.fbcdn.net/hphotos-ash4/c22.0.403.403/p403x403/263440_10152243591765596_773620816_n.jpg");
                        return;
                    }
                }
                else if (e.Attributes.ContainsKey("byrect"))
                {
                    var split = e.Attributes["byrect"].Split(',');
                    var rect = new Rect(Int32.Parse(split[0]), Int32.Parse(split[1]), Int32.Parse(split[2]), Int32.Parse(split[3]));
                    e.Callback(img ?? TryLoadResourceImage("htmlicon"), rect.X, rect.Y, rect.Width, rect.Height);
                    return;
                }
            }

            if (img != null)
                e.Callback(img);
        }
    }
}