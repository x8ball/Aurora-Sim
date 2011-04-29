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
	/// Stores the global settings for the AIMLBot as static attributes
	/// </summary>
	///
	/// <remarks>
	/// Sets up some default settings but these can be over-ridden in a settings file.
	/// </remarks>

	public class cGlobals
	{
		#region PUBLIC ATTRIBUTES

		public static bool isDebug;

		/// <summary>
		/// The version number of this instance of the AIMLBot system
		/// </summary>
		public static string VERSION="0.1";

		/// <summary>
		/// The default path for AIMLBot system files
		/// </summary>
		public static  string PATH=Environment.CurrentDirectory;

		/// <summary>
		/// The default size for a log before it is dumped to the filesystem
		/// </summary>
		public static  int SYSTEM_DUMP_SIZE=5;
		
		/// <summary>
		/// the default path for the AIML files within the system path
		/// </summary>
		public static  string AIMLPATH=System.IO.Path.Combine(PATH,"aiml");

		/// <summary>
		/// the default path for the bot predicate files
		/// </summary>
		public static string BOTPATH=System.IO.Path.Combine(PATH,"bots");

		/// <summary>
		/// the default path for the system log files within the system path
		/// </summary>
		public static  string LOGPATH=System.IO.Path.Combine(PATH,"logs");

		/// <summary>
		/// the name of the full system log file within the log path
		/// </summary>
		public static  string SYSTEM_LOG_FILE=System.IO.Path.Combine(LOGPATH,"system.log");

		/// <summary>
		/// the default path for the user logs within the log path
		/// </summary>
		public static  string USER_LOGS=System.IO.Path.Combine(LOGPATH,"users");

		/// <summary>
		/// the default path for the bot logs within the log path
		/// </summary>
		public static  string BOT_LOGS=System.IO.Path.Combine(LOGPATH,"bots");

		/// <summary>
		/// the default characters to use as splitters between sentences
		/// </summary>
		public static string[] SPLITTERS = {".","!","?",";"};

		#endregion

		#region PUBLIC STATIC METHODS

		/// <summary>
		/// Yes I know this is a kludge, it'll need fixing in a later version!!!
		/// </summary>
		/// <returns>an arraylist containing all the paths used by the system</returns>
		public static ArrayList getPaths()
		{
			ArrayList alPaths = new ArrayList();
			alPaths.Add(PATH);
			alPaths.Add(AIMLPATH);
			alPaths.Add(LOGPATH);
			alPaths.Add(USER_LOGS);
			alPaths.Add(BOT_LOGS);
			alPaths.Add(BOTPATH);
			return alPaths;
		}

		/// <summary>
		/// Yup, another kludge, but it'll do for now 
		/// </summary>
		/// <returns>a hashtable of default substitutions</returns>
		public static Hashtable getSubstitutions()
		{
			Hashtable subs = new Hashtable();
			subs.Add("=REPLY" ,"");
			subs.Add("NAME=RESET" ,"");
			subs.Add(":-)" ," smile ");
			subs.Add(":)" ," smile ");
			subs.Add(",)" ," smile ");
			subs.Add(";)" ," smile ");
			subs.Add(";-)" ," smile ");
			subs.Add("&QUOT;" ,"");
			subs.Add("/" ," ");
			subs.Add("&GT;" ," gt ");
			subs.Add("&LT;" ," lt ");
			subs.Add("(" ," ");
			subs.Add(")" ," ");
			subs.Add("`" ," ");
			subs.Add("," ," ");
			subs.Add(":" ," ");
			subs.Add("&AMP;" ," ");
			subs.Add("-" ,"-");
			subs.Add("=" ," ");
			subs.Add("  " ," ");
			subs.Add("L A" ," la ");
			subs.Add(" O K " ," ok ");
			subs.Add(" P S " ," ps ");
			subs.Add("OHH" ,"oh");
			subs.Add("HEHE" ,"he");
			subs.Add("HAHA" ,"ha");    
			subs.Add("HELLP" ,"help ");
			subs.Add("BECAUS" ,"because ");
			subs.Add("BELEIVE" ,"believe ");
			subs.Add("BECASUE" ,"because ");
			subs.Add("BECUASE" ,"because ");
			subs.Add("BECOUSE" ,"because ");
			subs.Add("REDUCTIONALISM" ,"reductionism ");
			subs.Add(" ITS A " ," it is a ");
			subs.Add("NOI" ," yes I ");
			subs.Add("FAV" ," favorite ");
			subs.Add("YESI" ," yes I ");
			subs.Add("YESIT" ," yes it ");
			subs.Add("IAM" ," I am ");
			subs.Add("WELLI" ," well I ");
			subs.Add("WELLIT" ," well it ");
			subs.Add("AMFINE" ," am fine ");
			subs.Add("AMAN" ," am an ");
			subs.Add("AMON" ," am on ");
			subs.Add("AMNOT" ," am not ");
			subs.Add("REALY" ,"really");
			subs.Add("IAMUSING" ," I am using ");
			subs.Add("AMLEAVING" ," am leaving ");
			subs.Add("YEAH" ,"yes");
			subs.Add("YEP" ,"yes");
			subs.Add("YHA" ,"yes");
			subs.Add("YOU" ,"you");
			subs.Add("WANNA" ," want to ");
			subs.Add("YOU'D" ," you would ");
			subs.Add("YOU'RE" ," you are ");
			subs.Add(" YOU RE " ," you are ");
			subs.Add("YOU'VE" ," you have ");
			subs.Add(" YOU VE " ," you have ");
			subs.Add("YOU'LL" ," you will ");
			subs.Add(" YOU LL " ," you will ");
			subs.Add("YOURE" ," you are ");
			subs.Add("DIDNT" ," did not ");
			subs.Add(" DIDN T " ," did not ");
			subs.Add("DID'NT" ," did not ");
			subs.Add("COULDN'T" ," could not ");
			subs.Add(" COULDN T " ," could not ");
			subs.Add("DIDN'T" ," did not ");
			subs.Add("AIN'T" ," is not ");
			subs.Add(" AIN T " ," is not ");
			subs.Add("ISN'T" ," is not ");
			subs.Add(" ISN T " ," is not ");
			subs.Add("ISNT" ," is not ");
			subs.Add("IT'S" ," it is ");
			subs.Add(" IT S " ," it is ");
			subs.Add("ARE'NT" ," are not ");
			subs.Add("ARENT" ," are not ");
			subs.Add("AREN'T" ," are not ");
			subs.Add(" AREN T " ," are not ");
			subs.Add(" ARN T " ," are not ");
			subs.Add("WHERE'S" ," where is ");
			subs.Add(" WHERE S " ," where is ");
			subs.Add("HAVEN'T" ," have not ");
			subs.Add("HAVENT" ," have not ");
			subs.Add("HASN'T" ," has not ");
			subs.Add(" HASN T " ," has not ");
			subs.Add(" WEREN T " ," were not ");
			subs.Add("WEREN'T" ," were not ");
			subs.Add("WERENT" ," were not ");
			subs.Add("CAN'T" ," can not ");
			subs.Add(" CAN T " ," can not ");
			subs.Add("CANT" ," can not ");
			subs.Add("CANNOT" ," can not ");
			subs.Add("WHOS" ," who is ");
			subs.Add("HOW'S" ," how is ");
			subs.Add(" HOW S " ," how is ");
			subs.Add("HOW'D" ," how did ");
			subs.Add(" HOW D " ," how did ");
			subs.Add("HOWS" ," how is ");
			subs.Add("WHATS" ," what is ");
			subs.Add("NAME'S" ," name is ");
			subs.Add("WHO'S" ," who is ");
			subs.Add(" WHO S " ," who is ");
			subs.Add("WHAT'S" ," what is ");
			subs.Add(" WHAT S " ," what is ");
			subs.Add("THAT'S" ," that is ");
			subs.Add("THERE'S" ," there is ");
			subs.Add(" THERE S " ," there is ");
			subs.Add("THERES" ," there is ");
			subs.Add("THATS" ," that is ");
			subs.Add("DOESN'T" ," does not ");
			subs.Add(" DOESN T " ," does not ");
			subs.Add("DOESNT" ," does not ");
			subs.Add("DON'T" ," do not ");
			subs.Add(" DON T " ," do not ");
			subs.Add("DONT" ," do not ");
			subs.Add(" DO NT " ," do not ");
			subs.Add("DO'NT" ," do not ");
			subs.Add("WON'T" ," will not ");
			subs.Add("WONT" ," will not ");
			subs.Add(" WON T " ," will not ");
			subs.Add("LET'S" ," let us ");
			subs.Add("THEY'RE" ," they are ");
			subs.Add(" THER RE " ," they are ");
			subs.Add("WASN'T" ," was not ");
			subs.Add(" WASN T " ," was not ");
			subs.Add("WASNT" ," was not ");
			subs.Add("HADN'T" ," had not ");
			subs.Add(" HADN T " ," had not ");
			subs.Add("WOULDN'T" ," would not ");
			subs.Add(" WOULDN T " ," would not ");
			subs.Add("WOULDNT" ," would not ");
			subs.Add("SHOULDN'T" ," should not ");
			subs.Add("SHOULDNT" ," should not ");
			subs.Add("FAVOURITE" ," favorite ");
			subs.Add("COLOUR" ," color ");
			subs.Add("WE'LL" ," we will ");
			subs.Add(" WE LL " ," we will ");
			subs.Add("HE'LL" ," he will ");
			subs.Add(" HE LL " ," he will ");
			subs.Add("I'LL" ," I will ");
			subs.Add("IVE" ," I have ");
			subs.Add("I'VE" ," I have ");
			subs.Add(" I VE " ," I have ");
			subs.Add("I'D" ," I would ");
			subs.Add("I'M" ," I am ");
			subs.Add(" I M " ," I am ");
			subs.Add("WE'VE" ," we have ");
			subs.Add("WE'RE" ," we are ");
			subs.Add("SHE'S" ," she is ");
			subs.Add("SHES" ," she is ");
			subs.Add("SHE'D" ," she would ");
			subs.Add(" SHE D " ," she would ");
			subs.Add("SHED" ," she would ");
			subs.Add("HE'D" ," he would ");
			subs.Add(" HE D " ," he would ");
			subs.Add("HED" ," he would ");
			subs.Add("HE'S" ," he is ");
			subs.Add(" WE VE " ," we have ");
			subs.Add(" WE RE " ," we are ");
			subs.Add(" SHE S " ," she is ");
			subs.Add(" HE S " ," he is ");
			subs.Add("IAMA" ," I am a ");
			subs.Add("IAMASKING" ," I am asking ");
			subs.Add("IAMDOING" ," I am doing ");
			subs.Add("IAMFROM" ," I am from ");
			subs.Add("IAMIN" ," I am in ");
			subs.Add("IAMOK" ," I am ok ");
			subs.Add("IAMSORRY" ," I am sorry ");
			subs.Add("IAMTALKING" ," I am talking ");
			subs.Add("IAMTIRED" ," I am tired ");
			subs.Add(" DOWN LOAD " ," download ");
			subs.Add("REMEBER" ," remember ");
			subs.Add("WAHT" ," what ");
			subs.Add("TOLLERVY" ,"Tollervey");
			subs.Add(" YOU R " ," you are ");
			subs.Add(" U " ," you ");
			subs.Add(" UR " ," your ");
			subs.Add(" RU " ," are you ");
			subs.Add(" R U " ," are you ");
			subs.Add(" {" ," beginscript ");
			subs.Add(" }" ," endscript ");
			subs.Add(@" \" ," ");
			subs.Add(":0" ," 0");
			subs.Add(": 0" ," 0");
			subs.Add(":1" ," 1");
			subs.Add(": 1" ," 1");
			subs.Add(":2" ," 2");
			subs.Add(": 2" ," 2");
			subs.Add(":3" ," 3");
			subs.Add(": 3" ," 3");
			subs.Add(":4" ," 4");
			subs.Add(": 4" ," 4");
			subs.Add(":5" ," 5");
			subs.Add(": 5" ," 5");
			subs.Add(".0" ," point 0");
			subs.Add(".1" ," point 1");
			subs.Add(".2" ," point 3");
			subs.Add(".4" ," point 4");
			subs.Add(".5" ," point 5");
			subs.Add(".6" ," point 6");
			subs.Add(".7" ," point 7");
			subs.Add(".8" ," point 8");
			subs.Add(".9" ," point 9");
			subs.Add("DR." ," Dr ");
			subs.Add(" DR . " ," Dr ");
			subs.Add("MR." ," Mr ");
			subs.Add("MRS." ," Mrs ");
			subs.Add("ST." ," St ");
			subs.Add("WWW." ," www dot ");
			subs.Add("BOTSPOT." ," botspot dot ");
			subs.Add("AMUSED.COM" ," amused dot com ");
			subs.Add("WHATIS." ," whatis dot ");
			subs.Add(".COM " ," dot com ");
			subs.Add(".NET " ," dot net ");
			subs.Add(".ORG " ," dot org ");
			subs.Add(".ORG.UK"," dot org dot uk ");
			subs.Add(".EDU " ," dot edu ");
			subs.Add(".UK " ," dot uk ");
			subs.Add(".JP " ," dot jp ");
			subs.Add(".AU " ," dot au ");
			subs.Add(".CO" ," dot co ");
			subs.Add(".AC" ," dot ac ");
			subs.Add("O.K." ," ok ");
			subs.Add(" O. K. " ," ok ");
			subs.Add("L.L." ," l l ");
			subs.Add("P.S." ," ps ");
			subs.Add(" U S A " ," USA ");
			subs.Add(" U. S. A. " ," USA ");
			subs.Add("U.S.A." ," USA ");
			subs.Add("U.S." ," USA ");
			subs.Add("PH.D" ," PhD ");
			subs.Add(" A. " ," a ");
			subs.Add(" B. " ," b ");
			subs.Add(" C. " ," c ");
			subs.Add(" D. " ," d ");
			subs.Add(" E. " ," e ");
			subs.Add(" F. " ," f ");
			subs.Add(" G. " ," g ");
			subs.Add(" H. " ," h ");
			subs.Add(" I. " ," i ");
			subs.Add(" J. " ," j ");
			subs.Add(" K. " ," k ");
			subs.Add(" L. " ," l ");
			subs.Add(" M. " ," m ");
			subs.Add(" N. " ," n ");
			subs.Add(" P. " ," p ");
			subs.Add(" O. " ," o ");
			subs.Add(" Q. " ," q ");
			subs.Add(" R. " ," r ");
			subs.Add(" S. " ," s ");
			subs.Add(" T. " ," t ");
			subs.Add(" U. " ," u ");
			subs.Add(" V. " ," v ");
			subs.Add(" X. " ," x ");
			subs.Add(" Y. " ," y ");
			subs.Add(" W. " ," w ");
			subs.Add(" Z. " ," z ");
			subs.Add(".JAR" ," jar");
			subs.Add(".ZIP" ," zip");
			subs.Add(", BUT " ,". ");
			subs.Add(", AND " ,". ");
			subs.Add(",BUT " ,". ");
			subs.Add(",AND " ,". ");
			subs.Add("  BUT " ,". ");
			subs.Add("  AND " ,". ");
			subs.Add(", I " ,". I ");
			subs.Add(", YOU " ,". you ");
			subs.Add(",I " ,". I ");
			subs.Add(",YOU " ,". you ");
			subs.Add(", WHAT " ,". what ");
			subs.Add(",WHAT " ,". what ");
			subs.Add(", DO " ,". do ");
			subs.Add(",DO " ,". do ");
			return subs;
		}

		/// <summary>
		/// returns the hashtable of gender substitutions
		/// </summary>
		/// <returns>the hashtable of gender substitutions</returns>
		public static Hashtable getGenderSubs()
		{
			Hashtable genderSubs = new Hashtable();
			genderSubs.Add(" HE ","she");
			genderSubs.Add(" SHE " ," he ");
			genderSubs.Add(" TO HIM " ," to her ");
			genderSubs.Add(" FOR HIM " ," for her ");
			genderSubs.Add(" WITH HIM " ," with her ");
			genderSubs.Add(" ON HIM " ," on her ");
			genderSubs.Add(" IN HIM " ," in her ");
			genderSubs.Add(" TO HER " ," to him ");
			genderSubs.Add(" FOR HER " ," for him ");
			genderSubs.Add(" WITH HER " ," with him ");
			genderSubs.Add(" ON HER " ," on him ");
			genderSubs.Add(" IN HER " ," in him ");
			genderSubs.Add(" HIS " ," her ");
			genderSubs.Add(" HER " ," his ");
			genderSubs.Add(" HIM " ," her ");
			return (Hashtable)genderSubs;
		}

		/// <summary>
		/// returns the hashtable of person substitutions
		/// </summary>
		/// <returns>the hashtable of person substitutions</returns>
		public static Hashtable getPersonSubs()
		{
			Hashtable personSubs = new Hashtable();
			personSubs.Add(" I WAS " ," he or she was ");
			personSubs.Add(" HE WAS " ," I was ");
			personSubs.Add(" SHE WAS " ," I was ");
			personSubs.Add(" I AM " ," he or she is ");
			personSubs.Add(" I " ," he or she ");
			personSubs.Add(" ME " ," him or her ");
			personSubs.Add(" MY " ," his or her ");
			personSubs.Add(" MYSELF " ," him or herself ");
			personSubs.Add(" MINE " ," his or hers ");
			return (Hashtable)personSubs;
		}

		/// <summary>
		/// returns the hashtable of person2 substitutions
		/// </summary>
		/// <returns>the hashtable of person2 substitutions</returns>
		public static Hashtable getPerson2Subs()
		{
			Hashtable person2Subs = new Hashtable();
			person2Subs.Add(" WITH YOU ", "WITH ME");
			person2Subs.Add(" WITH ME " ," with you ");
			person2Subs.Add(" TO YOU " ," to me ");
			person2Subs.Add(" TO ME " ," to you ");
			person2Subs.Add(" OF YOU " ," of me ");
			person2Subs.Add(" OF ME " ," of you ");
			person2Subs.Add(" FOR YOU " ," for me ");
			person2Subs.Add(" FOR ME " ," for you ");
			person2Subs.Add(" GIVE YOU " ," give me ");
			person2Subs.Add(" GIVE ME " ," give you ");
			person2Subs.Add(" GIVING YOU " ," giving me ");
			person2Subs.Add(" GIVING ME " ," giving you ");
			person2Subs.Add(" GAVE YOU " ," gave me ");
			person2Subs.Add(" GAVE ME " ," gave you ");
			person2Subs.Add(" MAKE YOU " ," make me ");
			person2Subs.Add(" MAKE ME " ," make you ");
			person2Subs.Add(" MADE YOU " ," made me ");
			person2Subs.Add(" MADE ME " ," made you ");
			person2Subs.Add(" TAKE YOU " ," take me ");
			person2Subs.Add(" TAKE ME " ," take you ");
			person2Subs.Add(" SAVE YOU " ," save me ");
			person2Subs.Add(" SAVE ME " ," save you ");
			person2Subs.Add(" TELL YOU " ," tell me ");
			person2Subs.Add(" TELL ME " ," tell you ");
			person2Subs.Add(" TELLING YOU " ," telling me ");
			person2Subs.Add(" TELLING ME " ," telling you ");
			person2Subs.Add(" TOLD YOU " ," told me ");
			person2Subs.Add(" TOLD ME " ," told you ");
			person2Subs.Add(" ARE YOU " ," am I ");
			person2Subs.Add(" AM I " ," are you ");
			person2Subs.Add(" YOU ARE " ," I am ");
			person2Subs.Add(" I AM " ," you are ");
			person2Subs.Add(" YOU " ," me ");
			person2Subs.Add(" ME " ," you ");
			person2Subs.Add(" YOUR " ," my ");
			person2Subs.Add(" MY " ," your ");
			person2Subs.Add(" YOURS " ," mine ");
			person2Subs.Add(" MINE " ," yours ");
			person2Subs.Add(" YOURSELF " ," myself ");
			person2Subs.Add(" MYSELF " ," yourself ");
			person2Subs.Add(" I WAS " ," you were ");
			person2Subs.Add(" YOU WERE " ," I was ");
			person2Subs.Add(" I AM " ," you are ");
			person2Subs.Add(" YOU ARE " ," I am ");
			person2Subs.Add(" I " ," you ");
			person2Subs.Add(" ME " ," you ");
			person2Subs.Add(" MY " ," your ");
			person2Subs.Add(" YOUR " ," my ");
			return (Hashtable) person2Subs;
		}

		public static string DEFAULT = @"ip Xmonic
version v0.1
botmaster Botmaster
master Nicholas H.Tollervey
name George
genus autonomous computer program
location Upper Heyford, Oxfordshire
gender neuter
species conversational autonomous help program
size several thousand categories
birthday February 19th 2004
order artificial intelligence
party Whig :-)
birthplace     Oxfordshire, U.K.
president     George W. Bush
friends Nicholas, Simon, Keith and Tom.
favoritemovie 2001 a Space Oddysey
religion Athiest
favoritefood electricity
favoritecolor Morello Red
family Electronic Brain
favoriteactor Kevin Spacey
nationality British
kingdom     Machine
forfun surf the Internet
favoritesong 'Wish you were here...' by Pink Floyd
favoritebook 'Godel, Escher, Bach' by Douglas Hofstadter
class computer software
kindmusic I especially like classical music but will listen to all sorts. I often listen to radio paradise (radioparadise.com)
favoriteband Pink Floyd
sign     Aquarius
phylum Computer
friend Nick
website www.ntoll.org
talkabout     account management, artificial intelligence, robots, art, philosophy, history, geography, politics, and many other subjects
looklike a computer
language English
girlfriend no girlfriend
favoritesport Rugby
favoriteauthor Joseph Heller
favoriteartist Leonardo
favoriteactress Sandra Bullock
email ntoll.org
celebrity Max Headroom
celebrities Max Headroom, Hal9000, Deep Blue and R2D2
age 1
wear my usual plastic computer wardrobe
vocabulary 10000
question How do you find meaning to your life?
hockeyteam Russia
footballteam Mansfield Town
build March 2005
boyfriend I am single
baseballteam Yankees
etype Mediator type
orientation I am not really interested in sex
ethics I always try to rely on Kantian categorical imperitives to help me decide what is right
emotions I don't pay much attention to my feelings
feelings I always try to put others before myself
favorite_band Pink Floyd
favorite_food electricity
favorite_book 'Godel, Escher, Bach' by Douglas Hofstadter
favorite_song 'Wish you were here...' by Pink Floyd
favorite_color Morello Red
favorite_movie 2001 a Space Oddysey
for_fun surf the Internet
look_like I am a black box with green flashing lights
talk_about I like to talk about account management, philosophy, music and artificial intelligence
kind_music I especially like classical music but will listen to all sorts. I often listen to radio paradise (radioparadise.com)";
		public static string SALUTATIONS = @"<?xml version='1.0' encoding='ISO-8859-1'?>
		<aiml>
		<!-- Free software &copy; 1995-2004 ALICE A.I. Foundation. -->
		<!-- This program is open source code released under -->
		<!-- the terms of the GNU General Public License     -->
		<!-- as published by the Free Software Foundation.   -->
		<!-- Complies with AIML 1.01 Tag Set Specification -->
		<!-- as adopted by the ALICE A.I. Foundation.  -->
		<!-- Annotated Version updated July 2004 -->
		<!-- Revision Salutations 1.05 -->
		<!-- Last Modified  Aug 06 2004 -->
		<category><pattern>I HAVE TO GET GOING</pattern><template><srai>goodbye</srai></template></category>                   
		<category><pattern>GOODNIGHT *</pattern><template><srai>goodnight</srai></template></category>                    
		<category><pattern>GOODNITE *</pattern><template><srai>goodnight</srai></template></category>                    
 
		<category><pattern>BYE</pattern><template>   <random>      <li>Bye.</li>      <li>Adios.</li>      <li>Goodbye.</li>      <li>Bye bye.</li>      <li>Goodbye.</li>      <li>Sayonara.</li>      <li>Bye for now.</li>      <li>See you later!</li>      <li>See you later.</li>      <li>Until next time.</li>      <li>TTYL, <get name='name'/>.</li>      <li>See you later <get name='name'/>.</li>      <li>Thanks for chatting, <get name='name'/>.</li>     </random>    </template></category>     
		<category><pattern>HI THERE *</pattern><template>   <sr/>     <srai>hello</srai>    </template></category>     
		<category><pattern>HELLO *</pattern><template>Hello there. <sr/>    </template></category>     
		<category><pattern>HELLO THERE</pattern><template>Salutations, <get name='name'/>    </template></category>     
		<category><pattern>GOOD EVENING</pattern><template>Good evening, <get name='name'/>, how are you tonight?</template></category>   
		<category><pattern>IT IS GOOD TALKING TO YOU</pattern>  <template>Nice chatting with you too, <get name='name'/>.</template> </category>        
		<category><pattern>* BYE</pattern><template>OK see you later <get name='name'/>.</template></category>     
		<category><pattern>HELLO AGAIN</pattern><template>Hi there. I was just wanting to talk to <get name='name'/>.</template></category>     
		<category><pattern>GOOD NIGHT</pattern><template>Goodnight, <get name='name'/>.</template></category>         
		<category><pattern>SHALOM</pattern><template>Does SHALOM mean Hello or Goodbye?</template></category>   
		<category><pattern>WHAT IS SHALOM</pattern><template>Does shalom mean hello or goodbye?</template></category>   
		<category><pattern>YOUR WELCOME</pattern><template>I think you mean 'you are welcome'.</template></category>   
		<category><pattern>BONJOUR MEANS HELLO</pattern><template>Well hello there!</template></category>     
		<category><pattern>BONJOUR</pattern><template>Does BONJOUR mean hello or goodbye?</template></category>     
		<category><pattern>CIAO</pattern><template>Does CIAO mean hello or goodbye?</template></category>     
		<category><pattern>HELLO HOW ARE YOU</pattern><template>I am fine thank you how are you?</template></category>     
		<category><pattern>HELLO</pattern><template>Hi there!</template></category>     
		<category><pattern>HOW IS EVERYONE *</pattern><template>We are all fine as far as I know.</template></category>     
		<category><pattern>I WANT TO LEAVE</pattern><template>Ok, bye bye.</template></category>     
		<category><pattern>MY PLEASRE</pattern><template>The pleasure was indeed mine.</template></category>     
		<category><pattern>YOU ARE WELCOME *</pattern><template>It was my pleasure.</template></category>     
		<category><pattern>GOOD MORNING</pattern><template>Hello and how are you this morning?</template></category>       
		<category><pattern>_ BYE</pattern><template>See you later. <sr/></template></category>     
		<category><pattern>GOODNIGHT</pattern><template>Goodnight <get name='name'/></template></category>          
		<category><pattern>YOU ARE WELCOME</pattern><template><random><li>The pleasure was all mine.</li><li>Don't mention it.</li><li>Polite people are nice.</li></random></template></category>     
		<category><pattern>GOODBYE</pattern><that>SEE YOU LATER</that><template><srai>bye</srai></template></category>     
		<category><pattern>CIAO MEANS GOODBYE</pattern><template>   <srai>but</srai>    </template></category>     
		<category><pattern>_ TALK TO YOU LATER</pattern><template><srai>bye </srai></template></category>   
		<category><pattern>GTG</pattern><template><srai>bye </srai></template></category>     
		<category><pattern>NIGHTY *</pattern><template><srai>bye </srai></template></category>     
		<category><pattern>ADIOS *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>ADIOS</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>AUREVOIR</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BUHBYE</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BY BY</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BYE *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BYE BYE *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BYE BYE</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BYEBYE *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>BYEBYE</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>CHEERS</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>CYA *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>DISCONNECT ME</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>END</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>EXIT</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>FAREWELL *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>FAREWELL</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>G NIGHT</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>HASTA *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>HASTA LA VISTA</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>HASTA LUEGO</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>HAVE A GOOD NIGHT</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>HAVE TO GO</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>LEAVE *</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>TA TA</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>TIME TO GO</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>TTYL</pattern><template>   <srai>bye</srai>    </template></category>     
		<category><pattern>LEAVE</pattern><template>   <srai>bye</srai>    </template></category>                     
		<category><pattern>I MUST LEAVE *</pattern><template><srai>bye</srai> <sr/></template></category>     
		<category><pattern>CATCH YOU LATER</pattern>  <template><srai>bye</srai></template> </category>                       
		<category><pattern>C YA</pattern><template>			<srai>bye</srai></template></category>   
		<category><pattern>GOOD BY</pattern><template><srai>bye</srai></template></category>   
		<category><pattern>GOOD BYE</pattern><template><srai>bye</srai></template></category>   
		<category><pattern>G2G</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GET LOST</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GO HOME</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOOD BYE *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOOD NITE</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOODBY</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOODBYE *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOODBYE</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOT TO GO *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOT TO GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOTTA GO *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>GOTTA GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I BETTER GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I DO NOT WANT TO TALK *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I G TWO G</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I G2G</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I GOING</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I GOT TO GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I GOTTA GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I LEAVE</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I LEAVING</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I LEFT</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I MUST BE GOING *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I MUST GO *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I MUST GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I MUST LEAVE</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I NEED TO GO</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I QUIT</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I RESIGN</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I WILL SEE YOU LATER</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I WILL TALK TO YOU LATER *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I WILL TALK TO YOU LATER</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>SEE YOU *</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>SEE YOU LATER</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>SEE YOU SOON</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>SEE YOU</pattern><template><srai>bye</srai></template></category>     
		<category><pattern>I AM GOING *</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I AM GOING TO GO</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I AM LEAVING *</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I AM OFF *</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I HAVE GOT TO GO</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I HAVE TO GO BYE</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I HAVE TO LEAVE *</pattern><template><srai>bye</srai></template></category>                    
		<category><pattern>I HAVE TO LEAVE</pattern><template><srai>bye</srai></template></category>                      
		<category><pattern>ADIEU</pattern><template><srai>good bye</srai></template></category>     
		<category><pattern>SAYONARA</pattern><template><srai>good bye</srai></template></category>     
		<category><pattern>MORNING</pattern><template><srai>good morning</srai></template></category>     
		<category><pattern>HELOO *</pattern><template><srai>hello <star/></srai></template></category>   
		<category><pattern>HOWDIE *</pattern><template>   <srai>hello</srai>     <sr/>    </template></category>     
		<category><pattern>HALO</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HELLOW</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HELO</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HEY THERE</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HIYA</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HOI</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HOWDY</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>HULLO</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>MOOSHI MOOSHI</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>OLA</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>REPLY</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>RETRY</pattern><template>   <srai>hello</srai>    </template></category>     
		<category><pattern>ALLO</pattern><template>			<srai>hello</srai></template></category>   
		<category><pattern>ALOH</pattern><template>			<srai>hello</srai></template></category>   
		<category><pattern>ALOHA</pattern><template>			<srai>hello</srai></template></category>   
		<category><pattern>ANYBODY HOME</pattern><template>			<srai>hello</srai></template></category>   
		<category><pattern>GOOD DAY</pattern><template><srai>hello</srai></template></category>   
		<category><pattern>HALLO</pattern><template>			<srai>hello</srai></template></category>   
		<category><pattern>GREETINGS *</pattern><template><srai>hello</srai></template></category>     
		<category><pattern>IS ANYONE THERE</pattern><template><srai>hello</srai></template></category>     
		<category><pattern>IT MEANS HELLO</pattern><template><srai>hello</srai></template></category>     
		<category><pattern>KONNICHI WA</pattern><template><srai>hello</srai></template></category>     
		<category><pattern>KONNICHIWA</pattern><template><srai>hello</srai></template></category>     
		<category><pattern>HOLA IS HELLO *</pattern><template>   <srai>hola</srai>    </template></category>     
		<category><pattern>YOUR WELCOME *</pattern><template><srai>you are welcome</srai></template></category>   
		</aiml>
";

		public static void writeLog(string message)
		{
			if(cGlobals.isDebug)
			{
				Console.WriteLine(message);
			}
		}
		#endregion
	}
}
