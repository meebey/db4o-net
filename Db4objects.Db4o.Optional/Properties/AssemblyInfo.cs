/* Copyright (C) 2009 Versant Inc.   http://www.db4o.com */

using System;
using System.Reflection;
using System.Security;

[assembly: AssemblyTitle("db4o - optional functionality")]
[assembly: AssemblyCompany("Versant Corp., Redwood City, CA, USA")]
[assembly: AssemblyProduct("db4o - database for objects")]
[assembly: AssemblyCopyright("Versant Corp. 2000 - 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// attributes are automatically set by the build
[assembly: AssemblyVersion("7.12.124.14040")]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyConfiguration(".NET")]
[assembly: AssemblyDescription("Db4objects.Db4o.Optional 7.12.124.14040 (.NET)")]

#if !CF && !SILVERLIGHT
[assembly: AllowPartiallyTrustedCallers]
#endif

[assembly: CLSCompliant(true)]