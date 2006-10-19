/* Copyright (C) 2005   db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace Sharpen.Lang
{
	internal class TypeReferenceParser
	{
		TypeReferenceLexer _lexer;
		Stack _stack;
		
		public TypeReferenceParser(string input)
		{
			_lexer = new TypeReferenceLexer(input);
			_stack = new Stack();
		}

		public TypeReference Parse()
		{
			SimpleTypeReference str = ParseSimpleTypeReference();
			TypeReference returnValue = ParseQualifiedTypeReference(str);
			Token token = NextToken();
			if (null != token)
			{
				switch (token.Kind)
				{
					case TokenKind.Comma:
						str.SetAssemblyName(ParseAssemblyName());
						break;
					default:
						UnexpectedToken(token);
						break;
				}
			}
			return returnValue;
		}
		
		private TypeReference ParseQualifiedTypeReference(TypeReference elementType)
		{
			TypeReference returnValue = elementType;
			
			Token token = null;
			while (null != (token = NextToken()))
			{
				switch (token.Kind)
				{
					case TokenKind.LBrack:
						returnValue = ParseArrayTypeReference(returnValue);
						break;
					case TokenKind.PointerQualifier:
						returnValue = new PointerTypeReference(returnValue);
						break;
					default:
						Push(token);
						return returnValue;
				}
			}
			
			return returnValue;
		}

		private TypeReference ParseArrayTypeReference(TypeReference str)
		{
			int rank = 1;
			Token token = NextToken();
			while (null != token && token.Kind == TokenKind.Comma)
			{
				++rank;
				token = NextToken();
			}
			AssertTokenKind(TokenKind.RBrack, token);

			return new ArrayTypeReference(str, rank);
		}

		private SimpleTypeReference ParseSimpleTypeReference()
		{
			Token id = Expect(TokenKind.Id);

			Token t = NextToken();
			if (null == t) return new SimpleTypeReference(id.Value);

			while (TokenKind.NestedQualifier == t.Kind)
			{
				Token nestedId = Expect(TokenKind.Id);
				id.Value += "+" + nestedId.Value;

				t = NextToken();
				if (null == t) return new SimpleTypeReference(id.Value);
			}
			
			if (t.Kind == TokenKind.GenericQualifier)
			{
				return ParseGenericTypeReference(id);
			}
			
			Push(t);
			return new SimpleTypeReference(id.Value);
		}

		private SimpleTypeReference ParseGenericTypeReference(Token id)
		{
			Token argcToken = Expect(TokenKind.Number);
			id.Value += "`" + argcToken.Value;
			
			int argc = int.Parse(argcToken.Value);

			Token t = NextToken();
			while (TokenKind.NestedQualifier == t.Kind)
			{
				Token nestedId = Expect(TokenKind.Id);
				id.Value += "+" + nestedId.Value;

				t = NextToken();
			}

			TypeReference[] args = new TypeReference[argc];
			AssertTokenKind(TokenKind.LBrack, t);
			for (int i = 0; i < argc; ++i)
			{
				if (i > 0) Expect(TokenKind.Comma);
				Expect(TokenKind.LBrack);
				args[i] = Parse();
				Expect(TokenKind.RBrack);
			}
			Expect(TokenKind.RBrack);

			return new GenericTypeReference(
					id.Value,
					args);
		}

		public System.Reflection.AssemblyName ParseAssemblyName()
		{
			Token simpleName = Expect(TokenKind.Id);
			
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = simpleName.Value;

			if (!CommaIdEquals()) return assemblyName;

			Token version = Expect(TokenKind.VersionNumber);
			assemblyName.Version = new System.Version(version.Value);

			if (!CommaIdEquals()) return assemblyName;
			
			Token culture = Expect(TokenKind.Id);
			if ("neutral" == culture.Value)
			{
				assemblyName.CultureInfo = CultureInfo.InvariantCulture;
			}
			else
			{
				assemblyName.CultureInfo = CultureInfo.CreateSpecificCulture(culture.Value);
			}

			if (!CommaIdEquals()) return assemblyName;
			
			Token token = NextToken();
			if ("null" != token.Value)
			{
				assemblyName.SetPublicKeyToken(ParsePublicKeyToken(token.Value));
			}

			return assemblyName;
		}

		byte[] ParsePublicKeyToken(string token)
		{
			int len = token.Length / 2;
			byte[] bytes = new byte[len];
			for (int i = 0; i < len; ++i)
			{
				bytes[i] = byte.Parse(token.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
			}
			return bytes;
		}

		private bool CommaIdEquals()
		{
			Token token = NextToken();
			if (null == token) return false;
			if (token.Kind != TokenKind.Comma)
			{
				Push(token);
				return false;
			}
			
			AssertTokenKind(TokenKind.Comma, token);
			Expect(TokenKind.Id);
			Expect(TokenKind.Equals);
			return true;
		}

		Token Expect(TokenKind expected)
		{
			Token actual = NextToken();
			AssertTokenKind(expected, actual);
			return actual;
		}

		private static void AssertTokenKind(TokenKind expected, Token actual)
		{
			if (null == actual || actual.Kind != expected)
			{
				UnexpectedToken(actual);
			}
		}

		private static void UnexpectedToken(Token token)
		{
			throw new ArgumentException(string.Format("Unexpected token: '{0}'", token));
		}
		
		private void Push(Token token)
		{
			_stack.Push(token);
		}
	
		private Token NextToken()
		{
			return _stack.Count > 0
				? (Token) _stack.Pop()
				: _lexer.NextToken();
		}

	}
}