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

using System;

namespace Framework.Qlh.Log
{
    public abstract class Appender
    {
        const string InternalAppenderNamePrefix = "Framework.Qlh.Log.Appenders.";
        const string InternalAppenderNameSuffix = "Appender";

        protected Appender()
        {
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";
        }

        private Layout _compiledlayout;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Layout
        {
            get { return _compiledlayout.Text; }
            set { _compiledlayout = new Layout(value); }
        }

        public Layout CompiledLayout
        {
            get { return _compiledlayout; }
            set { _compiledlayout = value; }
        }

        public abstract void Append(LogEventInfo ev);

        public static Appender Create(string name) {
            Type appenderType;
            
            if (name.IndexOf('.') < 0 && name.IndexOf(',') < 0) {
                // simple name - no dot nor comma - we create an internal appender
                string fullName = InternalAppenderNamePrefix + name + InternalAppenderNameSuffix;

                // get the type ignoring case and throw on error
                appenderType = typeof(Appender).Assembly.GetType(fullName, true);
            } else {
                appenderType = Type.GetType(name);
            }
            return (Appender)Activator.CreateInstance(appenderType);
        }

        public virtual int NeedsStackTrace()
        {
            return CompiledLayout.NeedsStackTrace();
        }
    }
}
