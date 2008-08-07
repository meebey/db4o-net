/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;

using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Query;

using Cecil.FlowAnalysis;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CodeStructure;
using Cecil.FlowAnalysis.Utilities;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Db4objects.Db4o.Linq.Internals;

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	internal class CodeQueryBuilder : AbstractCodeStructureVisitor
	{
		private QueryBuilderRecorder _recorder;

		public CodeQueryBuilder(QueryBuilderRecorder recorder)
		{
			_recorder = recorder;
		}

		public override void Visit(ArgumentReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(AssignExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(BinaryExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(CastExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(FieldReferenceExpression node)
		{
            Type descendingEnumType = ResolveDescendingEnumType(node);

            _recorder.Add(
                ctx =>
                    {
                        ctx.PushQuery(ctx.RootQuery.Descend(node.Field.Name));
                        ctx.PushDescendigFieldEnumType(descendingEnumType);
                    });
		}

		public override void Visit(LiteralExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(MethodInvocationExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(MethodReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(PropertyReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(ThisReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(UnaryExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(VariableReferenceExpression node)
		{
			CannotOptimize(node);
		}

		private static void CannotOptimize(Expression expression)
		{
			throw new QueryOptimizationException(ExpressionPrinter.ToString(expression));
		}

        private Type ResolveDescendingEnumType(FieldReferenceExpression node)
        {
            TypeDefinition type = ResolveType(node);
            if (type != null && type.IsEnum)
            {
                return Type.GetType((type.FullName + "," + type.Module.Assembly.Name.FullName).Replace('/', '+'));
            }

            return null;
        }

        private static TypeDefinition ResolveType(FieldReferenceExpression node)
        {
            TypeReference t = node.Field.FieldType;
            TypeDefinition typeDefinition = Resolve(t.Module, t.FullName);
            if (typeDefinition != null) return typeDefinition;

            IAssemblyResolver resolver = GetResolver(t);
            foreach (AssemblyNameReference assembyReference in t.Module.AssemblyReferences)
            {
                foreach (ModuleDefinition module in resolver.Resolve(assembyReference).Modules)
                {
                    typeDefinition = Resolve(module, t.FullName);
                    if (typeDefinition != null) return typeDefinition;
                }
            }

            return null;
        }

        private static TypeDefinition Resolve(ModuleDefinition module, string typeName)
        {
            return module.Types[typeName];
        }

		private static IAssemblyResolver GetResolver(TypeReference type)
		{
#if !CF_3_5
			return type.Module.Assembly.Resolver;
#else
			return CompactAssemblyResolver.Instance;
#endif
		}
	}
}
