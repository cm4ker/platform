.class private auto ansi abstract sealed beforefieldinit main
	extends [System.Private.CoreLib]System.Object
{
	// Methods
	.method public static 
		void main (
			class [Aquila.Runtime]Aquila.Core.AqContext '<ctx>'
		) cil managed 
	{
		// Method begins at RVA 0x35e8
		// Header size: 12
		// Code size: 32 (0x20)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldstr "HELLO "
		IL_0006: ldstr "WORLD"
		IL_000b: call string [System.Private.CoreLib]System.String::Concat(string, string)
		IL_0010: stloc.0
		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldloc.0
		IL_0014: call instance int32 [System.Private.CoreLib]System.String::get_Length()
		IL_0019: call void [Aquila.Library]Aquila.Library.SystemExtensions::print(class [Aquila.Runtime]Aquila.Core.AqContext, int32)
		IL_001e: nop
		IL_001f: ret
	} // end of method main::main

} // end of class main
