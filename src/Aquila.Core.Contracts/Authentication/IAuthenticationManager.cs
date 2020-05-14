namespace Aquila.Core.Contracts.Authentication
{
    /// <summary>
    /// Менеджер аутентификации
    /// </summary>
    public interface IAuthenticationManager
    {
        /// <summary>
        /// Аутентифицировать пользователя
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns></returns>
        IUser Authenticate(IAuthenticationToken token);


        void RegisterProvider(IAuthenticationProvider provider);
    }
}