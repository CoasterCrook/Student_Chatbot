﻿using Capstone.DAO.Interfaces;
using Capstone.Models;
using Capstone.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private IQuoteDAO quoteDAO;
        private ICurriculumDAO curriculumDAO;
        private IPathwayDAO pathwayDAO;


        public MessageController(IQuoteDAO quoteDAO, ICurriculumDAO curriculumDAO, IPathwayDAO pathwayDAO)
        {
            this.quoteDAO = quoteDAO;
            this.curriculumDAO = curriculumDAO;
            this.pathwayDAO = pathwayDAO;
        }

        [HttpPost()]
        public ActionResult<UserMessage> RetrieveMessage(UserMessage message)
        {
            message = ResponseMethods.SetLowerCase(message);
            message = ResponseMethods.SetContext(message);
            
            UserMessage returnMessage = new UserMessage();

            switch(message.Context)
            {
                case "greet":
                    returnMessage.Message = ResponseMethods.ReturnGreeting(message);
                    break;
                case "help":
                    returnMessage.Message = ResponseMethods.ReturnHelp(message);
                    break;
                case "quote":
                    Quote quote = quoteDAO.GetQuote();
                    returnMessage.Message = $"{quote.Message} - {quote.Author}";
                    break;
                case "curriculum1":
                    returnMessage = ResponseMethods.StartCurriculumHelp(message);
                    break;
                case "curriculum2":
                    Curriculum curriculum = curriculumDAO.GetCurriculumResponse(message);
                    if (message.Message.Contains("done"))
                    {
                        returnMessage.Message = $"<p>Ok! What else can I help you with?</p>" +
                            $"<li style=\"list-style:none\">Curriculum</li> " +
                            $"<li style=\"list-style:none\">Pathway</li>" +
                            $"<li style=\"list-style:none\">Motivation</li>" +
                            $"<li style=\"list-style:none\">Positions</li>";
                    }
                    else if (curriculum.Response == null)
                    {
                        returnMessage.Message = ResponseMethods.ErrorMessage(message);
                    }
                    else
                    {
                        returnMessage.Message = $"{curriculum.Response} " + 
                            $"<p>What else would you like to know about curriculum? " +
                            $"Tell me \"done\" at any point to stop learning about curriculum.</p>";
                    }
                    returnMessage.Context = ResponseMethods.StopCurriculumHelp(message);
                    break;
                case "pathway1":
                    returnMessage = ResponseMethods.StartPathwayHelp(message);
                    break;
                case "pathway2":
                    Pathway pathway = pathwayDAO.GetPathwayResponse(message);
                    if (message.Message.Contains("done"))
                    {
                        returnMessage.Message = $"<p>Ok! What else can I help you with?</p>" +
                            $"<li style=\"list-style:none\">Curriculum</li> " +
                            $"<li style=\"list-style:none\">Pathway</li>" +
                            $"<li style=\"list-style:none\">Motivation</li>" +
                            $"<li style=\"list-style:none\">Positions</li>";
                    }
                    else if (pathway.Response == null)
                    {
                        returnMessage.Message = ResponseMethods.ErrorMessage(message);
                    }
                    else
                    {
                        returnMessage.Message = $"{pathway.Response} " +
                            $"<p>What else would you like to know about pathway? " +
                            $"Tell me \"done\" at any point to stop learning about pathway.</p>";
                    }
                    returnMessage.Context = ResponseMethods.StopPathwayHelp(message);
                    break;
                case "error":
                    returnMessage.Message = ResponseMethods.ErrorMessage(message);
                    break;
            }
            return returnMessage;
        }
    }
}
