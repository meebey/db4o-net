/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: AssemblyTitle("db4o - instrumentation layer")]
[assembly: AssemblyCompany("db4objects Inc., San Mateo, CA, USA")]
[assembly: AssemblyProduct("db4o - database for objects")]
[assembly: AssemblyCopyright("db4o 2005 - 2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// attributes are automatically set by the build
[assembly: AssemblyVersion("7.1.29.9601")]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyConfiguration(".NET")]
[assembly: AssemblyDescription("db4o 7.0.0.001 .NET")]

#if !CF_2_0
[assembly: AllowPartiallyTrustedCallers]
#endif