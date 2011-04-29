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
	/// A simple data class that holds information about the reply from a bot.
	/// </summary>

	public class cResponse
	{
		#region ATTRIBUTES

		private string userID;
		/// <summary>
		/// the userID who caused this response
		/// </summary>
		public string sUserID
		{
			get{return this.userID;}
			set{this.userID=value;}
		}

		private string botID;
		/// <summary>
		/// the botID that generated this response
		/// </summary>
		public string sBotID
		{
			get{return this.botID;}
			set{this.botID=value;}
		}

		private string input;
		/// <summary>
		/// the raw input from the user
		/// </summary>
		public string sInput
		{
			get{return this.input;}
			set{this.input=value;}
		}

		private ArrayList output;
		/// <summary>
		/// the response from the bot
		/// </summary>
		public ArrayList alOutput
		{
			get{return this.output;}
			set{this.output=value;}
		}

		private string that;
		/// <summary>
		/// the "that" value (previous bot output)
		/// </summary>
		public string sThat
		{
			get{return this.that;}
			set{this.that=value;}
		}

		private string topic;
		/// <summary>
		/// the value of the "topic" predicate
		/// </summary>
		public string sTopic
		{
			get{return this.topic;}
			set{this.topic=value;}
		}

		private string file;
		/// <summary>
		/// the AIML file from where the response was defined (useful for debugging)
		/// </summary>
		public string sAIMLFile
		{
			get{return this.file;}
			set{this.file=value;}
		}

		#endregion

		#region PUBLIC METHOD

		/// <summary>
		/// Define the values contained within the cResponse object
		/// </summary>
		/// <param name="sInput">the user's input</param>
		/// <param name="sThat">the "that" value (previous bot output)</param>
		/// <param name="sTopic">the value of the "topic" predicate</param>
		/// <param name="sUserID">the user's ID</param>
		/// <param name="sBotID">the bot's ID</param>
		/// <param name="alOutput">the bot's reply</param>
		/// <param name="sFile">the file that defined the output AIML</param>
		public cResponse	(	string sInput, string sThat, string sTopic,
			string sUserID, string sBotID, ArrayList alOutput, string sFile)
		{
			this.input=sInput;
			this.that=sThat;
			this.topic=sTopic;
			this.userID=sUserID;
			this.botID=sBotID;
			this.output=alOutput;
			this.file=sFile;
		}

		/// <summary>
		/// Returns the bot's reply as a string rather than as the ArrayList representation that
		/// is stored in the cReply class
		/// </summary>
		/// <returns>the formatted reply string</returns>
		public string getOutput()
		{
			string sOutput="";
			if (alOutput.Count==0) return "";
			else
			{
				foreach(string sSentence in alOutput)
				{
					sOutput+=sSentence+" ";
				}
			}
			return sOutput;
		}

		/// <summary>
		/// Returns an appropriately processed arraylist of the output sentences for the next "that" 
		/// portion of a conversation
		/// </summary>
		/// <returns>an appropriately processed arraylist of the output sentences</returns>
		public ArrayList getThat()
		{
			ArrayList alReturn = new ArrayList();
			if (alOutput.Count==0) return alOutput;
			else
			{
				foreach(string sSentence in alOutput)
				{
					alReturn.Add(sSentence);
				}
				return alReturn;
			}
		}
		#endregion
	}
}
