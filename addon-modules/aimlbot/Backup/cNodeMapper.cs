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
	/// Represents an individual node in the AIML "brain" and maps the branches in the 
	/// Graphmaster structure. 
	/// </summary>
	///
	/// <remarks>
	/// This class incorporates much of the donkey work of
	/// searching the graphmaster structure to the specifications defined at:
	/// 
	/// http://www.alicebot.org/TR/2001/WD-aiml/
	/// 
	/// Specifically, the precedence and processing of the "_" and "*" wildcards as defined
	/// by the AIML standard (referenced above)/. 
	/// </remarks>

	public class cNodeMapper
	{
		#region PRIVATE ATTRIBUTES

		/// <summary>
		/// Represents the branches from this nodemapper
		/// </summary>
		private Hashtable htChildren = new Hashtable();

		/// <summary>
		/// the Category that is to be returned if this node is matched
		/// </summary>
		private cCategory thisCategory;

		#endregion

		#region PUBLIC ATTRIBUTES

		/// <summary>
		/// the word that identifies this nodemapper at the parent nodemapper (i.e. the
		/// nodemappers id within it's parent node)
		/// </summary>
		public string sWord;

		#endregion

		#region PUBLIC METHODS

		public cNodeMapper()
		{
			// move along please! nothing to see here :-)
		}

		/// <summary>
		/// Searches the nodemapper with the input string
		/// </summary>
		/// <param name="sInput">Normalised input concatenated with "that" and "topic" parameters</param>
		/// <param name="iMatchState">Denotes what part of the input path this node is a part of:
		/// 0=raw input
		/// 1="that"
		/// 2="topic"
		/// Used when pushing values represented by wildcards onto ArrayLists for
		/// the star, thatstar and topicstar AIML values</param>
		/// <param name="sWildcard">contents of the user input absorbed by the AIML wildcards "_" and "*"</param>
		/// <returns>the appropriate cCategory for the given input</returns>
		public cCategory evaluate(string sInput, int iMatchState, string sWildcard)
		{
			// make sure the input string is nice and tidy
			string sInputTrim = sInput.Trim();

			// o.k. if this is the end of a branch in the GraphMaster 
			// return the cCategory for this node
			if (thisCategory != null && htChildren.Count == 0)
			{
				cCategory tempCat;
				if (thisCategory!=null)
				{
					tempCat=new cCategory(thisCategory);
					tempCat.filename=thisCategory.filename;
					cGlobals.writeLog("Category found using path: "+thisCategory.sPath+"\r\nAnd with this template: "+thisCategory.sTemplate+"\r\nFrom this file: "+thisCategory.filename);
				}
				else
					tempCat=null;
				
				return tempCat;
			}

			// if we've matched all the words in the input sentence and this is the end
			// of the line then return the cCategory for this node
			if (sInputTrim.Length == 0)
			{
				cCategory tempCat;
				if (thisCategory!=null)
				{
					tempCat=new cCategory(thisCategory);
					tempCat.filename=thisCategory.filename;
				}
				else
					tempCat=null;
				return tempCat;
			}

			// otherwise split the input into it's component words
			string[] sWords = sInputTrim.Split(" \r\n\t".ToCharArray());

			// get the first word of the sentence
			string sFirstword;
			if ((sWords[0]=="<that>")||(sWords[0]=="<topic>")||(iMatchState>0))
			{
				sFirstword=sWords[0];
			}
			else
			{
				sFirstword = sWords[0].ToUpper();
			}

			// and concatenate the rest of the input into a suffix string
			string sMessagesuffix = sInputTrim.Substring(sFirstword.Length, sInputTrim.Length - sFirstword.Length);

			// first option is to see if this nodemapper contains a child  denoted by the "_" 
			// wildcard. "_" comes first in precedence in the AIML alphabet
			if (htChildren.ContainsKey("_"))
			{
				// o.k. look for the path in the child nodemapper denoted by "_"
				cNodeMapper LeafNode = (cNodeMapper)htChildren["_"];

				// Aha! we've found the right cNodeMapper (don't you just love it when stuff works!)
				if (LeafNode != null)
				{
					// move down into the identified branch of the GraphMaster structure
					cCategory tempcat = LeafNode.evaluate(sMessagesuffix, iMatchState, sWords[0]);
					// and if we get a result from the branch process and return it
					if (tempcat != null)
					{
						// capture and push the star content appropriate to the current matchstate
						switch(iMatchState)
						{
							case 0:
								if (sWildcard.Length>0)
								{
									tempcat.alInputStar.Insert(0, sWildcard);
								}
								break;
							case 1:
								if (sWildcard.Length>0)
								{
									tempcat.alThatStar.Insert(0, sWildcard);
								}
								break;
							case 2:
								if (sWildcard.Length>0)
								{
									tempcat.alTopicStar.Insert(0, sWildcard);
								}
								break;
						}
						return tempcat;
					}
				}
			}

			// second option - the nodemapper may have contained a "_" child, but led to no match
			// or it didn't contain a "_" child at all. So get the child nodemapper from this 
			// nodemapper that matches the first word of the input sentence.
			if (htChildren.ContainsKey(sFirstword))
			{
				
				// make sure we're not changing the iMatchState (encountering <that> or <topic> tags)
				if (sFirstword=="<that>") iMatchState=1;
				else if (sFirstword=="<topic>") iMatchState=2;
				else if ((iMatchState==2)&(sMessagesuffix.Length==0)) iMatchState=3;
				

				cNodeMapper LeafNode = (cNodeMapper)htChildren[sFirstword];

				// Aha! we've found the right cNodeMapper
				if (LeafNode != null)
				{
					// move down into the identified branch of the GraphMaster structure
					cCategory tempcat = LeafNode.evaluate(sMessagesuffix, iMatchState,"");
					// and if we get a result from the branch return it
					if (tempcat != null)
					{
						// capture and push the star content appropriate to the PREVIOUS matchstate
						switch(iMatchState)
						{
							case 0:
								if (sWildcard.Length>0)
								{
									tempcat.alInputStar.Insert(0, sWildcard);
								}
								break;
							case 1:
								if (sWildcard.Length>0)
								{
									tempcat.alInputStar.Insert(0, sWildcard);
								}
								break;
							case 2:
								if (sWildcard.Length>0)
								{
									tempcat.alThatStar.Insert(0, sWildcard);
								}
								break;
							case 3:
								if (sWildcard.Length>0)
								{
									tempcat.alTopicStar.Insert(0, sWildcard);
								}
								break;
						}
						return tempcat;
					}
				}
			}

			// third option - the input part of the path might have been matched so far but hasn't
			// returned a match, so check to see it contains the "*" wildcard. "*" comes last in
			// precedence in the AIML alphabet.
			if (htChildren.ContainsKey("*"))
			{
				// o.k. look for the path in the child nodemapper denoted by "*"
				cNodeMapper LeafNode = (cNodeMapper)htChildren["*"];

				// Aha! we've found the right cNodeMapper
				if (LeafNode != null)
				{
					// move down into the identified branch of the GraphMaster structure
					cCategory tempcat = LeafNode.evaluate(sMessagesuffix, iMatchState, sWords[0]);
					// and if we get a result from the branch process and return it
					if (tempcat != null)
					{
						// capture and push the star content appropriate to the current matchstate
						switch(iMatchState)
						{
							case 0:
								if (sWildcard.Length>0)
								{
									tempcat.alInputStar.Insert(0, sWildcard);
								}
								break;
							case 1:
								if (sWildcard.Length>0)
								{
									tempcat.alThatStar.Insert(0, sWildcard);
								}
								break;
							case 2:
								if (sWildcard.Length>0)
								{
									tempcat.alTopicStar.Insert(0, sWildcard);
								}
								break;
						}
						return tempcat;
					}
				}
			}

			// o.k. if the nodemapper has failed to match at all: the input contains neither 
			// a "_", the sFirstWord text, or "*" as a means of denoting a child node. However, 
			// if this node is itself representing a wildcard then the search continues to be
			// valid if we proceed with the tail.
			if ((sWord=="_")||(sWord=="*"))
			{
				return this.evaluate(sMessagesuffix, iMatchState, sWildcard+" "+sFirstword);			
			}

			// If we get here then we're at a dead end so return a null. Hopefully, if the
			// AIML files have been set up to include a "* <that> * <topic> *" catch-all this
			// state won't be reached.
			return null;
		}

		/// <summary>
		/// Adds a category to the graphmaster structure
		/// </summary>
		/// <param name="sEnd">a string to be mapped by this node</param>
		/// <param name="cat">the cCategory to be associated with the nodemapper</param>
		public void addCat(string sEnd, cCategory cat)
		{
			// this node represents the end of the sentence to be mapped so set 
			// the cCategory for this node to the cCategory that was passed to the 
			// function
			if (sEnd == "" || sEnd.Length <= 0)
			{
				thisCategory = cat;
				return;
			}

			// otherwise, this sentence requires further child nodemappers in order to
			// be fully mapped within the GraphMaster structure.

			// split the input into its component words
			string[] sWords = sEnd.Split( " ".ToCharArray() );

			// get the first word (to form the key for the child nodemapper)
			string sFirst = sWords[0];
			// concatenate the rest of the sentence into a suffix (to act as the
			// sEnd argument in the child nodemapper)
			string endstr = sEnd.Substring( sFirst.Length, sEnd.Length - sFirst.Length );
			endstr = endstr.Trim();

			// o.k. check we don't already have a child with the key from this sentence
			cNodeMapper wordmap = (cNodeMapper)htChildren[sFirst];

			// if we do then pass the handling of this sentence down the branch to the 
			// child nodemapper
			if (wordmap != null)
			{
				wordmap.addCat(endstr, cat);
			}
				// otherwise the child nodemapper doesn't yet exist, so create a new one
			else
			{
				cNodeMapper mapper = new cNodeMapper();
				mapper.sWord = sFirst;
				mapper.addCat(endstr, cat);
				htChildren.Add(mapper.sWord, mapper);
			}
		}

		#endregion
	}
}
