/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using Db4oUnit;
using Db4oAdmin.Tests.Core;
using Mono.Cecil.Cil;
using System;
using Mono.Cecil;
using Db4oAdmin.Core;
    
namespace Db4oAdmin.Tests.TA
{
    class TAInstrumentationAppliedMoreThanOnce : ITestCase
    {
        public void Test()
        {
            AssemblyDefinition testAssembly = GenerateAssembly();            
            InstrumentAssembly(testAssembly);

            MethodDefinition instrumented = InstrumentedMethod(testAssembly);
            string before = FormatMethodBody(instrumented);

            InstrumentAssembly(testAssembly);

            string after = FormatMethodBody(instrumented);
            Assert.AreEqual(before, after);
        }

        private MethodDefinition InstrumentedMethod(AssemblyDefinition testAssembly)
        {
            return testAssembly.MainModule.Types["InstrumentedType"].Methods.GetMethod("InstrumentedMethod")[0];
        }

        private string FormatMethodBody(MethodDefinition instrumented)
        {
            return Cecil.FlowAnalysis.Utilities.Formatter.FormatMethodBody(instrumented);
        }

        private AssemblyDefinition GenerateAssembly()
        {
            return AssemblyFactory.GetAssembly(
                        CompilationServices.EmitAssemblyFromResource(
                            ResourceServices.CompleteResourceName(
                                GetType(), 
                                "TADoubleInstrumentationSubject")));
        }

        private void InstrumentAssembly(AssemblyDefinition testAssembly)
        {
            InstrumentationContext context = new InstrumentationContext(Configuration(testAssembly.MainModule.Image.FileInformation.FullName), testAssembly);
            new Db4oAdmin.TA.TAInstrumentation().Run(context);
        }

        private Configuration Configuration(string assemblyLocation)
        {
            return new Configuration(assemblyLocation);
        }
    }
}
