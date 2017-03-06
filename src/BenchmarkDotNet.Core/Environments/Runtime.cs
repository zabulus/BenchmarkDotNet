﻿namespace BenchmarkDotNet.Environments
{
    public abstract class Runtime
    {
        /// <summary>
        /// Full .NET Framework (Windows only)
        /// </summary>
        public static readonly Runtime Clr = new ClrRuntime();

        /// <summary>
        /// Mono
        /// See also: http://www.mono-project.com/
        /// </summary>
        public static readonly Runtime Mono = new MonoRuntime();

        /// <summary>
        /// Cross-platform Core CLR runtime
        /// See also: https://docs.microsoft.com/en-us/dotnet/
        /// </summary>
        public static readonly Runtime Core = new CoreRuntime();
        
        public static readonly Runtime Uap = new UapRuntime();

        public string Name { get; }

        protected Runtime(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }

    public class ClrRuntime : Runtime
    {
        public ClrRuntime() : base("Clr")
        {
        }
    }

    public class CoreRuntime : Runtime
    {
        public CoreRuntime() : base("Core")
        {
        }
    }

    public class MonoRuntime : Runtime
    {
        public string CustomPath { get; }

        public MonoRuntime() : base("Mono")
        {
        }

        public MonoRuntime(string name, string customPath) : base(name)
        {
            CustomPath = customPath;
        }
    }

    public class UapRuntime : Runtime
    {
        public string DevicePortalUri { get; private set; }
        public string CsfrCookie { get; private set; }
        public string WmidCookie { get; private set; }
        public string UapBinariesPath { get; private set; }
        
        public UapRuntime() : base("Uap")
        { }

        public UapRuntime(string name, string devicePortalUri, string csfrCookie, string wmidCookie, string uapBinariesPath) : base(name)
        {
            this.DevicePortalUri = devicePortalUri;
            this.CsfrCookie = csfrCookie;
            this.WmidCookie = wmidCookie;
            this.UapBinariesPath = uapBinariesPath;
        }
    }
}