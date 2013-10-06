CODE GENERATION
===============

For classes
-----------

Code generation has two (independent) steps:
	- The writing of the header file
	- The writing of the code file

The writing of the header file has multiple steps:

1.1. Writing the #include guards, and all the #include directives for the other files
	- Always included:
		- <stdint.h>
		- <stdbool.h>
		- <stddef.h>
		- "boh_compiler.h"
			- This file contains boh-to-c and c-to-boh marshalling functions, used for
				- String <-> char pointer conversion (boh-to-c and c-to-boh)
				- Array <-> pointer conversion (boh-to-c and c-to-boh)
1.2. Writing the struct prototype
	- C doesn't actually have these, but it allows the use of the class as a pointer, which is exactly what we want
1.3. Writing function prototypes 
	- Only public and protected functions; private function may be declared in the code file itself using the 'static' modifier
1.4. Writing the struct vtable (both the vtable type, and the declaration for the vtable instance)
	- This also includes the virtual functions for the base classes recursively
1.5. Writing the struct itself
	- The vtable is always the first item in the struct
	- The fields of the base classes are recursively added at the start of the struct, after the vtable
		- do not include the vtables of the base classes in this
1.6. Writing the non-private static variables for the class
1.7. Close #include guards

The writing of the code file also has multiple steps:

2.1. Including the appropriate header file
2.2. Declaring private functions
	- use the 'static' modifier
2.3. Defining the vtable declared in the header file
2.4. Defining all static variables
	- Again, private static variables require the c 'static' modifier 
2.5. Implementing all functions


Issues
------

Step 2.5 is easier said than done. There are several issues where the compiled result and the input code are fundamentally different.

Consider the followin situation

```Boh
int i = obj.someFunction().someOtherFunction();
```

Where `someFunction` and `someOtherFunction` are virtual. For virtual function the following is required:

	- A vtable dereference for the object
	- Passing the object as a function param

If the function weren't virtual, compilation would've been easy:

```C
int32_t i = someOtherFunction(someFunction(obj));
```

However, since they're virtual, the following would be necessary:

```C
int32_t i = obj->vtable->someFunction(obj)->vtable->someOtherFunction(obj->vtable->someFunction(obj)));
```

Other than it being a mess, I hope you can see how `someFunction` is called twice, which is obviously not good: the return value needs to be saved:

```C
struct whatever *temp = obj->vtable->someFunction(obj);
int32_t i = temp->vtable->someOtherFunction(temp);
```

That's alright in and of itself, but now consider this:

```Boh
for (int i = obj.someFunction().someOtherFunction(); i < 10; ++i)
	;
```

What would the compiled code be then? Maybe:

```C
struct whatever *temp = obj->vtable->someFunction(obj);
for (int32_t i = temp->vtable->someOtherFunction(temp); i < 10; ++i)
	;
```

The `temp` variable needs to be placed before the for loop.

Now consider the following situation, even worse!

```Boh
for (int i = 0; i < obj.someFunction().someOtherFunction(); ++i)
	;
```

How would you compile that?!
Perhaps like this:

```C
struct whatever *temp;
for (int32_t i = 0; i < (temp = obj->vtable->someFunction(obj))->vtable->someOtherFunction(temp); ++i)
	;
```

Perhaps it's then even better to generisize that, so that the first example would be compiled to:

```C
struct whatever *temp = ;
int32_t i = (temp = obj->vtable->someFunction(obj))->vtable->someOtherFunction(temp);
```

And the second example would be compiled to:

```C
struct whatever *temp;
for (int32_t i = (temp = obj->vtable->someFunction(obj))->vtable->someOtherFunction(temp); i < 10; ++i)
	;
```

Enums
-----

Enums are very easy to compile:

1.1. #include guards, and #include all other headers in the project
1.2. create the C enum with the appropriate enumerators
1.3. declare the toString function for the enum
1.4. close #include guards

2.1. #include the appropriate header
2.2. implement the toString function for the enum

There aren't really any difficulties when dealing with enums

Interfaces
----------

Interfaces are essentially just an object reference and a vtable, the vtable needs to be assigned to when casting an object instance to an interface instance.

1.1. #include guards, and #include all other headers in the project
1.2. declare the interface vtable struct
1.3. declare the interface struct
1.4. declare the interface constructor

2.1. #include the appropriate header
2.2. implement the constructor

Step 2.2 is the most difficult one. The constructor essentially just takes an object reference, and a bunch of function pointers. These are then used to construct the interface object and the vtable.
