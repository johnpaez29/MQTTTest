using MQTTTest2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTTest2.AccessDb
{
    public interface IRepositoryTerminal
    {
        Terminal GetTerminal(string filter);
    }
}
