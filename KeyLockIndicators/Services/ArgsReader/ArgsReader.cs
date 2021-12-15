using KeyLockIndicators.Services.ToastBuilder;
using System;

namespace KeyLockIndicators.Services.ArgsReader
{

    public sealed class ArgsReader : IArgsReader
    {

        public string[] Args { get; set; } = Environment.GetCommandLineArgs();


        public ToastType ToastType => (ToastType)int.Parse(GetArgument(AppSettings.ToastSwitch, Args));


        private static string GetArgument(string prefix, string[] args)
        {
            foreach (var arg in args)
                if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return arg[prefix.Length..];

            return null;
        }

    }

}
