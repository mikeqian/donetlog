// 
// Copyright (c) 2004 Jaroslaw Kowalski <jaak@polbox.com>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of the Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

#if !NETCF
using System;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace NLog.LayoutAppenders
{
    [LayoutAppender("callsite")]
    public class CallSiteLayoutAppender : LayoutAppender
    {
        private bool _className = true;
        private bool _methodName = true;
        private bool _sourceFile = false;

        public bool ClassName
        {
            get { return _className; }
            set { _className = value; }
        }
        
        public bool MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }
        
        public bool FileName
        {
            get { return _sourceFile; }
            set { _sourceFile = value; }
        }
        
        public override int GetEstimatedBufferSize(LogEventInfo ev)
        {
            return 200;
        }
        
		public override int NeedsStackTrace()
		{
			return _sourceFile ? 2 : 1;
		}

        public override void Append(StringBuilder builder, LogEventInfo ev)
        {
            StackFrame frame = ev.UserStackFrame;
            if (frame != null)
            {
                StringBuilder sb2 = builder;
                if (Padding != 0)
                    sb2 = new StringBuilder();

                MethodBase method = frame.GetMethod();
                if (_className) {
                    sb2.Append(method.DeclaringType.FullName);
                }
                if (_methodName) {
                    if (_className) {
                        sb2.Append(".");
                    }
                    sb2.Append(method.Name);
                }
                if (_sourceFile) {
                    string fileName = frame.GetFileName();
                    if (fileName != null) {
                        sb2.Append("(");
                        sb2.Append(fileName);
                        sb2.Append(":");
                        sb2.Append(frame.GetFileLineNumber());
                        sb2.Append(")");
                    }
                }
                if (Padding != 0)
                    builder.Append(ApplyPadding(sb2.ToString()));
            }
        }
    }
}
#endif
