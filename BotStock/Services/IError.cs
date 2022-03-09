using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotStock.Services
{
    public interface IError
    {
        void AddError(string message);
    }
}
