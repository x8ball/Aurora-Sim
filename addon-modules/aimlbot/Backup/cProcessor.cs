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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace AIMLBot
{
	/// <summary>
	/// Contains all the methods for processing AIML found in the "template" part of the 
	/// cCategory returned by the Graphmaster.
	/// </summary>
	///
	/// <remarks>
	/// These routines are based upon the AIML specification v1.0.1 that can be found
	/// at:
	/// 
	/// http://www.alicebot.org/TR/2001/WD-aiml/
	/// </remarks>

	public class cProcessor
	{
		#region PRIVATE STATIC METHODS

		/// <summary>
		/// deals with the "random" tag.
		/// </summary>
		/// <param name="thisNode">the "random" node</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the text that is the result of processing this tag</returns>
		private static string random (XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// get the XML for processing
			string sContent=thisNode.OuterXml;
			XmlDocument choiceList=new XmlDocument();
			choiceList.LoadXml(sContent);
			
			// get the child nodes defining the list of outputs (one of which to be randomly
			// selected)
			XmlNodeList Choices=choiceList.GetElementsByTagName("li");

			// oops, no choices were found
			if (Choices.Count==0) return "";

			// o.k. select one of the choices at random and return it as a string
			Random r = new Random();

			// Process the selection and make sure we don't have to do anything else with it
			XmlNode Chosen=Choices.Item(r.Next(Choices.Count));

			// o.k. the <li> tag has AIML within it
			if (Chosen.HasChildNodes)
			{
				// process what is between the <li> tags
				string sNode=Chosen.InnerXml;
				cCategory LiCat = new cCategory(thisCat.sPath,"<template>"+sNode+"</template>");
				LiCat.alInputStar=thisCat.alInputStar;
				LiCat.alThatStar=thisCat.alThatStar;
				LiCat.alTopicStar=thisCat.alTopicStar;
				cCategory nullCat=process(LiCat,myBot,sUserID);
				return nullCat.sTemplate;
			}
				// otherwise return the plain text
			else
			{
				return (string)Chosen.InnerText;
			}
		}

		/// <summary>
		/// deals with the "star" tag.
		/// </summary>
		/// <param name="thisNode">the "star" node</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the text that is the result of processing this tag</returns>
		private static string star (XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// o.k. lets check that we actually have some InputStars (there is a small possibility that this
			// might happen if the AIML code is flunky)
			if (thisCat.alInputStar.Count>0)
			{
				// get the attributes for this tag
				XmlAttributeCollection myAttributes = thisNode.Attributes;
				
				// no attributes? then just return the last InputStar 
				if (myAttributes.Count==0)
				{
					return (string)thisCat.alInputStar[0];
				}
					// o.k. we have some attributes to process and, 
					// yeah I know upper case attributes are naff but I've seen 
					// some AIML with them in so I'm including this kludge just in case
				else
				{
					// to hold the location within the alIndexStar to be accessed 
					int iIndex;
					string sIndex;
					// get the value associated with the "index" attribute
					sIndex=thisNode.Attributes["index"].Value;
					if(sIndex!=null)
					{
						// get the iIndex of the right value
						iIndex=Convert.ToInt32(sIndex);
						// and remember that arrays start counting from 0! :-)
						iIndex-=1;
						// check we're within range and act appropriately
						if((iIndex<0)||(iIndex>thisCat.alInputStar.Count-1))
						{
							// check the bot logs!
							cGlobals.writeLog("The user "+sUserID+" caused the reference of an InputStar attribute that was out of range.\r\n");
							return "";
						}
						else
						{
							// Voila!
							return (string)thisCat.alInputStar[iIndex];
						}
					}
					else
						// get the value associated with the "INDEX" attribute
						sIndex=thisNode.Attributes["INDEX"].Value;
					if(sIndex!=null)
					{
						iIndex=Convert.ToInt32(sIndex);
						// ditto...
						if((iIndex<0)||(iIndex>thisCat.alInputStar.Count-1))
						{
							cGlobals.writeLog("The user "+sUserID+" caused the reference of an InputStar attribute that was out of range.\n");
							return "";
						}
						else
						{
							return (string)thisCat.alInputStar[iIndex];
						}
					}
				}
				// otherwise just return the last inputstar as default as the "index"
				// attribute wasn't found
				return (string)thisCat.alInputStar[0];
			}
			else
			{
				// if we get here it means the AIML is FUBAR
				cGlobals.writeLog("The user "+sUserID+" caused the return of a blank InputStar. CHECK THE AIML path: "+thisCat.sPath+"\n");
				return "";
			}
		}

		/// <summary>
		/// deals with the "that" tag.
		/// </summary>
		/// <param name="thisNode">the "thatstar" node</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string thatstar (XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// o.k. lets check that we actually have some ThatStars (there is a small possibility that this
			// might happen if the AIML code is flunky)
			if (thisCat.alThatStar.Count>0)
			{
				// get the attributes for this tag
				XmlAttributeCollection myAttributes = thisNode.Attributes;
				
				// no attributes? then just return the last ThatStar 
				if (myAttributes.Count==0)
				{
					return (string)thisCat.alThatStar[0];
				}
					// o.k. we have some attributes to process and, 
					// yeah I know upper case attributes are naff but I've seen 
					// some AIML with them in so I'm including this kludge just in case
				else
				{
					// to hold the location within the alThatStar to be accessed 
					int iIndex;
					string sIndex;
					// get the value associated with the "index" attribute
					sIndex=thisNode.Attributes["index"].Value;
					if(sIndex!=null)
					{
						// get the iIndex of the right value
						iIndex=Convert.ToInt32(sIndex);
						// and remember that arrays start counting from 0! :-)
						iIndex-=1;
						// check we're within range and act appropriately
						if((iIndex<0)||(iIndex>thisCat.alThatStar.Count-1))
						{
							// check the bot logs!
							cGlobals.writeLog("The user "+sUserID+" caused the reference of a ThatStar attribute that was out of range.\n");
							return "";
						}
						else
						{
							// Voila!
							return (string)thisCat.alThatStar[iIndex];
						}
					}
					else
						// get the value associated with the "INDEX" attribute
						sIndex=thisNode.Attributes["INDEX"].Value;
					if(sIndex!=null)
					{
						iIndex=Convert.ToInt32(sIndex);
						// ditto...
						if((iIndex<0)||(iIndex>thisCat.alThatStar.Count-1))
						{
							cGlobals.writeLog("The user "+sUserID+" caused the reference of a ThatStar attribute that was out of range.\n");
							return "";
						}
						else
						{
							return (string)thisCat.alThatStar[iIndex];
						}
					}
				}
				// otherwise just return the last thatstar as default as the "index"
				// attribute wasn't found
				return (string)thisCat.alThatStar[0];
			}
			else
			{
				// if we get here it means the AIML is FUBAR
				cGlobals.writeLog("The user "+sUserID+" caused the return of a blank ThatStar. CHECK THE AIML path: "+thisCat.sPath+"\n");
				return "";
			}
		}

		/// <summary>
		/// deals with the "topicstar" tag.
		/// </summary>
		/// <param name="thisNode">the "topicstar" node</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string topicstar (XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// o.k. lets check that we actually have some TopicStars (there is a small possibility that this
			// might happen if the AIML code is flunky)
			if (thisCat.alTopicStar.Count>0)
			{
				// get the attributes for this tag
				XmlAttributeCollection myAttributes = thisNode.Attributes;
				
				// no attributes? then just return the last ThatStar 
				if (myAttributes.Count==0)
				{
					return (string)thisCat.alTopicStar[0];
				}
					// o.k. we have some attributes to process and, 
					// yeah I know upper case attributes are naff but I've seen 
					// some AIML with them in so I'm including this kludge just in case
				else
				{
					// to hold the location within the alTopicStar to be accessed 
					int iIndex;
					string sIndex;
					// get the value associated with the "index" attribute
					sIndex=thisNode.Attributes["index"].Value;
					if (sIndex==null)
					{
						// get the value associated with the "INDEX" attribute
						sIndex=thisNode.Attributes["INDEX"].Value;
					}
					if(sIndex!=null)
					{
						// get the iIndex of the right value
						iIndex=Convert.ToInt32(sIndex);
						// and remember that arrays start counting from 0! :-)
						iIndex-=1;
						// check we're within range and act appropriately
						if((iIndex<0)||(iIndex>thisCat.alTopicStar.Count-1))
						{
							// check the bot logs!
							cGlobals.writeLog("The user "+sUserID+" caused the reference of a TopicStar attribute that was out of range.\n");
							return "";
						}
						else
						{
							// Voila!
							return (string)thisCat.alTopicStar[iIndex];
						}
					}
					else	
					{
						// the "index" is not an attribute so return the default value
						return (string)thisCat.alTopicStar[0];
					}
				}
			}
			else
			{
				// if we get here it means the AIML is FUBAR
				cGlobals.writeLog("The user "+sUserID+" caused the return of a blank TopicStar. CHECK THE AIML path: "+thisCat.sPath+"\n");
				return "";
			}
		}

		/// <summary>
		/// deals with the "that" tag
		/// </summary>
		/// <param name="thisNode">the "that" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string that(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// the conversation histories are stored in the cUser object for the user
			cUser thisUser = (cUser)cUsers.users[sUserID];

			// o.k. lets check that we actually have some "That"s (there is a small possibility that this
			// might happen if the AIML code is flunky)

			if (thisUser.alThat.Count>0)
			{
				// get the attributes for this tag
				XmlAttributeCollection myAttributes = thisNode.Attributes;
				
				// no attributes? then just return the last That
				if (myAttributes.Count==0)
				{
					ArrayList alLastThat=(ArrayList)thisUser.alThat[0];
					return (string)alLastThat[0];
				}
					// o.k. we have some attributes to process and, 
					// yeah I know upper case attributes are naff but I've seen 
					// some AIML with them in so I'm including this kludge just in case
				else
				{
					string sIndex;
					// get the value associated with the "index" attribute
					sIndex=thisNode.Attributes["index"].Value;
					if (sIndex==null)
					{
						// get the value associated with the "INDEX" attribute
						sIndex=thisNode.Attributes["INDEX"].Value;
					}

					if(sIndex!=null)
					{
						// o.k. if we're here then there is an index to a particular "that" statement
						
						// the index can be either in one or two dimensions
						string sFirstDimension, sSecondDimension;
						int iFirst,iSecond;

						// extract the first dimension
						sFirstDimension=(string)sIndex[0].ToString();
						iFirst=Convert.ToInt32(sFirstDimension);
						iFirst-=1;

						// check the first dimension is in range
						if((iFirst>thisUser.alThat.Count-1)||(iFirst<0))
						{
							// write a warning message to the bot log
							cGlobals.writeLog("The user "+sUserID+" caused a reference to a <that> that was out of range. Check AIML for path:"+thisCat.sPath+"\n");
							// for safety's sake return the default value for <that>
							ArrayList alLastThat=(ArrayList)thisUser.alThat[0];
							return (string)alLastThat[0];
						}
						// now check if we have a second dimension
						if(sIndex.Length==3) // sIndex will be something like "1,2"
						{
							sSecondDimension=(string)sIndex[2].ToString();
							iSecond=Convert.ToInt32(sSecondDimension);
							iSecond-=1;
						}
						else
						{
							iSecond=0;
						}
						
						// get the appropriate arraylist of sentences
						ArrayList alLastThatSentences=(ArrayList)thisUser.alThat[iFirst];

						// check the second dimension is in range
						if((iSecond>alLastThatSentences.Count-1)||(iSecond<0))
						{
							// write a warning message to the bot log
							cGlobals.writeLog("The user "+sUserID+" caused a reference to a two dimensional <that> that was out of range. Check AIML for path:"+thisCat.sPath+"\n");
							// for safety's sake return the default value for <that>
							ArrayList alLastThat=(ArrayList)thisUser.alThat[0];
							return (string)alLastThat[0];
						}

						// okay, we have two dimensions and they're in range so return the correct result!
						return (string)alLastThatSentences[iSecond];
					}
					else
					{
						// index is not one of the attributes so just return the default
						ArrayList alLastThat=(ArrayList)thisUser.alThat[0];
						return (string)alLastThat[0];
					}
				}
			}
			else
			{
				// if we get here it means the AIML is FUBAR
				cGlobals.writeLog("The user "+sUserID+" caused the return of a blank THAT. CHECK THE AIML path: "+thisCat.sPath+"\n");
				return "";
			}
		}

		/// <summary>
		/// deals with the "input" tag
		/// </summary>
		/// <param name="thisNode">the "input" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string input(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// the conversation histories are stored in the cUser object for the user
			cUser thisUser = (cUser)cUsers.users[sUserID];

			// o.k. lets check that we actually have some "That"s (there is a small possibility that this
			// might happen if the AIML code is flunky)

			if (thisUser.alInput.Count>0)
			{
				// get the attributes for this tag
				XmlAttributeCollection myAttributes = thisNode.Attributes;
				
				// no attributes? then just return the last That
				if (myAttributes.Count==0)
				{
					ArrayList alLastInput=(ArrayList)thisUser.alInput[0];
					return (string)alLastInput[0];
				}
					// o.k. we have some attributes to process and, 
					// yeah I know upper case attributes are naff but I've seen 
					// some AIML with them in so I'm including this kludge just in case
				else
				{
					string sIndex;
					// get the value associated with the "index" attribute
					sIndex=thisNode.Attributes["index"].Value;
					if (sIndex==null)
					{
						// get the value associated with the "INDEX" attribute
						sIndex=thisNode.Attributes["INDEX"].Value;
					}

					if(sIndex!=null)
					{
						// o.k. if we're here then there is an index to a particular "that" statement
						
						// the index can be either in one or two dimensions
						string sFirstDimension, sSecondDimension;
						int iFirst,iSecond;

						// extract the first dimension
						sFirstDimension=(string)sIndex[0].ToString();
						iFirst=Convert.ToInt32(sFirstDimension);
						iFirst-=1;

						// check the first dimension is in range
						if((iFirst>thisUser.alInput.Count-1)||(iFirst<0))
						{
							// write a warning message to the bot log
							cGlobals.writeLog("The user "+sUserID+" caused a reference to a <that> that was out of range. Check AIML for path:"+thisCat.sPath+"\n");
							// for safety's sake return the default value for <that>
							ArrayList alLastInput=(ArrayList)thisUser.alInput[0];
							return (string)alLastInput[0];
						}
						// now check if we have a second dimension
						if(sIndex.Length==3) // sIndex will be something like "1,2"
						{
							sSecondDimension=(string)sIndex[2].ToString();
							iSecond=Convert.ToInt32(sSecondDimension);
							iSecond-=1;
						}
						else
						{
							iSecond=0;
						}
						
						// get the appropriate arraylist of sentences
						ArrayList alLastInputSentences=(ArrayList)thisUser.alInput[iFirst];

						// check the second dimension is in range
						if((iSecond>alLastInputSentences.Count-1)||(iSecond<0))
						{
							// write a warning message to the bot log
							cGlobals.writeLog("The user "+sUserID+" caused a reference to a two dimensional <that> that was out of range. Check AIML for path:"+thisCat.sPath+"\n");
							// for safety's sake return the default value for <that>
							ArrayList alLastInput=(ArrayList)thisUser.alInput[0];
							return (string)alLastInput[0];
						}

						// okay, we have two dimensions and they're in range so return the correct result!
						return (string)alLastInputSentences[iSecond];
					}
					else
					{
						// index is not one of the attributes so just return the default
						ArrayList alLastInput=(ArrayList)thisUser.alInput[0];
						return (string)alLastInput[0];
					}
				}
			}
			else
			{
				// if we get here it means the AIML is FUBAR
				cGlobals.writeLog("The user "+sUserID+" caused the return of a blank INPUT. CHECK THE AIML path: "+thisCat.sPath+"\n");
				return "";
			}
		}

		/// <summary>
		/// deals with the "get" tag
		/// </summary>
		/// <param name="thisNode">the "get" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string getTag(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// get the attributes for this tag
			XmlAttributeCollection myAttributes = thisNode.Attributes;
				
			// no attributes? then just return a blank
			if (myAttributes.Count==0)
			{
				return "";
			}
			// o.k. we have an attribute to process
			// get the value associated with the "name" attribute
			string sName=thisNode.Attributes["name"].Value;
			cUser myUser = (cUser)cUsers.users[sUserID];
			string sValue=(string)myUser.Predicates[sName];
			if (sValue==null) sValue="";
			return sValue.Trim();
		}

		/// <summary>
		/// deals with the "bot" tag
		/// </summary>
		/// <param name="thisNode">the "bot" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string bot(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// get the attributes for this tag
			XmlAttributeCollection myAttributes = thisNode.Attributes;
				
			// no attributes? then just return a blank
			if (myAttributes.Count==0)
			{
				return "";
			}
			// o.k. we have an attribute to process
			// get the value associated with the "name" attribute
			string sName=thisNode.Attributes["name"].Value;
			return (string)myBot.getPredicate(sName);
		}

		/// <summary>
		/// deals with the "set" tag
		/// </summary>
		/// <param name="thisNode">the "set" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string setTag(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			// get the attributes for this tag
			XmlAttributeCollection myAttributes = thisNode.Attributes;
				
			// no attributes? then just return a blank
			if (myAttributes.Count==0)
			{
				return "";
			}
			// o.k. we have an attribute to process
			// get the value(s) associated with the "name" attribute
			string sName=thisNode.Attributes["name"].Value;
			string sContent=thisNode.InnerText;
			string sStarXML=thisNode.InnerXml;

			// ultimately holds the processed value for this predicate
			string sValue="";

			// check for further processing
			if(sStarXML.StartsWith("<star"))
			{
				XmlDocument starXML=new XmlDocument();
				starXML.LoadXml(sStarXML);
				// get the node that we need
				XmlNode starnode=starXML.SelectSingleNode("descendant::star");
				sValue=star(starnode, thisCat,myBot,sUserID);
			}
			else
			{
				sValue=sContent;
			}

			// make sure any names are stored correctly
			if (sName.ToUpper()=="NAME")
			{
				string sNewValue=formal(sValue);
				sValue=sNewValue;
				thisNode.InnerText=" "+sValue;
			}

			cUser myUser=(cUser)cUsers.users[sUserID];

			// handle the topic predicate otherwise it is a generic predicate
			if (sName.ToUpper()=="TOPIC")
			{
				if ((sValue.Length==0)||(sValue=="")||(sValue==" ")||(sValue==null))
				{
					sValue="*";
				}
				myUser.sTopic=sValue;
			}
			else
			{
				if(myUser.Predicates.Contains(sName))
				{
					myUser.Predicates.Remove(sName);
				}
				myUser.Predicates.Add(sName,sValue);
			}
			return thisNode.InnerText;
		}

		/// <summary>
		/// deals with the "upper" tag
		/// </summary>
		/// <param name="thisNode">the "upper" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string upper(XmlNode thisNode, cCategory thisCat)
		{
			return (string)thisNode.InnerText.ToUpper();
		}

		/// <summary>
		/// deals with the "lower" tag
		/// </summary>
		/// <param name="thisNode">the "lower" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string lower(XmlNode thisNode, cCategory thisCat)
		{
			return (string)thisNode.InnerText.ToLower();
		}

		/// <summary>
		/// deals with the "formal" tag
		/// </summary>
		/// <param name="thisNode">the "formal" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string formal(XmlNode thisNode, cCategory thisCat)
		{
			string sFormal="";
			// lowercase it all first
			string sText=(string)thisNode.InnerText.ToString().ToLower().Trim();
			// split into individual characters
			char[] cWords=sText.ToCharArray();
			for(int i=0; i<cWords.Length;i++)
			{
				if(i==0)
				{
					char cChanged = char.ToUpper(cWords[i]);
					cWords[i]=cChanged;
				}
				else
				{
					if(cWords[i-1]==' ') 
					{
						char cChanged =  char.ToUpper(cWords[i]);
						cWords[i]=cChanged;
					}
				}
				sFormal+=(string)Convert.ToString(cWords[i]);
			}
			return sFormal;
		}

		/// <summary>
		/// overloaded "formal" text processing
		/// </summary>
		/// <param name="thisNode">the "formal" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string formal(string sInput)
		{
			string sFormal="";
			// lowercase it all first
			string sText=sInput.ToLower().Trim();
			// split into individual characters
			char[] cWords=sText.ToCharArray();
			for(int i=0; i<cWords.Length;i++)
			{
				if(i==0)
				{
					char cChanged = char.ToUpper(cWords[i]);
					cWords[i]=cChanged;
				}
				else
				{
					if(cWords[i-1]==' ') 
					{
						char cChanged =  char.ToUpper(cWords[i]);
						cWords[i]=cChanged;
					}
				}
				sFormal+=(string)Convert.ToString(cWords[i]);
			}
			return sFormal;
		}

		/// <summary>
		/// deals with the "sentence" tag
		/// </summary>
		/// <param name="thisNode">the "sentence" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string sentence(XmlNode thisNode, cCategory thisCat)
		{
			string sSentence="";
			// lowercase it all first
			string sText=(string)thisNode.InnerText.ToString().ToLower().Trim();
			// split into individual characters
			char[] cWords=sText.ToCharArray();
			for(int i=0; i<cWords.Length;i++)
			{
				if(i==0)
				{
					char cChanged =  char.ToUpper(cWords[i]);
					cWords[i]=cChanged;
				}
				else
				{
					if(i>1)
					{
						if((cWords[i-2]=='.')&(cWords[i-1]==' ')) 
						{
							char cChanged =  char.ToUpper(cWords[i]);
							cWords[i]=cChanged;
						}
					}
				}	
				sSentence+=(string)Convert.ToString(cWords[i]);
			}
			return sSentence;
		}

		/// <summary>
		/// deals with the "think" tag
		/// </summary>
		/// <param name="thisNode">the "think" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		private static void think(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			string sNode=thisNode.InnerXml;
			cCategory thinkCat = new cCategory(thisCat.sPath,"<template>"+sNode+"</template>");
			thinkCat.alInputStar=thisCat.alInputStar;
			thinkCat.alThatStar=thinkCat.alThatStar;
			thinkCat.alTopicStar=thinkCat.alTopicStar;
			cCategory nullCat=process(thinkCat,myBot,sUserID);
		}

		/// <summary>
		/// deals with the "person" tag
		/// </summary>
		/// <param name="thisNode">the "person" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		private static string person(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			string sNode=thisNode.InnerText;
			if (sNode=="")
			{
				string sContent="<star/>";
				XmlDocument tempDoc = new XmlDocument();
				tempDoc.LoadXml(sContent);
				XmlNode myNode=tempDoc.SelectSingleNode("star");
				sNode=star(myNode, thisCat, myBot, sUserID);
			}
			string sOutput=(string)cNormalizer.substitute(cGlobals.getPersonSubs(),sNode);
			return (string)sOutput.ToLower();
		}

		/// <summary>
		/// deals with the "person2" tag
		/// </summary>
		/// <param name="thisNode">the "person2" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		private static string person2(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			string sNode=thisNode.InnerText;
			if (sNode=="")
			{
				string sContent="<star/>";
				XmlDocument tempDoc = new XmlDocument();
				tempDoc.LoadXml(sContent);
				XmlNode myNode=tempDoc.SelectSingleNode("star");
				sNode=star(myNode, thisCat, myBot, sUserID);
			}
			string sOutput=(string)cNormalizer.substitute(cGlobals.getPerson2Subs(),sNode);
			return (string)sOutput.ToLower();
		}

		/// <summary>
		/// deals with the "gender" tag
		/// </summary>
		/// <param name="thisNode">the "gender" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		private static string gender(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			string sNode=thisNode.InnerText;
			if (sNode=="")
			{
				string sContent="<star/>";
				XmlDocument tempDoc = new XmlDocument();
				tempDoc.LoadXml(sContent);
				XmlNode myNode=tempDoc.SelectSingleNode("star");
				sNode=star(myNode, thisCat, myBot, sUserID);
			}
			string sOutput=(string)cNormalizer.substitute(cGlobals.getGenderSubs(),sNode);
			return (string)sOutput.ToLower();
		}

		/// <summary>
		/// deals with the "srai" tag
		/// </summary>
		/// <param name="thisNode">the "srai" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string srai(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			string sNode=thisNode.InnerText;
			cResponse myReply = myBot.chat(sNode, sUserID);
			string sReply="";
			foreach(string sSentence in myReply.alOutput)
			{
				sReply+=sSentence+" ";
			}
			return sReply;
		}

		/// <summary>
		/// deals with the "sr" tag
		/// </summary>
		/// <param name="thisNode">the "sr" node in question</param>
		/// <param name="thisCat">the current category</param>
		/// <param name="myBot">the bot whose graphmaster returned the "template"</param>
		/// <param name="sUserID">the user who requires a reply</param>
		/// <returns>the string that results in the processing of this node</returns>
		private static string sr(XmlNode thisNode, cCategory thisCat, cBot myBot, string sUserID)
		{
			string sContent="<star/>";
			XmlDocument tempDoc = new XmlDocument();
			tempDoc.LoadXml(sContent);
			XmlNode myNode=tempDoc.SelectSingleNode("star");
			string sStar=star(myNode, thisCat, myBot, sUserID);

			string sNode="<srai>"+sStar+"</srai>";
			XmlDocument tempSR = new XmlDocument();
			tempSR.LoadXml(sNode);
			XmlNode srNode=tempSR.SelectSingleNode("srai");

			return (string)srai(srNode,thisCat,myBot,sUserID);
		}

		

		/// <summary>
		/// processes the individual aiml tags found within the "target" portion of a category
		/// </summary>
		/// <param name="thisNode">the xml node to be processed</param>
		/// <param name="InputCat">the category that the node is a part of</param>
		/// <param name="myBot">the bot (doh!)</param>
		/// <param name="sUserID">id's the user (what did you think it did?)</param>
		/// <returns></returns>
		private static string processtag(XmlNode thisNode, cCategory InputCat, cBot myBot, string sUserID)
		{
			// to be returned...
			string sOutput="";
			// holds the inner text from the child nodes in case they need to be displayed
			string sChildInnerText="";

			// get the name of the node
			string sNodeName=thisNode.Name.ToUpper();

			// check for children and process them
			if((thisNode.HasChildNodes)&((sNodeName!="THINK")&(sNodeName!="RANDOM")))
			{
				XmlNodeList TagChildren=thisNode.ChildNodes;
				foreach(XmlNode Child in TagChildren)
				{
					sChildInnerText+=processtag(Child, InputCat,myBot,sUserID);
				}
				thisNode.InnerText=sChildInnerText;
			}

			// do the on this particular node
			switch(sNodeName)
			{
				case "RANDOM":
					sOutput+= random(thisNode, InputCat, myBot, sUserID);
					break;
				case "SIZE":
				{
					sOutput+= Convert.ToString(myBot.Size);
					break;
				}
				case "ID":
					sOutput+= (string)sUserID;
					break;
				case "DATE":
					sOutput+= (string)DateTime.Now.ToString();
					break;
				case "VERSION":
					sOutput+= cGlobals.VERSION;
					break;
				case "STAR":
					sOutput+= " "+star(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "THAT":
					sOutput+= " "+that(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "INPUT":
					sOutput+= " "+input(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "THATSTAR":
					sOutput+= " "+thatstar(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "TOPICSTAR":
					sOutput+= " "+topicstar(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "GET":
					sOutput+= " "+getTag(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "BOT":
					sOutput+= " "+bot(thisNode, InputCat, myBot, sUserID)+" ";
					break;
				case "UPPERCASE":
					sOutput+= upper(thisNode, InputCat);
					break;
				case "LOWERCASE":
					sOutput+= lower(thisNode, InputCat);
					break;
				case "FORMAL":
					sOutput+= formal(thisNode, InputCat);
					break;
				case "SENTENCE":
					sOutput+= sentence(thisNode, InputCat);
					break;
				case "SR":
					sOutput+= sr(thisNode, InputCat, myBot, sUserID);
					break;
					/*case "CONDITION":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;*/
				case "SET":
					sOutput+= setTag(thisNode, InputCat, myBot, sUserID);
					break;
				case "GOSSIP":
					// Not really important so implement at a later date when some
					// use for it is found
					break;
				case "SRAI":
					sOutput+= srai(thisNode, InputCat, myBot, sUserID);
					break;
				case "PERSON":
					sOutput+= person(thisNode, InputCat, myBot, sUserID);
					break;
				case "PERSON2":
					sOutput+= person2(thisNode, InputCat, myBot, sUserID);
					break;
				case "GENDER":
					sOutput+= gender(thisNode, InputCat, myBot, sUserID);
					break;
				case "THINK":
					think(thisNode, InputCat, myBot, sUserID);
					break;
					/*case "LEARN":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;
						case "SYSTEM":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;
						case "JAVASCRIPT":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;
						/*case "":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;
						case "":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;
						case "":
							outputCategory.sTemplate+= func(thisNode, InputCat);
							break;

							...add modifications here*/
				default:
					sOutput+=thisNode.InnerText.Trim();
					break;
			}
			return " "+sOutput;
		}
		#endregion

		#region PUBLIC STATIC METHODS

		/// <summary>
		/// Given a cCategory, returns the same object but with the template section processed and changed appropriately 
		/// </summary>
		/// <param name="InputCat">the cCategory whose AIML template section is to be processed</param>
		/// <param name="myBot">the botID of the bot whose Graphmaster is being interrogated</param>
		/// <param name="sUserID">the userID of the person who requires a reply</param>
		/// <returns>the Ccategory to return whose sTemplate is the final reply sentence for the given input</returns>
		public static cCategory process(cCategory InputCat, cBot myBot, string sUserID)
		{
			// get the XML for processing
			string sContent=InputCat.ToString();
			XmlDocument Template=new XmlDocument();
			Template.LoadXml(sContent);
			if(Template.HasChildNodes)
			{

				// get the child nodes found within the template.
				XmlNodeList TemplateChildren=Template.DocumentElement.ChildNodes;

				// the cCategory to return
				cCategory outputCategory=new cCategory(InputCat);
			
				// replace the output part of the cCategory with a blank (to be filled in 
				// by the following processes)
				outputCategory.sTemplate="";

				// process each of the child nodes that have been found in the template
				foreach(XmlNode thisNode in TemplateChildren)
				{
					// go to a method that is basically a bit switch statement
					outputCategory.sTemplate+=cProcessor.processtag(thisNode,InputCat,myBot,sUserID);
				}

				// do some simple normalization on the template
				outputCategory.checktext();
				return outputCategory;
			}
			else

				// if no child nodes are found then the template must already contain a raw
				// string so just return it
				return InputCat;
		}
		#endregion
	}
}
