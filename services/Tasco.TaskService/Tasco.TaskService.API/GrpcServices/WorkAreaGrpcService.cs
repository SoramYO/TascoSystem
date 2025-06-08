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
    public class WorkAreaGrpcService : WorkAreaService.WorkAreaServiceBase
    {
        private readonly IWorkAreaService _workAreaService;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkAreaGrpcService> _logger;

        public WorkAreaGrpcService(
            IWorkAreaService workAreaService,
            IMapper mapper,
            ILogger<WorkAreaGrpcService> logger)
        {
            _workAreaService = workAreaService;
            _mapper = mapper;
            _logger = logger;
        }
    }
} 