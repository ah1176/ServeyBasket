﻿namespace Survey_Basket.Services
{
    public interface INotificationService
    {
        Task SendNewPollsNotification(int? pollId = null);
    }
}
