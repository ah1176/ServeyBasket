
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Survey_Basket.Helpers;

namespace Survey_Basket.Services
{
    public class NotificationService(
        ApplicationDbContext context 
        , UserManager<ApplicationUser> userManager,
        IEmailSender emailSender) : INotificationService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task SendNewPollsNotification(int? pollId = null)
        {
            IEnumerable<Poll> polls = [];

            if (pollId.HasValue)
            {
                var poll = await _context.Polls
                    .SingleOrDefaultAsync(x => x.Id == pollId && x.IsPublished);

            }
            else 
            {
                polls = await _context.Polls
                    .Where(x => x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow) && x.IsPublished)
                    .AsNoTracking()
                    .ToListAsync();
            }


            var users = await _userManager.Users.ToListAsync();
            foreach (var poll in polls) 
            {
                foreach (var user in users) 
                {
                    var placeholders = new Dictionary<string, string>() 
                    {
                        {"{{name}}",user.FirstName},
                        {"{{pollTill}}",poll.Title},
                        {"{{endDate}}",poll.EndsAt.ToString()},
                    };

                    var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);

                    await _emailSender.SendEmailAsync(user.Email!,"Survey Basket : New Poll",body);
                }
            }

            
        }
    }
}
