/*
AIMLBot - An implementation of the AIML specification found at
http://www.alicebot.org

Copyright (C) 2005 Xmonic http://www.xmonic.com/

Written by Nicholas H.Tollervey http://www.ntoll.org/

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections;

namespace AIMLBot
{
	/// <summary>
	/// A container class for cUser objects
	/// </summary>
	///
	/// <remarks>
	/// Another tarted up Hashtable! The key field is a unique string sUserID, 
	/// the value a cUser object
	/// </remarks>
	/// 
	/// <list type="bullet">
	/// <listheader>
	/// <term>File History</term>
	/// <description>Created by - Nicholas H.Tollervey on 10/02/2004</description>
	/// </listheader>
	///
	/// <item>
	/// <term>DD/MM/YYYY</term>
	/// <description>Describe the change or work done</description>
	/// </item>
	/// </list>

	public class cUsers
	{
		/// <summary>
		/// the hashtable of stored users
		/// </summary>
		public static Hashtable users = new Hashtable();
		public cUsers(){}
	}
}
