using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SignalRStresser.Enums;
using SignalRStresser.Models;

namespace SignalRStresser
{
    class Collector
    {
        private ConcurrentQueue<long> _callTimeResults;
        private ConcurrentBag<ConnectionResult> _connectionResults;

        public Collector()
        {
            _callTimeResults = new ConcurrentQueue<long>();
            _connectionResults = new ConcurrentBag<ConnectionResult>();
        }

        public void SendConnectionResult(string id, long time)
        {
            _connectionResults.Add(new ConnectionResult {
                Id = id,
                Time = time,
                UtcConnectedDate = DateTime.UtcNow
            });
        }

        public List<ConnectionResult> GetConnectionResults()
        {
            return new List<ConnectionResult>(_connectionResults);
        }

        public void SendResult(long elapsedTime, CollectorResultType resultType)
        {
            switch(resultType)
            {
                case CollectorResultType.CallerLatency:
                    _callTimeResults.Enqueue(elapsedTime);
                    break;
                default:
                    break;
            }
        }

        public List<long> GetResults(CollectorResultType resultType)
        {
            List<long> result = new List<long>();
            if(resultType == CollectorResultType.CallerLatency)
            {
                result = new List<long>(_callTimeResults);
            }

            return result;
        }
       
    }
}
