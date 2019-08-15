using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using SiloHost.Context;

namespace SiloHost.Filters
{
    public class LoggingFilter:IIncomingGrainCallFilter
    {
        private readonly GrainInfo _grainInfo;
        private readonly ILogger<LoggingFilter> _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOrleansRequestContext _context;


        public LoggingFilter(GrainInfo grainInfo,ILogger<LoggingFilter> logger,JsonSerializerSettings serializerSettings,IOrleansRequestContext context)
        {
            _grainInfo = grainInfo;
            _logger = logger;
            _serializerSettings = serializerSettings;
            _context = context;
        }
        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                if (ShowLog(context.ImplementationMethod.Name))
                {
                    var arguments = JsonConvert.SerializeObject(context.Arguments, _serializerSettings);
                    _logger.LogInformation($"LOGGINGFILTER TraceId {_context.TraceId}:{context.Grain.GetType()}-{context.ImplementationMethod.Name}:arguments{arguments} request");
                }
            
                await context.Invoke();
                if (ShowLog(context.ImplementationMethod.Name))
                {
                    var result = JsonConvert.SerializeObject(context.Result, _serializerSettings);
                    _logger.LogInformation($"LOGGINGFILTERTraceId {_context.TraceId}:{context.Grain.GetType()}-{context.ImplementationMethod.Name}:arguments{result} request");
                    _logger.LogInformation($"");
                }
            }
            catch (Exception ex)
            {
                var arguments = JsonConvert.SerializeObject(context.Arguments, _serializerSettings);
                var result=JsonConvert.SerializeObject(context.Result, _serializerSettings);
                _logger.LogError($"LOGGINGFILTERTraceId {_context.TraceId}:{context.Grain.GetType()}-{context.ImplementationMethod.Name}: threw exception: {nameof(ex)} request",ex);
                throw;
            }
            
        }

        private bool ShowLog(string methodName)
        {
            
            return _grainInfo.Methods.Contains(methodName);
        }
    }
}