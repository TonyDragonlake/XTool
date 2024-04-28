using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace FantaziaDesign.Core
{
	public interface IWeakDelegate
	{
		bool IsStatic { get; }
		bool IsHostAlived { get; }
		object Host { get; }
		bool TryGetDelegate(out Delegate @delegate);
	}

	public sealed class WeakDelegateFactory : IEquatable<WeakDelegateFactory>
	{
		public struct FactoryToken : IDeepCopyable<FactoryToken>, IEquatable<FactoryToken>
		{
			private readonly string m_asmQName;
			private readonly int m_mtdMdToken;
			private readonly string m_tgtDelegateName;

			public FactoryToken(string assemblyQualifiedName, int methodMetadataToken, string targetDelegateName)
			{
				m_asmQName = assemblyQualifiedName;
				m_mtdMdToken = methodMetadataToken;
				m_tgtDelegateName = targetDelegateName;
			}

			public FactoryToken(MethodInfo method, Type delegateType)
			{
				if (method is null)
				{
					throw new ArgumentNullException(nameof(method));
				}

				if (delegateType is null)
				{
					throw new ArgumentNullException(nameof(delegateType));
				}

				m_asmQName = method.ReflectedType.AssemblyQualifiedName;
				m_mtdMdToken = method.MetadataToken;
				m_tgtDelegateName = delegateType.FullName;
			}

			public FactoryToken(Delegate @delegate) : this(@delegate?.Method, @delegate?.GetType())
			{
			}

			public string AssemblyQualifiedName => m_asmQName;
			public int MethodMetadataToken => m_mtdMdToken;
			public string TargetDelegateName => m_tgtDelegateName;

			public FactoryToken DeepCopy()
			{
				return new FactoryToken(AssemblyQualifiedName, MethodMetadataToken, TargetDelegateName);
			}

			public void DeepCopyValueFrom(FactoryToken obj)
			{
				throw new InvalidOperationException("Read only: Deep copy value from QualifiedMethodToken is not allowed.");
			}

			public object Clone()
			{
				return DeepCopy();
			}

			public bool Equals(FactoryToken other)
			{
				return m_mtdMdToken == other.m_mtdMdToken
					&& m_asmQName == other.m_asmQName
					&& m_tgtDelegateName == other.m_tgtDelegateName;
			}

			public override bool Equals(object obj)
			{
				if (obj is FactoryToken token)
				{
					return Equals(token);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

			public override string ToString()
			{
				if (m_mtdMdToken == 0 || string.IsNullOrWhiteSpace(m_asmQName) || string.IsNullOrWhiteSpace(m_tgtDelegateName))
				{
					return string.Empty;
				}
				return string.Format("{0} Method {1:X} at Assembly {2}", m_tgtDelegateName, m_mtdMdToken, m_asmQName);
			}
		}

		private static readonly object s_lockObj = new object();
		private static readonly Dictionary<FactoryToken, WeakDelegateFactory> s_factories = new Dictionary<FactoryToken, WeakDelegateFactory>();

		private readonly FactoryToken m_token;
		private readonly MethodInfo m_method;
		private readonly Type m_targetDelegateType;
		private Func<Func<object>, Delegate> m_factoryDelegate;

		public FactoryToken Token => m_token;
		public int MethodMetadataToken => m_token.MethodMetadataToken;
		public string AssemblyQualifiedName => m_token.AssemblyQualifiedName;
		public MethodInfo Method => m_method;
		public Type TargetDelegateType => m_targetDelegateType;
		public Func<Func<object>, Delegate> FactoryDelegate => m_factoryDelegate;

		public static WeakDelegateFactory FromMethodInfo(MethodInfo method, Type targetDelegateType = null)
		{
			if (method is null)
			{
				return null;
			}
			if (targetDelegateType is null)
			{
				targetDelegateType = GetDefaultTargetDelegateType(method);
			}
			var token = new FactoryToken(method, targetDelegateType);
			WeakDelegateFactory result = null;
			if (Monitor.TryEnter(s_lockObj))
			{
				try
				{
					if (s_factories.Count == 0 || !s_factories.TryGetValue(token, out result))
					{
						var factory = new WeakDelegateFactory(method, targetDelegateType);
						factory.InitializeFactory();
						s_factories.Add(factory.m_token, factory);
						result = factory;
					}
				}
				finally
				{
					Monitor.Exit(s_lockObj);
				}
			}
			return result;
		}

		private static Type GetDefaultTargetDelegateType(MethodInfo method)
		{
			var parameters = method.GetParameters();
			if (parameters.Length > 16)
			{
				throw new ArgumentException("Inference Delegate type failed: The length of parameters exceeds the default delegate parameter limit (16).");
			}
			var paramTypes = parameters.Select(item => item.ParameterType);
			if (method.ReturnType != typeof(void))
			{
				var gTypes = CollectionHelper.EnumerationCombine(paramTypes, method.ReturnType).ToArray();
				return typeof(Func<>).MakeGenericType(gTypes);
			}
			if (parameters.Length == 0)
			{
				return typeof(Action);
			}
			return typeof(Action<>).MakeGenericType(paramTypes.ToArray());
		}

		private WeakDelegateFactory(MethodInfo method, Type targetDelegateType)
		{
			m_method = method;
			m_token = new FactoryToken(method, targetDelegateType);
			m_targetDelegateType = targetDelegateType;
		}

		private void InitializeFactory()
		{
			m_factoryDelegate = ReflectionUtil.CreateMethodDelegateFactory(m_method, m_targetDelegateType);
		}

		public bool Equals(WeakDelegateFactory other)
		{
			if (other is null)
			{
				return false;
			}
			return m_token.Equals(other.m_token);
		}
	}

	public class WeakDelegate : IWeakDelegate, IEquatable<WeakDelegate>
	{
		//private static object GetNullObject() => null;
		private WeakDelegateFactory m_factory;
		private WeakReference m_weakHost;

		public static WeakDelegate FromDelegate(Delegate @delegate)
		{
			if (@delegate is null)
			{
				return null;
			}
			if (@delegate.GetInvocationList().Length > 1)
			{
				throw new ArgumentException("Cannot initialize WeakDelegate from the Delegate that has more than one Invocation.");
			}
			var factory = WeakDelegateFactory.FromMethodInfo(@delegate.Method, @delegate.GetType());
			if (factory is null)
			{
				return null;
			}
			return new WeakDelegate() { m_factory = factory, m_weakHost = new WeakReference(@delegate.Target) };
		}

		private WeakDelegate()
		{
		}

		public WeakDelegateFactory Factory => m_factory;
		public bool IsStatic => m_factory.Method.IsStatic;
		public bool IsHostAlived => (m_weakHost?.IsAlive).GetValueOrDefault();
		public object Host => m_weakHost?.Target;

		public bool Equals(WeakDelegate other)
		{
			if (other is null)
			{
				return false;
			}
			return m_factory.Equals(other.m_factory);
		}

		public bool TargetEquals(Delegate @delegate)
		{
			if (@delegate is null)
			{
				return !IsStatic && !IsHostAlived;
			}
			return Host == @delegate.Target && Equals(m_factory.Method, @delegate.GetMethodInfo());
		}

		public bool TokenEquals(WeakDelegateFactory.FactoryToken token)
		{
			return m_factory.Token.Equals(token);
		}

		private object GetDelegateHost() => m_weakHost.Target;

		public bool TryGetDelegate(out Delegate @delegate)
		{
			var factoryDelegate = m_factory.FactoryDelegate;
			if (factoryDelegate is null)
			{
				@delegate = null;
				return false;
			}
			if (IsStatic)
			{
				//getInstance = GetNullObject;
				@delegate = factoryDelegate.Invoke(null);
				return true;
			}
			if (!IsHostAlived)
			{
				@delegate = null;
				return false;
			}
			@delegate = factoryDelegate.Invoke(GetDelegateHost);
			return true;
		}

		public TDelegate GetDelegateAs<TDelegate>() where TDelegate : Delegate
		{
			if (TryGetDelegate(out var @delegate))
			{
				var tDelegate = @delegate as TDelegate;
				if (tDelegate != null)
				{
					return tDelegate;
				}
			}
			return null;
		}
	}

}
