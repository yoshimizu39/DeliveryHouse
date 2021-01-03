using DeliveryHouse.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public interface IEmailHelper
    {
        Response SednMail(string to, string subject, string body);
    }
}
