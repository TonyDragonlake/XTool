using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection.Emit;

namespace FantaziaDesign.Core
{
	public delegate void RefAction<T>(ref T arg1);
	public delegate void RefAction<T1, T2>(ref T1 arg1, ref T2 arg2);
	public delegate void SingleRefAction<T1, T2>(T1 arg1, ref T2 arg2);
	public delegate void SingleRefAction<T1, T2, T3>(T1 arg1, T2 arg2, ref T3 arg3);

	public static class ReflectionUtil
	{
		private static readonly Type s_typeOfExpression = typeof(Expression);
		private static readonly Type s_typeOfAssignBinaryExpression = s_typeOfExpression.Assembly.GetType("System.Linq.Expressions.AssignBinaryExpression");

		private static readonly Type s_typeOfDelegate = typeof(Delegate);
		private static readonly Type[] s_typesOfGetInstance = new Type[] { typeof(Func<object>) };
		private static readonly Type s_typeOfDelegateFactory = typeof(Func<Func<object>, Delegate>);
		private static readonly MethodInfo s_Invoke_Func_object = typeof(Func<object>).GetMethod("Invoke");

		private static Func<Expression, Expression, BinaryExpression> newAssignBinaryExpression;

		public static BindingFlags NonPublicInstance => BindingFlags.NonPublic | BindingFlags.Instance;

		public static BindingFlags PublicInstance => BindingFlags.Public | BindingFlags.Instance;

		public static BindingFlags NonPublicStatic => BindingFlags.NonPublic | BindingFlags.Static;

		public static BindingFlags PublicStatic => BindingFlags.Public | BindingFlags.Static;

		public static BindingFlags AllInstance => BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

		public static BindingFlags AllStatic => BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

		public static T BindMethodToDelegate<T>(Type classType, string methodName, BindingFlags bindingFlags, params Type[] paramsTypes) where T : Delegate
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(methodName))
			{
				throw new ArgumentException($"methodName cannot be null or whitespace", nameof(methodName));
			}
			MethodInfo method;
			bool isEmptyParam = paramsTypes is null || paramsTypes.Length == 0;
			if (isEmptyParam)
			{
				method = classType.GetMethod(methodName, bindingFlags);
			}
			else
			{
				method = classType.GetMethod(methodName, bindingFlags, null, paramsTypes, null);
			}

			if (method is null)
			{
				return null;
			}

			return (T)Delegate.CreateDelegate(typeof(T), method);
		}

		public static T BindConstructorToDelegate<T>(Type classType, BindingFlags bindingFlags, params Type[] paramsTypes) where T : Delegate
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}

			bool isEmptyParam = paramsTypes is null || paramsTypes.Length == 0;

			if (isEmptyParam)
			{
				paramsTypes = Type.EmptyTypes;
			}
			var ctor = classType.GetConstructor(bindingFlags, null, paramsTypes, null);
			Expression<T> lambda;
			var instanceExpr = Expression.Parameter(classType, "instance");
			if (isEmptyParam)
			{
				lambda = Expression.Lambda<T>(
					Expression.Block(
						new ParameterExpression[] { instanceExpr },
						Expression.Assign(
							instanceExpr,
							Expression.New(ctor)
							)
						)
					);
			}
			else
			{
				var len = paramsTypes.Length;
				ParameterExpression[] parameterExprs = new ParameterExpression[len];
				for (int i = 0; i < len; i++)
				{
					parameterExprs[i] = Expression.Parameter(paramsTypes[i], $"arg{i}");
				}
				var ctorExpr = Expression.New(ctor, parameterExprs);

				lambda = Expression.Lambda<T>(
					Expression.Block(
						Expression.Assign(instanceExpr, ctorExpr)
						),
					parameterExprs
					);
			}

			return lambda.Compile();
		}

		public static T BindConstructorToDelegate<T>(Type baseClassType, Type classType, BindingFlags bindingFlags, params Type[] paramsTypes) where T : Delegate
		{
			if (baseClassType is null)
			{
				throw new ArgumentNullException(nameof(baseClassType));
			}

			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}

			if (!classType.IsSubclassOf(baseClassType))
			{
				throw new InvalidCastException($"Case to {baseClassType.FullName} is invalid");
			}
			bool isEmptyParam = paramsTypes is null || paramsTypes.Length == 0;

			if (isEmptyParam)
			{
				paramsTypes = Type.EmptyTypes;
			}
			var ctor = classType.GetConstructor(bindingFlags, null, paramsTypes, null);
			Expression<T> lambda;
			var instanceExpr = Expression.Parameter(baseClassType, "instance");
			if (isEmptyParam)
			{
				lambda = Expression.Lambda<T>(
					Expression.Block(
						Expression.Assign(instanceExpr, Expression.New(ctor))
						)
					);
			}
			else
			{
				var len = paramsTypes.Length;
				ParameterExpression[] parameterExprs = new ParameterExpression[len];
				for (int i = 0; i < len; i++)
				{
					parameterExprs[i] = Expression.Parameter(paramsTypes[i], $"arg{i}");
				}
				var ctorExpr = Expression.New(ctor, parameterExprs);

				lambda = Expression.Lambda<T>(
					Expression.Block(
						new ParameterExpression[] { instanceExpr },
						Expression.Assign(instanceExpr, ctorExpr)
						),
					parameterExprs
					);
			}

			return lambda.Compile();
		}

		public static T BindPropertyGetterToDelegate<T>(Type classType, string propertyName, BindingFlags bindingFlags, bool isGetterNonPublic) where T : Delegate
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"propertyName cannot be null or whitespace", nameof(propertyName));
			}
			var method = classType.GetProperty(propertyName, bindingFlags).GetGetMethod(isGetterNonPublic);
			return (T)Delegate.CreateDelegate(typeof(T), method);
		}

		public static Func<TClass, TProperty> BindInstancePropertyGetterToDelegate<TClass, TProperty>(string propertyName, BindingFlags bindingFlags, bool isGetterNonPublic)
		{
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"propertyName cannot be null or whitespace", nameof(propertyName));
			}
			if (!bindingFlags.HasFlag(BindingFlags.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			var method = typeof(TClass).GetProperty(propertyName, bindingFlags).GetGetMethod(isGetterNonPublic);
			return (Func<TClass, TProperty>)Delegate.CreateDelegate(typeof(Func<TClass, TProperty>), method);
		}

		public static Func<TProperty> BindStaticPropertyGetterToDelegate<TProperty>(Type classType, string propertyName, BindingFlags bindingFlags, bool isGetterNonPublic)
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"propertyName cannot be null or whitespace", nameof(propertyName));
			}
			if (!bindingFlags.HasFlag(BindingFlags.Static))
			{
				bindingFlags |= BindingFlags.Static;
			}
			var method = classType.GetProperty(propertyName, bindingFlags).GetGetMethod(isGetterNonPublic);
			return (Func<TProperty>)Delegate.CreateDelegate(typeof(Func<TProperty>), method);
		}

		public static T BindPropertySetterToDelegate<T>(Type classType, string propertyName, BindingFlags bindingFlags, bool isSetterNonPublic) where T : Delegate
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"propertyName cannot be null or whitespace", nameof(propertyName));
			}
			var method = classType.GetProperty(propertyName, bindingFlags).GetSetMethod(isSetterNonPublic);
			return (T)Delegate.CreateDelegate(typeof(T), method);
		}

		public static Action<TClass, TProperty> BindInstancePropertySetterToDelegate<TClass, TProperty>(string propertyName, BindingFlags bindingFlags, bool isSetterNonPublic)
		{
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"propertyName cannot be null or whitespace", nameof(propertyName));
			}
			if (!bindingFlags.HasFlag(BindingFlags.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			var method = typeof(TClass).GetProperty(propertyName, bindingFlags).GetSetMethod(isSetterNonPublic);
			return (Action<TClass, TProperty>)Delegate.CreateDelegate(typeof(Action<TClass, TProperty>), method);
		}

		public static Action<TProperty> BindStaticPropertySetterToDelegate<TProperty>(Type classType, string propertyName, BindingFlags bindingFlags, bool isSetterNonPublic)
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"propertyName cannot be null or whitespace", nameof(propertyName));
			}
			if (!bindingFlags.HasFlag(BindingFlags.Static))
			{
				bindingFlags |= BindingFlags.Static;
			}
			var method = classType.GetProperty(propertyName, bindingFlags).GetSetMethod(isSetterNonPublic);
			return (Action<TProperty>)Delegate.CreateDelegate(typeof(Action<TProperty>), method);
		}

		public static Func<TClass, TField> BindInstanceFieldGetterToDelegate<TClass, TField>(string fieldName, BindingFlags bindingFlags)
		{
			if (string.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($" {nameof(fieldName)} cannot be null or whitespace", nameof(fieldName));
			}
			var classType = typeof(TClass);
			var fieldType = typeof(TField);
			var m_Instance = Expression.Parameter(classType, "instance");
			var m_Field = Expression.Variable(fieldType, "result");
			if (!bindingFlags.HasFlag(BindingFlags.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			var body = Expression.Block(
				new ParameterExpression[] { m_Field },
				 Expression.Assign(m_Field, Expression.Field(m_Instance, classType.GetField(fieldName, bindingFlags)))
				);
			//foreach (var expr in body.Expressions)
			//{
			//	System.Diagnostics.Debug.WriteLine(expr);
			//}
			var lamba = Expression.Lambda<Func<TClass, TField>>(body, m_Instance);
			//var str = lamba.ToString();
			//System.Diagnostics.Debug.WriteLine(lamba.ToString());
			return lamba.Compile();
		}

		public static Action<TClass, TField> BindInstanceFieldSetterToDelegate<TClass, TField>(string fieldName, BindingFlags bindingFlags, bool allowAssignUnsafe = false)
		{
			if (string.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($" {nameof(fieldName)} cannot be null or whitespace", nameof(fieldName));
			}
			var classType = typeof(TClass);
			var fieldType = typeof(TField);
			var instance = Expression.Parameter(classType, "instance");
			var inputParam = Expression.Parameter(fieldType, "inputParam");
			if (!bindingFlags.HasFlag(BindingFlags.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			if (!bindingFlags.HasFlag(BindingFlags.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			var fieldInfo = classType.GetField(fieldName, bindingFlags);
			if (fieldInfo.IsLiteral)
			{
				throw new ArgumentException($"Cannot assign <Literal Field> : const {fieldName}");
			}
			var fieldExpr = Expression.Field(instance, fieldInfo);
			BinaryExpression assignExpr;
			if (fieldInfo.IsInitOnly)
			{
				if (allowAssignUnsafe)
				{
					assignExpr = CreateAssignBinaryExpression(fieldExpr, inputParam);
				}
				else
				{
					throw new ArgumentException($"Cannot assign <Read Only Field> : readonly {fieldName}");
				}
			}
			else
			{
				assignExpr = Expression.Assign(fieldExpr, inputParam);
			}

			var body = Expression.Block(
				 assignExpr
				);
			//foreach (var expr in body.Expressions)
			//{
			//	System.Diagnostics.Debug.WriteLine(expr);
			//}
			var lamba = Expression.Lambda<Action<TClass, TField>>(body, instance, inputParam);
			//var str = lamba.ToString();
			//System.Diagnostics.Debug.WriteLine(lamba.ToString());
			return lamba.Compile();
		}

		public static RefAction<TClass, TField> BindInstanceFieldSetterToDelegateByRef<TClass, TField>(string fieldName, BindingFlags bindingFlags, bool allowAssignUnsafe = false)
		{
			if (string.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($" {nameof(fieldName)} cannot be null or whitespace", nameof(fieldName));
			}
			var classType = typeof(TClass);
			var fieldType = typeof(TField);
			var instance = Expression.Parameter(classType.MakeByRefType(), "instance");
			var inputParam = Expression.Parameter(fieldType.MakeByRefType(), "inputParam");
			if (!bindingFlags.HasFlag(BindingFlags.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			var fieldInfo = classType.GetField(fieldName, bindingFlags);
			if (fieldInfo.IsLiteral)
			{
				throw new ArgumentException($"Cannot assign <Literal Field> : const {fieldName}");
			}

			var fieldExpr = Expression.Field(instance, fieldInfo);
			BinaryExpression assignExpr;
			if (fieldInfo.IsInitOnly)
			{
				if (allowAssignUnsafe)
				{
					assignExpr = CreateAssignBinaryExpression(fieldExpr, inputParam);
				}
				else
				{
					throw new ArgumentException($"Cannot assign <Read Only Field> : readonly {fieldName}");
				}
			}
			else
			{
				assignExpr = Expression.Assign(fieldExpr, inputParam);
			}

			var body = Expression.Block(
				 assignExpr
				);
			var lambda = Expression.Lambda<RefAction<TClass, TField>>(body, instance, inputParam);
			return lambda.Compile();
		}

		public static BinaryExpression CreateAssignBinaryExpression(Expression left, Expression right)
		{
			if (newAssignBinaryExpression is null)
			{
				newAssignBinaryExpression = BindConstructorToDelegate<Func<Expression, Expression, BinaryExpression>>(
					typeof(BinaryExpression), s_typeOfAssignBinaryExpression, AllInstance, s_typeOfExpression, s_typeOfExpression
					);
			}
			return newAssignBinaryExpression(left, right);
		}

		public static Func<TField> BindStaticFieldGetterToDelegate<TField>(Type classType, string fieldName, BindingFlags bindingFlags)
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($"{nameof(fieldName)} cannot be null or whitespace", nameof(fieldName));
			}
			var fieldType = typeof(TField);
			var m_Field = Expression.Variable(fieldType, "result");
			if (!bindingFlags.HasFlag(BindingFlags.Static))
			{
				bindingFlags |= BindingFlags.Static;
			}
			var body = Expression.Block(
				new ParameterExpression[] { m_Field },
				 Expression.Assign(m_Field, Expression.Field(null, classType.GetField(fieldName, bindingFlags)))
				);
			var lamba = Expression.Lambda<Func<TField>>(body);
			return lamba.Compile();
		}

		public static Action<TField> BindStaticFieldSetterToDelegate<TField>(Type classType, string fieldName, BindingFlags bindingFlags, bool allowAssignUnsafe = false)
		{
			if (classType is null)
			{
				throw new ArgumentNullException(nameof(classType));
			}
			if (string.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($" {nameof(fieldName)} cannot be null or whitespace", nameof(fieldName));
			}
			var fieldType = typeof(TField);
			var inputParam = Expression.Parameter(fieldType, "inputParam");
			if (!bindingFlags.HasFlag(BindingFlags.Static))
			{
				bindingFlags |= BindingFlags.Static;
			}
			var fieldInfo = classType.GetField(fieldName, bindingFlags);
			if (fieldInfo.IsLiteral)
			{
				throw new ArgumentException($"Cannot assign <Literal Field> : const {fieldName}");
			}
			var fieldExpr = Expression.Field(null, fieldInfo);
			BinaryExpression assignExpr;
			if (fieldInfo.IsInitOnly)
			{
				if (allowAssignUnsafe)
				{
					assignExpr = CreateAssignBinaryExpression(fieldExpr, inputParam);
				}
				else
				{
					throw new ArgumentException($"Cannot assign <Read Only Field> : readonly {fieldName}");
				}
			}
			else
			{
				assignExpr = Expression.Assign(fieldExpr, inputParam);
			}

			var body = Expression.Block(
				 assignExpr
				);
			var lambda = Expression.Lambda<Action<TField>>(body, inputParam);

			return lambda.Compile();
		}

		//public static Func<Func<object>, Delegate> CreateMethodDelegateFactory(MethodInfo method, Type targetDelegateType)
		//{
		//	if (method is null)
		//	{
		//		throw new ArgumentNullException(nameof(method));
		//	}

		//	var parameters = method.GetParameters();
		//	ParameterExpression[] paramExprs = null;
		//	if (parameters != null && parameters.Length > 0)
		//	{
		//		// when ParameterExpression as any input for multi-expression, it will start enumeration
		//		// cannot use iteritor instead because it will be at the end of the enumeration when it be used at second expression.
		//		// use array instead
		//		paramExprs = parameters.Select(item => Expression.Parameter(item.ParameterType, item.Name)).ToArray();
		//	}

		//	var getInstance = Expression.Parameter(typeof(Func<object>), "getInstance");
		//	// Func<object> getInstance;
		//	Expression bodyExpr;
		//	if (method.IsStatic)
		//	{
		//		bodyExpr = Expression.Call(method, paramExprs);
		//	}
		//	else
		//	{
		//		var instance = Expression.Convert(Expression.Invoke(getInstance), method.ReflectedType);
		//		// getInstance.Invoke() as TReflected; where TReflected : methodInfo.ReflectedType;
		//		bodyExpr = Expression.Condition(
		//			Expression.NotEqual(
		//				instance, 
		//				Expression.Constant(null)
		//				),
		//			Expression.Call(instance, method, paramExprs),
		//			Expression.Default(method.ReturnType)
		//		);
		//	}
		//	// if ((getInstance.Invoke() as TReflected) != null) ? methodInfo.Call((getInstance.Invoke() as TReflected), parray) : return default(methodInfo.ReturnType)
		//	var lambdaExpr = Expression.Lambda(bodyExpr, paramExprs);
		//	var enclosureExpr = Expression.Lambda<Func<Func<object>, Delegate>>(lambdaExpr, getInstance);
		//	return enclosureExpr.Compile();
		//}


		public static Func<Func<object>, Delegate> CreateMethodDelegateFactory(MethodInfo method, Type targetDelegateType)
		{
			if (method is null)
			{
				throw new ArgumentNullException(nameof(method));
			}
			var dmtdName = $"DYNMTD_{method.Name}";
			var dmtd = new DynamicMethod(dmtdName, s_typeOfDelegate, s_typesOfGetInstance, true);
	
			var il = dmtd.GetILGenerator();
			/* Sample MSIL Instance
			.locals init (
				[0] class [FantaziaDesign.Test]FantaziaDesign.Test.TestClass,
				[1] bool,
				[2] class [mscorlib]System.Delegate
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: callvirt  instance !0 class [mscorlib]System.Func`1<object>::Invoke()
			IL_0007: isinst    FantaziaDesign.Test.TestClass
			IL_000C: stloc.0
			IL_000D: ldloc.0
			IL_000E: ldnull
			IL_000F: cgt.un
			IL_0011: stloc.1
			IL_0012: ldloc.1
			IL_0013: brfalse.s IL_0025

			IL_0015: nop
			IL_0016: ldloc.0
			IL_0017: ldftn     instance int32 FantaziaDesign.Test.TestClass::InstanceMethod(string)
			IL_001D: newobj    instance void class [mscorlib]System.Func`2<string, int32>::.ctor(object, native int)
			IL_0022: stloc.2
			IL_0023: br.s      IL_0029

			IL_0025: ldnull
			IL_0026: stloc.2
			IL_0027: br.s      IL_0029

			IL_0029: ldloc.2
			IL_002A: ret
			 */

			/* Sample MSIL Static
			.locals init (
				[0] class [mscorlib]System.Delegate
			)

			IL_0000: nop
			IL_0001: ldnull
			IL_0002: ldftn     void FantaziaDesign.Test.TestClass::StaticMethod(string)
			IL_0008: newobj    instance void class [mscorlib]System.Action`1<string>::.ctor(object, native int)
			IL_000D: stloc.0
			IL_000E: br.s      IL_0010

			IL_0010: ldloc.0
			IL_0011: ret
			 */
			method.WriteDelegateFactoryIL(il, targetDelegateType);
			return dmtd.CreateDelegate(s_typeOfDelegateFactory) as Func<Func<object>, Delegate>;
		}

		internal static void WriteDelegateFactoryIL(this MethodInfo method, ILGenerator il, Type targetType)
		{
			if (il is null)
			{
				throw new ArgumentNullException(nameof(il));
			}

			if (targetType is null)
			{
				throw new ArgumentNullException(nameof(targetType));
			}

			var tgtDelegateCtor = targetType.GetConstructors().First();
			if (method.IsStatic)
			{
				il.DeclareLocal(s_typeOfDelegate);
				il.Emit(OpCodes.Nop);
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Ldftn, method);
				il.Emit(OpCodes.Newobj, tgtDelegateCtor);
				il.Emit(OpCodes.Stloc_0);
				var IL_0010 = il.DefineLabel();
				il.Emit(OpCodes.Br_S, IL_0010);

				il.MarkLabel(IL_0010);
				il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ret);
			}
			else
			{
				il.DeclareLocal(method.ReflectedType);
				il.DeclareLocal(typeof(bool));
				il.DeclareLocal(s_typeOfDelegate);

				il.Emit(OpCodes.Nop);
				il.Emit(OpCodes.Ldarg_0);
				il.EmitCall(OpCodes.Callvirt, s_Invoke_Func_object, null);
				il.Emit(OpCodes.Isinst, method.ReflectedType);
				il.Emit(OpCodes.Stloc_0);
				il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Cgt_Un);
				il.Emit(OpCodes.Stloc_1);
				il.Emit(OpCodes.Ldloc_1);
				var IL_0025 = il.DefineLabel();
				il.Emit(OpCodes.Brfalse_S, IL_0025);

				il.Emit(OpCodes.Nop);
				il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ldftn, method);
				il.Emit(OpCodes.Newobj, tgtDelegateCtor);
				il.Emit(OpCodes.Stloc_2);
				var IL_0029 = il.DefineLabel();
				il.Emit(OpCodes.Br_S, IL_0029);

				il.MarkLabel(IL_0025);
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Stloc_2);
				il.Emit(OpCodes.Br_S, IL_0029);

				il.MarkLabel(IL_0029);
				il.Emit(OpCodes.Ldloc_2);
				il.Emit(OpCodes.Ret);
			}

		}

	}

}
