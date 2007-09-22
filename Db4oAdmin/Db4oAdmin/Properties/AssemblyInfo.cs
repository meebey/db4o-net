/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Db4oAdmin")]
[assembly: AssemblyDescription("Db4o command line utility.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("db4objects Inc., San Mateo, CA, USA")]
[assembly: AssemblyProduct("db4o - database for objects")]
[assembly: AssemblyCopyright("db4o 2005")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.7.001")]

[assembly: Mono.Author("Jean Baptiste Evain")]
[assembly: Mono.Author("Rodrigo B. de Oliveira")]
[assembly: Mono.Author("Klaus Wuestefeld")]
[assembly: Mono.Author("Patrick Roemer")]

[assembly: Mono.About("")]
[assembly: Mono.UsageComplement("<assembly>")]

#if !CF_1_0 && !CF_2_0
[assembly: AllowPartiallyTrustedCallers]
#endif
