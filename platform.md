# Introduction
Platform is a SDK for .net platform based on Peachpie compiler and Roslyn compiler. For
building project uses ```msbuild``` => you can use ```dotnet build``` command.

# Project structure
- Root
  - Project file ```.aqproj```
    - Files with metadata ```.aqmd```
    - Files with roles ```.aqsec```
    - Files with views ```.aqview```
    - Files with logic ```.aq```
  
# Metadata declaration
In progress...

# Security policy declaration
In progress...

# Query language
In progress... 

# Language

### Naming conventions

* Types - CamelCase
* Members - snake_case

### operations
```
arithmetic 
+ 
-
/
*
^

logical
&&
||
==
!=
<=
>=
>
<
is

bitwise
&
|
<<
>>

```

### Functions

```
[pub] fn [(var_name type_name)] fn_name([arg_list])
{
    [fn body]
}
```

Declared functions can be two types: 
- Module related functions (1)
- Type related functions (2)

Type (1) can be called in any time, you just need import module.
```Module_name.function_name()```

Type (2) can be called only for instance of object with certain type (declared as ```type_name``` argument)


Example function type (1)
```
pub fn add(int x, int y) int
{
    reutrn x + y;
}
```

Example function type (2)
```
pub fn this_is_module_func(i Invoice)
{
    var result = i.instance_func();
}

pub fn (i Invoice) instance_func(int x, int y) string
{
    i.delete();
    reutrn "Invoice has been deleted";
}
```

### Types

```
[part] type Type_name 
{
    [type_body]
}
```

# Predefined elements

### Default types

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

### Built in functions

```
print()
println()
typeof()
sizeof()
```


