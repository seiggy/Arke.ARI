using System;
using System.Threading.Tasks;

namespace Arke.ARI
{
    interface IAriDispatcher : IDisposable
    {
        Task QueueAction(Action action);
    }
}
