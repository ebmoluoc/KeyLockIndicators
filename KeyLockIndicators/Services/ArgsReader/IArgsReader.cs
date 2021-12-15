using KeyLockIndicators.Services.ToastBuilder;

namespace KeyLockIndicators.Services.ArgsReader
{
    public interface IArgsReader
    {
        string[] Args { get; set; }
        ToastType ToastType { get; }
    }
}
