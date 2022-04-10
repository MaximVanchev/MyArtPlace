using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Models.Common;

namespace MyArtPlace.Controllers
{
    public class BaseController : Controller
    {
        public async Task CheckMessages()
        {
            if (MessageViewModel.Message.ContainsKey(MessageConstants.SuccessMessage))
            {
                string message = MessageConstants.SuccessMessage;
                ViewData[message] = MessageViewModel.Message[message];
                MessageViewModel.Message.Remove(message);
            }
            else if (MessageViewModel.Message.ContainsKey(MessageConstants.ErrorMessage))
            {
                string message = MessageConstants.ErrorMessage;
                ViewData[message] = MessageViewModel.Message[message];
                MessageViewModel.Message.Remove(message);
            }
            else if (MessageViewModel.Message.ContainsKey(MessageConstants.WarningMessage))
            {
                string message = MessageConstants.WarningMessage;
                ViewData[message] = MessageViewModel.Message[message];
                MessageViewModel.Message.Remove(message);
            }
        }
    }
}
