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
	/// The pattern matching algorithm that is the heart of an AIML bot.
	/// Based upon the work of Dr.Richard Wallace (http://www.alicebot.org/).
	/// </summary>
	///
	/// <remarks>
	/// To quote from the Java Graphmaster implementation:
	/// 
	/// "The Graphmaster is the brain of an [AIML]bot. It consists of a collection
	/// of nodes called Nodemappers [represented by cNodeMapper in this C# version - NT].
	/// These Nodemappers map the branches from each node. The brances are either
	/// singe words or wildcards.<br/>
	///
	/// The root of the Graphmaster is a Nodemapper with many branches, one for each
	/// of the first words of all the patterns (40,000 in the case of the original
	/// A.L.I.C.E. brain). The number of leaf nodes in the graph is equal to the number
	/// of categories, and each leaf node contains the template tag."
	/// 
	/// For a clear specification of what is going on here read the AIML specification 
	/// v1.0.1 that is available at:
	/// 
	/// http://www.alicebot.org/TR/2001/WD-aiml/
	/// </remarks>

	public class cGraphMaster
	{
		#region PRIVATE ATTRIBUTES

		/// <summary>
		/// This nodemapper is the root of the Graphmaster structure
		/// </summary>
		private cNodeMapper rootNodeMapper = new cNodeMapper();

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Constructor does nothing
		/// </summary>
		public cGraphMaster()
		{
			// nothing needed here
		}

		/// <summary>
		/// Adds a new category to the Graphmaster structure
		/// </summary>
		/// <param name="sPath">the PATH element of the new cCategory. NB Normalization should be done to the PATH before calling this method</param>
		/// <param name="sTemplate">the TEMPLATE element for the new cCategory</param>
		public void addCat(string sPath, string sTemplate, string filename)
		{
			// create the Category
			cCategory cat = new cCategory(sPath, sTemplate);

			// for debugging
			cat.filename=filename;

			// just to make sure the Path is tidy
			string sEnd = sPath.Trim();

			// o.k. add the category to the root node of the graphmaster and let the cNodeMapper
			// class methods deal with it.
			rootNodeMapper.addCat(sEnd, cat);
		}

		/// <summary>
		/// Searches for a cCategory within the Graphmaster structure given a certain input
		/// </summary>
		/// <param name="sInput">Normalised input concatenated with "that" and "topic" parameters - i.e. the PATH</param>
		/// <param name="sBotID">the botID of the bot whose GraphMaster structure this is</param>
		/// <param name="sUserID">the userID of the person who requires a reply</param>
		/// <returns>a string representing the bot's response</returns>
		public string[] evaluate(string sInput, cBot myBot, string sUserID)
		{
			// to hold the raw output
			string[] sRawOutput={"",""};

			// deal with void input
			if (sInput.Length==0) return sRawOutput;

			// create a category to represent the returned category
			cCategory cat = rootNodeMapper.evaluate(sInput,0,"");

			// if we've found something then process it and return a string
			if (cat != null)
			{
				cCategory procCat = cProcessor.process(cat, myBot, sUserID);
				sRawOutput[0]=cat.filename;
				sRawOutput[1]=procCat.ToString().Trim();
				return sRawOutput;
			}

				// this shouldn't happen but is here just in case :-)
			else
			{
				if(cGlobals.isDebug)
				{
					sRawOutput[1]="No match found for input path: "+sInput;
				}
				else
				{
					sRawOutput[1]="I'm afraid I don't understand. Perhaps some of my AIML files are missing. You can download them from http://www.alicebot.org/ and place them in the following directory: "+cGlobals.AIMLPATH;
				}
				return sRawOutput;
			}
		}

		#endregion
	}
}
