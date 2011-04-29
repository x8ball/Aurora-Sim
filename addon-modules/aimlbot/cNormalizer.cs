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
	/// A utility class that performs the various tasks involved in normalizing the
	/// user input.
	/// </summary>
	///
	/// <remarks>
	/// The functionality of this class is based upon the requirements outlined at 
	/// http://www.alicebot.org/TR/2001/WD-aiml/#section-input-normalization 
	/// 
	/// To quote from the above source:
	/// 
	/// An AIML interpreter must perform a "normalization" function on all inputs before 
	/// attempting to match. The minimum set of normalizations is called pattern-fitting 
	/// normalizations. Additional normalizations performed at user option are called 
	/// sentence-splitting normalizations and substitution normalizations (or just 
	/// "substitutions").
	/// </remarks>

	public class cNormalizer
	{
		#region PRIVATE ATTRIBUTES

		/// <summary>
		/// Helpful definition of an empty string
		/// </summary>
		private static string EMPTY="";

		#endregion
		
		#region PUBLIC METHODS

		public cNormalizer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Performs substitution normalisations on the raw input. Returns a string containing
		/// the processed input.
		/// </summary>
		///
		/// <param name="SubsTable"> A hashtable that contains all the target strings and the 
		/// appropriate replacements
		/// </param>
		/// <param name="sRawInput"> The raw input to be processed
		/// </param>

		public static string substitute(Hashtable SubsTable, string sRawInput)
		{
			// Check we have some valid input else return a blank
			if (sRawInput.Length<1) return (string) EMPTY;

			// Check we have stuff to search and replace with
			// if not return the raw input
			if (SubsTable.Count<1) return (string) sRawInput;

			// holds the results of the sustitution
			string sOutput="";

			//string sUpperInput=sRawInput.ToUpper();
			string sUpperInput=sRawInput;
			
			// Iterate through the hashtable
			IDictionaryEnumerator myHashEnumerator = SubsTable.GetEnumerator();
			while (myHashEnumerator.MoveNext())
			{
				// Get the target string and its replacement from the hashtable
				string sFind= (string) myHashEnumerator.Key; 
				string sReplace= (string) myHashEnumerator.Value;	
			
				// do the find / replace
				sOutput=sUpperInput.Replace(sFind,sReplace);
				sUpperInput=sOutput;
			}
			
			string sFinal = sOutput.Trim();

			// Wham-bang-thankyou-very-much!
			return (string) sFinal;
		}

		/// <summary>
		/// performs sentence splitting normalisation. Given an input and definition of
		/// splitter tokens this method will return a string array of sentences that make
		/// up the raw input.
		/// </summary>
		///
		/// <param name="splitters"> An arraylist containing the characters that are used to 
		/// define the end of sentences; ".", "!" and "?" being the most common.
		/// </param>
		/// <param name="sRawInput"> The raw input to be split.
		/// would go.
		/// </param>

		public static ArrayList sentencesplit(ArrayList alSplitters, string sRawInput)
		{
			// check we have some data to split, else return an empty array.
			ArrayList alEmptyArray = new ArrayList();
			alEmptyArray.Add(" ");
			if (sRawInput.Length==0) return alEmptyArray;

			// check we have some splitters to work with
			// else return the raw input
			if (alSplitters.Count==0) 
			{
				ArrayList alOutput = new ArrayList();
				alOutput.Add(sRawInput);
				return alOutput;
			}
			// to store the sentence array
			ArrayList alIndices = new ArrayList();

			// so far so good... lets split those sentences!

			// Iterate through the char array of splitter tokens
			for(int i=0; i<alSplitters.Count;i++)
			{
				string sToken=(string)alSplitters[i];
				
				// get the position of the splitter token in the input
				int iIndex = sRawInput.IndexOf(sToken);
				
				// keep going as long as the token exists within the string
				while (iIndex!=-1)
				{
					// store it in the list of indices
					alIndices.Add((int)iIndex);

					// keep going from the point we've just reached
					iIndex = sRawInput.IndexOf(sToken,iIndex+1);
				}
			}

			// if we haven't found any splitters return the raw input as a single
			// sentence in an Arraylist
			if (alIndices.Count==0)
			{
				ArrayList alOutput = new ArrayList();
				alOutput.Add(sRawInput);
				return alOutput;
			}

			// Sort the list of all indices
			alIndices.Sort();

			// holds the processed indices
			ArrayList alProcessedIndices = new ArrayList();

			// Iterate through the indices and remove consecutive instances (eg ".." or "?!?!")
			IEnumerator myIndicesEnumerator = alIndices.GetEnumerator();
			
			// get the position of the first instance of a target splitter
			myIndicesEnumerator.MoveNext();
			int iPreviousIndex = (int)myIndicesEnumerator.Current;
			// and put it in a new arraylist
			alProcessedIndices.Add(iPreviousIndex);
			
			// iterate and compare the positions of target splitters
			while (myIndicesEnumerator.MoveNext())
			{
				int iNextIndex = (int)myIndicesEnumerator.Current;
				// if the next target splitter is not adjacent to the last one
				// add it to the new array list of processed indices
				if (iNextIndex!=(iPreviousIndex+1))
				{
					alProcessedIndices.Add(iNextIndex);
				}
				// update our position in the indices array
				iPreviousIndex=iNextIndex;
			}

			// temporary store of the sentences 
			ArrayList alSplitSentences = new ArrayList();

			// Now lets iterate through the raw input and split it into 
			// sentences at the points held in alProcessedIndices
			IEnumerator myProcessedEnumerator = alProcessedIndices.GetEnumerator();
			int iStart=0;
			int iEnd=sRawInput.Length-1;
			while (myProcessedEnumerator.MoveNext())
			{
				iEnd=(int)myProcessedEnumerator.Current;
				int iStop=iEnd-iStart;
				string sTempSentence=sRawInput.Substring(iStart, iStop+1).Trim();
				alSplitSentences.Add((string)sTempSentence);
				iStart=iEnd+1;
			}

			// REMEMBER to add what is left at the end :-) doh!
			if (iStart<sRawInput.Length-1)
			{
				string sTempSentence=sRawInput.Substring(iStart).Trim();
				alSplitSentences.Add((string)sTempSentence);
			}

			return alSplitSentences;
		}

		/// <summary>
		/// Takes some raw input and removes all non-standard characters (puntuation
		/// and so on) and returns an uppercase version of the processed string.
		/// </summary>
		///
		/// <param name="sRawInput"> The raw input to be processed
		/// </param>

		public static string patternfit(string sRawInput)
		{
			// check we have some input
			if (sRawInput.Length==0)
			{
				return EMPTY;
			}

			// convert to a char array
			char[] cTempArray = sRawInput.ToCharArray();
			
			ArrayList alProcessedChars = new ArrayList();

			// iterate through the char array 
			for (int i=0; i<cTempArray.Length; i++)
			{
				// check its a valid character...
				// valid in this case means:
				// " "(space), "0-9", "a-z" and "A-Z"
				if (	(cTempArray[i]>64)&(cTempArray[i]<91)	|| // A-Z
					(cTempArray[i]>96)&(cTempArray[i]<123)	|| // a-z
					(cTempArray[i]>47)&(cTempArray[i]<58) || // 0-9
					(cTempArray[i]==32))  // space
				{
					alProcessedChars.Add((char)cTempArray[i]);
				}
			}

			// turn the arraylist into a char array
			char[] cProcessed = new char[alProcessedChars.Count];
			
			for (int i=0; i<alProcessedChars.Count;i++)
			{
				cProcessed[i]=(char)alProcessedChars[i];
			}
			string sOutput = new string(cProcessed);
			
			// some final beautification (strip extra spaces and turn to uppercase)
			//string sUpper=sOutput.ToUpper();
			//string sReturn=sUpper.Trim();
			string sReturn=sOutput.Trim();

			// et voila!
			return sReturn;
		}

		#endregion
	}
}
