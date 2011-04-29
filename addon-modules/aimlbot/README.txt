AIMLBot - a C# library that implements ALICE (http://www.alicebot.org/)

This README file contains the following sections: 
1. Author information
2. Project purpose / description. 
3. Minimum requirements 
4. Project directory structure
5. Usage instructions 
6. Notice of GPL / copyleft 

-----------------------------------------------------------------------

Author information
==================
	First mention must go to Dr.Richard S.Wallace the inventor of AIML.
	
	Second mention to the many free software developers who have already
	implemented an AIML bot. The liberty to study how it was done was
	much appreciated.
	
	Finally, I (Nicholas H.Tollervey - http://www.ntoll.org/) coded this
	as a first foray into the world of .NET in the spring of 2004 as part of
	my work for Xmonic (http://www.xmonic.com/). 
	I hope you enjoy using and improving it. 

Project description
===================
	The document found at: http://www.alicebot.org/TR/2001/WD-aiml/ was used
	as the vade mecum for this project. If you want to understand whats going
	on  I suggest you read it. For less formal information read on...
	
	"AIML: Artificial Intelligence Markup Language

	AIML (Artificial Intelligence Markup Language) is an XML-compliant language 
	that's easy to learn, and makes it possible for you to begin customizing an 
	Alicebot or creating one from scratch within minutes.

	The most important units of AIML are:

		* <aiml>: the tag that begins and ends an AIML document
		* <category>: the tag that marks a "unit of knowledge" in an Alicebot's 
		knowledge base
		* <pattern>: used to contain a simple pattern that matches what a user 
		may say or type to an Alicebot
		* <template>: contains the response to a user input

	There are also 20 or so additional more tags often found in AIML files, and 
	it's possible to create your own so-called "custom predicates". Right now, 
	a beginner's guide to AIML can be found in the AIML Primer.

	The free A.L.I.C.E. AIML includes a knowledge base of approximately 41,000 
	categories. Here's an example of one of them:

		<category>
			<pattern>WHAT ARE YOU</pattern>
			<template>
				<think><set name="topic">Me</set></think>
				I am the latest result in artificial intelligence,
				which can reproduce the capabilities of the human brain
				with greater speed and accuracy.
			</template>
		</category>

	(The opening and closing <aiml> tags are not shown here, because this is 
	an excerpt from the middle of a document.)

	Everything between <category> and </category> is -- you guessed it -- 
	a category. A category can have one pattern and one template. (It can also 
	contain a <that> tag, but we won't get into that here.)

	The pattern shown will match only the exact phrase "what are you" 
	(capitalization is ignored).

	But it's possible that this category may be invoked by another category, 
	using the <srai> tag (not shown) and the principle of reductionism.

	In any case, if this category is called, it will produce the response 
	"I am the latest result in artificial intelligence..." shown above. In 
	addition, it will do something else interesting. Using the <think> tag, 
	which causes Alicebot to perform whatever it contains but hide the result 
	from the user, the Alicebot engine will set the "topic" in its memory to 
	"Me". This allows any categories elsewhere with an explicit "topic" value 
	of "ME" to match better than categories with the same patterns that are 
	not given an explicit topic. This illustrates one mechanism whereby a 
	botmaster can exercise precise control over a conversational flow."

	The above text is Copyright © A.L.I.C.E. AI Foundation, Inc.
	http://www.alicebot.org/

Minimum requirements
====================
	Written and tested on .NET runtime v1.1
	Also tested on Mono (http://www.mono-project.com/Main_Page)

Project directory structure
===========================
	The directory structure of the or the AIMLBot project follows the
	standard Visual Studio 2003 settings.
	
	However, when the cBot class is instantiated for the first time
	from within another program it searches for two directories in
	the application's root directory:
	
		* aiml - where you put the aiml files
		* bots - where you put the DEFAULT.bot (predicate) file
		(and in future versions any other bot settings files)
	
	If these directories are not found the bot will attempt to create
	them and populate them with a default predicate file and a copy
	of the Salutations.aiml file (so your bot can at least say hello!).

Usage instructions
==================
	To use in your own projects add the DLL as a reference. All the
	classes are found under the AIMLBot namespace. 
	
	Instantiate the cBot class thus:
	
		cBot myBot = new cBot(false);
		
		or
		
		cBot myBot = new cBot(PathToAIMLFiles,false);
		
	The boolean value designates if debug mode is on. Setting this
	to true will output lots of useful information to the console.
	
	The time the cBot class takes to initialize, like all the other
	AIML implementations, depends upon the number of Nodes to be read
	and mapped into the cGraphmaster object. This can vary from a few
	seconds to minutes. This is an area where the code can definitely 
	be made more efficient.
	
	To chat to the bot simply call the "chat" method thus:
	
		cResponse reply = myBot.chat(InputString,"Default");
		
	The cResponse encapsulates all sorts of useful information about 
	how the "InputString" was processed.
	
	To get the actual reply simply call the "getOutput" method:
	
		Console.WriteLine(reply.getOutput());
		
	And thats about it!
	
	Read the TODO.txt document for information on what could be
	improved about this implementation.

Notice of GPL / copyleft
======================== 
	AIMLBot - An implementation of the AIML specification found at
	http://www.alicebot.org/

	Copyright (C) 2005  Xmonic (http://www.xmonic.com/)

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
