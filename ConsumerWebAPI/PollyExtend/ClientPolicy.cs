using Polly;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumerWebAPI.PollyExtend
{
    public class ClientPolicy
    {
        private static readonly ClientPolicy m_Instance = new ClientPolicy();
        private PolicyWrap<string> m_WrapPolicy;

        public static ClientPolicy Instance
        {
            get { return m_Instance; }
        }

        public PolicyWrap<string> WrapPolicy
        {
            get
            {
                return m_WrapPolicy;
            }
        }

        /// <summary>
        /// 自定义Polly策略
        /// </summary>
        private ClientPolicy()
        {

            var retryPolicy = Policy<string>.Handle<Exception>()
                                  .Retry(3, (ex, retryCount) =>
                                  { 
                                      Console.WriteLine($"重新次数：{retryCount}: " + ex.Exception.Message);
                                  });       

            var fallbackPolicy = Policy<string>.Handle<Exception>()
                                         .Fallback(() =>
                                         {
                                             return "[]";
                                         });

            var circuitBreakerPolicy = Policy<string>.Handle<Exception>()
                                             .CircuitBreaker(1,
                                                              TimeSpan.FromSeconds(30),
                                                              onBreak: (Exception, TimeSpan) =>
                                                              {
                                                                  Console.WriteLine("-------断路器：开启状态");
                                                              },
                                                              onReset: () =>
                                                              {
                                                                  Console.WriteLine("-------断路器：关闭状态");
                                                              },
                                                              onHalfOpen: () =>
                                                              {
                                                                  Console.WriteLine("-------断路器：半开启状态");
                                                              });

            m_WrapPolicy = Policy.Wrap(fallbackPolicy, circuitBreakerPolicy, retryPolicy);   // 策略的优先级是右到左
        }
       
    }
}
