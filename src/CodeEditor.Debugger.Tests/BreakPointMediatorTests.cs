﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.Tests
{
	[TestFixture]
	public class BreakPointMediatorTests
	{
		private Mock<IDebuggerSession> _session;
		private Mock<IDebugBreakPointProvider> _breakPointProvider;
		private IDebugTypeProvider _typeProvider;
		private SourceToTypeMapper _sourceToTypeMapper;

		[SetUp]
		public void SetUp()
		{
			_session = new Mock<IDebuggerSession>();
			_breakPointProvider = new Mock<IDebugBreakPointProvider>();
			_typeProvider = new DebugTypeProvider(_session.Object);
			_sourceToTypeMapper = new SourceToTypeMapper(_typeProvider);
			new BreakpointMediator(_session.Object, _breakPointProvider.Object, _typeProvider, _sourceToTypeMapper, new Mock<ILogProvider>().Object);
		}

		[Test]
		public void WhenTypeLoadsMatchingExistingBreakPoint_CreatesBreakRequest()
		{
			AddBreakpoint(MockBreakPointFor("myfile.cs"));
			RaiseTypeLoad(MockTypeFor("myfile.cs"));
			VerifyCreateBreakpointRequest();
		}

		[Test]
		public void BreakPointGetsAddedMatchingAlreadyLoadedType_CreatesBreakRequest()
		{
			RaiseTypeLoad(MockTypeFor("myfile.cs"));
			AddBreakpoint(MockBreakPointFor("myfile.cs"));
			VerifyCreateBreakpointRequest();
		}

		private void VerifyCreateBreakpointRequest()
		{
			_session.Verify(s => s.CreateBreakpointRequest(It.Is<IDebugLocation>(location => true)));
		}

		private void AddBreakpoint(Mock<IBreakPoint> breakpoint)
		{
			_breakPointProvider.Raise(bpp => bpp.BreakpointAdded += null, breakpoint.Object);
		}

		private void RaiseTypeLoad(Mock<IDebugType> debugType)
		{
			_session.Raise(tp => tp.TypeLoaded += null, debugType.Object);
		}

		private static Mock<IDebugType> MockTypeFor(string sourceFile)
		{
			var debugType = new Mock<IDebugType>();
			debugType.SetupGet(t => t.SourceFiles).Returns(new[] {sourceFile});
			return debugType;
		}

		private static Mock<IBreakPoint> MockBreakPointFor(string file)
		{
			var breakpoint = new Mock<IBreakPoint>();
			breakpoint.SetupGet(bp => bp.File).Returns(file);
			return breakpoint;
		}
	}
}
