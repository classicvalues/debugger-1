﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;

namespace CodeEditor.Debugger
{
	public interface IDebugBreakPointProvider
	{
		IBreakPoint GetBreakPointAt(string file, int lineNumber);
		void ToggleBreakPointAt(string fileName, int lineNumber);
		event Action<IBreakPoint> BreakPointAdded;
		event Action<IBreakPoint> BreakPointRemoved;
	}

	[Export(typeof(IDebugBreakPointProvider))]
	class DebugBreakPointProvider : IDebugBreakPointProvider
	{
		readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public event Action<IBreakPoint> BreakPointAdded;
		public event Action<IBreakPoint> BreakPointRemoved;

		public IBreakPoint GetBreakPointAt(string file, int lineNumber)
		{
			return _breakPoints.FirstOrDefault(bp => bp.File == file && bp.LineNumber == lineNumber);
		}

		public void ToggleBreakPointAt(string fileName, int lineNumber)
		{
			var breakPoint = GetBreakPointAt(fileName, lineNumber);
			if (breakPoint == null)
				AddBreakPoint(new BreakPoint(fileName, lineNumber));
			else
				RemoveBreakPoint(breakPoint);
		}

		private void AddBreakPoint(BreakPoint point)
		{
			_breakPoints.Add(point);
			BreakPointAdded(point);
		}

		private void RemoveBreakPoint(IBreakPoint breakPoint)
		{
			_breakPoints.Remove(breakPoint);
			BreakPointRemoved(breakPoint);
		}
	}
}
