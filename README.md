adrienne_fsharp
===============

F-sharp program that parses, filters, reads from one file type and writes results to a different file type.

When dealing with a huge file that is actually multiple YAML files mashed together as one big YAML file, Ruby's Nokogiri gem may not be enough by itself to handle data deserialization and re-serialization and conversion from YAML to XML (or JSON). But F#, which handles complex math proofs in areas such as analysis of the real variable, set theory, and algebraic topology, certainly can do the job easier. And this program handles the difficult YAML parsing situation of one large YAML file containing multiple smaller YAML files, extracts the smaller YAML files, and then parses and writes those to indivual XML files. Enjoy.

The Task
=========
1. read from, filter a few selected YAML file objects from the larger YAML file object.
2. parse and write to separate XML files.

