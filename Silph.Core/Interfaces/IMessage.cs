using Silph.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silph.Core.Interfaces
{
    public interface IMessage
    {
        public int Code { get; }
        public string Title { get; }
        public string Template { get; }
        public MessageType Type { get; }
        LogScope? LogScopeOverride { get; }
        public string Format(params object[] args);
    }
}
