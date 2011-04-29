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
	/// Represents a basic category found within the AIML based brain
	/// </summary>
	///
	/// <remarks>
	/// The sPath attribute represents what is being matched with the user's input and
	/// includes the input pattern (from the user's raw input), the "that" element that
	/// holds what the bot's last reply was and the "topic" element that describes the 
	/// value of the topic property for this conversation. An asterisk '*' in the "that"
	/// of "topic" elements denotes empty.
	/// 
	/// The sTemplate attribute describes how the bot should respond. This string is 
	/// processed by an AIML parser according to the guidelines found in the AIML v1.0.1
	/// specification document (http://www.alicebot.org/TR/2001/WD-aiml/)
	/// </remarks>

	public class cCategory
	{
		#region PUBLIC ATTRIBUTES

		/// <summary>
		/// Holds the category path
		/// </summary>
		public string sPath;

		/// <summary>
		/// Holds the response template
		/// </summary>
		public string sTemplate;

		/// <summary>
		/// NOTA BENE: this attribute is only set by a nodemapper when a category 
		/// is returned in response to some user input.
		/// If the raw input part of the PATH contains a wildcard then this attribute 
		/// will contain the block of text (as individual words) that the user has 
		/// inputted that is represented by the wildcard. 
		/// </summary>
		public ArrayList alInputStar=new ArrayList();

		/// <summary>
		/// NOTA BENE: this attribute is only set by a nodemapper when a category 
		/// is returned in response to some user input.
		/// If the "that" part of the PATH contains a wildcard then this attribute 
		/// will contain the block of text (as individual words) that the user has 
		/// inputted that is represented by the wildcard. 
		/// </summary>
		public ArrayList alThatStar=new ArrayList();

		/// <summary>
		/// NOTA BENE: this attribute is only set by a nodemapper when a category 
		/// is returned in response to some user input.
		/// If the "topic" part of the PATH contains a wildcard then this attribute 
		/// will contain the block of text (as individual words) that the user has 
		/// inputted that is represented by the wildcard. 
		/// </summary>
		public ArrayList alTopicStar=new ArrayList();

		/// <summary>
		/// For debugging purposes - holds the name of the file where the aiml that created
		/// this category came from.
		/// </summary>
		public string filename;

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Set the PATH and TEMPLATE attributes for this category
		/// </summary>
		/// <param name="sPath">the PATH attribute for this category</param>
		/// <param name="sTemplate">the response TEMPLATE for this category</param>
		public cCategory(string sPath, string sTemplate)
		{
			this.sPath=sPath;
			this.sTemplate=sTemplate;
		}

		/// <summary>
		/// Overload that allows the new cCategory to be a clone of the passed cCategory
		/// </summary>
		/// <param name="SourceCat">the cCategory to be cloned</param>
		public cCategory(cCategory SourceCat)
		{
			//this.alInputStar.AddRange((ArrayList)SourceCat.alInputStar);
			//this.alThatStar.AddRange((ArrayList)SourceCat.alThatStar);
			//this.alTopicStar.AddRange((ArrayList)SourceCat.alTopicStar);
			this.sPath=(string)SourceCat.sPath.ToString();
			this.sTemplate=(string)SourceCat.sTemplate.ToString();
		}
		
		/// <summary>
		/// Returns the category response template
		/// </summary>
		/// <returns>the category response template</returns>
		override public string ToString()
		{
			return sTemplate;
		}

		/// <summary>
		/// Does some simple normalization on the sTemplate (strips whitespaces 
		/// replaces "\n" etc
		/// </summary>
		public void checktext()
		{
			string sProcessed = this.sTemplate.Trim();
			this.sTemplate = sProcessed.Replace("\n","");
		}

		#endregion
	}
}
