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
using System.IO;
using System.Resources;
using System.Reflection;

namespace AIMLBot
{
	/// <summary>
	/// Encapsulates a single instance of an AIML parsing chatterbot.
	/// See http://www.alicebot.org/TR/2001/WD-aiml/ for the specifications.
	/// </summary>
	public class cBot
	{
		#region PRIVATE ATTRIBUTES

		/// <summary>
		/// the Graphmaster structure for this particular bot
		/// </summary>
		private cGraphMaster GraphMaster = new cGraphMaster();

		/// <summary>
		/// The substitutions list to be applied during the normalisation process
		/// </summary>
		private Hashtable Substitutions = new Hashtable();

		/// <summary>
		/// Contains all the "tokens" used when splitting sentences during the normalisation
		/// process. The default initialisation adds ".", "!" and "?" for you.
		/// </summary>
		private ArrayList Splitters = new ArrayList();

		/// <summary>
		/// Holds the list of predicates (the keys) and their associated values
		/// </summary>
		private Hashtable Predicates = new Hashtable();

		/// <summary>
		/// keeps tabs of the number of categories currently stored in the Graphmaster brain
		/// </summary>
		private int size;

		#endregion

 		#region PUBLIC ATTRIBUTES

		/// <summary>
		/// The number of categories currently stored in the Graphmaster brain
		/// </summary>
		public int Size
		{
			get{return this.size;}
		}

		#endregion

		#region PRIVATE METHODS
		
		/// <summary>
		/// Returns an ArrayList of the names of all the .aiml files found within a 
		/// particular directory
		/// </summary>
		/// <param name="sPath">The directory path</param>
		/// <returns>The ArrayList containing the names of all the aiml files</returns>
		private ArrayList getAIMLFileList(string sPath)
		{
			// Create a reference to the AIML directory
			DirectoryInfo di = new DirectoryInfo(sPath);

			// Create an array representing the files in the current directory.
			FileInfo[] fi = di.GetFiles();

			// identify the files that are of type *.aiml and populate the ArrayList
			ArrayList alAIMLFileNames=new ArrayList();
			foreach (FileInfo fiTemp in fi)
			{
				if(fiTemp.Name.EndsWith(".aiml"))
				{
					alAIMLFileNames.Add(fiTemp.Name);
				}
			}

			return alAIMLFileNames;
		}

		/// <summary>
		/// Populates the predicates associated with the bot
		/// </summary>
		/// <param name="sPath">Path to the bot's predicate file (usually "botname.bot")</param>
		private void loadPredicates(string sPath)
		{
			try 
			{
				using (StreamReader sr = new StreamReader(System.IO.Path.Combine(sPath,"DEFAULT.bot"))) 
				{
					string sLine;
					while ((sLine = sr.ReadLine()) != null) 
					{
						string[] sWords = sLine.Split(" \r\n\t".ToCharArray());
						if (sWords.Length>0)
						{
							string sPredicate = sWords[0];
							string sValue = sLine.Substring(sPredicate.Length, sLine.Length - sPredicate.Length).Trim();
							if(!Predicates.ContainsKey(sPredicate))
								Predicates.Add(sPredicate,sValue);
						}
					}
					sr.Close();
					cGlobals.writeLog("Predicates loaded...");
				}
			}
			catch
			{
				cGlobals.writeLog("Unable to get the specified bot predicate file: "+sPath+"\n");
				cGlobals.writeLog("Attempting to create one...");
				
				try
				{
					Directory.CreateDirectory(cGlobals.BOTPATH);
					StreamWriter myFile = File.CreateText(System.IO.Path.Combine(cGlobals.BOTPATH,"DEFAULT.bot"));
					myFile.Write(cGlobals.DEFAULT);
					myFile.Close();
					this.loadPredicates(sPath);
				}
				catch
				{
					cGlobals.writeLog("Failed :-(\r\n\r\nCreate a DEFAULT.bot file in the bots directory");
				}
			}
		}


		/// <summary>
		/// Initialises the bot with the default (core brain) settings
		/// </summary>
		private void init()
		{
			this.init(cGlobals.AIMLPATH);
		}
		
		/// <summary>
		/// Initialises the bot with the default (core brain) settings
		/// </summary>
		/// <param name="path">Path to AIML files</param>
		private void init(string path)
		{
			cGlobals.writeLog("Starting Bot initialization...");
			// load the default predicates
			loadPredicates(cGlobals.BOTPATH);

			// get the .aiml file-names in the default directory
			ArrayList alAimlFiles = getAIMLFileList(path);

			// check we have some files to load!
			if (alAimlFiles.Count==0)
			{
				cGlobals.writeLog("No .aiml files were found in the directory: "+path);
				return;
			}
			
			// o.k. if we've got this far we should be able to populate the Graphmaster

			// load the aiml files into the GraphMaster
			cGlobals.writeLog("Starting to populate the graphmaster... (this may take some time)");
			cAIMLLoader loader = new cAIMLLoader(GraphMaster);
			cGlobals.writeLog(loader.loadAIML(cGlobals.AIMLPATH));
			
			size+=loader.counter;

			cGlobals.writeLog("Default AIML file initialisation finished. Bot has processed "+Convert.ToString(size)+" categories.");
			
			// populate the substitutions Hashtable
			Substitutions=cGlobals.getSubstitutions();

			cGlobals.writeLog("Default substitutions initialised...");
			
			this.grabSplitters();
		}

		private void grabSplitters()
		{
			// TODO: populate the splitters ArrayList from a file
			// some nice default values to have
			foreach(string sToken in cGlobals.SPLITTERS)
			{
				Splitters.Add(sToken);
			}
		}

		#endregion
		
		#region PUBLIC METHODS

		/// <summary>
		/// Initialises and sets up the bot and its related resources (such as the GraphMaster)
		/// </summary>
		/// <param name="sAIMLPath">the path to the specialist AIML files for this bot personality</param>
		/// <param name="isDebug">If set to true all sorts of useful information gets sent to the command line</param>
		public cBot(string sAIMLPath, bool isDebug)
		{
			cGlobals.isDebug=isDebug;
			this.init(sAIMLPath);
			cGlobals.writeLog("Initialisation of Bot completed.\n\nWaiting for conversations to start...\n");
		}
		
		/// <summary>
		/// Initialises and sets up the bot and its related resources (such as the GraphMaster)
		/// </summary>
		/// <param name="isDebug">If set to true all sorts of useful information gets sent to the command line</param>
		public cBot(bool isDebug)
		{
			cGlobals.isDebug=isDebug;
			this.init();
			cGlobals.writeLog("Initialisation of Bot completed.\n\nWaiting for conversations to start...\n");
		}

		/// <summary>
		/// Takes the user's raw input and returns a cResponse object for this bot
		/// </summary>
		/// <param name="sRawInput">the user's raw input</param>
		/// <param name="sUserID">the user's id</param>
		/// <returns>the cResponse object containing information about the reply from the bot</returns>
		public cResponse chat(string sRawInput, string sUserID)
		{ 
			// process the user
			cUser myUser;
			if(cUsers.users.ContainsKey(sUserID))
			{
				myUser=(cUser)cUsers.users[sUserID];
			}
			else
			{
				myUser=new cUser(sUserID);
				cUsers.users.Add(sUserID,myUser);
			}
			// get the "that" and "topic" fields
			string sNewThat=cNormalizer.substitute(cGlobals.getSubstitutions(), myUser.getThat());
			string sThat=cNormalizer.patternfit(sNewThat);
			if (sThat.Length==0) sThat="*";
			string sTopic=myUser.sTopic;

			// do some normalisation on the raw input
			string sSubstitutedInput = cNormalizer.substitute(Substitutions,sRawInput);
			ArrayList sSplitInput = cNormalizer.sentencesplit(Splitters,sSubstitutedInput);

			// to hold the bot's reply
			string[] sRawOutput={"",""};
			ArrayList alReply=new ArrayList();

			for (int i=0; i<sSplitInput.Count; i++)
			{
				string sFullInput=cNormalizer.patternfit((string)sSplitInput[i])+" <that> "+sThat+" <topic> "+sTopic;
				sRawOutput = GraphMaster.evaluate(sFullInput,this, sUserID);
				string sReply = (string)sRawOutput[1];
				// make sure everything is spaced out properly
				string[] sWords=sReply.Split(" \r\n\t".ToCharArray());
				sReply="";
				foreach(string sThisWord in sWords)
				{
					string sWord=sThisWord.Trim();
					if((sWord==".")||(sWord==",")||(sWord=="!")||(sWord=="?"))
					{
						sReply+=sWord;
					}
					else if (sWord=="")
					{
					}
					else
					{
						sReply+=" "+sWord;
					}
				}
				string sProcessedReply=sReply.Trim();
				alReply.Add(sProcessedReply);
			}

			// The following should never happen if you have your AIML sorted out properly!
			// Hint: try having a '*' pattern to catch all unknown input with responses like "Huh?", 
			// "I'm sorry I didn't understand that..." or "I'm afraid I don't understand"
			if (alReply.Count==0)
			{
				if(cGlobals.isDebug)
				{
					alReply.Add("WARNING! No reply generated by Graphmaster algorithm. Please contact the administrator of this software.");
					cGlobals.writeLog("WARNING! The Graphmaster algorithm failed to generate a reply under the following conditions.\r\nRawInput: "+sRawInput+"\r\n'that': "+sThat+"\r\n'topic': "+sTopic+"\r\n'UserID': "+sUserID+"\r\nPlease check your AIML files to make sure this input can be processed.");
				}
				else
				{
					alReply.Add("Hmmm, I'm not sure I understand what you mean. I think parts of my brain are missing. Please make sure I have access to a large set of AIML files. See http://www.alicebot.org/ for more information.");
				}
			}

			// Create the response object that is to be returned
			cResponse ReplyObject = new cResponse	(	sRawInput,sThat,sTopic, 
				sUserID, "Default", 
				alReply, sRawOutput[0]);

			// store the new <that> setting
			myUser.alThat.Insert(0,ReplyObject.getThat());

			// store the user's input into the "Input" arraylist
			myUser.alInput.Insert(0,cNormalizer.sentencesplit(this.Splitters,ReplyObject.sInput));

			return ReplyObject;
		}

		/// <summary>
		/// Allows for the logging of messages about this bot from other objects in the 
		/// AIML system
		/// </summary>
		/// <param name="sMessage">the message to be logged</param>
		public void log(string sMessage)
		{
			cGlobals.writeLog(sMessage);
		}

		/// <summary>
		/// Returns the value of a read-only bot predicate. Returns "" if the predicate doesn't exist
		/// </summary>
		/// <param name="Name">the predicate's name</param>
		/// <returns>the value associated with the predicate</returns>
		public string getPredicate(string Name)
		{
			if (!Predicates.Contains(Name)) return "";
			else
				return (string)Predicates[Name];
		}
		#endregion
	}
}
