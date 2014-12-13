adrienne_fsharp
===============
This is a parser written in F-sharp. It is an F-sharp program that parses, filters, reads from one file type and writes results to a different file type. (Basically, it deals with the situation of a clobbered YAML file.)

When dealing with a huge file that is actually multiple YAML files mashed together as one big YAML file, Ruby's Nokogiri gem may not be enough by itself to handle data deserialization and re-serialization and conversion from YAML to XML (or JSON). But F#, which handles complex math proofs in areas such as analysis of the real variable, set theory, and algebraic topology, certainly can do the job easier. And this program handles the difficult YAML parsing situation of one large YAML file containing multiple smaller YAML files, extracts the smaller YAML files, and then parses and writes those to indivual XML files. Enjoy.

To Run and Use this Parser, You Need:
======================================
1. F# and either Xamarin for Windows or Mac, or Monodevelop for Linux installed
2. A clobbered YAML file you need to parse and rewrite to XML
3. To change the names of the file objects in your particular clobbered YAML file. In this particular case of the file this parser was written for, the YAML file objects to be selected, filtered, parsed and rewritten into XML were Asset, EvtEvent, and  NwsPost. If you have a clobbered YAML file with different Ruby/YAML class objects, then you will have to modify this code accordingly by renaming them in the type declaration.

The Task
=========
1. read from, filter a few selected YAML file objects from a particular larger YAML file object schema dump.
2. parse and write to separate XML files.

