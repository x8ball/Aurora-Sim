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
	/// Represents a single user. Stores helpful information like a predicate list 
	/// specific to this user and instantiates a log for the user.
	/// </summary>
	///
	/// <remarks>
	/// This is really just a helpful class to have around when administering the system.
	/// </remarks>

	public class cUser
	{
		#region PRIVATE ATTRIBUTES

		/// <summary>
		/// a copy of the sUserID that is the object's key in the Users hashtable in cAIMLBot
		/// </summary>
		private string sUserID;

		#endregion

		#region PUBLIC ATTRIBUTES
		/// <summary>
		/// the last utterence from the bot to the user
		/// </summary>
		public ArrayList alThat=new ArrayList();

		/// <summary>
		/// the last input from the user to the bot
		/// </summary>
		public ArrayList alInput=new ArrayList();

		/// <summary>
		/// the value of the "topic" predicate
		/// </summary>
		public string sTopic;

		/// <summary>
		/// the predicates associated with this particular user
		/// </summary>
		public Hashtable Predicates = new Hashtable();

		#endregion
		
		#region PUBLIC METHODS

		public cUser(string sUserID)
		{
			// set up the private attributes with appropriate values
			this.sUserID=sUserID;
			sTopic="*";
			// some nice default user predicates
			Predicates.Add("name","un-named user");
			Predicates.Add("he","somebody");
			Predicates.Add("she","somebody");
			Predicates.Add("it","something");
			Predicates.Add("they","something");
			Predicates.Add("age","unknown");
			Predicates.Add("birthday","unknown");
			Predicates.Add("boyfriend","unknown");
			Predicates.Add("brother","unknown");
			Predicates.Add("cat","unknown");
			Predicates.Add("does","unknown");
			Predicates.Add("dog","unknown");
			Predicates.Add("email","unknown");
			Predicates.Add("father","unknown");
			Predicates.Add("favcolor","unknown");
			Predicates.Add("favmovie","unknown");
			Predicates.Add("friend","unknown");
			Predicates.Add("fullname","unknown");
			Predicates.Add("gender","unknown");
			Predicates.Add("girlfriend","unknown");
			Predicates.Add("has","unknown");
			Predicates.Add("heard","unknown");
			Predicates.Add("husband","unknown");
			Predicates.Add("is","unknown");
			Predicates.Add("job","unknown");
			Predicates.Add("lastname","unknown");
			Predicates.Add("like","unknown");
			Predicates.Add("location","unknown");
			Predicates.Add("looklike","unknown");
			Predicates.Add("memory","unknown");
			Predicates.Add("meta","unknown");
			Predicates.Add("nickname","unknown");
			Predicates.Add("middlename","unknown");
			Predicates.Add("mother","unknown");
			Predicates.Add("password","unknown");
			Predicates.Add("personality","unknown");
			Predicates.Add("phone","unknown");
			Predicates.Add("sign","unknown");
			Predicates.Add("sister","unknown");
			Predicates.Add("them","unknown");
			Predicates.Add("thought","unknown");
			Predicates.Add("want","unknown");
			Predicates.Add("we","unknown");																																						Predicates.Add("wife","unknown");
		}

		/// <summary>
		/// adds a log message for this particular user
		/// </summary>
		/// <param name="sLogMessage">the message to add to the log</param>
		public void addlog(string sLogMessage)
		{
			cGlobals.writeLog(sLogMessage);
		}

		/// <summary>
		/// returns the last full reply from the bot as a single string (as oppose to 
		/// the arraylist that stores it)
		/// </summary>
		/// <returns>the bot's last reply</returns>
		public string getThat()
		{
			if (alThat.Count==0) return "*";
			string sThat="";
			ArrayList alLastThat=(ArrayList)alThat[0];
			foreach (string sSentence in alLastThat)
			{
				sThat+=sSentence+". ";
			}
			return sThat;
		}

		#endregion
	}
}
