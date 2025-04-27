using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResApp.Application.ROA.Zone.Models;
using Microsoft.EntityFrameworkCore;

namespace ResApp.Application.ROA.Union.Command
{
   
    public class CreateUnionCommand : IRequest<Result<ZoneDto>>
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int ThanaId { get; set; }
    }

    public class CreateUnionCommandHandler : IRequestHandler<CreateUnionCommand, Result<ZoneDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public CreateUnionCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<ZoneDto>> Handle(CreateUnionCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }


                var result = new Result<ZoneDto>();

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit Union Council Permission Denied");
                        return result;
                    }
                    var checkUnionExist = await _context.UnionInfos
                       .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken: cancellationToken);

                    if (checkUnionExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkUnionExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        //bool isDuplicate = await _context.UnionInfos
                        //    .AnyAsync(d => d.Id != checkUnionExist.Id &&
                        //                  (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        //if (isDuplicate)
                        //{
                        //    result.HasError = true;
                        //    result.Messages.Add("This Union Name is already existed!!!");
                        //    return result;
                        //}


                        checkUnionExist.EnglishName = request.EnglishName;
                        checkUnionExist.BanglaName = request.BanglaName;
                        checkUnionExist.ThanaId = request.ThanaId;

                        _context.UnionInfos.Update(checkUnionExist);

                        if (await _context.SaveChangesAsync(cancellationToken) > 0)
                        {

                            result.HasError = false;
                            result.Messages.Add("Union Info has been updated successfully!!");

                            return result;
                        }
                        else
                        {
                            result.HasError = true;
                            result.Messages.Add("something wrong");
                            return result;
                        }

                    }
                    result.HasError = true;
                    result.Messages.Add("Data does not exist!!!");
                    return result;

                }

                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4001))
                    {
                        result.HasError = true;
                        result.Messages.Add("Create Union Council Permission Denied");
                        return result;
                    }

                    var checkUnionExist = await _context.UnionInfos
                       .FirstOrDefaultAsync(q => q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName, cancellationToken: cancellationToken);

                    if (checkUnionExist != null)
                    {
                        //result.HasError = true;
                        //result.Messages.Add("This Zone Name is already existed!!!");
                        //return result;
                    }



                    var zone = new UnionInfo
                    {
                        EnglishName = request.EnglishName,
                        BanglaName = request.BanglaName,
                        ThanaId = request.ThanaId,
                        IsActive = true
                    };

                    _context.UnionInfos.Add(zone);
                    for(int i=0;i<9;i++)
                    {
                        var ward = new Ward
                        {
                            EnglishName = (i+1).ToString(),
                            BanglaName= ConvertToBengaliNumber((i+1).ToString()),
                            IsActive=true,
                            UnionInfo=zone
                        };
                        _context.Wards.Add(ward);
                    }
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("Union Council Info has been created successfully!!");

                        return result;
                    }
                    else
                    {
                        result.HasError = true;
                        result.Messages.Add("something wrong");
                        return result;
                    }
                    //return result;
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }
        }

        public static string ConvertToBengaliNumber(string input)
        {
            var banglaDigits = new[] { "০", "১", "২", "৩", "৪", "৫", "৬", "৭", "৮", "৯" };
            var output = new StringBuilder();

            foreach (var ch in input)
            {
                if (char.IsDigit(ch))
                {
                    output.Append(banglaDigits[ch - '0']);
                }
                else
                {
                    output.Append(ch);
                }
            }

            return output.ToString();
        }

    }
}
