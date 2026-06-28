using System;
using System.Collections.Generic;
using System.Text;

namespace Silph.Core.Interfaces
{
    internal interface ILog
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }
}
