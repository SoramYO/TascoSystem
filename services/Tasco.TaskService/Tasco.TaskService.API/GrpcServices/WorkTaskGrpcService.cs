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
    public class WorkTaskGrpcService : WorkTaskService.WorkTaskServiceBase
    {
        private readonly IWorkTaskService _workTaskService;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkTaskGrpcService> _logger;

        public WorkTaskGrpcService(
            IWorkTaskService workTaskService,
            IMapper mapper,
            ILogger<WorkTaskGrpcService> logger)
        {
            _workTaskService = workTaskService;
            _mapper = mapper;
            _logger = logger;
        }

    }
} 