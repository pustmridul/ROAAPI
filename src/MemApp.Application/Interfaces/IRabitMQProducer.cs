using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IRabitMQProducer
    {
        public void SendProductMessage<T>(T message, string Queue);
    }
}
