﻿/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2006-12-16
 * Time: 2:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace Shove.CharsetDetector
{
	/// <summary>
	/// Description of ICharsetDetectionObserver.
	/// </summary>
	public interface ICharsetDetectionObserver
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		void Notify(string charset) ;
	}
}
