namespace ApiProject.Services.Interface
{
    public interface IEmailService
    {
        Task SendWinnerEmail(string email, string winnerName, string giftName);
    }
}
