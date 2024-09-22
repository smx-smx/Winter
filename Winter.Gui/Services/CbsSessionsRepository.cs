using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Smx.Winter.Gui.Controllers;

namespace Smx.Winter.Gui.Services
{
    public class CbsSessionsRepository : IDisposable
    {
        private readonly Dictionary<Guid, WinterSession> _sessions;

        public CbsSessionsRepository(){
            _sessions = new Dictionary<Guid, WinterSession>();
        }

        public bool TryGetSession(Guid sessionId, [MaybeNullWhen(false)] out WinterSession session){
            return _sessions.TryGetValue(sessionId, out session);
        }

        internal void Add(Guid sessionId, WinterSession sess)
        {
            _sessions.Add(sessionId, sess);
        }

        public void Dispose()
        {
            foreach(var sess in _sessions.Values){
                sess.Dispose();
            }
        }

        public IEnumerable<WinterSession> All => _sessions.Values;
    }
}