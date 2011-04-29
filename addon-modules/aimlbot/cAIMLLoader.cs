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
using System.Xml;
using System.IO;

namespace AIMLBot
{
	/// <summary>
	/// Takes care of processing and loading AIML files into the graphmaster
	/// </summary>
	///
	/// <remarks>
	/// Used the specification of AIML v.1.0.1 as the basis for storing the various
	/// categories. See http://www.alicebot.org/TR/2001/WD-aiml/ for more information.
	/// </remarks>

	public class cAIMLLoader
	{
		#region PRIVATE ATTRIBUTES

		/// <summary>
		/// Holds the message the the cAIMLLoader returns to the bot that calls it
		/// for inclusion in the bot's logfile
		/// </summary>
		private string message;

		/// <summary>
		/// represents the Graphmaster to be populated by the AIML files
		/// </summary>
		private cGraphMaster myGraphMaster;

		#endregion

		#region PUBLIC ATTRIBUTES

		/// <summary>
		/// counts the number of categories added to the bot's brain
		/// </summary>
		public int counter;

		#endregion

		#region PRIVATE METHODS
		
		/// <summary>
		/// processes "topic" nodes and adds any categories found as children into the 
		/// graphmaster's set of nodes
		/// </summary>
		/// <param name="thisTopic">the topic XmlNode to be processed</param>
		/// <returns>An arraylist of cCategories that are contained within this topic</returns>
		private void processtopic(XmlNode thisTopic, string filename)
		{
			// o.k. lets process this topic!

			// turn the XmlNode into an XmlDocument for easier processing
			string topic=thisTopic.OuterXml;
			XmlDocument topicDoc = new XmlDocument();
			topicDoc.LoadXml(topic);

			// get the root of the newly created XmlDoc (i.e. the <topic name="Topic name"> tag)
			XmlElement root = topicDoc.DocumentElement;

			// get the value of the name value from the <topic name="Topic name"> tag
			string sTopicName=root.Attributes[0].Value;

			// if we have a topic tag thats knackered or empty then make it the default
			// empty value (an asterisk)
			if ((sTopicName.Length<1)||(sTopicName==null))
			{
				sTopicName="*";
			}

			// get the list of category nodes found within this topic
			XmlNodeList topiccategories=topicDoc.GetElementsByTagName("category");
			
			// o.k. process all the category nodes
			foreach(XmlNode thisNode in topiccategories)
			{
				// o.k., create and store a cCategory with the values from the aiml file
				processcategory(thisNode,sTopicName,filename);
			}
		}

		/// <summary>
		/// processes "category" nodes into cCategory objects for the graphmaster
		/// </summary>
		/// <param name="thisCategory">the category to be added to the bot's "brain"</param>
		private void processcategory(XmlNode thisCategory, string filename)
		{
			processcategory(thisCategory,"*", filename);
		}

		/// <summary>
		/// processes "category" nodes into cCategory objects for the graphmaster
		/// topic tag
		/// </summary>
		/// <param name="thisCategory">the category to be added to the bot's "brain"</param>
		private void processcategory(XmlNode thisCategory, string sTopic, string filename)
		{
			// turn the XmlNode into an XmlDocument for easier processing
			string node=thisCategory.OuterXml;
			XmlDocument cat=new XmlDocument();
			cat.LoadXml(node);
			
			// get the three types of node that we need
			XmlNode pattern=cat.SelectSingleNode("descendant::pattern");
			XmlNode that=cat.SelectSingleNode("descendant::that");
			XmlNode template=cat.SelectSingleNode("descendant::template");

			// will hold the PATH for the new category
			string aimlPath;
			// will hold the response TEMPLATE for the new category
			string aimlTemplate;

			// check and add the <pattern> node's contents to the PATH
			aimlPath=pattern.InnerText;
			// split the pattern into it's component words
			string[] sPathWords = aimlPath.Split(" \r\n\t".ToCharArray());
			string sProcessedInput="";
			// Normalize all words unless they're the AIML wildcards "*" and "_"
			foreach (string sWord in sPathWords)
			{
				string sFinalWord;
				if ((sWord=="*")||(sWord=="_"))
				{
					sFinalWord=sWord;
				}
				else
				{
					string sNormalizedPath = cNormalizer.substitute(cGlobals.getSubstitutions(),sWord);
					string sPatternFit = cNormalizer.patternfit(sNormalizedPath);
					sFinalWord=sPatternFit.ToUpper();
				}
				sProcessedInput+=sFinalWord.ToUpper()+" ";
			}
			// just in case...
			aimlPath=sProcessedInput.Trim();			

			// normalize the "that" parameter of the AIML
			string sThat;
			if (that!=null)
			{
				if (that.InnerText=="")
				{
					sThat="*";
				}
				else
				{
					string sThatPath = that.InnerText;
					// split the pattern into it's component words
					string[] sThatWords = sThatPath.Split(" \r\n\t".ToCharArray());
					sProcessedInput="";
					// Normalize all words unless they're the AIML wildcards "*" and "_"
					foreach (string sWord in sThatWords)
					{
						string sFinalWord;
						if ((sWord=="*")||(sWord=="_"))
						{
							sFinalWord=sWord;
						}
						else
						{
							string sNormalizedThat = cNormalizer.substitute(cGlobals.getSubstitutions(),sWord);
							string sPatternFit = cNormalizer.patternfit(sNormalizedThat);
							sFinalWord=sPatternFit.ToUpper();
						}
						sProcessedInput+=sFinalWord+" ";
					}
					sThat = sProcessedInput.Trim();
				}
			}
			else
			{
				sThat="*";
			}
			// normalize the "topic" parameter of the AIML
			string sNormTopic = cNormalizer.substitute(cGlobals.getSubstitutions(), sTopic);
			sTopic = cNormalizer.patternfit(sNormTopic);
			if (sTopic=="") sTopic="*";

			// Add the <that> node's contents to the path
			aimlPath+=" <that> "+sThat;
			
			// append an approptiate topic entry
			aimlPath+=" <topic> "+sTopic;

			// put the total XML content of the <template> tag in the TEMPLATE string
			aimlTemplate=template.OuterXml;

			// o.k., add the processed AIML to the GraphMaster structure
			try
			{
				myGraphMaster.addCat(aimlPath, aimlTemplate, filename);
				// keep count of the number of categories that have been processed
				counter++;
			}
			catch
			{
				message+="WARNING!!!!\n Failed to load a new category into brain where:\nPATH= "+aimlPath+"\nTEMPLATE= "+aimlTemplate+"\n";
			}
		}

		/// <summary>
		/// Processes the categories found within a certain file 
		/// </summary>
		/// <param name="sFilename">the AIML file in question</param>
		private void addfile(string sFilename)
		{
			cGlobals.writeLog("Processing file: "+sFilename);
			
			// Reference the AIML file that is to be loaded
			XmlTextReader reader = new XmlTextReader(sFilename);
			reader.WhitespaceHandling = WhitespaceHandling.None;

			// Create an XMLDocument object to be interpreted and populate it
			// with the XML data from the AIML file
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			
				// Get a list of the nodes that are children of the <aiml> tag
				// these nodes should only be either <topic> or <category>
				// the <topic> nodes will contain more <category> nodes
				XmlNodeList RootChildren=doc.DocumentElement.ChildNodes;

				// process each of these child nodes
				foreach(XmlNode thisNode in RootChildren)
				{
					if (thisNode.Name=="topic")
					{
						processtopic(thisNode, sFilename);
					}
					else if (thisNode.Name=="category")
					{
						processcategory(thisNode, sFilename);
					}
				}
				reader.Close();
			}
			catch(Exception e)
			{
				cGlobals.writeLog("Error: \r\n"+e.Message);
			}
		}

		/// <summary>
		/// Given a directory path get the aiml files and extract the categories from them
		/// </summary>
		/// <param name="pathToAIML">the path to the AIML directory</param>
		/// <returns></returns>
		//private ArrayList processdirectory(string pathToAIML)
		private void processdirectory(string pathToAIML)
		{
			// TODO: This has already happened by the time the program gets here.
			// Remove duplication!

			// Process the list of files found in the directory.
			string[] fileEntries = Directory.GetFiles(pathToAIML);
			ArrayList alAIMLFiles=new ArrayList();
			foreach(string sFileName in fileEntries)
			{
				if(sFileName.EndsWith(".aiml"))
				{
					alAIMLFiles.Add(sFileName);
				}
			}

			// Check we've got some .aiml files to process
			if(alAIMLFiles.Count==0)
			{
				message+="WARNING!!!!!\nNo AIML files (ending in .aiml) found in the directory: "+pathToAIML+"\n";
			}
			
			// o.k., process each of the .aiml files into cCategory objects and return them
			foreach(string sFileName in alAIMLFiles)
			{
				addfile(sFileName);
			}
		}

		#endregion

		#region PUBLIC ATTRIBUTES
		
		/// <summary>
		/// Doh! This is a constructor - gets the graphmaster brain to be loaded and 
		/// resets everything to nice values for the logs :-)
		/// </summary>
		/// <param name="CurrentGraphMaster">the Graphmaster structure to be populated by the AIML code</param>
		/// <param name="sBotID">the bot ID used for identification in the logs</param>
		public cAIMLLoader(cGraphMaster CurrentGraphMaster)
		{
			myGraphMaster=CurrentGraphMaster;
			// reset everything to nice values for the logs :-)
			counter=0;
			message="AIMLLoader object created on"+(string)DateTime.Now.ToString()+"\n";
		} 

		/// <summary>
		/// Given a path to a directory, this method will look for *.aiml files, process them
		/// into cCategory objects and add them to the bot's "brain". Returns any messages about
		/// the status of the upload into the GraphMaster brain.
		/// </summary>
		/// <param name="pathToAIML">the path to the directory holding the AIML files</param>
		/// <returns>a message string to be included in the bot's log file</returns>
		public string loadAIML(string pathToAIML)
		{
			if(Directory.Exists(pathToAIML)) 
			{
				// This path is a directory
				processdirectory(pathToAIML);
				message+="\nProcessing of directory: "+pathToAIML+" completed at time: "+DateTime.Now.ToString()+"\n\n";
				message+=(string)this.counter.ToString()+" categories added to the bot.\r\n";
				return message;
			}
			else 
			{
				// Oops! we don't seem to have a valid directory path
				message+="WARNING!!!!!!!\nThe directory: "+pathToAIML+" doesn't seem to exist.\nDirectory skipped\n";
				return message;
			}        
		}

		#endregion	
	}
}
