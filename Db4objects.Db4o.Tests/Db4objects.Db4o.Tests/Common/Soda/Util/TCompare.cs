namespace Db4objects.Db4o.Tests.Common.Soda.Util
{
	public class TCompare
	{
		public static bool IsEqual(object a_compare, object a_with)
		{
			return IsEqual(a_compare, a_with, null, null);
		}

		public static bool IsEqual(object a_compare, object a_with, string a_path, System.Collections.Stack
			 a_stack)
		{
			if (a_path == null || a_path.Length < 1)
			{
				if (a_compare != null)
				{
					a_path = a_compare.GetType().FullName + ":";
				}
				else
				{
					if (a_with != null)
					{
						a_path = a_with.GetType().FullName + ":";
					}
				}
			}
			string path = a_path;
			if (a_compare == null)
			{
				return a_with == null;
			}
			if (a_with == null)
			{
				return false;
			}
			System.Type clazz = a_compare.GetType();
			if (clazz != a_with.GetType())
			{
				return false;
			}
			if (Db4objects.Db4o.Platform4.IsSimple(clazz))
			{
				return a_compare.Equals(a_with);
			}
			if (a_stack == null)
			{
				a_stack = new System.Collections.Stack();
			}
			if (a_stack.Contains(a_compare))
			{
				return true;
			}
			a_stack.Push(a_compare);
			System.Reflection.FieldInfo[] fields = Sharpen.Runtime.GetDeclaredFields(clazz);
			for (int i = 0; i < fields.Length; i++)
			{
				if (Db4objects.Db4o.Tests.Db4oUnitPlatform.IsStoreableField(fields[i]))
				{
					Db4objects.Db4o.Platform4.SetAccessible(fields[i]);
					try
					{
						path = a_path + fields[i].Name + ":";
						object compare = fields[i].GetValue(a_compare);
						object with = fields[i].GetValue(a_with);
						if (compare == null)
						{
							if (with != null)
							{
								return false;
							}
						}
						else
						{
							if (with == null)
							{
								return false;
							}
							else
							{
								if (compare.GetType().IsArray)
								{
									if (!with.GetType().IsArray)
									{
										return false;
									}
									else
									{
										compare = NormalizeNArray(compare);
										with = NormalizeNArray(with);
										int len = Sharpen.Runtime.GetArrayLength(compare);
										if (len != Sharpen.Runtime.GetArrayLength(with))
										{
											return false;
										}
										else
										{
											for (int j = 0; j < len; j++)
											{
												object elementCompare = Sharpen.Runtime.GetArrayValue(compare, j);
												object elementWith = Sharpen.Runtime.GetArrayValue(with, j);
												if (!IsEqual(elementCompare, elementWith, path, a_stack))
												{
													return false;
												}
												else
												{
													if (elementCompare == null)
													{
														if (elementWith != null)
														{
															return false;
														}
													}
													else
													{
														if (elementWith == null)
														{
															return false;
														}
														else
														{
															System.Type elementCompareClass = elementCompare.GetType();
															if (elementCompareClass != elementWith.GetType())
															{
																return false;
															}
															if (HasPublicConstructor(elementCompareClass))
															{
																if (!IsEqual(elementCompare, elementWith, path, a_stack))
																{
																	return false;
																}
															}
															else
															{
																if (!elementCompare.Equals(elementWith))
																{
																	return false;
																}
															}
														}
													}
												}
											}
										}
									}
								}
								else
								{
									if (HasPublicConstructor(fields[i].GetType()))
									{
										if (!IsEqual(compare, with, path, a_stack))
										{
											return false;
										}
									}
									else
									{
										if (!compare.Equals(with))
										{
											return false;
										}
									}
								}
							}
						}
					}
					catch (System.MemberAccessException ex)
					{
						return true;
					}
					catch (System.Exception e)
					{
						System.Console.Error.WriteLine("TCompare failure executing path:" + path);
						Sharpen.Runtime.PrintStackTrace(e);
						return false;
					}
				}
			}
			return true;
		}

		internal static bool HasPublicConstructor(System.Type a_class)
		{
			if (a_class != typeof(string))
			{
				try
				{
					return System.Activator.CreateInstance(a_class) != null;
				}
				catch (System.Exception t)
				{
				}
			}
			return false;
		}

		internal static object NormalizeNArray(object a_object)
		{
			if (Sharpen.Runtime.GetArrayLength(a_object) > 0)
			{
				object first = Sharpen.Runtime.GetArrayValue(a_object, 0);
				if (first != null && first.GetType().IsArray)
				{
					int[] dim = ArrayDimensions(a_object);
					object all = new object[ArrayElementCount(dim)];
					NormalizeNArray1(a_object, all, 0, dim, 0);
					return all;
				}
			}
			return a_object;
		}

		internal static int NormalizeNArray1(object a_object, object a_all, int a_next, int[]
			 a_dim, int a_index)
		{
			if (a_index == a_dim.Length - 1)
			{
				for (int i = 0; i < a_dim[a_index]; i++)
				{
					Sharpen.Runtime.SetArrayValue(a_all, a_next++, Sharpen.Runtime.GetArrayValue(a_object
						, i));
				}
			}
			else
			{
				for (int i = 0; i < a_dim[a_index]; i++)
				{
					a_next = NormalizeNArray1(Sharpen.Runtime.GetArrayValue(a_object, i), a_all, a_next
						, a_dim, a_index + 1);
				}
			}
			return a_next;
		}

		internal static int[] ArrayDimensions(object a_object)
		{
			int count = 0;
			for (System.Type clazz = a_object.GetType(); clazz.IsArray; clazz = clazz.GetElementType
				())
			{
				count++;
			}
			int[] dim = new int[count];
			for (int i = 0; i < count; i++)
			{
				dim[i] = Sharpen.Runtime.GetArrayLength(a_object);
				a_object = Sharpen.Runtime.GetArrayValue(a_object, 0);
			}
			return dim;
		}

		internal static int ArrayElementCount(int[] a_dim)
		{
			int elements = a_dim[0];
			for (int i = 1; i < a_dim.Length; i++)
			{
				elements *= a_dim[i];
			}
			return elements;
		}

		private TCompare()
		{
		}
	}
}
