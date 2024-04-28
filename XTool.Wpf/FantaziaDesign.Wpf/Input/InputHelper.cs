using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using FantaziaDesign.Core;
using System.Windows.Interop;
using LinqExpr = System.Linq.Expressions;
using FantaziaDesign.Interop;

namespace FantaziaDesign.Wpf.Input
{
	[Flags]
	public enum WpfRawMouseActions
	{
		None = 0,
		AttributesChanged = 1,
		Activate = 2,
		Deactivate = 4,
		RelativeMove = 8,
		AbsoluteMove = 16,
		VirtualDesktopMove = 32,
		Button1Press = 64,
		Button1Release = 128,
		Button2Press = 256,
		Button2Release = 512,
		Button3Press = 1024,
		Button3Release = 2048,
		Button4Press = 4096,
		Button4Release = 8192,
		Button5Press = 16384,
		Button5Release = 32768,
		VerticalWheelRotate = 65536,
		HorizontalWheelRotate = 131072,
		QueryCursor = 262144,
		CancelCapture = 524288
	}

	[Flags]
	public enum WpfRawKeyboardActions
	{
		None = 0,
		AttributesChanged = 1,
		Activate = 2,
		Deactivate = 4,
		KeyDown = 8,
		KeyUp = 16
	}

	public static class InputHelper
	{
		private static Func<MouseDevice, CaptureMode> GetCaptureMode_MouseDevice;

		public static CaptureMode CriticalGetMouseCaptureMode(this MouseDevice mouseDevice)
		{
			if (mouseDevice is null)
			{
				throw new ArgumentNullException(nameof(mouseDevice));
			}

			if (GetCaptureMode_MouseDevice is null)
			{
				GetCaptureMode_MouseDevice = ReflectionUtil.BindInstancePropertyGetterToDelegate<MouseDevice, CaptureMode>(
					"CapturedMode",
					ReflectionUtil.NonPublicInstance,
					true
					);
			}
			return GetCaptureMode_MouseDevice(mouseDevice);
		}

		public static IInputElement GetMouseAbsolutelyOver(this MouseDevice mouseDevice, bool clearCapture = false)
		{
			if (mouseDevice is null)
			{
				throw new ArgumentNullException(nameof(mouseDevice));
			}

			var capturedElement = mouseDevice.Captured;
			var captureMode = CriticalGetMouseCaptureMode(mouseDevice);
			bool hasCapturedElement = capturedElement != null;
			if (hasCapturedElement)
			{
				//clear capture
				mouseDevice.Capture(null, CaptureMode.None);
			}
			var directlyOverElement = mouseDevice.DirectlyOver;
			if (hasCapturedElement && !clearCapture)
			{
				// recapture if need
				mouseDevice.Capture(capturedElement, captureMode);
			}
			return directlyOverElement;
		}

		private static readonly Assembly s_sysWndInputAsm = Assembly.GetAssembly(typeof(InputEventArgs));
		private static readonly Type s_type_RawMouseInputReport = s_sysWndInputAsm.GetType("System.Windows.Input.RawMouseInputReport");
		private static readonly Type s_type_RawStylusInputReport = s_sysWndInputAsm.GetType("System.Windows.Input.RawStylusInputReport");
		private static readonly Type s_type_RawTextInputReport = s_sysWndInputAsm.GetType("System.Windows.Input.RawTextInputReport");
		private static readonly Type s_type_RawAppCommandInputReport = s_sysWndInputAsm.GetType("System.Windows.Input.RawAppCommandInputReport");
		private static readonly Type s_type_RawKeyboardInputReport = s_sysWndInputAsm.GetType("System.Windows.Input.RawKeyboardInputReport");
		private static readonly Type s_type_InputReportEventArgs = s_sysWndInputAsm.GetType("System.Windows.Input.InputReportEventArgs");
		private static readonly Type s_type_RawMouseActions = s_sysWndInputAsm.GetType("System.Windows.Input.RawMouseActions");
		private static readonly Type s_type_RawKeyboardActions = s_sysWndInputAsm.GetType("System.Windows.Input.RawKeyboardActions");

		private static Func<RoutedEvent> GetPreviewInputReportEvent_InputManager;

		private static Func<InputMode, int, PresentationSource, int, int, int, int, IntPtr, InputDevice, InputEventArgs> CreateInputReportEventFromRawMouseInput;

		public static bool SimulateRawMouseInput(IntPtr hwnd, int pointX, int pointY, WpfRawMouseActions rawMouseActions, int wheel)
		{
			//Type rawMouseActionsType = targetAssembly.GetType("System.Windows.Input.RawMouseActions");
			int msgTime = User32.GetMessageTime();

			if (CreateInputReportEventFromRawMouseInput is null)
			{
				CreateInputReportEventFromRawMouseInput = MakeRawMouseInputReportEventBuilder();
			}
			var inputReportEventArgs = CreateInputReportEventFromRawMouseInput(
					InputMode.Foreground,
					msgTime,
					HwndSource.FromHwnd(hwnd),
					(int)rawMouseActions,
					pointX,
					pointY,
					wheel,
					IntPtr.Zero,
					Mouse.PrimaryDevice
				);

			if (GetPreviewInputReportEvent_InputManager is null)
			{
				GetPreviewInputReportEvent_InputManager = ReflectionUtil.BindStaticFieldGetterToDelegate<RoutedEvent>(typeof(InputManager), "PreviewInputReportEvent", ReflectionUtil.NonPublicStatic);
			}
			inputReportEventArgs.RoutedEvent = GetPreviewInputReportEvent_InputManager();

			return InputManager.Current.ProcessInput(inputReportEventArgs);
		}

		private static Func<InputMode, int, PresentationSource, int, int, int, int, IntPtr, InputDevice, InputEventArgs>
			MakeRawMouseInputReportEventBuilder()
		{
			var type_InputReport = s_type_RawMouseInputReport;
			const int ctorParamsLen = 8;
			var rawMouseInputReportCtor = type_InputReport.GetConstructors()[0];
			var mouseInputReportExpr = LinqExpr.Expression.Parameter(type_InputReport, "mouseInputReport");
			/* 
			 * InputMode mode, 
			 * int timestamp, 
			 * PresentationSource inputSource, 
			 * RawMouseActions actions, 
			 * int x, 
			 * int y, 
			 * int wheel, 
			 * IntPtr extraInformation
			*/
			var paramsTypes = new Type[ctorParamsLen]
			{
				typeof(InputMode),
				typeof(int),
				typeof(PresentationSource),
				typeof(int),// need convert to System.Windows.Input.RawMouseActions
				typeof(int),
				typeof(int),
				typeof(int),
				typeof(IntPtr)
			};
			var paramExpr = new LinqExpr.ParameterExpression[ctorParamsLen + 1];
			var argsExprs = new LinqExpr.Expression[ctorParamsLen];
			for (int i = 0; i < ctorParamsLen; i++)
			{
				paramExpr[i] = LinqExpr.Expression.Parameter(paramsTypes[i], $"arg{i}");
				if (i == 3)
				{
					argsExprs[i] = LinqExpr.Expression.Convert(paramExpr[i], s_type_RawMouseActions);
				}
				else
				{
					argsExprs[i] = paramExpr[i];
				}
			}
			// var mouseInputReport = new RawMouseInputReport(arg0,..., arg7);
			var assignExpr01 = LinqExpr.Expression.Assign(
				mouseInputReportExpr,
				LinqExpr.Expression.New(rawMouseInputReportCtor, argsExprs)
				);
			// mouseInputReport._isSynchronize = true;
			var assignExpr02 = LinqExpr.Expression.Assign(
				LinqExpr.Expression.Field(
					mouseInputReportExpr,
					type_InputReport.GetField("_isSynchronize", ReflectionUtil.NonPublicInstance)
				),
				LinqExpr.Expression.Constant(true)
				);

			var inputReportEventArgsCtor = s_type_InputReportEventArgs.GetConstructors()[0];
			var inputReportEventArgsExpr = LinqExpr.Expression.Parameter(typeof(InputEventArgs), "inputReportEventArgs");
			var inputDeviceExpr = LinqExpr.Expression.Parameter(typeof(InputDevice), "inputDevice");
			paramExpr[ctorParamsLen] = inputDeviceExpr;
			// var inputReportEventArgs = new InputReportEventArgs(inputDevice, mouseInputReport);
			var assignExpr03 = LinqExpr.Expression.Assign(
				inputReportEventArgsExpr,
				LinqExpr.Expression.Convert(
					LinqExpr.Expression.New(inputReportEventArgsCtor, inputDeviceExpr, mouseInputReportExpr),
					typeof(InputEventArgs)
					)
				);

			var lambda = LinqExpr.Expression.Lambda<Func<InputMode, int, PresentationSource, int, int, int, int, IntPtr, InputDevice, InputEventArgs>>(
				LinqExpr.Expression.Block(
					new LinqExpr.ParameterExpression[] { mouseInputReportExpr, inputReportEventArgsExpr },
					assignExpr01,
					assignExpr02,
					assignExpr03
					),
				paramExpr
				);
			return lambda.Compile();
		}

		private static Func<PresentationSource,InputMode, int, int, InputType, InputType, InputDevice, InputEventArgs>
			MakeRawAppCommandInputReportEventBuilder()
		{
			var type_InputReport = s_type_RawAppCommandInputReport;
			const int ctorParamsLen = 6;
			var rawInputReportCtor = type_InputReport.GetConstructors()[0];
			var mouseInputReportExpr = LinqExpr.Expression.Parameter(type_InputReport, "inputReport");
			/* 
			PresentationSource inputSource, 
			InputMode mode, 
			int timestamp, 
			int appCommand, 
			InputType device, 
			InputType inputType
			*/
			var paramsTypes = new Type[ctorParamsLen]
			{
				typeof(PresentationSource),
				typeof(InputMode),
				typeof(int),
				typeof(int),
				typeof(InputType),
				typeof(InputType)
			};
			var paramExpr = new LinqExpr.ParameterExpression[ctorParamsLen + 1];
			var argsExprs = new LinqExpr.Expression[ctorParamsLen];
			for (int i = 0; i < ctorParamsLen; i++)
			{
				paramExpr[i] = LinqExpr.Expression.Parameter(paramsTypes[i], $"arg{i}");
				argsExprs[i] = paramExpr[i];
			}
			// var inputReport = new RawAppCommandInputReport(arg0,..., arg5);
			var assignExpr01 = LinqExpr.Expression.Assign(
				mouseInputReportExpr,
				LinqExpr.Expression.New(rawInputReportCtor, argsExprs)
				);

			var inputReportEventArgsCtor = s_type_InputReportEventArgs.GetConstructors()[0];
			var inputReportEventArgsExpr = LinqExpr.Expression.Parameter(typeof(InputEventArgs), "inputReportEventArgs");
			var inputDeviceExpr = LinqExpr.Expression.Parameter(typeof(InputDevice), "inputDevice");
			paramExpr[ctorParamsLen] = inputDeviceExpr;
			// var inputReportEventArgs = new InputReportEventArgs(inputDevice, mouseInputReport);
			var assignExpr03 = LinqExpr.Expression.Assign(
				inputReportEventArgsExpr,
				LinqExpr.Expression.Convert(
					LinqExpr.Expression.New(inputReportEventArgsCtor, inputDeviceExpr, mouseInputReportExpr),
					typeof(InputEventArgs)
					)
				);

			var lambda = LinqExpr.Expression.Lambda<Func<PresentationSource, InputMode, int, int, InputType, InputType, InputDevice, InputEventArgs>>(
				LinqExpr.Expression.Block(
					new LinqExpr.ParameterExpression[] { mouseInputReportExpr, inputReportEventArgsExpr },
					assignExpr01,
					assignExpr03
					),
				paramExpr
				);
			return lambda.Compile();
		}

		private static Func<PresentationSource, InputMode, int,int,int ,bool,bool,int,IntPtr, InputDevice, InputEventArgs>
			MakeRawKeyboardInputReportEventBuilder()
		{
			var type_InputReport = s_type_RawKeyboardInputReport;
			const int ctorParamsLen = 9;
			var rawInputReportCtor = type_InputReport.GetConstructors()[0];
			var mouseInputReportExpr = LinqExpr.Expression.Parameter(type_InputReport, "inputReport");

			// PresentationSource inputSource,
			// InputMode mode,
			// int timestamp,
			// RawKeyboardActions actions,
			// int scanCode,
			// bool isExtendedKey,
			// bool isSystemKey,
			// int virtualKey,
			// IntPtr extraInformation
			var paramsTypes = new Type[ctorParamsLen]
			{
				typeof(PresentationSource),
				typeof(InputMode),
				typeof(int),
				typeof(int), // need convert to System.Windows.Input.RawKeyboardActions
				typeof(int),
				typeof(bool),
				typeof(bool),
				typeof(int),
				typeof(IntPtr)
			};
			var paramExpr = new LinqExpr.ParameterExpression[ctorParamsLen + 1];
			var argsExprs = new LinqExpr.Expression[ctorParamsLen];
			for (int i = 0; i < ctorParamsLen; i++)
			{
				paramExpr[i] = LinqExpr.Expression.Parameter(paramsTypes[i], $"arg{i}");
				if (i == 3)
				{
					argsExprs[i] = LinqExpr.Expression.Convert(paramExpr[i], s_type_RawKeyboardActions);
				}
				else
				{
					argsExprs[i] = paramExpr[i];
				}
			}
			// var inputReport = new RawKeyboardActions(arg0,..., arg8);
			var assignExpr01 = LinqExpr.Expression.Assign(
				mouseInputReportExpr,
				LinqExpr.Expression.New(rawInputReportCtor, argsExprs)
				);

			var inputReportEventArgsCtor = s_type_InputReportEventArgs.GetConstructors()[0];
			var inputReportEventArgsExpr = LinqExpr.Expression.Parameter(typeof(InputEventArgs), "inputReportEventArgs");
			var inputDeviceExpr = LinqExpr.Expression.Parameter(typeof(InputDevice), "inputDevice");
			paramExpr[ctorParamsLen] = inputDeviceExpr;
			// var inputReportEventArgs = new InputReportEventArgs(inputDevice, mouseInputReport);
			var assignExpr03 = LinqExpr.Expression.Assign(
				inputReportEventArgsExpr,
				LinqExpr.Expression.Convert(
					LinqExpr.Expression.New(inputReportEventArgsCtor, inputDeviceExpr, mouseInputReportExpr),
					typeof(InputEventArgs)
					)
				);

			var lambda = LinqExpr.Expression.Lambda<Func<PresentationSource, InputMode, int, int, int, bool, bool, int, IntPtr, InputDevice, InputEventArgs>>(
				LinqExpr.Expression.Block(
					new LinqExpr.ParameterExpression[] { mouseInputReportExpr, inputReportEventArgsExpr },
					assignExpr01,
					assignExpr03
					),
				paramExpr
				);
			return lambda.Compile();
		}



	}




}
