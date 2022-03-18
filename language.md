1. Project structure

+ Project file (contents references and some props)
+ Included / Excluded files (all files with extension .aqmd, .aq included by default)

Language

Naming conventions

* Types - CamelCase
* Members - snake_case

Simple program

```
println("Hello world"); // print to the console "Hello world" and return caret to the new line
```

Functions

```
[pub] fn [(type_name var_name)] fn_name([arg_list])
{
    [fn body]
}
```

ex

```
pub fn add(int x, int y) int
{
    reutrn x + y;
}
```

Types

```
type [Type_name] | 
{
    [type_body]
}
```

Extend

You can extend described metadata by algorithms using the following syntax. This scoped onto the file

```
extend metadata [Metadata_name];
//following members are using for injecting into the metadata classes
```

Import

```
import [Namespace_qualified_name] [as Alias_name];
```

Namespaces

Scope - file

```
//default namespace - project name
ns [Namespace_qualified_name];
```

Default types

```
int
string
bool
char
double
long
byte
decimal
datetime
date
time
```

Array

```
byte[]
```

Not null by default

```
 Invoice i = null; //error
 Invoice? i = null; //success
 
 
 Default checking for null
 
 ///return true if we set date
 pub fn (i Invoice) try_set_shipping_date(Contractor c) bool
 {
    /*
        parameter 'c' not Contractor? 
        runtime automatically check it for you 
    */ 
    
    if(c.creation_date > d"20200101")
    {
        i.shipping_date = date;
        i.save();
                
        return true;
    }
    else
    {
        return false;
    }
 }
```

Built in functions

```
print()
println()
typeof()
sizeof()
```

Example

```
file - main.aq

//look modules in source files (can use external lib)
import math;

//auto-synthesized module
//do we need add any special character for this
import entity;

//import clr namespace
import clr System;

//import clr class
import clr System.Console;

//you can alias imported static members for more readability
import clr System.Console { method1_name as m1, method2_name as m2 }


part type Invocie {} 
// we can extend the class here this class is in SourceAsembly
// => we incapsulate the methods and fields inside the class

part type string 
{
    //error if add any members here
} 
// this type from reference assembly
// we just can add static method for this 

fn main ()
{
    // static methods import as a [class_name]_[method_name] ex:
    console_write("some string");
    
    // just write() on importing System.Console;

    m1();
    m2();
    
    //call add function form math module
    math.add(10, 1);
}

file - math.aq

//declare new module
module math

//create async method with _async postfix
pub fn add_async(x int, y int) Task<int>
{
    // use await for create state machine for waiting result and continue op
    // it helps create write async code as sync
    return await go {
                        do_stuff();
                        return x + y;
                     };
}

// create private for module function
fn do_stuff()
{
    // do some havy work
}

//sample pass lambda delegate
fn lamda_sample(func fn(int) int) int
{
    return func(10);
}

// create public for module function
pub fn add(int x, int y) int
{
    // use wait operator for waiting the result
    return wait add_async(x, y);
}
```


