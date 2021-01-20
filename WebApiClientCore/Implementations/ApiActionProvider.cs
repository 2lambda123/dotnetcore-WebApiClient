﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// Action提供者的接口
    /// </summary>
    sealed class ApiActionProvider : IApiActionProvider
    {
        /// <summary>
        /// 获取静态实例
        /// </summary>
        public static IApiActionProvider Default { get; } = new ApiActionProvider();

        /// <summary>
        /// 创建Action描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        public ApiActionDescriptor CreateDescriptor(MethodInfo method, Type interfaceType)
        {
            return new ApiActionDescriptorImpl(method, interfaceType);
        }

        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">Action描述</param>
        /// <returns></returns>
        public IActionInvoker CreateInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = apiAction.Return.ReturnType.IsInheritFrom<Task>()
                ? typeof(TaskActionInvoker<>).MakeGenericType(resultType)
                : typeof(ITaskActionInvoker<>).MakeGenericType(resultType);

            return invokerType.CreateInstance<IActionInvoker>(apiAction);
        }
    }
}