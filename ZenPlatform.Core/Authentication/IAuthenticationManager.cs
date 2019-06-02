namespace ZenPlatform.Core.Authentication
{
    public interface IAuthenticationManager
    {
        IUser Authenticate(IAuthenticationToken token);
        void RegisterProvider(IAuthenticationProvider provider);
    }
}