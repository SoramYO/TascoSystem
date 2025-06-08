using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tasco.TaskService.API.Models;
using Tasco.TaskService.API.Protos;
using Tasco.TaskService.Service.BusinessModels;
using Tasco.TaskService.Service.Interfaces;

namespace Tasco.TaskService.API.GrpcServices
{
    public class TaskActionGrpcService : TaskActionService.TaskActionServiceBase
    {
        private readonly ITaskActionService _taskActionService;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskActionGrpcService> _logger;

        public TaskActionGrpcService(
            ITaskActionService taskActionService,
            IMapper mapper,
            ILogger<TaskActionGrpcService> logger)
        {
            _taskActionService = taskActionService;
            _mapper = mapper;
            _logger = logger;
        }

    }
} 