PARSING
=======

Passes:

1. First parse the outline of types (files), gathering stuff such as:
	- type name (store in a central place, along with the package name, required for resolving types)
	- imported packages (store as a file context, required for resolving return types for functions, and variable types)
	- generic type parameters
2. Then parse the outline of types (files), gathering stuff such as:
	- super class
	- implemented interfaces
3. Then skim the stuff inside the types:
	- for enums
		- you can't parse some initial values, some of them might refer to static final integer fields...
		- you CAN parse the enumerators
	- for interfaces
		- get method names, conclude their return types and parameter types and names (using file context)
	- for classes
		- get method names, conclude their return types and parameter types and names (using file context)
			- if name == "this", it's a constructor, keep that in mind
			- parse generic type parameters
		- get field names, conclude their types (using file context)
4. Then parse stuff such as:
	- for enums
		- the initial value for the enumerators
	- for interfaces
		- they're already done...
	- for classes
		- the initial values for variables and function parameters
5. Then, for classes:
	- carefully parse the bodies of functions

The codenames for these steps shall be:
	1. Type Skimming (TS)
	2. Type Parsing (TP)
	3. Type Content Skimming (TCS)
	4. Type Content Parsing (TCP)
	5. Code Parsing (CP)

Note that each pass can be done in parallel for each file, since the passes never depends on the same pass from a different file while still doing the pass.
For example: the TS step for file1.boh and file2.boh can be performed in parallel, they don't depend on eachother.
The same holds for every other step.

Difficulties for each step:
	1:
	- Should generic type parameters be analysed?
		- Yes. Yes they should.
		  types should then generate GenericTypes instead of Types, very important
	3:
	- Should generic type parameters for functions be analyzed?
		- Yes. Yes they should.
		  functions will then be GenericFunction instead of function, very important once again

	General:
	- Why doesn't GenericType inherit Type (and GenericFunction inherit Function)?
		- Because generic types and functions, by their very definition, are incomplete,
		  they are only abstract ideas. They need type parameters to become concrete types
		  and functions. (unless you compile them using type erasure, but if you're even
		  taking type erasure into consideration, I doubt you should be contributing to
		  this project)

Proposed changes
----------------

Only parse import directives in the Type Parsing step.
The import directives can then be verified since the available packages are known.
